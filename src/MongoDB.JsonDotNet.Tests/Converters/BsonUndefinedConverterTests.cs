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
    public class BsonUndefinedConverterTests : JsonConverterTestsBase
    {
        [Test]
        public void Instance_get_returns_cached_result()
        {
            var result1 = BsonUndefinedConverter.Instance;
            var result2 = BsonUndefinedConverter.Instance;

            result2.Should().BeSameAs(result1);
        }

        [Test]
        public void Instance_get_returns_expected_result()
        {
            var result = BsonUndefinedConverter.Instance;

            result.Should().NotBeNull();
            result.Should().BeOfType<BsonUndefinedConverter>();
        }

        [TestCase("{ x : null }", null)]
        [TestCase("{ x : { $$undefined : true } }", true)]
        public void ReadJson_should_return_expected_result_when_using_native_bson_reader(string json, bool? nullableUndefined)
        {
            var subject = new BsonUndefinedConverter();
            var expectedResult = nullableUndefined == null ? null : BsonUndefined.Value;

            var result = ReadJsonUsingNativeBsonReader<BsonUndefined>(subject, ToBson(json), mustBeNested: true);

            result.Should().Be(expectedResult);
        }

        [TestCase("null", null)]
        [TestCase("{ $undefined : true }", true)]
        public void ReadJson_should_return_expected_result_when_using_native_json_reader(string json, bool? nullableUndefined)
        {
            var subject = new BsonUndefinedConverter();
            var expectedResult = nullableUndefined == null ? null : BsonUndefined.Value;

            var result = ReadJsonUsingNativeJsonReader<BsonUndefined>(subject, json);

            result.Should().Be(expectedResult);
        }

        [TestCase("{ x : null }", null)]
        [TestCase("{ x : { $undefined : true } }", true)]
        [TestCase("{ x : { $$undefined : true } }", true)]
        public void ReadJson_should_return_expected_result_when_using_wrapped_bson_reader(string json, bool? nullableUndefined)
        {
            var subject = new BsonUndefinedConverter();
            var expectedResult = nullableUndefined == null ? null : BsonUndefined.Value;

            var result = ReadJsonUsingWrappedBsonReader<BsonUndefined>(subject, ToBson(json), mustBeNested: true);

            result.Should().Be(expectedResult);
        }

        [TestCase("null", null)]
        [TestCase("{ $undefined : true }", true)]
        public void ReadJson_should_return_expected_result_when_using_wrapped_json_reader(string json, bool? nullableUndefined)
        {
            var subject = new BsonUndefinedConverter();
            var expectedResult = nullableUndefined == null ? null : BsonUndefined.Value;

            var result = ReadJsonUsingWrappedJsonReader<BsonUndefined>(subject, json);

            result.Should().Be(expectedResult);
        }

        [Test]
        public void ReadJson_should_throw_when_token_type_is_invalid()
        {
            var subject = new BsonUndefinedConverter();
            var json = "1";

            Action action = () => { var _ = ReadJsonUsingNativeJsonReader<BsonUndefined>(subject, json); };

            action.ShouldThrow<Newtonsoft.Json.JsonReaderException>();
        }

        [TestCase(null, "{ x : null }")]
        [TestCase(true, "{ x : undefined }")]
        public void WriteJson_should_have_expected_result_when_using_native_bson_writer(bool? nullableUndefined, string expectedResult)
        {
            var subject = new BsonUndefinedConverter();
            var value = nullableUndefined == null ? null : BsonUndefined.Value;

            var result = WriteJsonUsingNativeBsonWriter(subject, value, mustBeNested: true);

            result.Should().Equal(ToBson(expectedResult));
        }

        [TestCase(null, "null")]
        [TestCase(true, "undefined")]
        public void WriteJson_should_have_expected_result_when_using_native_json_writer(bool? nullableUndefined, string expectedResult)
        {
            var subject = new BsonUndefinedConverter();
            var value = nullableUndefined == null ? null : BsonUndefined.Value;

            var result = WriteJsonUsingNativeJsonWriter(subject, value);

            result.Should().Be(expectedResult);
        }

        [TestCase(null, "{ x : null }")]
        [TestCase(true, "{ x : { $undefined : true } }")]
        public void WriteJson_should_have_expected_result_when_using_wrapped_bson_writer(bool? nullableUndefined, string expectedResult)
        {
            var subject = new BsonUndefinedConverter();
            var value = nullableUndefined == null ? null : BsonUndefined.Value;

            var result = WriteJsonUsingWrappedBsonWriter(subject, value, mustBeNested: true);

            result.Should().Equal(ToBson(expectedResult));
        }

        [TestCase(null, "null")]
        [TestCase(true, "undefined")]
        public void WriteJson_should_have_expected_result_when_using_wrapped_json_writer(bool? nullableUndefined, string expectedResult)
        {
            var subject = new BsonUndefinedConverter();
            var value = nullableUndefined == null ? null : BsonUndefined.Value;

            var result = WriteJsonUsingWrappedJsonWriter(subject, value);

            result.Should().Be(expectedResult);
        }
    }
}
