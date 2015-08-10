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
    public class BsonNullConverterTests : JsonConverterTestsBase
    {
        [Test]
        public void Instance_get_returns_cached_result()
        {
            var result1 = BsonNullConverter.Instance;
            var result2 = BsonNullConverter.Instance;

            result2.Should().BeSameAs(result1);
        }

        [Test]
        public void Instance_get_returns_expected_result()
        {
            var result = BsonNullConverter.Instance;

            result.Should().NotBeNull();
            result.Should().BeOfType<BsonNullConverter>();
        }

        [TestCase("{ x : null }")]
        public void ReadJson_should_return_expected_result_when_using_native_bson_reader(string json)
        {
            var subject = new BsonNullConverter();

            var result = ReadJsonUsingNativeBsonReader<BsonNull>(subject, ToBson(json), mustBeNested: true);

            result.Should().Be(BsonNull.Value);
        }

        [TestCase("null")]
        public void ReadJson_should_return_expected_result_when_using_native_json_reader(string json)
        {
            var subject = new BsonNullConverter();

            var result = ReadJsonUsingNativeJsonReader<BsonNull>(subject, json);

            result.Should().Be(BsonNull.Value);
        }

        [TestCase("{ x : null }")]
        public void ReadJson_should_return_expected_result_when_using_wrapped_bson_reader(string json)
        {
            var subject = new BsonNullConverter();

            var result = ReadJsonUsingWrappedBsonReader<BsonNull>(subject, ToBson(json), mustBeNested: true);

            result.Should().Be(BsonNull.Value);
        }

        [TestCase("null")]
        public void ReadJson_should_return_expected_result_when_using_wrapped_json_reader(string json)
        {
            var subject = new BsonNullConverter();

            var result = ReadJsonUsingWrappedJsonReader<BsonNull>(subject, json);

            result.Should().Be(BsonNull.Value);
        }

        [Test]
        public void ReadJson_should_throw_when_token_type_is_invalid()
        {
            var subject = new BsonNullConverter();
            var json = "undefined";

            Action action = () => { var _ = ReadJsonUsingNativeJsonReader<BsonNull>(subject, json); };

            action.ShouldThrow<Newtonsoft.Json.JsonReaderException>();
        }

        [TestCase(null, "{ x : null }")] // TODO: should C# null have a different representation?
        [TestCase(true, "{ x : null }")]
        public void WriteJson_should_have_expected_result_when_using_native_bson_writer(bool? nullableNull, string expectedResult)
        {
            var subject = new BsonNullConverter();
            var value = nullableNull == null ? null : BsonNull.Value;

            var result = WriteJsonUsingNativeBsonWriter(subject, value, mustBeNested: true);

            result.Should().Equal(ToBson(expectedResult));
        }

        [TestCase(null, "null")] // TODO: should C# null have a different representation?
        [TestCase(true, "null")]
        public void WriteJson_should_have_expected_result_when_using_native_json_writer(bool? nullableNull, string expectedResult)
        {
            var subject = new BsonNullConverter();
            var value = nullableNull == null ? null : BsonNull.Value;

            var result = WriteJsonUsingNativeJsonWriter(subject, value);

            result.Should().Be(expectedResult);
        }

        [TestCase(null, "{ x : null }")] // TODO: should C# null have a different representation?
        [TestCase(true, "{ x : null }")]
        public void WriteJson_should_have_expected_result_when_using_wrapped_bson_writer(bool? nullableNull, string expectedResult)
        {
            var subject = new BsonNullConverter();
            var value = nullableNull == null ? null : BsonNull.Value;

            var result = WriteJsonUsingWrappedBsonWriter(subject, value, mustBeNested: true);

            result.Should().Equal(ToBson(expectedResult));
        }

        [TestCase(null, "null")] // TODO: should C# null have a different representation?
        [TestCase(true, "null")]
        public void WriteJson_should_have_expected_result_when_using_wrapped_json_writer(bool? nullableNull, string expectedResult)
        {
            var subject = new BsonNullConverter();
            var value = nullableNull == null ? null : BsonNull.Value;

            var result = WriteJsonUsingWrappedJsonWriter(subject, value);

            result.Should().Be(expectedResult);
        }
    }
}
