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
using System.IO;
using FluentAssertions;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using NSubstitute;
using NUnit.Framework;

namespace MongoDB.Integrations.JsonDotNet.Tests
{
    [TestFixture]
    public class BsonWriterAdapterTests
    {
        [TestCase(false)]
        [TestCase(true)]
        public void Close_should_close_wrapped_writer_when_CloseOutput_is_true(bool closeOutput)
        {
            var wrappedWriter = Substitute.For<IBsonWriter>();
            var subject = new BsonWriterAdapter(wrappedWriter);
            subject.CloseOutput = closeOutput;

            subject.Close();

            wrappedWriter.Received(closeOutput ? 1 : 0).Close();
        }

        [Test]
        public void constructor_should_initialize_instance()
        {
            var wrappedWriter = Substitute.For<IBsonWriter>();

            var result = new BsonWriterAdapter(wrappedWriter);

            result.WrappedWriter.Should().BeSameAs(wrappedWriter);
        }

        [Test]
        public void Flush_should_flush_wrapped_writer()
        {
            var wrappedWriter = Substitute.For<IBsonWriter>();
            var subject = new BsonWriterAdapter(wrappedWriter);

            subject.Flush();

            wrappedWriter.Received(1).Flush();
        }

        [Test]
        public void WrappedWriter_get_should_return_expected_result()
        {
            var wrappedWriter = Substitute.For<IBsonWriter>();
            var subject = new BsonWriterAdapter(wrappedWriter);

            var result = subject.WrappedWriter;

            result.Should().BeSameAs(wrappedWriter);
        }

        [Test]
        public void WriteBinaryData_should_have_expected_result()
        {
            var subject = CreateSubject();
            var value = new BsonBinaryData(new byte[] { 0 }, BsonBinarySubType.UserDefined);

            WriteNested(subject, () => subject.WriteBinaryData(value));

            AssertBsonEquals(subject, "{ x : { $binary : \"AA==\", $type : \"80\" } }");
        }

        [Test]
        public void WriteDateTime_should_have_expected_result()
        {
            var subject = CreateSubject();
            var value = 0;

            WriteNested(subject, () => subject.WriteDateTime(value));

            AssertBsonEquals(subject, "{ x : { $date : 0 } }");
        }

        [Test]
        public void WriteEndArray_should_have_expected_result()
        {
            var subject = CreateSubject();

            WriteNested(subject, () =>
            {
                subject.WriteStartArray();
                subject.WriteEndArray();
            });

            AssertBsonEquals(subject, "{ x : [] }");
        }

        [Test]
        public void WriteEndConstructor_should_throw()
        {
            var subject = CreateSubject();

            Action action = () => { subject.WriteEndConstructor(); };

            action.ShouldThrow<NotSupportedException>();
        }

        [Test]
        public void WriteEndObject_should_have_expected_result()
        {
            var subject = CreateSubject();

            WriteNested(subject, () =>
            {
                subject.WriteStartObject();
                subject.WriteEndObject();
            });

            AssertBsonEquals(subject, "{ x : { } }");
        }

        [Test]
        public void WriteInt32_should_have_expected_result()
        {
            var subject = CreateSubject();
            var value = 0;

            WriteNested(subject, () => subject.WriteInt32(value));

            AssertBsonEquals(subject, "{ x : 0 }");
        }

        [Test]
        public void WriteJavaScript_should_have_expected_result()
        {
            var subject = CreateSubject();
            var code = "abc";

            WriteNested(subject, () => subject.WriteJavaScript(code));

            AssertBsonEquals(subject, "{ x : { $code:\"abc\" } }");
        }

        [Test]
        public void WriteJavaScriptWithScope_should_have_expected_result()
        {
            var subject = CreateSubject();
            var code = "abc";
            var scope = new BsonDocument("x", 1);

            WriteNested(subject, () => 
            {
                subject.WriteJavaScriptWithScope(code);
                BsonDocumentSerializer.Instance.Serialize(BsonSerializationContext.CreateRoot(subject.WrappedWriter), scope);
            });

            AssertBsonEquals(subject, "{ x : { $code:\"abc\", $scope : { x : 1 } } }");
        }

        [Test]
        public void WriteMaxKey_should_have_expected_result()
        {
            var subject = CreateSubject();

            WriteNested(subject, () => subject.WriteMaxKey());

            AssertBsonEquals(subject, "{ x : { $maxKey : 1 } }");
        }

        [Test]
        public void WriteMinKey_should_have_expected_result()
        {
            var subject = CreateSubject();

            WriteNested(subject, () => subject.WriteMinKey());

            AssertBsonEquals(subject, "{ x : { $minKey : 1 } }");
        }

        [Test]
        public void WriteNull_should_have_expected_result()
        {
            var subject = CreateSubject();

            WriteNested(subject, () => subject.WriteNull());

            AssertBsonEquals(subject, "{ x : null }");
        }

        [Test]
        public void WriteObjectId_should_have_expected_result()
        {
            var subject = CreateSubject();
            var value = ObjectId.Parse("112233445566778899aabbcc");

            WriteNested(subject, () => subject.WriteObjectId(value));

            AssertBsonEquals(subject, "{ x : { $oid : \"112233445566778899aabbcc\" } }");
        }

        [Test]
        public void WriteWritePropertyName_should_have_expected_result()
        {
            var subject = CreateSubject();

            subject.WriteStartObject();
            subject.WritePropertyName("x");
            subject.WriteNull();
            subject.WriteEndObject();

            AssertBsonEquals(subject, "{ x : null }");
        }

        [Test]
        public void WriteRaw_should_throw()
        {
            var subject = CreateSubject();

            Action action = () => { subject.WriteRaw("abc"); };

            action.ShouldThrow<NotSupportedException>();
        }

        [Test]
        public void WriteRawValue_should_throw()
        {
            var subject = CreateSubject();

            Action action = () => { subject.WriteRawValue("abc"); };

            action.ShouldThrow<NotSupportedException>();
        }

        [Test]
        public void WriteRegularExpression_should_have_expected_result()
        {
            var subject = CreateSubject();
            var value = new BsonRegularExpression("abc", "i");

            WriteNested(subject, () => subject.WriteRegularExpression(value));

            AssertBsonEquals(subject, "{ x : /abc/i }");
        }

        [Test]
        public void WriteStartArray_should_have_expected_result()
        {
            var subject = CreateSubject();

            WriteNested(subject, () =>
            {
                subject.WriteStartArray();
                subject.WriteEndArray();
            });

            AssertBsonEquals(subject, "{ x : [] }");
        }

        [Test]
        public void WriteStartConstructor_should_throw()
        {
            var subject = CreateSubject();

            Action action = () => { subject.WriteStartConstructor("x"); };

            action.ShouldThrow<NotSupportedException>();
        }

        [Test]
        public void WriteStartObject_should_have_expected_result()
        {
            var subject = CreateSubject();

            WriteNested(subject, () =>
            {
                subject.WriteStartObject();
                subject.WriteEndObject();
            });

            AssertBsonEquals(subject, "{ x : { } }");
        }

        [Test]
        public void WriteSymbol_should_have_expected_result()
        {
            var subject = CreateSubject();
            var value = "name";

            WriteNested(subject, () => subject.WriteSymbol(value));

            AssertBsonEquals(subject, "{ x : { $symbol : \"name\" } }");
        }

        [Test]
        public void WriteTimestamp_should_have_expected_result()
        {
            var subject = CreateSubject();
            var value = 0x0000000100000002;

            WriteNested(subject, () => subject.WriteTimestamp(value));

            AssertBsonEquals(subject, "{ x : { $timestamp : { t : 1, i : 2 } } }");
        }

        [Test]
        public void WriteUndefined_should_have_expected_result()
        {
            var subject = CreateSubject();

            WriteNested(subject, () => subject.WriteUndefined());

            AssertBsonEquals(subject, "{ x : undefined }");
        }

        [Test]
        public void WriteValue_bool_should_have_expected_result()
        {
            var subject = CreateSubject();
            var value = true;

            WriteNested(subject, () => subject.WriteValue(value));

            AssertBsonEquals(subject, "{ x : true } }");
        }

        [Test]
        public void WriteValue_byte_should_have_expected_result()
        {
            var subject = CreateSubject();
            var value = (byte)1;

            WriteNested(subject, () => subject.WriteValue(value));

            AssertBsonEquals(subject, "{ x : 1 } }");
        }

        [TestCase(null, "{ x : null }")]
        [TestCase(new byte[] { 0 }, "{ x : { $binary : \"AA==\", $type : \"00\" } }")]
        public void WriteValue_bytes_should_have_expected_result(byte[] value, string expectedResult)
        {
            var subject = CreateSubject();

            WriteNested(subject, () => subject.WriteValue(value));

            AssertBsonEquals(subject, expectedResult);
        }

        [Test]
        public void WriteValue_char_should_have_expected_result()
        {
            var subject = CreateSubject();
            var value = 'a';

            WriteNested(subject, () => subject.WriteValue(value));

            AssertBsonEquals(subject, "{ x : \"a\" } }");
        }

        [Test]
        public void WriteValue_DateTime_should_have_expected_result()
        {
            var subject = CreateSubject();
            var value = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            WriteNested(subject, () => subject.WriteValue(value));

            AssertBsonEquals(subject, "{ x : { $date : 0 } }");
        }

        [Test]
        public void WriteValue_DateTimeOffset_should_have_expected_result()
        {
            var subject = CreateSubject();
            var value = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

            WriteNested(subject, () => subject.WriteValue(value));

            AssertBsonEquals(subject, "{ x : { $date : 0 } }");
        }

        [Test]
        public void WriteValue_decimal_should_have_expected_result()
        {
            var subject = CreateSubject();
            var value = 1.5M;

            WriteNested(subject, () => subject.WriteValue(value));

            AssertBsonEquals(subject, "{ x : 1.5 }");
        }

        [Test]
        public void WriteValue_double_should_have_expected_result()
        {
            var subject = CreateSubject();
            var value = 1.5;

            WriteNested(subject, () => subject.WriteValue(value));

            AssertBsonEquals(subject, "{ x : 1.5 }");
        }

        [Test]
        public void WriteValue_float_should_have_expected_result()
        {
            var subject = CreateSubject();
            var value = 1.5f;

            WriteNested(subject, () => subject.WriteValue(value));

            AssertBsonEquals(subject, "{ x : 1.5 }");
        }

        [Test]
        public void WriteValue_Guid_should_have_expected_result()
        {
            var subject = CreateSubject();
            var value = Guid.Parse("01020304-0506-0708-090a-0b0c0d0e0f10");

            WriteNested(subject, () => subject.WriteValue(value));

            AssertBsonEquals(subject, "{ x : HexData(3, \"0403020106050807090a0b0c0d0e0f10\") }");
        }

        [Test]
        public void WriteValue_int_should_have_expected_result()
        {
            var subject = CreateSubject();
            var value = 1;

            WriteNested(subject, () => subject.WriteValue(value));

            AssertBsonEquals(subject, "{ x : 1 }");
        }

        [Test]
        public void WriteValue_long_should_have_expected_result()
        {
            var subject = CreateSubject();
            var value = 1L;

            WriteNested(subject, () => subject.WriteValue(value));

            AssertBsonEquals(subject, "{ x : NumberLong(1) }");
        }

        [TestCase(1, "{ x : 1 }")]
        [TestCase(-1, "{ x : -1 }")]
        public void WriteValue_sbyte_should_have_expected_result(sbyte value, string expectedResult)
        {
            var subject = CreateSubject();

            WriteNested(subject, () => subject.WriteValue(value));

            AssertBsonEquals(subject, expectedResult);
        }

        [TestCase(1, "{ x : 1 }")]
        [TestCase(-1, "{ x : -1 }")]
        public void WriteValue_short_should_have_expected_result(short value, string expectedResult)
        {
            var subject = CreateSubject();

            WriteNested(subject, () => subject.WriteValue(value));

            AssertBsonEquals(subject, expectedResult);
        }

        [TestCase(null, "{ x : null }")]
        [TestCase("abc", "{ x : \"abc\" }")]
        public void WriteValue_string_should_have_expected_result(string value, string expectedResult)
        {
            var subject = CreateSubject();

            WriteNested(subject, () => subject.WriteValue(value));

            AssertBsonEquals(subject, expectedResult);
        }

        [TestCase(1, "{ x : \"00:00:01\" }")]
        public void WriteValue_TimeSpan_should_have_expected_result(int seconds, string expectedResult)
        {
            var subject = CreateSubject();
            var value = TimeSpan.FromSeconds(seconds);

            WriteNested(subject, () => subject.WriteValue(value));

            AssertBsonEquals(subject, expectedResult);
        }

        [TestCase(1U, "{ x : 1 }")]
        public void WriteValue_uint_should_have_expected_result(uint value, string expectedResult)
        {
            var subject = CreateSubject();

            WriteNested(subject, () => subject.WriteValue(value));

            AssertBsonEquals(subject, expectedResult);
        }

        [TestCase(1UL, "{ x : NumberLong(1) }")]
        public void WriteValue_ulong_should_have_expected_result(ulong value, string expectedResult)
        {
            var subject = CreateSubject();

            WriteNested(subject, () => subject.WriteValue(value));

            AssertBsonEquals(subject, expectedResult);
        }

        [TestCase(null, "{ x : null }")]
        [TestCase("http://localhost/", "{ x : \"http://localhost/\" }")]
        public void WriteValue_Uri_should_have_expected_result(string uriString, string expectedResult)
        {
            var subject = CreateSubject();
            var value = uriString == null ? null : new Uri(uriString);

            WriteNested(subject, () => subject.WriteValue(value));

            AssertBsonEquals(subject, expectedResult);
        }

        [TestCase(1U, "{ x : 1 }")]
        public void WriteValue_ushort_should_have_expected_result(uint uintValue, string expectedResult)
        {
            var subject = CreateSubject();
            var value = (ushort)uintValue;

            WriteNested(subject, () => subject.WriteValue(value));

            AssertBsonEquals(subject, expectedResult);
        }

        [Test]
        public void WriteWhitespace_should_be_ignored()
        {
            var subject = CreateSubject();
            var value = 0;

            WriteNested(subject, () => 
            {
                subject.WriteWhitespace(" ");
                subject.WriteInt32(value);
            });

            AssertBsonEquals(subject, "{ x : 0 }");
        }

        // private methods
        private void AssertBsonEquals(BsonWriterAdapter adapter, string json)
        {
            var wrappedWriter = (BsonBinaryWriter)adapter.WrappedWriter;
            var stream = (MemoryStream)wrappedWriter.BaseStream;
            var bson = stream.ToArray();

            byte[] expectedBson;
            using (var bsonReader = new JsonReader(json, new JsonReaderSettings { GuidRepresentation = GuidRepresentation.Unspecified }))
            {
                var context = BsonDeserializationContext.CreateRoot(bsonReader);
                var document = BsonDocumentSerializer.Instance.Deserialize(context);
                expectedBson = document.ToBson(writerSettings: new BsonBinaryWriterSettings { GuidRepresentation = GuidRepresentation.Unspecified });
            }

            bson.Should().Equal(expectedBson);
        }

        private BsonWriterAdapter CreateSubject()
        {
            var stream = new MemoryStream();
            var wrappedWriter = new BsonBinaryWriter(stream);
            return new BsonWriterAdapter(wrappedWriter);
        }

        private void WriteNested(BsonWriterAdapter adapter, Action writer)
        {
            adapter.WriteStartObject();
            adapter.WritePropertyName("x");
            writer();
            adapter.WriteEndObject();
        }
    }
}
