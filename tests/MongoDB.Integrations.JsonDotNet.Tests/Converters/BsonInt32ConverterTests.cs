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
using MongoDB.Integrations.JsonDotNet.Converters;
using NUnit.Framework;

namespace MongoDB.Integrations.JsonDotNet.Tests.Converters
{
    [TestFixture]
    public class BsonInt32ConverterTests : JsonConverterTestsBase
    {
        [Test]
        public void Instance_get_returns_cached_result()
        {
            var result1 = BsonInt32Converter.Instance;
            var result2 = BsonInt32Converter.Instance;

            result2.Should().BeSameAs(result1);
        }

        [Test]
        public void Instance_get_returns_expected_result()
        {
            var result = BsonInt32Converter.Instance;

            result.Should().NotBeNull();
            result.Should().BeOfType<BsonInt32Converter>();
        }

        [TestCase("{ x : null }", null)]
        [TestCase("{ x : 0 }", 0)]
        [TestCase("{ x : 1 }", 1)]
        [TestCase("{ x : \"1\" }", 1)]
        public void ReadJson_should_return_expected_result_when_using_native_bson_reader(string json, int? nullableInt32)
        {
            var subject = new BsonInt32Converter();
            var expectedResult = nullableInt32 == null ? null : (BsonInt32)nullableInt32.Value;

            var result = ReadJsonUsingNativeBsonReader<BsonInt32>(subject, ToBson(json), mustBeNested: true);

            result.Should().Be(expectedResult);
        }

        [TestCase("null", null)]
        [TestCase("0", 0)]
        [TestCase("1", 1)]
        [TestCase("\"1\"", 1)]
        public void ReadJson_should_return_expected_result_when_using_native_json_reader(string json, int? nullableInt32)
        {
            var subject = new BsonInt32Converter();
            var expectedResult = nullableInt32 == null ? null : (BsonInt32)nullableInt32.Value;

            var result = ReadJsonUsingNativeJsonReader<BsonInt32>(subject, json);

            result.Should().Be(expectedResult);
        }

        [TestCase("{ x : null }", null)]
        [TestCase("{ x : 0 }", 0)]
        [TestCase("{ x : 1 }", 1)]
        [TestCase("{ x : \"1\" }", 1)]
        public void ReadJson_should_return_expected_result_when_using_wrapped_bson_reader(string json, int? nullableInt32)
        {
            var subject = new BsonInt32Converter();
            var expectedResult = nullableInt32 == null ? null : (BsonInt32)nullableInt32.Value;

            var result = ReadJsonUsingWrappedBsonReader<BsonInt32>(subject, ToBson(json), mustBeNested: true);

            result.Should().Be(expectedResult);
        }

        [TestCase("null", null)]
        [TestCase("0", 0)]
        [TestCase("1", 1)]
        [TestCase("\"1\"", 1)]
        public void ReadJson_should_return_expected_result_when_using_wrapped_json_reader(string json, int? nullableInt32)
        {
            var subject = new BsonInt32Converter();
            var expectedResult = nullableInt32 == null ? null : (BsonInt32)nullableInt32.Value;

            var result = ReadJsonUsingWrappedJsonReader<BsonInt32>(subject, json);

            result.Should().Be(expectedResult);
        }

        [Test]
        public void ReadJson_should_throw_when_token_type_is_invalid()
        {
            var subject = new BsonInt32Converter();
            var json = "undefined";

            Action action = () => { var _ = ReadJsonUsingNativeJsonReader<BsonInt32>(subject, json); };

            action.ShouldThrow<Newtonsoft.Json.JsonReaderException>();
        }

        [TestCase(null, "{ x : null }")]
        [TestCase(0, "{ x : NumberLong(0) }")]
        [TestCase(1, "{ x : NumberLong(1) }")]
        public void WriteJson_should_have_expected_result_when_using_native_bson_writer(int? nullableInt32, string expectedResult)
        {
            var subject = new BsonInt32Converter();
            var value = nullableInt32 == null ? null : (BsonInt32)nullableInt32.Value;

            var result = WriteJsonUsingNativeBsonWriter(subject, value, mustBeNested: true);

            result.Should().Equal(ToBson(expectedResult));
        }

        [TestCase(null, "null")]
        [TestCase(0, "0")]
        [TestCase(1, "1")]
        public void WriteJson_should_have_expected_result_when_using_native_json_writer(int? nullableInt32, string expectedResult)
        {
            var subject = new BsonInt32Converter();
            var value = nullableInt32 == null ? null : (BsonInt32)nullableInt32.Value;

            var result = WriteJsonUsingNativeJsonWriter(subject, value);

            result.Should().Be(expectedResult);
        }

        [TestCase(null, "{ x : null }")]
        [TestCase(0, "{ x : 0 }")]
        [TestCase(1, "{ x : 1 }")]
        public void WriteJson_should_have_expected_result_when_using_wrapped_bson_writer(int? nullableInt32, string expectedResult)
        {
            var subject = new BsonInt32Converter();
            var value = nullableInt32 == null ? null : (BsonInt32)nullableInt32.Value;

            var result = WriteJsonUsingWrappedBsonWriter(subject, value, mustBeNested: true);

            result.Should().Equal(ToBson(expectedResult));
        }

        [TestCase(null, "null")]
        [TestCase(0, "0")]
        [TestCase(1, "1")]
        public void WriteJson_should_have_expected_result_when_using_wrapped_json_writer(int? nullableInt32, string expectedResult)
        {
            var subject = new BsonInt32Converter();
            var value = nullableInt32 == null ? null : (BsonInt32)nullableInt32.Value;

            var result = WriteJsonUsingWrappedJsonWriter(subject, value);

            result.Should().Be(expectedResult);
        }
    }
}
