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
    public class BsonBinaryDataConverterTests : JsonConverterTestsBase
    {
        [Test]
        public void Instance_get_returns_cached_result()
        {
            var result1 = BsonBinaryDataConverter.Instance;
            var result2 = BsonBinaryDataConverter.Instance;

            result2.Should().BeSameAs(result1);
        }

        [Test]
        public void Instance_get_returns_expected_result()
        {
            var result = BsonBinaryDataConverter.Instance;

            result.Should().NotBeNull();
            result.Should().BeOfType<BsonBinaryDataConverter>();
        }

        [TestCase("{ x : null }", 0, null, 0)]
        [TestCase("{ x : { $binary : \"ESIz\", $type : \"00\" } }", 0, "112233", BsonBinarySubType.Binary)]
        [TestCase("{ x : { $binary : \"ESIz\", $type : \"80\" } }", 0, "112233", BsonBinarySubType.Binary)] // note: Json.NET loses the subType
        [TestCase("{ x : { $binary : \"ESIzRFVmd4iZqrvM3e7/AA==\", $type : \"03\" } }", GuidRepresentation.CSharpLegacy, "112233445566778899aabbccddeeff00", BsonBinarySubType.Binary)] // note: Json.NET loses the subType
        [TestCase("{ x : { $$binary : \"ESIz\", $$type : \"00\" } }", 0, "112233", BsonBinarySubType.Binary)]
        [TestCase("{ x : { $$binary : \"ESIz\", $$type : \"80\" } }", 0, "112233", BsonBinarySubType.UserDefined)] // note: when using extended Json the subType is not lost
        [TestCase("{ x : { $$binary : \"ESIzRFVmd4iZqrvM3e7/AA==\", $$type : \"03\" } }", GuidRepresentation.CSharpLegacy, "112233445566778899aabbccddeeff00", BsonBinarySubType.UuidLegacy)] // note: when using extended Json the subType is not lost
        [TestCase("{ x : { $$binary : \"ESIzRFVmd4iZqrvM3e7/AA==\", $$type : \"04\" } }", GuidRepresentation.Standard, "112233445566778899aabbccddeeff00", BsonBinarySubType.UuidStandard)] // note: when using extended Json the subType is not lost
        public void ReadJson_should_return_expected_result_when_using_native_bson_reader(string json, GuidRepresentation guidRepresentation, string nullableHexBytes, BsonBinarySubType subType)
        {
            var subject = new BsonBinaryDataConverter();
            var expectedResult = nullableHexBytes == null ? null : new BsonBinaryData(BsonUtils.ParseHexString(nullableHexBytes), subType);

            var result = ReadJsonUsingNativeBsonReader<BsonBinaryData>(subject, ToBson(json, guidRepresentation), mustBeNested: true);

            result.Should().Be(expectedResult);
        }

        [TestCase("null", null, 0)]
        [TestCase("{ $binary : \"ESIz\", $type : \"00\" }", "112233", BsonBinarySubType.Binary)]
        [TestCase("{ $binary : \"ESIz\", $type : \"80\" }", "112233", BsonBinarySubType.UserDefined)]
        [TestCase("{ $binary : \"ESIzRFVmd4iZqrvM3e7/AA==\", $type : \"03\" }", "112233445566778899aabbccddeeff00", BsonBinarySubType.UuidLegacy)]
        [TestCase("{ $binary : \"ESIzRFVmd4iZqrvM3e7/AA==\", $type : \"04\" }", "112233445566778899aabbccddeeff00", BsonBinarySubType.UuidStandard)]
        public void ReadJson_should_return_expected_result_when_using_native_json_reader(string json, string nullableHexBytes, BsonBinarySubType subType)
        {
            var subject = new BsonBinaryDataConverter();
            var expectedResult = nullableHexBytes == null ? null : new BsonBinaryData(BsonUtils.ParseHexString(nullableHexBytes), subType);

            var result = ReadJsonUsingNativeJsonReader<BsonBinaryData>(subject, json);

            result.Should().Be(expectedResult);
        }

        [TestCase("{ x : null }", 0, null, 0)]
        [TestCase("{ x : { $binary : \"ESIz\", $type : \"00\" } }", 0, "112233", BsonBinarySubType.Binary)]
        [TestCase("{ x : { $binary : \"ESIz\", $type : \"80\" } }", 0, "112233", BsonBinarySubType.UserDefined)]
        [TestCase("{ x : { $binary : \"ESIzRFVmd4iZqrvM3e7/AA==\", $type : \"03\" } }", GuidRepresentation.CSharpLegacy, "112233445566778899aabbccddeeff00", BsonBinarySubType.UuidLegacy)]
        [TestCase("{ x : { $binary : \"ESIzRFVmd4iZqrvM3e7/AA==\", $type : \"04\" } }", GuidRepresentation.Standard, "112233445566778899aabbccddeeff00", BsonBinarySubType.UuidStandard)]
        [TestCase("{ x : { $$binary : \"ESIz\", $$type : \"00\" } }", 0, "112233", BsonBinarySubType.Binary)]
        [TestCase("{ x : { $$binary : \"ESIz\", $$type : \"80\" } }", 0, "112233", BsonBinarySubType.UserDefined)]
        [TestCase("{ x : { $$binary : \"ESIzRFVmd4iZqrvM3e7/AA==\", $$type : \"03\" } }", GuidRepresentation.CSharpLegacy, "112233445566778899aabbccddeeff00", BsonBinarySubType.UuidLegacy)]
        [TestCase("{ x : { $$binary : \"ESIzRFVmd4iZqrvM3e7/AA==\", $$type : \"04\" } }", GuidRepresentation.Standard, "112233445566778899aabbccddeeff00", BsonBinarySubType.UuidStandard)]
        public void ReadJson_should_return_expected_result_when_using_wrapped_bson_reader(string json, GuidRepresentation guidRepresentation, string nullableHexBytes, BsonBinarySubType subType)
        {
            var subject = new BsonBinaryDataConverter();
            var expectedResult = nullableHexBytes == null ? null : new BsonBinaryData(BsonUtils.ParseHexString(nullableHexBytes), subType);

            var result = ReadJsonUsingWrappedBsonReader<BsonBinaryData>(subject, ToBson(json, guidRepresentation), mustBeNested: true, guidRepresentation: guidRepresentation);

            result.Should().Be(expectedResult);
        }

        [TestCase("null", null, 0)]
        [TestCase("{ $binary : \"ESIz\", $type : \"00\" }", "112233", BsonBinarySubType.Binary)]
        [TestCase("{ $binary : \"ESIz\", $type : \"80\" }", "112233", BsonBinarySubType.UserDefined)]
        [TestCase("{ $binary : \"ESIzRFVmd4iZqrvM3e7/AA==\", $type : \"03\" }", "112233445566778899aabbccddeeff00", BsonBinarySubType.UuidLegacy)]
        [TestCase("{ $binary : \"ESIzRFVmd4iZqrvM3e7/AA==\", $type : \"04\" }", "112233445566778899aabbccddeeff00", BsonBinarySubType.UuidStandard)]
        public void ReadJson_should_return_expected_result_when_using_wrapped_json_reader(string json, string nullableHexBytes, BsonBinarySubType subType)
        {
            var subject = new BsonBinaryDataConverter();
            var expectedResult = nullableHexBytes == null ? null : new BsonBinaryData(BsonUtils.ParseHexString(nullableHexBytes), subType);

            var result = ReadJsonUsingWrappedJsonReader<BsonBinaryData>(subject, json);

            result.Should().Be(expectedResult);
        }

        [TestCase("{ x : { $binary : \"ESIzRFVmd4iZqrvM3e7/AA==\", $type : \"04\" } }", GuidRepresentation.Standard)]
        public void ReadJson_should_throw_when_reading_a_guid_using_native_bson_reader(string json, GuidRepresentation guidRepresentation)
        {
            var subject = new BsonBinaryDataConverter();

            Action action = () => { var _ = ReadJsonUsingNativeBsonReader<BsonBinaryData>(subject, ToBson(json, guidRepresentation), mustBeNested: true); };

            action.ShouldThrow<Newtonsoft.Json.JsonReaderException>();
        }

        [Test]
        public void ReadJson_should_throw_when_token_type_is_invalid()
        {
            var subject = new BsonBinaryDataConverter();
            var json = "undefined";

            Action action = () => { var _ = ReadJsonUsingNativeJsonReader<BsonBinaryData>(subject, json); };

            action.ShouldThrow<Newtonsoft.Json.JsonReaderException>();
        }

        [TestCase(null, 0, "{ x : null }", 0)]
        [TestCase("112233", BsonBinarySubType.Binary, "{ x : { $binary : \"ESIz\", $type : \"00\" } }", 0)]
        [TestCase("112233", BsonBinarySubType.UserDefined, "{ x : { $$binary : \"ESIz\", $$type : \"80\" } }", 0)]
        [TestCase("112233445566778899aabbccddeeff00", BsonBinarySubType.UuidLegacy, "{ x : { $$binary : \"ESIzRFVmd4iZqrvM3e7/AA==\", $$type : \"03\" } }", GuidRepresentation.CSharpLegacy)]
        [TestCase("112233445566778899aabbccddeeff00", BsonBinarySubType.UuidStandard, "{ x : { $$binary : \"ESIzRFVmd4iZqrvM3e7/AA==\", $$type : \"04\" } }", GuidRepresentation.Standard)]
        public void WriteJson_should_have_expected_result_when_using_native_bson_writer(string nullableHexBytes, BsonBinarySubType subType, string expectedResult, GuidRepresentation resultGuidRepresentation)
        {
            var subject = new BsonBinaryDataConverter();
            var value = nullableHexBytes == null ? null : new BsonBinaryData(BsonUtils.ParseHexString(nullableHexBytes), subType);

            var result = WriteJsonUsingNativeBsonWriter(subject, value, mustBeNested: true);

            result.Should().Equal(ToBson(expectedResult, resultGuidRepresentation));
        }

        [TestCase(null, 0, "null")]
        [TestCase("112233", BsonBinarySubType.Binary, "{\"$binary\":\"ESIz\",\"$type\":\"00\"}")]
        [TestCase("112233", BsonBinarySubType.UserDefined, "{\"$binary\":\"ESIz\",\"$type\":\"80\"}")]
        [TestCase("112233445566778899aabbccddeeff00", BsonBinarySubType.UuidLegacy, "{\"$binary\":\"ESIzRFVmd4iZqrvM3e7/AA==\",\"$type\":\"03\"}")]
        [TestCase("112233445566778899aabbccddeeff00", BsonBinarySubType.UuidStandard, "{\"$binary\":\"ESIzRFVmd4iZqrvM3e7/AA==\",\"$type\":\"04\"}")]
        public void WriteJson_should_have_expected_result_when_using_native_json_writer(string nullableHexBytes, BsonBinarySubType subType, string expectedResult)
        {
            var subject = new BsonBinaryDataConverter();
            var value = nullableHexBytes == null ? null : new BsonBinaryData(BsonUtils.ParseHexString(nullableHexBytes), subType);

            var result = WriteJsonUsingNativeJsonWriter(subject, value);

            result.Should().Be(expectedResult);
        }

        [TestCase(null, 0, "{ x : null }", 0)]
        [TestCase("112233", BsonBinarySubType.Binary, "{ x : { $binary : \"ESIz\", $type : \"00\" } }", 0)]
        [TestCase("112233", BsonBinarySubType.UserDefined, "{ x : { $binary : \"ESIz\", $type : \"80\" } }", 0)]
        [TestCase("112233445566778899aabbccddeeff00", BsonBinarySubType.UuidLegacy, "{ x : { $binary : \"ESIzRFVmd4iZqrvM3e7/AA==\", $type : \"03\" } }", GuidRepresentation.CSharpLegacy)]
        [TestCase("112233445566778899aabbccddeeff00", BsonBinarySubType.UuidStandard, "{ x : { $binary : \"ESIzRFVmd4iZqrvM3e7/AA==\", $type : \"04\" } }", GuidRepresentation.Standard)]
        public void WriteJson_should_have_expected_result_when_using_wrapped_bson_writer(string nullableHexBytes, BsonBinarySubType subType, string expectedResult, GuidRepresentation resultGuidRepresentation)
        {
            var subject = new BsonBinaryDataConverter();
            var value = nullableHexBytes == null ? null : new BsonBinaryData(BsonUtils.ParseHexString(nullableHexBytes), subType, resultGuidRepresentation);

            var result = WriteJsonUsingWrappedBsonWriter(subject, value, mustBeNested: true, guidRepresentation : resultGuidRepresentation);

            result.Should().Equal(ToBson(expectedResult, resultGuidRepresentation));
        }

        [TestCase(null, 0, "null", 0)]
        [TestCase("112233", BsonBinarySubType.Binary, "new BinData(0, \"ESIz\")", 0)]
        [TestCase("112233", BsonBinarySubType.UserDefined, "new BinData(128, \"ESIz\")", 0)]
        [TestCase("112233445566778899aabbccddeeff00", BsonBinarySubType.UuidLegacy, "CSUUID(\"44332211-6655-8877-99aa-bbccddeeff00\")", GuidRepresentation.CSharpLegacy)]
        [TestCase("112233445566778899aabbccddeeff00", BsonBinarySubType.UuidStandard, "UUID(\"11223344-5566-7788-99aa-bbccddeeff00\")", GuidRepresentation.Standard)]
        public void WriteJson_should_have_expected_result_when_using_wrapped_json_writer(string nullableHexBytes, BsonBinarySubType subType, string expectedResult, GuidRepresentation resultGuidRepresentation)
        {
            var subject = new BsonBinaryDataConverter();
            var value = nullableHexBytes == null ? null : new BsonBinaryData(BsonUtils.ParseHexString(nullableHexBytes), subType, resultGuidRepresentation);

            var result = WriteJsonUsingWrappedJsonWriter(subject, value);

            result.Should().Be(expectedResult);
        }
    }
}
