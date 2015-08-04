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
    public class BsonRegularExpressionConverterTests : JsonConverterTestsBase
    {
        [Test]
        public void Instance_get_returns_cached_result()
        {
            var result1 = BsonRegularExpressionConverter.Instance;
            var result2 = BsonRegularExpressionConverter.Instance;

            result2.Should().BeSameAs(result1);
        }

        [Test]
        public void Instance_get_returns_expected_result()
        {
            var result = BsonRegularExpressionConverter.Instance;

            result.Should().NotBeNull();
            result.Should().BeOfType<BsonRegularExpressionConverter>();
        }

        [TestCase("{ x : null }", null, null)]
        [TestCase("{ x : \"abc\" }", "abc", "")]
        [TestCase("{ x : \"def\" }", "def", "")]
        [TestCase("{ x : { $regex : \"abc\", $options : \"i\" } }", "abc", "i")]
        [TestCase("{ x : { $regex : \"def\", $options : \"m\" } }", "def", "m")]
        [TestCase("{ x : { $$regex : \"abc\", $$options : \"i\" } }", "abc", "i")]
        [TestCase("{ x : { $$regex : \"def\", $$options : \"m\" } }", "def", "m")]
        public void ReadJson_should_return_expected_result_when_using_native_bson_reader(string json, string nullablePattern, string nullableOptions)
        {
            var subject = new BsonRegularExpressionConverter();
            var expectedResult = nullablePattern == null ? null : new BsonRegularExpression(nullablePattern, nullableOptions);

            var result = ReadJsonUsingNativeBsonReader<BsonRegularExpression>(subject, ToBson(json), mustBeNested: true);

            result.Should().Be(expectedResult);
        }

        [TestCase("null", null, null)]
        [TestCase("\"abc\"", "abc", "")]
        [TestCase("\"def\"", "def", "")]
        [TestCase("{ $regex : \"abc\", $options : \"i\" }", "abc", "i")]
        [TestCase("{ $regex : \"def\", $options : \"m\" }", "def", "m")]
        public void ReadJson_should_return_expected_result_when_using_native_json_reader(string json, string nullablePattern, string nullableOptions)
        {
            var subject = new BsonRegularExpressionConverter();
            var expectedResult = nullablePattern == null ? null : new BsonRegularExpression(nullablePattern, nullableOptions);

            var result = ReadJsonUsingNativeJsonReader<BsonRegularExpression>(subject, json);

            result.Should().Be(expectedResult);
        }

        [TestCase("{ x : null }", null, null)]
        [TestCase("{ x : \"abc\" }", "abc", "")]
        [TestCase("{ x : \"def\" }", "def", "")]
        [TestCase("{ x : { $regex : \"abc\", $options : \"i\" } }", "abc", "i")]
        [TestCase("{ x : { $regex : \"def\", $options : \"m\" } }", "def", "m")]
        [TestCase("{ x : { $$regex : \"abc\", $$options : \"i\" } }", "abc", "i")]
        [TestCase("{ x : { $$regex : \"def\", $$options : \"m\" } }", "def", "m")]
        public void ReadJson_should_return_expected_result_when_using_wrapped_bson_reader(string json, string nullablePattern, string nullableOptions)
        {
            var subject = new BsonRegularExpressionConverter();
            var expectedResult = nullablePattern == null ? null : new BsonRegularExpression(nullablePattern, nullableOptions);

            var result = ReadJsonUsingWrappedBsonReader<BsonRegularExpression>(subject, ToBson(json), mustBeNested: true);

            result.Should().Be(expectedResult);
        }

        [TestCase("null", null, null)]
        [TestCase("\"abc\"", "abc", "")]
        [TestCase("\"def\"", "def", "")]
        [TestCase("{ $regex : \"abc\", $options : \"i\" }", "abc", "i")]
        [TestCase("{ $regex : \"def\", $options : \"m\" }", "def", "m")]
        public void ReadJson_should_return_expected_result_when_using_wrapped_json_reader(string json, string nullablePattern, string nullableOptions)
        {
            var subject = new BsonRegularExpressionConverter();
            var expectedResult = nullablePattern == null ? null : new BsonRegularExpression(nullablePattern, nullableOptions);

            var result = ReadJsonUsingWrappedJsonReader<BsonRegularExpression>(subject, json);

            result.Should().Be(expectedResult);
        }

        [Test]
        public void ReadJson_should_throw_when_token_type_is_invalid()
        {
            var subject = new BsonRegularExpressionConverter();
            var json = "undefined";

            Action action = () => { var _ = ReadJsonUsingNativeJsonReader<BsonRegularExpression>(subject, json); };

            action.ShouldThrow<Newtonsoft.Json.JsonReaderException>();
        }

        [TestCase(null, null, "{ x : null }")]
        [TestCase("abc", "i", "{ x : { $regex : \"abc\", $options : \"i\" } }")]
        [TestCase("def", "m", "{ x : { $regex : \"def\", $options : \"m\" } }")]
        public void WriteJson_should_have_expected_result_when_using_native_bson_writer(string nullablePattern, string nullableOptions, string expectedResult)
        {
            var subject = new BsonRegularExpressionConverter();
            var value = nullablePattern == null ? null : new BsonRegularExpression(nullablePattern, nullableOptions);

            var result = WriteJsonUsingNativeBsonWriter(subject, value, mustBeNested: true);

            result.Should().Equal(ToBson(expectedResult));
        }

        [TestCase(null, null, "null")]
        [TestCase("abc", "i", "{\"$regex\":\"abc\",\"$options\":\"i\"}")]
        [TestCase("def", "m", "{\"$regex\":\"def\",\"$options\":\"m\"}")]
        public void WriteJson_should_have_expected_result_when_using_native_json_writer(string nullablePattern, string nullableOptions, string expectedResult)
        {
            var subject = new BsonRegularExpressionConverter();
            var value = nullablePattern == null ? null : new BsonRegularExpression(nullablePattern, nullableOptions);

            var result = WriteJsonUsingNativeJsonWriter(subject, value);

            result.Should().Be(expectedResult);
        }

        [TestCase(null, null, "{ x : null }")]
        [TestCase("abc", "i", "{ x : { $regex : \"abc\", $options : \"i\" } }")]
        [TestCase("def", "m", "{ x : { $regex : \"def\", $options : \"m\" } }")]
        public void WriteJson_should_have_expected_result_when_using_wrapped_bson_writer(string nullablePattern, string nullableOptions, string expectedResult)
        {
            var subject = new BsonRegularExpressionConverter();
            var value = nullablePattern == null ? null : new BsonRegularExpression(nullablePattern, nullableOptions);

            var result = WriteJsonUsingWrappedBsonWriter(subject, value, mustBeNested: true);

            result.Should().Equal(ToBson(expectedResult));
        }

        [TestCase(null, null, "null")]
        [TestCase("abc", "i", "/abc/i")]
        [TestCase("def", "m", "/def/m")]
        public void WriteJson_should_have_expected_result_when_using_wrapped_json_writer(string nullablePattern, string nullableOptions, string expectedResult)
        {
            var subject = new BsonRegularExpressionConverter();
            var value = nullablePattern == null ? null : new BsonRegularExpression(nullablePattern, nullableOptions);

            var result = WriteJsonUsingWrappedJsonWriter(subject, value);

            result.Should().Be(expectedResult);
        }
    }
}
