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
    public class BsonDoubleConverterTests : JsonConverterTestsBase
    {
        [Test]
        public void Instance_get_returns_cached_result()
        {
            var result1 = BsonDoubleConverter.Instance;
            var result2 = BsonDoubleConverter.Instance;

            result2.Should().BeSameAs(result1);
        }

        [Test]
        public void Instance_get_returns_expected_result()
        {
            var result = BsonDoubleConverter.Instance;

            result.Should().NotBeNull();
            result.Should().BeOfType<BsonDoubleConverter>();
        }

        [TestCase("{ x : null }", null)]
        [TestCase("{ x : 0.0 }", 0.0)]
        [TestCase("{ x : 1 }", 1.0)]
        [TestCase("{ x : 1.0 }", 1.0)]
        [TestCase("{ x : \"1.0\" }", 1.0)]
        public void ReadJson_should_return_expected_result_when_using_native_bson_reader(string json, double? nullableDouble)
        {
            var subject = new BsonDoubleConverter();
            var expectedResult = nullableDouble == null ? null : (BsonDouble)nullableDouble.Value;

            var result = ReadJsonUsingNativeBsonReader<BsonDouble>(subject, ToBson(json), mustBeNested: true);

            result.Should().Be(expectedResult);
        }

        [TestCase("null", null)]
        [TestCase("0.0", 0.0)]
        [TestCase("1", 1.0)]
        [TestCase("1.0", 1.0)]
        [TestCase("\"1.0\"", 1.0)]
        public void ReadJson_should_return_expected_result_when_using_native_json_reader(string json, double? nullableDouble)
        {
            var subject = new BsonDoubleConverter();
            var expectedResult = nullableDouble == null ? null : (BsonDouble)nullableDouble.Value;

            var result = ReadJsonUsingNativeJsonReader<BsonDouble>(subject, json);

            result.Should().Be(expectedResult);
        }

        [TestCase("{ x : null }", null)]
        [TestCase("{ x : 0.0 }", 0.0)]
        [TestCase("{ x : 1 }", 1.0)]
        [TestCase("{ x : 1.0 }", 1.0)]
        [TestCase("{ x : \"1.0\" }", 1.0)]
        public void ReadJson_should_return_expected_result_when_using_wrapped_bson_reader(string json, double? nullableDouble)
        {
            var subject = new BsonDoubleConverter();
            var expectedResult = nullableDouble == null ? null : (BsonDouble)nullableDouble.Value;

            var result = ReadJsonUsingWrappedBsonReader<BsonDouble>(subject, ToBson(json), mustBeNested: true);

            result.Should().Be(expectedResult);
        }

        [TestCase("null", null)]
        [TestCase("0.0", 0.0)]
        [TestCase("1", 1.0)]
        [TestCase("1.0", 1.0)]
        [TestCase("\"1.0\"", 1.0)]
        public void ReadJson_should_return_expected_result_when_using_wrapped_json_reader(string json, double? nullableDouble)
        {
            var subject = new BsonDoubleConverter();
            var expectedResult = nullableDouble == null ? null : (BsonDouble)nullableDouble.Value;

            var result = ReadJsonUsingWrappedJsonReader<BsonDouble>(subject, json);

            result.Should().Be(expectedResult);
        }

        [Test]
        public void ReadJson_should_throw_when_token_type_is_invalid()
        {
            var subject = new BsonDoubleConverter();
            var json = "undefined";

            Action action = () => { var _ = ReadJsonUsingNativeJsonReader<BsonDouble>(subject, json); };

            action.ShouldThrow<Newtonsoft.Json.JsonReaderException>();
        }

        [TestCase(null, "{ x : null }")]
        [TestCase(0.0, "{ x : 0.0 }")]
        [TestCase(1.0, "{ x : 1.0 }")]
        public void WriteJson_should_have_expected_result_when_using_native_bson_writer(double? nullableDouble, string expectedResult)
        {
            var subject = new BsonDoubleConverter();
            var value = nullableDouble == null ? null : (BsonDouble)nullableDouble.Value;

            var result = WriteJsonUsingNativeBsonWriter(subject, value, mustBeNested: true);

            result.Should().Equal(ToBson(expectedResult));
        }

        [TestCase(null, "null")]
        [TestCase(0.0, "0.0")]
        [TestCase(1.0, "1.0")]
        public void WriteJson_should_have_expected_result_when_using_native_json_writer(double? nullableDouble, string expectedResult)
        {
            var subject = new BsonDoubleConverter();
            var value = nullableDouble == null ? null : (BsonDouble)nullableDouble.Value;

            var result = WriteJsonUsingNativeJsonWriter(subject, value);

            result.Should().Be(expectedResult);
        }

        [TestCase(null, "{ x : null }")]
        [TestCase(0.0, "{ x : 0.0 }")]
        [TestCase(1.0, "{ x : 1.0 }")]
        public void WriteJson_should_have_expected_result_when_using_wrapped_bson_writer(double? nullableDouble, string expectedResult)
        {
            var subject = new BsonDoubleConverter();
            var value = nullableDouble == null ? null : (BsonDouble)nullableDouble.Value;

            var result = WriteJsonUsingWrappedBsonWriter(subject, value, mustBeNested: true);

            result.Should().Equal(ToBson(expectedResult));
        }

        [TestCase(null, "null")]
        [TestCase(0.0, "0.0")]
        [TestCase(1.0, "1.0")]
        public void WriteJson_should_have_expected_result_when_using_wrapped_json_writer(double? nullableDouble, string expectedResult)
        {
            var subject = new BsonDoubleConverter();
            var value = nullableDouble == null ? null : (BsonDouble)nullableDouble.Value;

            var result = WriteJsonUsingWrappedJsonWriter(subject, value);

            result.Should().Be(expectedResult);
        }
    }
}
