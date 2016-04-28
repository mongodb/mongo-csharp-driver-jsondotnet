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
    public class BsonTimestampConverterTests : JsonConverterTestsBase
    {
        [Test]
        public void Instance_get_returns_cached_result()
        {
            var result1 = BsonTimestampConverter.Instance;
            var result2 = BsonTimestampConverter.Instance;

            result2.Should().BeSameAs(result1);
        }

        [Test]
        public void Instance_get_returns_expected_result()
        {
            var result = BsonTimestampConverter.Instance;

            result.Should().NotBeNull();
            result.Should().BeOfType<BsonTimestampConverter>();
        }

        [TestCase("{ x : null }", null, null)]
        [TestCase("{ x : { $timestamp : { t : 1, i : 2 } } }", 1, 2)]
        [TestCase("{ x : { $timestamp : { t : 3, i : 4 } } }", 3, 4)]
        [TestCase("{ x : { $$timestamp : { t : 1, i : 2 } } }", 1, 2)]
        [TestCase("{ x : { $$timestamp : { t : 3, i : 4 } } }", 3, 4)]
        public void ReadJson_should_return_expected_result_when_using_native_bson_reader(string json, int? nullableTimestamp, int? nullableIncrement)
        {
            var subject = new BsonTimestampConverter();
            var expectedResult = nullableTimestamp == null ? null : new BsonTimestamp(nullableTimestamp.Value, nullableIncrement.Value);

            var result = ReadJsonUsingNativeBsonReader<BsonTimestamp>(subject, ToBson(json), mustBeNested: true);

            result.Should().Be(expectedResult);
        }

        [TestCase("null", null, null)]
        [TestCase("{ $timestamp : { t : 1, i : 2 } }", 1, 2)]
        [TestCase("{ $timestamp : { t : 3, i : 4 } }", 3, 4)]
        public void ReadJson_should_return_expected_result_when_using_native_json_reader(string json, int? nullableTimestamp, int? nullableIncrement)
        {
            var subject = new BsonTimestampConverter();
            var expectedResult = nullableTimestamp == null ? null : new BsonTimestamp(nullableTimestamp.Value, nullableIncrement.Value);

            var result = ReadJsonUsingNativeJsonReader<BsonTimestamp>(subject, json);

            result.Should().Be(expectedResult);
        }

        [TestCase("{ x : null }", null, null)]
        [TestCase("{ x : { $timestamp : { t : 1, i : 2 } } }", 1, 2)]
        [TestCase("{ x : { $timestamp : { t : 3, i : 4 } } }", 3, 4)]
        [TestCase("{ x : { $$timestamp : { t : 1, i : 2 } } }", 1, 2)]
        [TestCase("{ x : { $$timestamp : { t : 3, i : 4 } } }", 3, 4)]
        public void ReadJson_should_return_expected_result_when_using_wrapped_bson_reader(string json, int? nullableTimestamp, int? nullableIncrement)
        {
            var subject = new BsonTimestampConverter();
            var expectedResult = nullableTimestamp == null ? null : new BsonTimestamp(nullableTimestamp.Value, nullableIncrement.Value);

            var result = ReadJsonUsingWrappedBsonReader<BsonTimestamp>(subject, ToBson(json), mustBeNested: true);

            result.Should().Be(expectedResult);
        }

        [TestCase("null", null, null)]
        [TestCase("{ $timestamp : { t : 1, i : 2 } }", 1, 2)]
        [TestCase("{ $timestamp : { t : 3, i : 4 } }", 3, 4)]
        public void ReadJson_should_return_expected_result_when_using_wrapped_json_reader(string json, int? nullableTimestamp, int? nullableIncrement)
        {
            var subject = new BsonTimestampConverter();
            var expectedResult = nullableTimestamp == null ? null : new BsonTimestamp(nullableTimestamp.Value, nullableIncrement.Value);

            var result = ReadJsonUsingWrappedJsonReader<BsonTimestamp>(subject, json);

            result.Should().Be(expectedResult);
        }

        [Test]
        public void ReadJson_should_throw_when_token_type_is_invalid()
        {
            var subject = new BsonTimestampConverter();
            var json = "undefined";

            Action action = () => { var _ = ReadJsonUsingNativeJsonReader<BsonTimestamp>(subject, json); };

            action.ShouldThrow<Newtonsoft.Json.JsonReaderException>();
        }

        [TestCase(null, null, "{ x : null }")]
        [TestCase(1, 2, "{ x : { $$timestamp : { t : 1, i : 2 } } }")]
        [TestCase(3, 4, "{ x : { $$timestamp : { t : 3, i : 4 } } }")]
        public void WriteJson_should_have_expected_result_when_using_native_bson_writer(int? nullableTimestamp, int? nullableIncrement, string expectedResult)
        {
            var subject = new BsonTimestampConverter();
            var value = nullableTimestamp == null ? null : new BsonTimestamp(nullableTimestamp.Value, nullableIncrement.Value);

            var result = WriteJsonUsingNativeBsonWriter(subject, value, mustBeNested: true);

            result.Should().Equal(ToBson(expectedResult));
        }

        [TestCase(null, null, "null")]
        [TestCase(1, 2, "{\"$timestamp\":{\"t\":1,\"i\":2}}")]
        [TestCase(3, 4, "{\"$timestamp\":{\"t\":3,\"i\":4}}")]
        public void WriteJson_should_have_expected_result_when_using_native_json_writer(int? nullableTimestamp, int? nullableIncrement, string expectedResult)
        {
            var subject = new BsonTimestampConverter();
            var value = nullableTimestamp == null ? null : new BsonTimestamp(nullableTimestamp.Value, nullableIncrement.Value);

            var result = WriteJsonUsingNativeJsonWriter(subject, value);

            result.Should().Be(expectedResult);
        }

        [TestCase(null, null, "{ x : null }")]
        [TestCase(1, 2, "{ x : { $timestamp : { t : 1, i : 2 } } }")]
        [TestCase(3, 4, "{ x : { $timestamp : { t : 3, i : 4 } } }")]
        public void WriteJson_should_have_expected_result_when_using_wrapped_bson_writer(int? nullableTimestamp, int? nullableIncrement, string expectedResult)
        {
            var subject = new BsonTimestampConverter();
            var value = nullableTimestamp == null ? null : new BsonTimestamp(nullableTimestamp.Value, nullableIncrement.Value);

            var result = WriteJsonUsingWrappedBsonWriter(subject, value, mustBeNested: true);

            result.Should().Equal(ToBson(expectedResult));
        }

        [TestCase(null, null, "null")]
        [TestCase(1, 2, "Timestamp(1, 2)")]
        [TestCase(3, 4, "Timestamp(3, 4)")]
        public void WriteJson_should_have_expected_result_when_using_wrapped_json_writer(int? nullableTimestamp, int? nullableIncrement, string expectedResult)
        {
            var subject = new BsonTimestampConverter();
            var value = nullableTimestamp == null ? null : new BsonTimestamp(nullableTimestamp.Value, nullableIncrement.Value);

            var result = WriteJsonUsingWrappedJsonWriter(subject, value);

            result.Should().Be(expectedResult);
        }
    }
}
