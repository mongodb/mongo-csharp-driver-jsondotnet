/* Copyright 2015-2016 MongoDB Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NSubstitute;
using NUnit.Framework;

namespace MongoDB.Integrations.JsonDotNet.Tests
{
    // public methods
    [TestFixture]
    public class JsonSerializerAdapterTests
    {
        [Test]
        public void TryGetItemSerializationInfo_should_return_expected_result()
        {
            var subject = new JsonSerializerAdapter<int[]>();

            BsonSerializationInfo info;
            var result = subject.TryGetItemSerializationInfo(out info);

            result.Should().BeTrue();
            info.ElementName.Should().BeNull();
            info.NominalType.Should().Be(typeof(int));
            info.Serializer.Should().BeOfType<JsonSerializerAdapter<int>>();
            info.Serializer.ValueType.Should().Be(typeof(int));
        }

        [Test]
        public void TryGetItemSerializationInfo_should_throw_when_contract_has_a_converter()
        {
            var wrappedSerializer = new Newtonsoft.Json.JsonSerializer();
            var intContract = new Newtonsoft.Json.Serialization.JsonArrayContract(typeof(int[]))
            {
                Converter = Substitute.For<Newtonsoft.Json.JsonConverter>()
            };
            wrappedSerializer.ContractResolver = new DictionaryContractResolver(
                new Dictionary<Type, JsonContract>
                {
                    { typeof(int[]), intContract }
                });
            var subject = new JsonSerializerAdapter<int[]>(wrappedSerializer);

            BsonSerializationInfo info;
            Action action = () => subject.TryGetItemSerializationInfo(out info);

            action.ShouldThrow<BsonSerializationException>().And.Message.Should().Contain("has a Converter");
        }

        [Test]
        public void TryGetItemSerializationInfo_should_return_false_when_contract_is_not_an_array_contract()
        {
            var subject = new JsonSerializerAdapter<C>();

            BsonSerializationInfo info;
            var result = subject.TryGetItemSerializationInfo(out info);
            result.Should().BeFalse();
        }

        [Test]
        public void TryGetMemberSerializationInfo_should_return_expected_result_for_int_member()
        {
            var subject = new JsonSerializerAdapter<C>();

            BsonSerializationInfo info;
            var result = subject.TryGetMemberSerializationInfo("N", out info);

            result.Should().BeTrue();
            info.ElementName.Should().Be("n");
            info.NominalType.Should().Be(typeof(int));
            info.Serializer.Should().BeOfType<JsonSerializerAdapter<int>>();
            info.Serializer.ValueType.Should().Be(typeof(int));
        }

        [Test]
        public void TryGetMemberSerializationInfo_should_return_expected_result_for_nested_document_member()
        {
            var subject = new JsonSerializerAdapter<C>();

            BsonSerializationInfo info1;
            var result1 = subject.TryGetMemberSerializationInfo("D", out info1);

            result1.Should().BeTrue();
            info1.ElementName.Should().Be("d");
            info1.NominalType.Should().Be(typeof(D));
            info1.Serializer.Should().BeOfType<JsonSerializerAdapter<D>>();
            info1.Serializer.ValueType.Should().Be(typeof(D));

            BsonSerializationInfo info2;
            var result2 = ((IBsonDocumentSerializer)info1.Serializer).TryGetMemberSerializationInfo("B", out info2);

            result2.Should().BeTrue();
            info2.ElementName.Should().Be("b");
            info2.NominalType.Should().Be(typeof(bool));
            info2.Serializer.Should().BeOfType<JsonSerializerAdapter<bool>>();
            info2.Serializer.ValueType.Should().Be(typeof(bool));
        }

        [Test]
        public void TryGetMemberSerializationInfo_should_return_expected_result_for_string_member()
        {
            var subject = new JsonSerializerAdapter<C>();

            BsonSerializationInfo info;
            var result = subject.TryGetMemberSerializationInfo("S", out info);

            result.Should().BeTrue();
            info.ElementName.Should().Be("s");
            info.NominalType.Should().Be(typeof(string));
            info.Serializer.Should().BeOfType<JsonSerializerAdapter<string>>();
            info.Serializer.ValueType.Should().Be(typeof(string));
        }

        [Test]
        public void TryGetMemberSerializationInfo_should_return_false_for_non_existent_member()
        {
            var subject = new JsonSerializerAdapter<C>();

            BsonSerializationInfo info;
            var result = subject.TryGetMemberSerializationInfo("X", out info);

            result.Should().BeFalse();
            info.Should().BeNull();
        }

        [Test]
        public void TryGetMemberSerializationInfo_should_return_false_when_class_has_converter()
        {
            var subject = new JsonSerializerAdapter<E>();

            BsonSerializationInfo info;
            Action action = () => subject.TryGetMemberSerializationInfo("F", out info);

            action.ShouldThrow<BsonSerializationException>();
        }

        // nested types
        private class C
        {
            [Newtonsoft.Json.JsonProperty("a")]
            public int[] A { get; set; }
            [Newtonsoft.Json.JsonProperty("d")]
            public D D { get; set; }
            [Newtonsoft.Json.JsonProperty("n")]
            public int N { get; set; }
            [Newtonsoft.Json.JsonProperty("s")]
            public string S { get; set; }
        }

        private class D
        {
            [Newtonsoft.Json.JsonProperty("b")]
            public bool B { get; set; }
        }

        [Newtonsoft.Json.JsonConverter(typeof(EConverter))]
        private class E
        {
            [Newtonsoft.Json.JsonProperty("f")]
            public double F { get; set; }
        }

        private class EConverter : Newtonsoft.Json.JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(E);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }

        private class DictionaryContractResolver : Newtonsoft.Json.Serialization.IContractResolver
        {
            private readonly Dictionary<Type, Newtonsoft.Json.Serialization.JsonContract> _contracts;

            public DictionaryContractResolver(Dictionary<Type, Newtonsoft.Json.Serialization.JsonContract> contracts)
            {
                _contracts = contracts;
            }

            public JsonContract ResolveContract(Type type)
            {
                JsonContract contract;
                if (_contracts.TryGetValue(type, out contract))
                {
                    return contract;
                }
                return null;
            }
        }
    }
}
