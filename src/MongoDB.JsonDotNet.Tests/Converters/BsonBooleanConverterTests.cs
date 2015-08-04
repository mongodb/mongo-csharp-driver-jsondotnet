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
using MongoDB.JsonDotNet.Converters;
using NUnit.Framework;

namespace MongoDB.JsonDotNet.Tests.Converters
{
    [TestFixture]
    public class BsonBooleanConverterTests : JsonConverterTestsBase
    {
        [Test]
        public void Instance_get_returns_cached_result()
        {
            var result1 = BsonBooleanConverter.Instance;
            var result2 = BsonBooleanConverter.Instance;

            result2.Should().BeSameAs(result1);
        }

        [Test]
        public void Instance_get_returns_expected_result()
        {
            var result = BsonBooleanConverter.Instance;

            result.Should().NotBeNull();
            result.Should().BeOfType<BsonBooleanConverter>();
        }

        [TestCase("{ x : null }", null)]
        [TestCase("{ x : false }", false)]
        [TestCase("{ x : true }", true)]
        public void ReadJson_should_return_expected_result_when_using_native_bson_reader(string json, bool? nullableBoolean)
        {
            var subject = new BsonBooleanConverter();
            var expectedResult = nullableBoolean == null ? null : (BsonBoolean)nullableBoolean.Value;

            var result = ReadJsonUsingNativeBsonReader<BsonBoolean>(subject, ToBson(json), mustBeNested: true);

            result.Should().Be(expectedResult);
        }

        [TestCase("null", null)]
        [TestCase("false", false)]
        [TestCase("true", true)]
        public void ReadJson_should_return_expected_result_when_using_native_json_reader(string json, bool? nullableBoolean)
        {
            var subject = new BsonBooleanConverter();
            var expectedResult = nullableBoolean == null ? null : (BsonBoolean)nullableBoolean.Value;

            var result = ReadJsonUsingNativeJsonReader<BsonBoolean>(subject, json);

            result.Should().Be(expectedResult);
        }

        [TestCase("{ x : null }", null)]
        [TestCase("{ x : false }", false)]
        [TestCase("{ x : true }", true)]
        public void ReadJson_should_return_expected_result_when_using_wrapped_bson_reader(string json, bool? nullableBoolean)
        {
            var subject = new BsonBooleanConverter();
            var expectedResult = nullableBoolean == null ? null : (BsonBoolean)nullableBoolean.Value;

            var result = ReadJsonUsingWrappedBsonReader<BsonBoolean>(subject, ToBson(json), mustBeNested: true);

            result.Should().Be(expectedResult);
        }

        [TestCase("null", null)]
        [TestCase("false", false)]
        [TestCase("true", true)]
        public void ReadJson_should_return_expected_result_when_using_wrapped_json_reader(string json, bool? nullableBoolean)
        {
            var subject = new BsonBooleanConverter();
            var expectedResult = nullableBoolean == null ? null : (BsonBoolean)nullableBoolean.Value;

            var result = ReadJsonUsingWrappedJsonReader<BsonBoolean>(subject, json);

            result.Should().Be(expectedResult);
        }

        [Test]
        public void ReadJson_should_throw_when_token_type_is_invalid()
        {
            var subject = new BsonBooleanConverter();
            var json = "undefined";

            Action action = () => { var _ = ReadJsonUsingNativeJsonReader<BsonBoolean>(subject, json); };

            action.ShouldThrow<Newtonsoft.Json.JsonReaderException>();
        }

        [TestCase(null, "{ x : null }")]
        [TestCase(false, "{ x : false }")]
        [TestCase(true, "{ x : true }")]
        public void WriteJson_should_have_expected_result_when_using_native_bson_writer(bool? nullableBoolean, string expectedResult)
        {
            var subject = new BsonBooleanConverter();
            var value = nullableBoolean == null ? null : (BsonBoolean)nullableBoolean.Value;

            var result = WriteJsonUsingNativeBsonWriter(subject, value, mustBeNested: true);

            result.Should().Equal(ToBson(expectedResult));
        }

        [TestCase(null, "null")]
        [TestCase(false, "false")]
        [TestCase(true, "true")]
        public void WriteJson_should_have_expected_result_when_using_native_json_writer(bool? nullableBoolean, string expectedResult)
        {
            var subject = new BsonBooleanConverter();
            var value = nullableBoolean == null ? null : (BsonBoolean)nullableBoolean.Value;

            var result = WriteJsonUsingNativeJsonWriter(subject, value);

            result.Should().Be(expectedResult);
        }

        [TestCase(null, "{ x : null }")]
        [TestCase(false, "{ x : false }")]
        [TestCase(true, "{ x : true }")]
        public void WriteJson_should_have_expected_result_when_using_wrapped_bson_writer(bool? nullableBoolean, string expectedResult)
        {
            var subject = new BsonBooleanConverter();
            var value = nullableBoolean == null ? null : (BsonBoolean)nullableBoolean.Value;

            var result = WriteJsonUsingWrappedBsonWriter(subject, value, mustBeNested: true);

            result.Should().Equal(ToBson(expectedResult));
        }

        [TestCase(null, "null")]
        [TestCase(false, "false")]
        [TestCase(true, "true")]
        public void WriteJson_should_have_expected_result_when_using_wrapped_json_writer(bool? nullableBoolean, string expectedResult)
        {
            var subject = new BsonBooleanConverter();
            var value = nullableBoolean == null ? null : (BsonBoolean)nullableBoolean.Value;

            var result = WriteJsonUsingWrappedJsonWriter(subject, value);

            result.Should().Be(expectedResult);
        }
    }
}
