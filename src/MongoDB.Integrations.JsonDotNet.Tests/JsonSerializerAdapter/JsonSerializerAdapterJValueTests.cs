/* Copyright 2015-2016 MongoDB Inc.
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
using MongoDB.Bson.Serialization;
using NUnit.Framework;

namespace MongoDB.Integrations.JsonDotNet.Tests.JsonSerializerAdapter
{
    [TestFixture]
    public class JsonSerializerAdapterJValueTests : JsonSerializerAdapterTestsBase
    {
        [TestCase("BsonBoolean", "{ x : true }")]
        [TestCase("BsonBinaryData", "{ x : { $binary : \"AQ==\", $type : \"00\" } }")]
        [TestCase("BsonDateTime", "{ x : { $date : 0 } }")]
        [TestCase("BsonDouble", "{ x : 1.5 }")]
        [TestCase("BsonInt32", "{ x : 1 }")]
        [TestCase("BsonInt64", "{ x : NumberLong(1) }")]
        [TestCase("BsonJavaScript", "{ x : { $code : \"abc\" } }")]
        [TestCase("BsonNull", "{ x : null }")]
        [TestCase("BsonObjectId", "{ x : ObjectId(\"0102030405060708090a0b0c\") }")]
        [TestCase("BsonRegularExpression", "{ x : { $regex : \"abc\", $options : \"i\" } }")]
        [TestCase("BsonString", "{ x : \"abc\" }")]
        [TestCase("BsonSymbol", "{ x : { $symbol : \"abc\" } }")]
        [TestCase("BsonTimestamp", "{ x : { $timestamp : { t : 1, i : 2 } } }")]
        [TestCase("BsonUndefined", "{ x : undefined }")]
        public void Deserialize_should_return_expected_result(string type, string json)
        {
            var subject = CreateSubject();
            var bson = ToBson(json);

            var result = Deserialize(subject, bson, mustBeNested: true);

            var expectedResult = DeserializeUsingNewtonsoftReader<Newtonsoft.Json.Linq.JToken>(bson, mustBeNested: true);
            result.Should().Be(expectedResult);
        }

        [TestCase("BsonJavaScriptWithScope", "{ x : { $code : \"abc\", $scope : { x : 1 } } }")]
        public void Deserialize_should_return_expected_result_for_BsonJavaScriptWithScope(string type, string json)
        {
            var subject = CreateSubject();
            var bson = ToBson(json);

            var result = Deserialize(subject, bson, mustBeNested: true);

            // note: native reader throws, our reader adapter returns the code and drops the scope
            var expectedResult = new Newtonsoft.Json.Linq.JValue("abc");
            result.Should().Be(expectedResult);
        }

        [TestCase("BsonMaxKey", "{ x : { $maxKey : 1 } }")]
        [TestCase("BsonMinKey", "{ x : { $minKey : 1 } }")]
        public void Deserialize_should_return_expected_result_for_BsonMaxKey(string type, string json)
        {
            var subject = CreateSubject();
            var bson = ToBson(json);

            var result = Deserialize(subject, bson, mustBeNested: true);

            // note: native reader throws, our reader adapter returns null
            var expectedResult = Newtonsoft.Json.Linq.JValue.CreateNull();
            result.Should().Be(expectedResult);
        }

        [TestCase('a', "{ x : \"a\" }")]
        public void Serialize_char_should_have_expected_result(char charValue, string expectedResult)
        {
            var value = new Newtonsoft.Json.Linq.JValue(charValue);

            AssertSerializesTheSame(value);
        }

        [TestCase("1970-01-01T00:00:00Z", "{ x : { $date : 0 } }")]
        public void Serialize_DateTime_should_have_expected_result(string stringValue, string expectedResult)
        {
            var value = new Newtonsoft.Json.Linq.JValue(DateTime.Parse(stringValue));

            AssertSerializesTheSame(value);
        }

        [TestCase("1970-01-01T00:00:00Z", "{ x : { $date : 0 } }")]
        public void Serialize_DateTimeOffset_should_have_expected_result(string stringValue, string expectedResult)
        {
            var value = new Newtonsoft.Json.Linq.JValue(DateTimeOffset.Parse(stringValue));

            AssertSerializesTheSame(value);
        }

        [TestCase("1.5", "{ x : 1.5 }")]
        public void Serialize_decimal_should_have_expected_result(string stringValue, string expectedResult)
        {
            var value = new Newtonsoft.Json.Linq.JValue(decimal.Parse(stringValue));

            AssertSerializesTheSame(value);
        }

        [TestCase("01020304-0506-0708-090a-0b0c0d0e0f10", GuidRepresentation.Standard, "{ x : \"01020304-0506-0708-090a-0b0c0d0e0f10\" }")]
        public void Serialize_Guid_should_have_expected_result(string stringValue, GuidRepresentation guidRepresentation, string expectedResult)
        {
            var value = new Newtonsoft.Json.Linq.JValue(Guid.Parse(stringValue));

            AssertSerializesTheSame(value);
        }

        [TestCase(null, "{ x : null }")]
        public void Serialize_null_should_have_expected_result(object objectValue, string expectedResult)
        {
            var value = new Newtonsoft.Json.Linq.JValue(objectValue);

            AssertSerializesTheSame(value);
        }

        [TestCase("00:00:01", "{ x : \"00:00:01\" }")]
        public void Serialize_TimeSpan_should_have_expected_result(string stringValue, string expectedResult)
        {
            var value = new Newtonsoft.Json.Linq.JValue(TimeSpan.Parse(stringValue));

            AssertSerializesTheSame(value);
        }

        [TestCase("http://xyz.com", "{ x : \"http://xyz.com/\" }")]
        public void Serialize_Uri_should_have_expected_result(string stringValue, string expectedResult)
        {
            var value = new Newtonsoft.Json.Linq.JValue(new Uri(stringValue));

            AssertSerializesTheSame(value);
        }

        [TestCase("bool", true, "{ x : true }")]
        [TestCase("byte", (byte)1, "{ x : 1 }")]
        [TestCase("byte[]", new byte[] { 1 }, "{ x : { $binary : \"AQ==\", $type : \"00\" } }")]
        [TestCase("double", 1.5, "{ x : 1.5 }")]
        [TestCase("float", 1.5F, "{ x : 1.5 }")]
        [TestCase("int", 1, "{ x : 1 }")]
        [TestCase("long", 1L, "{ x : NumberLong(1) }")]
        [TestCase("sbyte", (sbyte)1, "{ x : 1 }")]
        [TestCase("sbyte", (sbyte)-1, "{ x : -1 }")]
        [TestCase("short", (short)1, "{ x : 1 }")]
        [TestCase("short", (short)-1, "{ x : -1 }")]
        [TestCase("string", "abc", "{ x : \"abc\" }")]
        [TestCase("uint", 1U, "{ x : NumberLong(1) }")]
        [TestCase("ulong", 1UL, "{ x : NumberLong(1) }")]
        [TestCase("ushort", (ushort)1, "{ x : 1 }")]
        public void Serialize_value_should_have_expected_result(string type, object objectValue, string _expectedResult)
        {
            var value = new Newtonsoft.Json.Linq.JValue(objectValue);

            AssertSerializesTheSame(value);
        }

        // private methods
        private void AssertSerializesTheSame(Newtonsoft.Json.Linq.JValue value)
        {
            var subject = CreateSubject();

            var result = Serialize(subject, value, mustBeNested: true);

            var expectedResult = SerializeUsingNewtonsoftWriter(value, mustBeNested: true);
            result.Should().Equal(expectedResult);
        }

        private IBsonSerializer<Newtonsoft.Json.Linq.JToken> CreateSubject()
        {
            //var subject = new JValueSerializer();
            var subject = new JsonSerializerAdapter<Newtonsoft.Json.Linq.JToken>();
            return subject;
        }
    }
}
