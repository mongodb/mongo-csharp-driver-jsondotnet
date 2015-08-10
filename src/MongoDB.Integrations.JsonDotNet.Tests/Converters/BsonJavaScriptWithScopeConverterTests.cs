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
    public class BsonJavaScriptWithScopeConverterTests : JsonConverterTestsBase
    {
        [Test]
        public void Instance_get_returns_cached_result()
        {
            var result1 = BsonJavaScriptWithScopeConverter.Instance;
            var result2 = BsonJavaScriptWithScopeConverter.Instance;

            result2.Should().BeSameAs(result1);
        }

        [Test]
        public void Instance_get_returns_expected_result()
        {
            var result = BsonJavaScriptWithScopeConverter.Instance;

            result.Should().NotBeNull();
            result.Should().BeOfType<BsonJavaScriptWithScopeConverter>();
        }

        [TestCase("{ x : null }", null, null)]
        [TestCase("{ x : { $code : \"abc\", $scope : { x : 1 } } }", "abc", "{ x : 1 }")]
        [TestCase("{ x : { $code : \"def\", $scope : { x : 2 } } }", "def", "{ x : 2 }")]
        [TestCase("{ x : { $$code : \"abc\", $scope : { x : 1 } } }", "abc", "{ x : 1 }")]
        [TestCase("{ x : { $$code : \"def\", $scope : { x : 2 } } }", "def", "{ x : 2 }")]
        public void ReadJson_should_return_expected_result_when_using_native_bson_reader(string json, string nullableCode, string nullableScope)
        {
            var subject = new BsonJavaScriptWithScopeConverter();
            var expectedResult = nullableCode == null ? null : new BsonJavaScriptWithScope(nullableCode, BsonDocument.Parse(nullableScope));

            var result = ReadJsonUsingNativeBsonReader<BsonJavaScriptWithScope>(subject, ToBson(json), mustBeNested: true);

            result.Should().Be(expectedResult);
        }

        [TestCase("null", null, null)]
        [TestCase("{ $code : \"abc\", $scope : { x : 1 } }", "abc", "{ x : 1 }")]
        [TestCase("{ $code : \"def\", $scope : { x : 2 } }", "def", "{ x : 2 }")]
        public void ReadJson_should_return_expected_result_when_using_native_json_reader(string json, string nullableCode, string nullableScope)
        {
            var subject = new BsonJavaScriptWithScopeConverter();
            var expectedResult = nullableCode == null ? null : new BsonJavaScriptWithScope(nullableCode, BsonDocument.Parse(nullableScope));

            var result = ReadJsonUsingNativeJsonReader<BsonJavaScriptWithScope>(subject, json);

            result.Should().Be(expectedResult);
        }

        [TestCase("{ x : null }", null, null)]
        [TestCase("{ x : { $code : \"abc\", $scope : { x : 1 } } }", "abc", "{ x : 1 }")]
        [TestCase("{ x : { $code : \"def\", $scope : { x : 2 } } }", "def", "{ x : 2 }")]
        [TestCase("{ x : { $$code : \"abc\", $scope : { x : 1 } } }", "abc", "{ x : 1 }")]
        [TestCase("{ x : { $$code : \"def\", $scope : { x : 2 } } }", "def", "{ x : 2 }")]
        public void ReadJson_should_return_expected_result_when_using_wrapped_bson_reader(string json, string nullableCode, string nullableScope)
        {
            var subject = new BsonJavaScriptWithScopeConverter();
            var expectedResult = nullableCode == null ? null : new BsonJavaScriptWithScope(nullableCode, BsonDocument.Parse(nullableScope));

            var result = ReadJsonUsingWrappedBsonReader<BsonJavaScriptWithScope>(subject, ToBson(json), mustBeNested: true);

            result.Should().Be(expectedResult);
        }

        [TestCase("null", null, null)]
        [TestCase("{ $code : \"abc\", $scope : { x : 1 } }", "abc", "{ x : 1 }")]
        [TestCase("{ $code : \"def\", $scope : { x : 2 } }", "def", "{ x : 2 }")]
        public void ReadJson_should_return_expected_result_when_using_wrapped_json_reader(string json, string nullableCode, string nullableScope)
        {
            var subject = new BsonJavaScriptWithScopeConverter();
            var expectedResult = nullableCode == null ? null : new BsonJavaScriptWithScope(nullableCode, BsonDocument.Parse(nullableScope));

            var result = ReadJsonUsingWrappedJsonReader<BsonJavaScriptWithScope>(subject, json);

            result.Should().Be(expectedResult);
        }

        [Test]
        public void ReadJson_should_throw_when_token_type_is_invalid()
        {
            var subject = new BsonJavaScriptWithScopeConverter();
            var json = "undefined";

            Action action = () => { var _ = ReadJsonUsingNativeJsonReader<BsonJavaScriptWithScope>(subject, json); };

            action.ShouldThrow<Newtonsoft.Json.JsonReaderException>();
        }

        [TestCase(null, null, "{ x : null }")]
        [TestCase("abc", "{ x : 1 }", "{ x : { $$code : \"abc\", $$scope : { x : NumberLong(1) } } }")]
        [TestCase("def", "{ x : 2 }", "{ x : { $$code : \"def\", $$scope : { x : NumberLong(2) } } }")]
        public void WriteJson_should_have_expected_result_when_using_native_bson_writer(string nullableCode, string nullableScope, string expectedResult)
        {
            var subject = new BsonJavaScriptWithScopeConverter();
            var value = nullableCode == null ? null : new BsonJavaScriptWithScope(nullableCode, BsonDocument.Parse(nullableScope));

            var result = WriteJsonUsingNativeBsonWriter(subject, value, mustBeNested: true);

            result.Should().Equal(ToBson(expectedResult));
        }

        [TestCase(null, null, "null")]
        [TestCase("abc", "{ x : 1 }", "{\"$code\":\"abc\",\"$scope\":{\"x\":1}}")]
        [TestCase("def", "{ x : 2 }", "{\"$code\":\"def\",\"$scope\":{\"x\":2}}")]
        public void WriteJson_should_have_expected_result_when_using_native_json_writer(string nullableCode, string nullableScope, string expectedResult)
        {
            var subject = new BsonJavaScriptWithScopeConverter();
            var value = nullableCode == null ? null : new BsonJavaScriptWithScope(nullableCode, BsonDocument.Parse(nullableScope));

            var result = WriteJsonUsingNativeJsonWriter(subject, value);

            result.Should().Be(expectedResult);
        }

        [TestCase(null, null, "{ x : null }")]
        [TestCase("abc", "{ x : 1 }", "{ x : { $code : \"abc\", $scope : { x : 1 } } }")]
        [TestCase("def", "{ x : 2 }", "{ x : { $code : \"def\", $scope : { x : 2 } } }")]
        public void WriteJson_should_have_expected_result_when_using_wrapped_bson_writer(string nullableCode, string nullableScope, string expectedResult)
        {
            var subject = new BsonJavaScriptWithScopeConverter();
            var value = nullableCode == null ? null : new BsonJavaScriptWithScope(nullableCode, BsonDocument.Parse(nullableScope));

            var result = WriteJsonUsingWrappedBsonWriter(subject, value, mustBeNested: true);

            result.Should().Equal(ToBson(expectedResult));
        }

        [TestCase(null, null, "null")]
        [TestCase("abc", "{ x : 1 }", "{ \"$code\" : \"abc\", \"$scope\" : { \"x\" : 1 } }")]
        [TestCase("def", "{ x : 2 }", "{ \"$code\" : \"def\", \"$scope\" : { \"x\" : 2 } }")]
        public void WriteJson_should_have_expected_result_when_using_wrapped_json_writer(string nullableCode, string nullableScope, string expectedResult)
        {
            var subject = new BsonJavaScriptWithScopeConverter();
            var value = nullableCode == null ? null : new BsonJavaScriptWithScope(nullableCode, BsonDocument.Parse(nullableScope));

            var result = WriteJsonUsingWrappedJsonWriter(subject, value);

            result.Should().Be(expectedResult);
        }
    }
}
