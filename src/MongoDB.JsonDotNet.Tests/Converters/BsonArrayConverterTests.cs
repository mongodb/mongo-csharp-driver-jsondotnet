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
using System.Linq;
using FluentAssertions;
using MongoDB.Bson;
using MongoDB.JsonDotNet.Converters;
using NUnit.Framework;

namespace MongoDB.JsonDotNet.Tests.Converters
{
    [TestFixture]
    public class BsonArrayConverterTests : JsonConverterTestsBase
    {
        [Test]
        public void Instance_get_returns_cached_result()
        {
            var result1 = BsonArrayConverter.Instance;
            var result2 = BsonArrayConverter.Instance;

            result2.Should().BeSameAs(result1);
        }

        [Test]
        public void Instance_get_returns_expected_result()
        {
            var result = BsonArrayConverter.Instance;

            result.Should().NotBeNull();
            result.Should().BeOfType<BsonArrayConverter>();
        }

        [TestCase("{ x : null }", null)]
        [TestCase("{ x : [] }", new object[0])]
        [TestCase("{ x : [1] }", new object[] { 1L })]
        [TestCase("{ x : [1, \"abc\"] }", new object[] { 1L, "abc" })]
        public void ReadJson_should_return_expected_result_when_using_native_bson_reader(string json, object[] nullableItems)
        {
            var subject = new BsonArrayConverter();
            var expectedResult = nullableItems == null ? null : new BsonArray(nullableItems.Select(i => BsonValue.Create(i)));

            var result = ReadJsonUsingNativeBsonReader<BsonArray>(subject, ToBson(json), mustBeNested: true);

            result.Should().Be(expectedResult);
        }

        [TestCase("null", null)]
        [TestCase("[]", new object[0])]
        [TestCase("[1]", new object[] { 1L })]
        [TestCase("[1, \"abc\"]", new object[] { 1L, "abc" })]
        public void ReadJson_should_return_expected_result_when_using_native_json_reader(string json, object[] nullableItems)
        {
            var subject = new BsonArrayConverter();
            var expectedResult = nullableItems == null ? null : new BsonArray(nullableItems.Select(i => BsonValue.Create(i)));

            var result = ReadJsonUsingNativeJsonReader<BsonArray>(subject, json);

            result.Should().Be(expectedResult);
        }

        [TestCase("{ x : null }", null)]
        [TestCase("{ x : [] }", new object[0])]
        [TestCase("{ x : [1] }", new object[] { 1 })]
        [TestCase("{ x : [1, \"abc\"] }", new object[] { 1, "abc" })]
        public void ReadJson_should_return_expected_result_when_using_wrapped_bson_reader(string json, object[] nullableItems)
        {
            var subject = new BsonArrayConverter();
            var expectedResult = nullableItems == null ? null : new BsonArray(nullableItems.Select(i => BsonValue.Create(i)));

            var result = ReadJsonUsingWrappedBsonReader<BsonArray>(subject, ToBson(json), mustBeNested: true);

            result.Should().Be(expectedResult);
        }

        [TestCase("null", null)]
        [TestCase("[]", new object[0])]
        [TestCase("[1]", new object[] { 1 })]
        [TestCase("[1, \"abc\"]", new object[] { 1, "abc" })]
        public void ReadJson_should_return_expected_result_when_using_wrapped_json_reader(string json, object[] nullableItems)
        {
            var subject = new BsonArrayConverter();
            var expectedResult = nullableItems == null ? null : new BsonArray(nullableItems.Select(i => BsonValue.Create(i)));

            var result = ReadJsonUsingWrappedJsonReader<BsonArray>(subject, json);

            result.Should().Be(expectedResult);
        }

        [Test]
        public void ReadJson_should_throw_when_token_type_is_invalid()
        {
            var subject = new BsonArrayConverter();
            var json = "undefined";

            Action action = () => { var _ = ReadJsonUsingNativeJsonReader<BsonArray>(subject, json); };

            action.ShouldThrow<Newtonsoft.Json.JsonReaderException>();
        }

        [TestCase(null, "{ x : null }")]
        [TestCase(new object[0], "{ x : [] }")]
        [TestCase(new object[] { 1 }, "{ x : [NumberLong(1)] }")]
        [TestCase(new object[] { 1, "abc" }, "{ x : [NumberLong(1), \"abc\"] }")]
        public void WriteJson_should_have_expected_result_when_using_native_bson_writer(object[] nullableItems, string expectedResult)
        {
            var subject = new BsonArrayConverter();
            var value = nullableItems == null ? null : new BsonArray(nullableItems.Select(i => BsonValue.Create(i)));

            var result = WriteJsonUsingNativeBsonWriter(subject, value, mustBeNested: true);

            result.Should().Equal(ToBson(expectedResult));
        }

        [TestCase(null, "null")]
        [TestCase(new object[0], "[]")]
        [TestCase(new object[] { 1 }, "[1]")]
        [TestCase(new object[] { 1, "abc" }, "[1,\"abc\"]")]
        public void WriteJson_should_have_expected_result_when_using_native_json_writer(object[] nullableItems, string expectedResult)
        {
            var subject = new BsonArrayConverter();
            var value = nullableItems == null ? null : new BsonArray(nullableItems.Select(i => BsonValue.Create(i)));

            var result = WriteJsonUsingNativeJsonWriter(subject, value);

            result.Should().Be(expectedResult);
        }

        [TestCase(null, "{ x : null }")]
        [TestCase(new object[0], "{ x : [] }")]
        [TestCase(new object[] { 1 }, "{ x : [1] }")]
        [TestCase(new object[] { 1, "abc" }, "{ x : [1, \"abc\"] }")]
        public void WriteJson_should_have_expected_result_when_using_wrapped_bson_writer(object[] nullableItems, string expectedResult)
        {
            var subject = new BsonArrayConverter();
            var value = nullableItems == null ? null : new BsonArray(nullableItems.Select(i => BsonValue.Create(i)));

            var result = WriteJsonUsingWrappedBsonWriter(subject, value, mustBeNested: true);

            result.Should().Equal(ToBson(expectedResult));
        }

        [TestCase(null, "null")]
        [TestCase(new object[0], "[]")]
        [TestCase(new object[] { 1 }, "[1]")]
        [TestCase(new object[] { 1, "abc" }, "[1, \"abc\"]")]
        public void WriteJson_should_have_expected_result_when_using_wrapped_json_writer(object[] nullableItems, string expectedResult)
        {
            var subject = new BsonArrayConverter();
            var value = nullableItems == null ? null : new BsonArray(nullableItems.Select(i => BsonValue.Create(i)));

            var result = WriteJsonUsingWrappedJsonWriter(subject, value);

            result.Should().Be(expectedResult);
        }
    }
}
