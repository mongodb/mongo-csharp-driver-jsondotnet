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
    public class BsonDocumentConverterTests : JsonConverterTestsBase
    {
        [Test]
        public void Instance_get_returns_cached_result()
        {
            var result1 = BsonDocumentConverter.Instance;
            var result2 = BsonDocumentConverter.Instance;

            result2.Should().BeSameAs(result1);
        }

        [Test]
        public void Instance_get_returns_expected_result()
        {
            var result = BsonDocumentConverter.Instance;

            result.Should().NotBeNull();
            result.Should().BeOfType<BsonDocumentConverter>();
        }

        [TestCase("{ x : null }", null)]
        [TestCase("{ x : { } }", "{ }")]
        [TestCase("{ x : { x : 1 } }", "{ x : 1 }")]
        [TestCase("{ x : { x : 1, y : \"abc\" } }", "{ x : 1, y : \"abc\" }")]
        public void ReadJson_should_return_expected_result_when_using_native_bson_reader(string json, string nullableDocument)
        {
            var subject = new BsonDocumentConverter();
            var expectedResult = nullableDocument == null ? null : BsonDocument.Parse(nullableDocument);

            var result = ReadJsonUsingNativeBsonReader<BsonDocument>(subject, ToBson(json), mustBeNested: true);

            result.Should().Be(expectedResult);
        }

        [TestCase("null", null)]
        [TestCase("{ }", "{ }")]
        [TestCase("{ x : 1 }", "{ x : 1 }")]
        [TestCase("{ x : 1, y : \"abc\" }", "{ x : 1, y : \"abc\" }")]
        public void ReadJson_should_return_expected_result_when_using_native_json_reader(string json, string nullableDocument)
        {
            var subject = new BsonDocumentConverter();
            var expectedResult = nullableDocument == null ? null : BsonDocument.Parse(nullableDocument);

            var result = ReadJsonUsingNativeJsonReader<BsonDocument>(subject, json);

            result.Should().Be(expectedResult);
        }

        [TestCase("{ x : null }", null)]
        [TestCase("{ x : { } }", "{ }")]
        [TestCase("{ x : { x : 1 } }", "{ x : 1 }")]
        [TestCase("{ x : { x : 1, y : \"abc\" } }", "{ x : 1, y : \"abc\" }")]
        public void ReadJson_should_return_expected_result_when_using_wrapped_bson_reader(string json, string nullableDocument)
        {
            var subject = new BsonDocumentConverter();
            var expectedResult = nullableDocument == null ? null : BsonDocument.Parse(nullableDocument);

            var result = ReadJsonUsingWrappedBsonReader<BsonDocument>(subject, ToBson(json), mustBeNested: true);

            result.Should().Be(expectedResult);
        }

        [TestCase("null", null)]
        [TestCase("{ }", "{ }")]
        [TestCase("{ x : 1 }", "{ x : 1 }")]
        [TestCase("{ x : 1, y : \"abc\" }", "{ x : 1, y : \"abc\" }")]
        public void ReadJson_should_return_expected_result_when_using_wrapped_json_reader(string json, string nullableDocument)
        {
            var subject = new BsonDocumentConverter();
            var expectedResult = nullableDocument == null ? null : BsonDocument.Parse(nullableDocument);

            var result = ReadJsonUsingWrappedJsonReader<BsonDocument>(subject, json);

            result.Should().Be(expectedResult);
        }

        [Test]
        public void ReadJson_should_throw_when_token_type_is_invalid()
        {
            var subject = new BsonDocumentConverter();
            var json = "undefined";

            Action action = () => { var _ = ReadJsonUsingNativeJsonReader<BsonDocument>(subject, json); };

            action.ShouldThrow<Newtonsoft.Json.JsonReaderException>();
        }

        [TestCase(null, "{ x : null }")]
        [TestCase("{ }", "{ x : { } }")]
        [TestCase("{ x : 1 }", "{ x : { x : NumberLong(1) } }")]
        [TestCase("{ x : 1, y : \"abc\" }", "{ x : { x : NumberLong(1), y : \"abc\" } }")]
        public void WriteJson_should_have_expected_result_when_using_native_bson_writer(string nullableDocument, string expectedResult)
        {
            var subject = new BsonDocumentConverter();
            var value = nullableDocument == null ? null : BsonDocument.Parse(nullableDocument);

            var result = WriteJsonUsingNativeBsonWriter(subject, value, mustBeNested: true);

            result.Should().Equal(ToBson(expectedResult));
        }

        [TestCase(null, "null")]
        [TestCase("{ }", "{}")]
        [TestCase("{ x : 1 }", "{\"x\":1}")]
        [TestCase("{ x : 1, y : \"abc\" }", "{\"x\":1,\"y\":\"abc\"}")]
        public void WriteJson_should_have_expected_result_when_using_native_json_writer(string nullableDocument, string expectedResult)
        {
            var subject = new BsonDocumentConverter();
            var value = nullableDocument == null ? null : BsonDocument.Parse(nullableDocument);

            var result = WriteJsonUsingNativeJsonWriter(subject, value);

            result.Should().Be(expectedResult);
        }

        [TestCase(null, "{ x : null }")]
        [TestCase("{ }", "{ x : { } }")]
        [TestCase("{ x : 1 }", "{ x : { x : 1 } }")]
        [TestCase("{ x : 1, y : \"abc\" }", "{ x : { x : 1, y : \"abc\" } }")]
        public void WriteJson_should_have_expected_result_when_using_wrapped_bson_writer(string nullableDocument, string expectedResult)
        {
            var subject = new BsonDocumentConverter();
            var value = nullableDocument == null ? null : BsonDocument.Parse(nullableDocument);

            var result = WriteJsonUsingWrappedBsonWriter(subject, value, mustBeNested: true);

            result.Should().Equal(ToBson(expectedResult));
        }

        [TestCase(null, "null")]
        [TestCase("{ }", "{ }")]
        [TestCase("{ x : 1 }", "{ \"x\" : 1 }")]
        [TestCase("{ x : 1, y : \"abc\" }", "{ \"x\" : 1, \"y\" : \"abc\" }")]
        public void WriteJson_should_have_expected_result_when_using_wrapped_json_writer(string nullableDocument, string expectedResult)
        {
            var subject = new BsonDocumentConverter();
            var value = nullableDocument == null ? null : BsonDocument.Parse(nullableDocument);

            var result = WriteJsonUsingWrappedJsonWriter(subject, value);

            result.Should().Be(expectedResult);
        }
    }
}
