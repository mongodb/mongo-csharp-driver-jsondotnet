/* Copyright 2015 MongoDB Inc.
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
using FluentAssertions;
using MongoDB.Bson;
using NSubstitute;
using NUnit.Framework;

namespace MongoDB.Integrations.JsonDotNet.Tests.JsonDotNetSerializer
{
    [TestFixture]
    public class JsonDotNetSerializerTests
    {
        [Test]
        public void constructor_should_initialize_instance()
        {
            var result = new JsonDotNetSerializer<object>();

            result.ValueType.Should().Be(typeof(object));
        }

        [Test]
        public void constructor_with_wrappedSerializer_should_initialize_instance()
        {
            var wrappedSerializer = Substitute.For<Newtonsoft.Json.JsonSerializer>();
            var result = new JsonDotNetSerializer<object>(wrappedSerializer);

            result.ValueType.Should().Be(typeof(object));
        }

        [Test]
        public void constructor_with_wrappedSerializer_should_throw_when_wrappedSerializer_is_null()
        {
            Action action = () => { var result = new JsonDotNetSerializer<object>(null); };

            action.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("wrappedSerializer");
        }
    }

    [TestFixture]
    public class JsonDotNetSerializerClassWithBsonInt32Tests : JsonDotNetSerializerTestsBase
    {
        private class C
        {
            public BsonInt32 V { get; set; }
        }

        [TestCase("{ V : null }", null)]
        [TestCase("{ V : 1 }", 1)]
        public void Deserialize_should_return_expected_result(string json, int? nullableInt32)
        {
            var subject = new JsonDotNetSerializer<C>();
            var expectedResult = nullableInt32 == null ? null : (BsonInt32)nullableInt32.Value;

            var result = Deserialize<C>(subject, ToBson(json));

            result.V.Should().Be(expectedResult);
        }

        [TestCase(null, "{ \"V\" : null }")]
        [TestCase(1, "{ \"V\" : 1 }")]
        public void Serialize_should_have_expected_result(int? nullableInt32, string expectedResult)
        {
            var subject = new JsonDotNetSerializer<C>();
            var value = new C { V = nullableInt32 == null ? null : (BsonInt32)nullableInt32.Value };

            var result = Serialize(subject, value);

            result.Should().Equal(ToBson(expectedResult));
        }
    }

    [TestFixture]
    public class JsonDotNetSerializerClassWithBsonMaxKeyTests : JsonDotNetSerializerTestsBase
    {
        private class C
        {
            public BsonMaxKey V { get; set; }
        }

        [TestCase("{ V : null }", null)]
        [TestCase("{ V : { $maxKey : 1 } }", true)]
        public void Deserialize_should_return_expected_result(string json, bool? nullableMaxKey)
        {
            var subject = new JsonDotNetSerializer<C>();
            var expectedResult = nullableMaxKey == null ? null : BsonMaxKey.Value;

            var result = Deserialize<C>(subject, ToBson(json));

            result.V.Should().Be(expectedResult);
        }

        [TestCase(null, "{ \"V\" : null }")]
        [TestCase(true, "{ \"V\" : MaxKey }")]
        public void Serialize_should_have_expected_result(bool? nullableMaxKey, string expectedResult)
        {
            var subject = new JsonDotNetSerializer<C>();
            var value = new C { V = nullableMaxKey == null ? null : BsonMaxKey.Value };

            var result = Serialize(subject, value);

            result.Should().Equal(ToBson(expectedResult));
        }
    }

    [TestFixture]
    public class JsonDotNetSerializerClassWithIntTests : JsonDotNetSerializerTestsBase
    {
        private class C
        {
            public int X { get; set; }
        }

        [TestCase("{ X : 1 }", 1)]
        [TestCase("{ X : 2 }", 2)]
        public void Deserialize_should_return_expected_result(string json, int expectedResult)
        {
            var subject = new JsonDotNetSerializer<C>();

            var result = Deserialize<C>(subject, ToBson(json));

            result.X.Should().Be(expectedResult);
        }

        [TestCase(1, "{ X : 1 }")]
        [TestCase(2, "{ X : 2 }")]
        public void Serialize_should_have_expected_result(int x, string expectedResult)
        {
            var subject = new JsonDotNetSerializer<C>();
            var value = new C { X = x };

            var result = Serialize(subject, value);

            result.Should().Equal(ToBson(expectedResult));
        }
    }

    [TestFixture]
    public class JsonDotNetSerializerClassWithObjectIdTests : JsonDotNetSerializerTestsBase
    {
        private class C
        {
            [Newtonsoft.Json.JsonProperty("_id")]
            public ObjectId Id { get; set; }
        }

        [TestCase("{ _id : ObjectId(\"112233445566778899aabbcc\") }", "112233445566778899aabbcc")]
        [TestCase("{ _id : ObjectId(\"2233445566778899aabbccdd\") }", "2233445566778899aabbccdd")]
        public void Deserialize_should_return_expected_result(string json, string expectedResult)
        {
            var subject = new JsonDotNetSerializer<C>();

            var result = Deserialize<C>(subject, ToBson(json));

            result.Id.Should().Be(ObjectId.Parse(expectedResult));
        }

        [TestCase("112233445566778899aabbcc", "{ \"_id\" : ObjectId(\"112233445566778899aabbcc\") }")]
        [TestCase("2233445566778899aabbccdd", "{ \"_id\" : ObjectId(\"2233445566778899aabbccdd\") }")]
        public void Serialize_should_have_expected_result(string hexValue, string expectedResult)
        {
            var subject = new JsonDotNetSerializer<C>();
            var value = new C { Id = ObjectId.Parse(hexValue) };

            var result = Serialize(subject, value);

            result.Should().Equal(ToBson(expectedResult));
        }
    }
}
