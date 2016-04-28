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
    public class ObjectIdConverterTests : JsonConverterTestsBase
    {
        [Test]
        public void Instance_get_returns_cached_result()
        {
            var result1 = ObjectIdConverter.Instance;
            var result2 = ObjectIdConverter.Instance;

            result2.Should().BeSameAs(result1);
        }

        [Test]
        public void Instance_get_returns_expected_result()
        {
            var result = ObjectIdConverter.Instance;

            result.Should().NotBeNull();
            result.Should().BeOfType<ObjectIdConverter>();
        }

        [TestCase("{ x : { $oid : \"112233445566778899aabbcc\" } }", "112233445566778899aabbcc")]
        [TestCase("{ x : { $oid : \"2233445566778899aabbccdd\" } }", "2233445566778899aabbccdd")]
        [TestCase("{ x : { $$oid : \"112233445566778899aabbcc\" } }", "112233445566778899aabbcc")]
        [TestCase("{ x : { $$oid : \"2233445566778899aabbccdd\" } }", "2233445566778899aabbccdd")]
        public void ReadJson_should_return_expected_result_when_using_native_bson_reader(string json, string hexValue)
        {
            var subject = new ObjectIdConverter();
            var expectedResult = ObjectId.Parse(hexValue);

            var result = ReadJsonUsingNativeBsonReader<ObjectId>(subject, ToBson(json), mustBeNested: true);

            result.Should().Be(expectedResult);
        }

        [TestCase("{ $oid : \"112233445566778899aabbcc\" }", "112233445566778899aabbcc")]
        [TestCase("{ $oid : \"2233445566778899aabbccdd\" }", "2233445566778899aabbccdd")]
        public void ReadJson_should_return_expected_result_when_using_native_json_reader(string json, string hexValue)
        {
            var subject = new ObjectIdConverter();
            var expectedResult = ObjectId.Parse(hexValue);

            var result = ReadJsonUsingNativeJsonReader<ObjectId>(subject, json);

            result.Should().Be(expectedResult);
        }

        [TestCase("{ x : { $oid : \"112233445566778899aabbcc\" } }", "112233445566778899aabbcc")]
        [TestCase("{ x : { $oid : \"2233445566778899aabbccdd\" } }", "2233445566778899aabbccdd")]
        [TestCase("{ x : { $$oid : \"112233445566778899aabbcc\" } }", "112233445566778899aabbcc")]
        [TestCase("{ x : { $$oid : \"2233445566778899aabbccdd\" } }", "2233445566778899aabbccdd")]
        public void ReadJson_should_return_expected_result_when_using_wrapped_bson_reader(string json, string hexValue)
        {
            var subject = new ObjectIdConverter();
            var expectedResult = ObjectId.Parse(hexValue);

            var result = ReadJsonUsingWrappedBsonReader<ObjectId>(subject, ToBson(json), mustBeNested: true);

            result.Should().Be(expectedResult);
        }

        [TestCase("{ $oid : \"112233445566778899aabbcc\" }", "112233445566778899aabbcc")]
        [TestCase("{ $oid : \"2233445566778899aabbccdd\" }", "2233445566778899aabbccdd")]
        public void ReadJson_should_return_expected_result_when_using_wrapped_json_reader(string json, string hexValue)
        {
            var subject = new ObjectIdConverter();
            var expectedResult = ObjectId.Parse(hexValue);

            var result = ReadJsonUsingWrappedJsonReader<ObjectId>(subject, json);

            result.Should().Be(expectedResult);
        }

        [Test]
        public void ReadJson_should_throw_when_token_type_is_invalid()
        {
            var subject = new ObjectIdConverter();
            var json = "undefined";

            Action action = () => { var _ = ReadJsonUsingNativeJsonReader<ObjectId>(subject, json); };

            action.ShouldThrow<Newtonsoft.Json.JsonReaderException>();
        }

        [TestCase("112233445566778899aabbcc", "{ x : { $oid : \"112233445566778899aabbcc\" } }")]
        [TestCase("2233445566778899aabbccdd", "{ x : { $oid : \"2233445566778899aabbccdd\" } }")]
        public void WriteJson_should_have_expected_result_when_using_native_bson_writer(string hexValue, string expectedResult)
        {
            var subject = new ObjectIdConverter();
            var value = ObjectId.Parse(hexValue);

            var result = WriteJsonUsingNativeBsonWriter(subject, value, mustBeNested: true);

            result.Should().Equal(ToBson(expectedResult));
        }

        [TestCase("112233445566778899aabbcc", "{\"$oid\":\"112233445566778899aabbcc\"}")]
        [TestCase("2233445566778899aabbccdd", "{\"$oid\":\"2233445566778899aabbccdd\"}")]
        public void WriteJson_should_have_expected_result_when_using_native_json_writer(string hexValue, string expectedResult)
        {
            var subject = new ObjectIdConverter();
            var value = ObjectId.Parse(hexValue);

            var result = WriteJsonUsingNativeJsonWriter(subject, value);

            result.Should().Be(expectedResult);
        }

        [TestCase("112233445566778899aabbcc", "{ x : { $oid : \"112233445566778899aabbcc\" } }")]
        [TestCase("2233445566778899aabbccdd", "{ x : { $oid : \"2233445566778899aabbccdd\" } }")]
        public void WriteJson_should_have_expected_result_when_using_wrapped_bson_writer(string hexValue, string expectedResult)
        {
            var subject = new ObjectIdConverter();
            var value = ObjectId.Parse(hexValue);

            var result = WriteJsonUsingWrappedBsonWriter(subject, value, mustBeNested: true);

            result.Should().Equal(ToBson(expectedResult));
        }

        [TestCase("112233445566778899aabbcc", "ObjectId(\"112233445566778899aabbcc\")")]
        [TestCase("2233445566778899aabbccdd", "ObjectId(\"2233445566778899aabbccdd\")")]
        public void WriteJson_should_have_expected_result_when_using_wrapped_json_writer(string hexValue, string expectedResult)
        {
            var subject = new ObjectIdConverter();
            var value = ObjectId.Parse(hexValue);

            var result = WriteJsonUsingWrappedJsonWriter(subject, value);

            result.Should().Be(expectedResult);
        }
    }
}
