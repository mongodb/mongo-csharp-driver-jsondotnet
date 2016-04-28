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

using FluentAssertions;
using MongoDB.Integrations.JsonDotNet.Converters;
using MongoDB.Bson.Serialization;
using NUnit.Framework;
using MongoDB.Bson;

namespace MongoDB.Integrations.JsonDotNet.Tests.Converters
{
    [TestFixture]
    public class BsonValueConverterTests : JsonConverterTestsBase
    {
        [Test]
        public void Instance_get_returns_cached_result()
        {
            var result1 = BsonValueConverter.Instance;
            var result2 = BsonValueConverter.Instance;

            result2.Should().BeSameAs(result1);
        }

        [Test]
        public void Instance_get_returns_expected_result()
        {
            var result = BsonValueConverter.Instance;

            result.Should().NotBeNull();
            result.Should().BeOfType<BsonValueConverter>();
        }

        [TestCase("{ x : null }", "null")]
        [TestCase("{ x : undefined }", "undefined")]
        [TestCase("{ x : 0 }", "NumberLong(0)")]
        [TestCase("{ x : 0.0 }", "0.0")]
        [TestCase("{ x : NumberLong(0) }", "NumberLong(0)")]
        [TestCase("{ x : false }", "false")]
        [TestCase("{ x : \"abc\" }", "\"abc\"")]
        [TestCase("{ x : [1, \"abc\"] }", "[NumberLong(1), \"abc\"]")]
        [TestCase("{ x : { x : 1, y : \"abc\" } }", "{ x : 1, y : \"abc\" }")]
        [TestCase("{ x : { $binary : \"AA==\", $type : \"00\" } }", "{ $binary : \"AA==\", $type : \"00\" }")]
        [TestCase("{ x : { $binary : \"AA==\", $type : \"80\" } }", "{ $binary : \"AA==\", $type : \"00\" }")] // note: Json.NET loses the subType
        [TestCase("{ x : { $$binary : \"AA==\", $$type : \"00\" } }", "{ $binary : \"AA==\", $type : \"00\" }")]
        [TestCase("{ x : { $$binary : \"AA==\", $$type : \"80\" } }", "{ $binary : \"AA==\", $type : \"80\" }")]
        [TestCase("{ x : { $code : \"abc\" } }", "\"abc\"")] // note: Json.NET reads BsonJavaScript as a string
        [TestCase("{ x : { $code : \"abc\", $scope : { x : 1 } } }", "{ $code : \"abc\", $scope : { x : 1 } }")]
        [TestCase("{ x : { $$code : \"abc\" } }", "{ $code : \"abc\" }")]
        [TestCase("{ x : { $$code : \"abc\", $$scope : { x : 1 } } }", "{ $code : \"abc\", $scope : { x : NumberLong(1) } }")]
        [TestCase("{ x : { $date : 0 } }", "{ $date : 0 }")]
        [TestCase("{ x : { $$date : 0 } }", "{ $date : 0 }")]
        [TestCase("{ x : { $$date : \"1970-01-01T00:00:00.000Z\" } }", "{ $date : 0 }")]
        [TestCase("{ x : { $$date : { $$numberLong : \"0\" } } }", "{ $date : 0 }")]
        [TestCase("{ x : { $$maxKey : 1 } }", "{ $maxKey : 1 }")] // note: Json.NET can't read a BsonMaxKey
        [TestCase("{ x : { $$minKey : 1 } }", "{ $minKey : 1 }")] // note: Json.NET can't read a BsonMinKey
        [TestCase("{ x : { $oid: \"112233445566778899aabbcc\" } }", "HexData(0, \"112233445566778899aabbcc\")")] // note: Json.NET turns ObjectId into bytes
        [TestCase("{ x : { $$oid: \"112233445566778899aabbcc\" } }", "{ $oid: \"112233445566778899aabbcc\" }")]
        [TestCase("{ x : { $regex : \"abc\", $options : \"i\" } }", "\"/abc/i\"")] // note: Json.NET turns a BsonRegularExpression into a string
        [TestCase("{ x : { $$regex : \"abc\", $$options : \"i\" } }", "{ $regex : \"abc\", $options : \"i\" }")]
        [TestCase("{ x : { $symbol : \"abc\" } }", "\"abc\"")] // note: Json.NET turns a BsonSymbol into a string
        [TestCase("{ x : { $$symbol : \"abc\" } }", "{ $symbol : \"abc\" }")]
        [TestCase("{ x : { $timestamp : { t : 1, i : 2 } } }", "4294967298")] // note: Json.NET turns a BsonTimestamp into a long
        [TestCase("{ x : { $$timestamp : { t : 1, i : 2 } } }", "{ $timestamp : { t : 1, i : 2 } }")]
        public void ReadJson_should_return_expected_result_when_using_native_bson_reader(string json, string expectedResult)
        {
            var subject = new BsonValueConverter();

            var result = ReadJsonUsingNativeBsonReader<BsonValue>(subject, ToBson(json), mustBeNested: true);

            result.Should().Be(expectedResult);
        }

        [TestCase("null", "null")]
        [TestCase("undefined", "undefined")]
        [TestCase("0", "NumberLong(0)")]
        [TestCase("0.0", "0.0")]
        [TestCase("false", "false")]
        [TestCase("\"abc\"", "\"abc\"")]
        [TestCase("[1, \"abc\"]", "[NumberLong(1), \"abc\"]")]
        [TestCase("{ x : 1, y : \"abc\" }", "{ x : 1, y : \"abc\" }")]
        [TestCase("{ $binary : \"AA==\", $type : \"00\" }", "{ $binary : \"AA==\", $type : \"00\" }")]
        [TestCase("{ $binary : \"AA==\", $type : \"80\" }", "{ $binary : \"AA==\", $type : \"80\" }")]
        [TestCase("{ $code : \"abc\" }", "{ $code : \"abc\" }")]
        [TestCase("{ $code : \"abc\", $scope : { x : 1 } }", "{ $code : \"abc\", $scope : { x : NumberLong(1) } }")]
        [TestCase("{ $date : 0 }", "{ $date : 0 }")]
        [TestCase("{ $date : \"1970-01-01T00:00:00.000Z\" }", "{ $date : 0 }")]
        [TestCase("{ $date : { $numberLong : \"0\" } }", "{ $date : 0 }")]
        [TestCase("{ $maxKey : 1 }", "{ $maxKey : 1 }")]
        [TestCase("{ $minKey : 1 }", "{ $minKey : 1 }")]
        [TestCase("{ $oid: \"112233445566778899aabbcc\" }", "{ $oid: \"112233445566778899aabbcc\" }")]
        [TestCase("{ $regex : \"abc\", $options : \"i\" }", "{ $regex : \"abc\", $options : \"i\" }")]
        [TestCase("{ $symbol : \"abc\" }", "{ $symbol : \"abc\" }")]
        [TestCase("{ $timestamp : { t : 1, i : 2 } }", "{ $timestamp : { t : 1, i : 2 } }")]
        public void ReadJson_should_return_expected_result_when_using_native_json_reader(string json, string expectedResult)
        {
            var subject = new BsonValueConverter();

            var result = ReadJsonUsingNativeJsonReader<BsonValue>(subject, json);

            result.Should().Be(BsonSerializer.Deserialize<BsonValue>(expectedResult));
        }

        [TestCase("{ x : null }", "null")]
        [TestCase("{ x : undefined }", "undefined")]
        [TestCase("{ x : 0 }", "0")]
        [TestCase("{ x : 0.0 }", "0.0")]
        [TestCase("{ x : NumberLong(0) }", "NumberLong(0)")]
        [TestCase("{ x : false }", "false")]
        [TestCase("{ x : \"abc\" }", "\"abc\"")]
        [TestCase("{ x : [1, \"abc\"] }", "[1, \"abc\"]")]
        [TestCase("{ x : { x : 1, y : \"abc\" } }", "{ x : 1, y : \"abc\" }")]
        [TestCase("{ x : { $binary : \"AA==\", $type : \"00\" } }", "{ $binary : \"AA==\", $type : \"00\" }")]
        [TestCase("{ x : { $binary : \"AA==\", $type : \"80\" } }", "{ $binary : \"AA==\", $type : \"80\" }")]
        [TestCase("{ x : { $$binary : \"AA==\", $$type : \"00\" } }", "{ $binary : \"AA==\", $type : \"00\" }")]
        [TestCase("{ x : { $$binary : \"AA==\", $$type : \"80\" } }", "{ $binary : \"AA==\", $type : \"80\" }")]
        [TestCase("{ x : { $code : \"abc\" } }", "{ $code : \"abc\" }")]
        [TestCase("{ x : { $code : \"abc\", $scope : { x : 1 } } }", "{ $code : \"abc\", $scope : { x : 1 } }")]
        [TestCase("{ x : { $$code : \"abc\" } }", "{ $code : \"abc\" }")]
        [TestCase("{ x : { $$code : \"abc\", $$scope : { x : 1 } } }", "{ $code : \"abc\", $scope : { x : NumberLong(1) } }")]
        [TestCase("{ x : { $date : 0 } }", "{ $date : 0 }")]
        [TestCase("{ x : { $$date : 0 } }", "{ $date : 0 }")]
        [TestCase("{ x : { $$date : \"1970-01-01T00:00:00.000Z\" } }", "{ $date : 0 }")]
        [TestCase("{ x : { $$date : { $$numberLong : \"0\" } } }", "{ $date : 0 }")]
        [TestCase("{ x : { $maxKey : 1 } }", "{ $maxKey : 1 }")]
        [TestCase("{ x : { $$maxKey : 1 } }", "{ $maxKey : 1 }")]
        [TestCase("{ x : { $minKey : 1 } }", "{ $minKey : 1 }")]
        [TestCase("{ x : { $$minKey : 1 } }", "{ $minKey : 1 }")]
        [TestCase("{ x : { $oid: \"112233445566778899aabbcc\" } }", "{ $oid: \"112233445566778899aabbcc\" }")]
        [TestCase("{ x : { $$oid: \"112233445566778899aabbcc\" } }", "{ $oid: \"112233445566778899aabbcc\" }")]
        [TestCase("{ x : { $regex : \"abc\", $options : \"i\" } }", "{ $regex : \"abc\", $options : \"i\" } }")]
        [TestCase("{ x : { $$regex : \"abc\", $$options : \"i\" } }", "{ $regex : \"abc\", $options : \"i\" }")]
        [TestCase("{ x : { $symbol : \"abc\" } }", "{ $symbol : \"abc\" }")]
        [TestCase("{ x : { $$symbol : \"abc\" } }", "{ $symbol : \"abc\" }")]
        [TestCase("{ x : { $timestamp : { t : 1, i : 2 } } }", "{ $timestamp : { t : 1, i : 2 } }")]
        [TestCase("{ x : { $$timestamp : { t : 1, i : 2 } } }", "{ $timestamp : { t : 1, i : 2 } }")]
        public void ReadJson_should_return_expected_result_when_using_wrapped_bson_reader(string json, string expectedResult)
        {
            var subject = new BsonValueConverter();

            var result = ReadJsonUsingWrappedBsonReader<BsonValue>(subject, ToBson(json), mustBeNested: true);

            result.Should().Be(BsonSerializer.Deserialize<BsonValue>(expectedResult));
        }

        [TestCase("null", "null")]
        [TestCase("undefined", "undefined")]
        [TestCase("0", "0")]
        [TestCase("0.0", "0.0")]
        [TestCase("false", "false")]
        [TestCase("\"abc\"", "\"abc\"")]
        [TestCase("[1, \"abc\"]", "[1, \"abc\"]")]
        [TestCase("{ x : 1, y : \"abc\" }", "{ x : 1, y : \"abc\" }")]
        [TestCase("{ $binary : \"AA==\", $type : \"00\" }", "{ $binary : \"AA==\", $type : \"00\" }")]
        [TestCase("{ $binary : \"AA==\", $type : \"80\" }", "{ $binary : \"AA==\", $type : \"80\" }")]
        [TestCase("{ $code : \"abc\" }", "{ $code : \"abc\" }")]
        [TestCase("{ $code : \"abc\", $scope : { x : 1 } }", "{ $code : \"abc\", $scope : { x : NumberLong(1) } }")]
        [TestCase("{ $date : 0 }", "{ $date : 0 }")]
        [TestCase("{ $date : \"1970-01-01T00:00:00.000Z\" }", "{ $date : 0 }")]
        // [TestCase("{ $date : { $numberLong : \"0\" } }", "{ $date : 0 }")] // note: .NET driver doesn't recognize this extended JSON format yet
        [TestCase("{ $maxKey : 1 }", "{ $maxKey : 1 }")]
        [TestCase("{ $minKey : 1 }", "{ $minKey : 1 }")]
        [TestCase("{ $oid: \"112233445566778899aabbcc\" }", "{ $oid: \"112233445566778899aabbcc\" }")]
        [TestCase("{ $regex : \"abc\", $options : \"i\" }", "{ $regex : \"abc\", $options : \"i\" }")]
        [TestCase("{ $symbol : \"abc\" }", "{ $symbol : \"abc\" }")]
        [TestCase("{ $timestamp : { t : 1, i : 2 } }", "{ $timestamp : { t : 1, i : 2 } }")]
        public void ReadJson_should_return_expected_result_when_using_wrapped_json_reader(string json, string expectedResult)
        {
            var subject = new BsonValueConverter();

            var result = ReadJsonUsingWrappedJsonReader<BsonValue>(subject, json);

            result.Should().Be(BsonSerializer.Deserialize<BsonValue>(expectedResult));
        }

        [TestCase(null, "{ x : null }")]
        [TestCase("null", "{ x : null }")]
        [TestCase("undefined", "{ x : undefined }")]
        [TestCase("0", "{ x : NumberLong(0) }")] // note: Json.NET can't write a BsonInt32
        [TestCase("0.0", "{ x : 0.0 }")]
        [TestCase("NumberLong(0)", "{ x : NumberLong(0) }")]
        [TestCase("false", "{ x : false }")]
        [TestCase("\"abc\"", "{ x : \"abc\" }")]
        [TestCase("[1, \"abc\"]", "{ x : [NumberLong(1), \"abc\"] }")] // note: Json.NET can't write a BsonInt32
        [TestCase("{ x : 1, y : \"abc\" }", "{ x : { x : NumberLong(1), y : \"abc\" } }")] // note: Json.NET can't write a BsonInt32
        [TestCase("{ $binary : \"AA==\", $type : \"00\" }", "{ x : { $binary : \"AA==\", $type : \"00\" } }")]
        [TestCase("{ $binary : \"AA==\", $type : \"80\" }", "{ x : { $$binary : \"AA==\", $$type : \"80\" } }")]
        [TestCase("{ $code : \"abc\" }", "{ x : { $$code : \"abc\" } }")] // note: Json.NET can't write a BsonJavaScript
        [TestCase("{ $code : \"abc\", $scope : { x : 1 } }", "{ x : { $$code : \"abc\", $$scope : { x : NumberLong(1) } } }")] // note: Json.NET can't write a BsonJavaScriptWithCode or a BsonInt32
        [TestCase("{ $date : 0 }", "{ x : { $date : 0 } }")]
        [TestCase("{ $maxKey : 1 }", "{ x : { $$maxKey : 1 } }")]
        [TestCase("{ $minKey : 1 }", "{ x : { $$minKey : 1 } }")]
        [TestCase("{ $oid: \"112233445566778899aabbcc\" }", "{ x : { $oid: \"112233445566778899aabbcc\" } }")]
        [TestCase("{ $regex : \"abc\", $options : \"i\" }", "{ x : { $regex : \"abc\", $options : \"i\" } }")]
        [TestCase("{ $symbol : \"abc\" }", "{ x : { $$symbol : \"abc\" } }")] // note: Json.NET can't write a BsonSymbol
        [TestCase("{ $timestamp : { t : 1, i : 2 } }", "{ x : { $$timestamp : { t : 1, i : 2 } } }")] // note: Json.NET can't write a BsonTimestamp
        public void WriteJson_should_have_expected_result_when_using_native_bson_writer(string nullableValue, string expectedResult)
        {
            var subject = new BsonValueConverter();
            var value = nullableValue == null ? null : BsonSerializer.Deserialize<BsonValue>(nullableValue);

            var result = WriteJsonUsingNativeBsonWriter(subject, value, mustBeNested: true);

            result.Should().Equal(ToBson(expectedResult));
        }

        [TestCase(null, "null")]
        [TestCase("null", "null")]
        [TestCase("undefined", "undefined")]
        [TestCase("0", "0")]
        [TestCase("0.0", "0.0")]
        [TestCase("NumberLong(0)", "0")] // note: Json.NET doesn't distinguish between Int32 and Int64
        [TestCase("false", "false")]
        [TestCase("\"abc\"", "\"abc\"")]
        [TestCase("[1, \"abc\"]", "[1,\"abc\"]")]
        [TestCase("{ x : 1, y : \"abc\" }", "{\"x\":1,\"y\":\"abc\"}")]
        [TestCase("{ $binary : \"AA==\", $type : \"00\" }", "{\"$binary\":\"AA==\",\"$type\":\"00\"}")]
        [TestCase("{ $binary : \"AA==\", $type : \"80\" }", "{\"$binary\":\"AA==\",\"$type\":\"80\"}")]
        [TestCase("{ $code : \"abc\" }", "{\"$code\":\"abc\"}")]
        [TestCase("{ $code : \"abc\", $scope : { x : 1 } }", "{\"$code\":\"abc\",\"$scope\":{\"x\":1}}")]
        [TestCase("{ $date : 0 }", "\"1970-01-01T00:00:00Z\"")] // note: Json.NET writes BsonDateTime as a string
        [TestCase("{ $maxKey : 1 }", "{\"$maxKey\":1}")]
        [TestCase("{ $minKey : 1 }", "{\"$minKey\":1}")]
        [TestCase("{ $oid : \"112233445566778899aabbcc\" }", "{\"$oid\":\"112233445566778899aabbcc\"}")]
        [TestCase("{ $regex : \"abc\", $options : \"i\" }", "{\"$regex\":\"abc\",\"$options\":\"i\"}")]
        [TestCase("{ $symbol : \"abc\" }", "{\"$symbol\":\"abc\"}")]
        [TestCase("{ $timestamp : { t : 1, i : 2 } }", "{\"$timestamp\":{\"t\":1,\"i\":2}}")]
        public void WriteJson_should_have_expected_result_when_using_native_json_writer(string nullableValue, string expectedResult)
        {
            var subject = new BsonValueConverter();
            var value = nullableValue == null ? null : BsonSerializer.Deserialize<BsonValue>(nullableValue);

            var result = WriteJsonUsingNativeJsonWriter(subject, value);

            result.Should().Be(expectedResult);
        }

        [TestCase(null, "{ x : null }")]
        [TestCase("null", "{ x : null }")]
        [TestCase("undefined", "{ x : undefined }")]
        [TestCase("0", "{ x : 0 }")]
        [TestCase("0.0", "{ x : 0.0 }")]
        [TestCase("NumberLong(0)", "{ x : NumberLong(0) }")]
        [TestCase("false", "{ x : false }")]
        [TestCase("\"abc\"", "{ x : \"abc\" }")]
        [TestCase("[1, \"abc\"]", "{ x : [1, \"abc\"] }")]
        [TestCase("{ x : 1, y : \"abc\" }", "{ x : { x : 1, y : \"abc\" } }")]
        [TestCase("{ $binary : \"AA==\", $type : \"00\" }", "{ x : { $binary : \"AA==\", $type : \"00\" } }")]
        [TestCase("{ $binary : \"AA==\", $type : \"80\" }", "{ x : { $binary : \"AA==\", $type : \"80\" } }")]
        [TestCase("{ $code : \"abc\" }", "{ x : { $code : \"abc\" } }")]
        [TestCase("{ $code : \"abc\", $scope : { x : 1 } }", "{ x : { $code : \"abc\", $scope : { x : 1 } } }")]
        [TestCase("{ $date : 0 }", "{ x : { $date : 0 } }")]
        [TestCase("{ $maxKey : 1 }", "{ x : { $maxKey : 1 } }")]
        [TestCase("{ $minKey : 1 }", "{ x : { $minKey : 1 } }")]
        [TestCase("{ $oid: \"112233445566778899aabbcc\" }", "{ x : { $oid: \"112233445566778899aabbcc\" } }")]
        [TestCase("{ $regex : \"abc\", $options : \"i\" }", "{ x : { $regex : \"abc\", $options : \"i\" } }")]
        [TestCase("{ $symbol : \"abc\" }", "{ x : { $symbol : \"abc\" } }")]
        [TestCase("{ $timestamp : { t : 1, i : 2 } }", "{ x : { $timestamp : { t : 1, i : 2 } } }")]
        public void WriteJson_should_have_expected_result_when_using_wrapped_bson_writer(string nullableValue, string expectedResult)
        {
            var subject = new BsonValueConverter();
            var value = nullableValue == null ? null : BsonSerializer.Deserialize<BsonValue>(nullableValue);

            var result = WriteJsonUsingWrappedBsonWriter(subject, value, mustBeNested: true);

            result.Should().Equal(ToBson(expectedResult));
        }

        [TestCase(null, "null")]
        [TestCase("null", "null")]
        [TestCase("undefined", "undefined")]
        [TestCase("0", "0")]
        [TestCase("0.0", "0.0")]
        [TestCase("NumberLong(0)", "NumberLong(0)")]
        [TestCase("false", "false")]
        [TestCase("\"abc\"", "\"abc\"")]
        [TestCase("[1, \"abc\"]", "[1, \"abc\"]")]
        [TestCase("{ x : 1, y : \"abc\" }", "{ \"x\" : 1, \"y\" : \"abc\" }")]
        [TestCase("{ $binary : \"AA==\", $type : \"00\" }", "new BinData(0, \"AA==\")")]
        [TestCase("{ $binary : \"AA==\", $type : \"80\" }", "new BinData(128, \"AA==\")")]
        [TestCase("{ $code : \"abc\" }", "{ \"$code\" : \"abc\" }")]
        [TestCase("{ $code : \"abc\", $scope : { x : 1 } }", "{ \"$code\" : \"abc\", \"$scope\" : { \"x\" : 1 } }")]
        [TestCase("{ $date : 0 }", "ISODate(\"1970-01-01T00:00:00Z\")")]
        [TestCase("{ $maxKey : 1 }", "MaxKey")]
        [TestCase("{ $minKey : 1 }", "MinKey")]
        [TestCase("{ $oid: \"112233445566778899aabbcc\" }", "ObjectId(\"112233445566778899aabbcc\")")]
        [TestCase("{ $regex : \"abc\", $options : \"i\" }", "/abc/i")]
        [TestCase("{ $symbol : \"abc\" }", "{ \"$symbol\" : \"abc\" }")]
        [TestCase("{ $timestamp : { t : 1, i : 2 } }", "Timestamp(1, 2)")]
        public void WriteJson_should_have_expected_result_when_using_wrapped_json_writer(string nullableValue, string expectedResult)
        {
            var subject = new BsonValueConverter();
            var value = nullableValue == null ? null : BsonSerializer.Deserialize<BsonValue>(nullableValue);

            var result = WriteJsonUsingWrappedJsonWriter(subject, value);

            result.Should().Be(expectedResult);
        }
    }
}
