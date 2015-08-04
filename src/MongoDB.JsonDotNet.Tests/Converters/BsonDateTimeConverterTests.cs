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
    public class BsonDateTimeConverterTests : JsonConverterTestsBase
    {
        [Test]
        public void Instance_get_returns_cached_result()
        {
            var result1 = BsonDateTimeConverter.Instance;
            var result2 = BsonDateTimeConverter.Instance;

            result2.Should().BeSameAs(result1);
        }

        [Test]
        public void Instance_get_returns_expected_result()
        {
            var result = BsonDateTimeConverter.Instance;

            result.Should().NotBeNull();
            result.Should().BeOfType<BsonDateTimeConverter>();
        }

        [TestCase("{ x : null }", null)]
        [TestCase("{ x : { $date : \"1970-01-01T00:00:00.000Z\" } }", 0L)]
        [TestCase("{ x : { $date : \"1970-01-01T00:00:00.001Z\" } }", 1L)]
        [TestCase("{ x : { $date : \"1969-12-31T23:59:59.999Z\" } }", -1L)]
        [TestCase("{ x : { $$date : \"1970-01-01T00:00:00.000Z\" } }", 0L)]
        [TestCase("{ x : { $$date : \"1970-01-01T00:00:00.001Z\" } }", 1L)]
        [TestCase("{ x : { $$date : \"1969-12-31T23:59:59.999Z\" } }", -1L)]
        [TestCase("{ x : { $$date : { $$numberLong : \"9223372036854775807\" } } }", long.MaxValue)]
        public void ReadJson_should_return_expected_result_when_using_native_bson_reader(string json, long? nullableMillisecondsSinceEpoch)
        {
            var subject = new BsonDateTimeConverter();
            var expectedResult = nullableMillisecondsSinceEpoch == null ? null : new BsonDateTime(nullableMillisecondsSinceEpoch.Value);

            var result = ReadJsonUsingNativeBsonReader<BsonDateTime>(subject, ToBson(json), mustBeNested: true);

            result.Should().Be(expectedResult);
        }

        [TestCase("null", null)]
        [TestCase("\"1970-01-01T00:00:00.000Z\"", 0L)]
        [TestCase("\"1970-01-01T00:00:00.001Z\"", 1L)]
        [TestCase("\"1969-12-31T23:59:59.999Z\"", -1L)]
        [TestCase("{ $date : \"1970-01-01T00:00:00.000Z\" }", 0L)]
        [TestCase("{ $date : \"1970-01-01T00:00:00.001Z\" }", 1L)]
        [TestCase("{ $date : \"1969-12-31T23:59:59.999Z\" }", -1L)]
        [TestCase("{ $date : { $numberLong : \"9223372036854775807\" } } }", long.MaxValue)]
        public void ReadJson_should_return_expected_result_when_using_native_json_reader(string json, long? nullableMillisecondsSinceEpoch)
        {
            var subject = new BsonDateTimeConverter();
            var expectedResult = nullableMillisecondsSinceEpoch == null ? null : new BsonDateTime(nullableMillisecondsSinceEpoch.Value);

            var result = ReadJsonUsingNativeJsonReader<BsonDateTime>(subject, json);

            result.Should().Be(expectedResult);
        }

        [TestCase("{ x : null }", null)]
        [TestCase("{ x : { $date : \"1970-01-01T00:00:00.000Z\" } }", 0L)]
        [TestCase("{ x : { $date : \"1970-01-01T00:00:00.001Z\" } }", 1L)]
        [TestCase("{ x : { $date : \"1969-12-31T23:59:59.999Z\" } }", -1L)]
        [TestCase("{ x : { $$date : \"1970-01-01T00:00:00.000Z\" } }", 0L)]
        [TestCase("{ x : { $$date : \"1970-01-01T00:00:00.001Z\" } }", 1L)]
        [TestCase("{ x : { $$date : \"1969-12-31T23:59:59.999Z\" } }", -1L)]
        [TestCase("{ x : { $$date : { $$numberLong : \"9223372036854775807\" } } }", long.MaxValue)]
        public void ReadJson_should_return_expected_result_when_using_wrapped_bson_reader(string json, long? nullableMillisecondsSinceEpoch)
        {
            var subject = new BsonDateTimeConverter();
            var expectedResult = nullableMillisecondsSinceEpoch == null ? null : new BsonDateTime(nullableMillisecondsSinceEpoch.Value);

            var result = ReadJsonUsingWrappedBsonReader<BsonDateTime>(subject, ToBson(json), mustBeNested: true);

            result.Should().Be(expectedResult);
        }

        [TestCase("null", null)]
        [TestCase("{ $date : \"1970-01-01T00:00:00.000Z\" }", 0L)]
        [TestCase("{ $date : \"1970-01-01T00:00:00.001Z\" }", 1L)]
        [TestCase("{ $date : \"1969-12-31T23:59:59.999Z\" }", -1L)]
        public void ReadJson_should_return_expected_result_when_using_wrapped_json_reader(string json, long? nullableMillisecondsSinceEpoch)
        {
            var subject = new BsonDateTimeConverter();
            var expectedResult = nullableMillisecondsSinceEpoch == null ? null : new BsonDateTime(nullableMillisecondsSinceEpoch.Value);

            var result = ReadJsonUsingWrappedJsonReader<BsonDateTime>(subject, json);

            result.Should().Be(expectedResult);
        }

        [Test]
        public void ReadJson_should_throw_when_token_type_is_invalid()
        {
            var subject = new BsonDateTimeConverter();
            var json = "undefined";

            Action action = () => { var _ = ReadJsonUsingNativeJsonReader<BsonDateTime>(subject, json); };

            action.ShouldThrow<Newtonsoft.Json.JsonReaderException>();
        }

        [TestCase(null, "{ x : null }")]
        [TestCase(0L, "{ x : { $date : \"1970-01-01T00:00:00.000Z\" } }")]
        [TestCase(1L, "{ x : { $date : \"1970-01-01T00:00:00.001Z\" } }")]
        [TestCase(-1L, "{ x : { $date : \"1969-12-31T23:59:59.999Z\" } }")]
        [TestCase(long.MaxValue, "{ x : { $$date : { $$numberLong : \"9223372036854775807\" } } }")]
        public void WriteJson_should_have_expected_result_when_using_native_bson_writer(long? nullableMillisecondsSinceEpoch, string expectedResult)
        {
            var subject = new BsonDateTimeConverter();
            var value = nullableMillisecondsSinceEpoch == null ? null : new BsonDateTime(nullableMillisecondsSinceEpoch.Value);

            var result = WriteJsonUsingNativeBsonWriter(subject, value, mustBeNested: true);

            result.Should().Equal(ToBson(expectedResult));
        }

        [TestCase(null, "null")]
        [TestCase(0L, "\"1970-01-01T00:00:00Z\"")] // note: Json.NET writes BsonDateTime as string
        [TestCase(1L, "\"1970-01-01T00:00:00.001Z\"")] // note: Json.NET writes BsonDateTime as string
        [TestCase(-1L, "\"1969-12-31T23:59:59.999Z\"")] // note: Json.NET writes BsonDateTime as string
        [TestCase(long.MaxValue, "{\"$date\":{\"$numberLong\":\"9223372036854775807\"}}")]
        public void WriteJson_should_have_expected_result_when_using_native_json_writer(long? nullableMillisecondsSinceEpoch, string expectedResult)
        {
            var subject = new BsonDateTimeConverter();
            var value = nullableMillisecondsSinceEpoch == null ? null : new BsonDateTime(nullableMillisecondsSinceEpoch.Value);

            var result = WriteJsonUsingNativeJsonWriter(subject, value);

            result.Should().Be(expectedResult);
        }

        [TestCase(null, "{ x : null }")]
        [TestCase(0L, "{ x : { $date : \"1970-01-01T00:00:00.000Z\" } }")]
        [TestCase(1L, "{ x : { $date : \"1970-01-01T00:00:00.001Z\" } }")]
        [TestCase(-1L, "{ x : { $date : \"1969-12-31T23:59:59.999Z\" } }")]
        [TestCase(long.MaxValue, "{ x : { $date : 9223372036854775807 } }")]
        public void WriteJson_should_have_expected_result_when_using_wrapped_bson_writer(long? nullableMillisecondsSinceEpoch, string expectedResult)
        {
            var subject = new BsonDateTimeConverter();
            var value = nullableMillisecondsSinceEpoch == null ? null : new BsonDateTime(nullableMillisecondsSinceEpoch.Value);

            var result = WriteJsonUsingWrappedBsonWriter(subject, value, mustBeNested: true);

            result.Should().Equal(ToBson(expectedResult));
        }

        [TestCase(null, "null")]
        [TestCase(0L,  "ISODate(\"1970-01-01T00:00:00Z\")")]
        [TestCase(1L,  "ISODate(\"1970-01-01T00:00:00.001Z\")")]
        [TestCase(-1L, "ISODate(\"1969-12-31T23:59:59.999Z\")")]
        [TestCase(long.MaxValue, "new Date(9223372036854775807)")]
        public void WriteJson_should_have_expected_result_when_using_wrapped_json_writer(long? nullableMillisecondsSinceEpoch, string expectedResult)
        {
            var subject = new BsonDateTimeConverter();
            var value = nullableMillisecondsSinceEpoch == null ? null : new BsonDateTime(nullableMillisecondsSinceEpoch.Value);

            var result = WriteJsonUsingWrappedJsonWriter(subject, value);

            result.Should().Be(expectedResult);
        }
    }
}
