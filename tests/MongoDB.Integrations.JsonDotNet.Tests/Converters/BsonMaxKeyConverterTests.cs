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
    public class BsonMaxKeyConverterTests : JsonConverterTestsBase
    {
        [Test]
        public void Instance_get_returns_cached_result()
        {
            var result1 = BsonMaxKeyConverter.Instance;
            var result2 = BsonMaxKeyConverter.Instance;

            result2.Should().BeSameAs(result1);
        }

        [Test]
        public void Instance_get_returns_expected_result()
        {
            var result = BsonMaxKeyConverter.Instance;

            result.Should().NotBeNull();
            result.Should().BeOfType<BsonMaxKeyConverter>();
        }

        [TestCase("{ x : null }", null)]
        [TestCase("{ x : { $$maxKey : 1 } }", true)]
        public void ReadJson_should_return_expected_result_when_using_native_bson_reader(string json, bool? nullableMaxKey)
        {
            var subject = new BsonMaxKeyConverter();
            var expectedResult = nullableMaxKey == null ? null : BsonMaxKey.Value;

            var result = ReadJsonUsingNativeBsonReader<BsonMaxKey>(subject, ToBson(json), mustBeNested: true);

            result.Should().Be(expectedResult);
        }

        [TestCase("null", null)]
        [TestCase("{ $maxKey : 1 }", true)]
        public void ReadJson_should_return_expected_result_when_using_native_json_reader(string json, bool? nullableMaxKey)
        {
            var subject = new BsonMaxKeyConverter();
            var expectedResult = nullableMaxKey == null ? null : BsonMaxKey.Value;

            var result = ReadJsonUsingNativeJsonReader<BsonMaxKey>(subject, json);

            result.Should().Be(expectedResult);
        }

        [TestCase("{ x : null }", null)]
        [TestCase("{ x : { $maxKey : 1 } }", true)]
        [TestCase("{ x : { $$maxKey : 1 } }", true)]
        public void ReadJson_should_return_expected_result_when_using_wrapped_bson_reader(string json, bool? nullableMaxKey)
        {
            var subject = new BsonMaxKeyConverter();
            var expectedResult = nullableMaxKey == null ? null : BsonMaxKey.Value;

            var result = ReadJsonUsingWrappedBsonReader<BsonMaxKey>(subject, ToBson(json), mustBeNested: true);

            result.Should().Be(expectedResult);
        }

        [TestCase("null", null)]
        [TestCase("{ $maxKey : 1 }", true)]
        public void ReadJson_should_return_expected_result_when_using_wrapped_json_reader(string json, bool? nullableMaxKey)
        {
            var subject = new BsonMaxKeyConverter();
            var expectedResult = nullableMaxKey == null ? null : BsonMaxKey.Value;

            var result = ReadJsonUsingWrappedJsonReader<BsonMaxKey>(subject, json);

            result.Should().Be(expectedResult);
        }

        [Test]
        public void ReadJson_should_throw_when_token_type_is_invalid()
        {
            var subject = new BsonMaxKeyConverter();
            var json = "undefined";

            Action action = () => { var _ = ReadJsonUsingNativeJsonReader<BsonMaxKey>(subject, json); };

            action.ShouldThrow<Newtonsoft.Json.JsonReaderException>();
        }

        [TestCase(null, "{ x : null }")]
        [TestCase(true, "{ x : { $$maxKey : 1 } }")]
        public void WriteJson_should_have_expected_result_when_using_native_bson_writer(bool? nullableMaxKey, string expectedResult)
        {
            var subject = new BsonMaxKeyConverter();
            var value = nullableMaxKey == null ? null : BsonMaxKey.Value;

            var result = WriteJsonUsingNativeBsonWriter(subject, value, mustBeNested: true);

            result.Should().Equal(ToBson(expectedResult));
        }

        [TestCase(null, "null")]
        [TestCase(true, "{\"$maxKey\":1}")]
        public void WriteJson_should_have_expected_result_when_using_native_json_writer(bool? nullableMaxKey, string expectedResult)
        {
            var subject = new BsonMaxKeyConverter();
            var value = nullableMaxKey == null ? null : BsonMaxKey.Value;

            var result = WriteJsonUsingNativeJsonWriter(subject, value);

            result.Should().Be(expectedResult);
        }

        [TestCase(null, "{ x : null }")]
        [TestCase(true, "{ x : { $maxKey : 1 } }")]
        public void WriteJson_should_have_expected_result_when_using_wrapped_bson_writer(bool? nullableMaxKey, string expectedResult)
        {
            var subject = new BsonMaxKeyConverter();
            var value = nullableMaxKey == null ? null : BsonMaxKey.Value;

            var result = WriteJsonUsingWrappedBsonWriter(subject, value, mustBeNested: true);

            result.Should().Equal(ToBson(expectedResult));
        }

        [TestCase(null, "null")]
        [TestCase(true, "MaxKey")]
        public void WriteJson_should_have_expected_result_when_using_wrapped_json_writer(bool? nullableMaxKey, string expectedResult)
        {
            var subject = new BsonMaxKeyConverter();
            var value = nullableMaxKey == null ? null : BsonMaxKey.Value;

            var result = WriteJsonUsingWrappedJsonWriter(subject, value);

            result.Should().Be(expectedResult);
        }
    }
}
