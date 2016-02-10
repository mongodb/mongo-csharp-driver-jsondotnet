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
using System.Globalization;
using FluentAssertions;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using NSubstitute;
using NUnit.Framework;

namespace MongoDB.Integrations.JsonDotNet.Tests
{
    [TestFixture]
    public class BsonReaderAdapterTests
    {
        [TestCase(false)]
        [TestCase(true)]
        public void Close_should_close_wrapped_reader_when_CloseInput_is_true(bool closeInput)
        {
            var wrappedReader = Substitute.For<IBsonReader>();
            var subject = new BsonReaderAdapter(wrappedReader);
            subject.CloseInput = closeInput;

            subject.Close();

            wrappedReader.Received(closeInput ? 1 : 0).Close();
        }

        [Test]
        public void constructor_should_initialize_instance()
        {
            var wrappedReader = Substitute.For<IBsonReader>();

            var result = new BsonReaderAdapter(wrappedReader);

            result.Should().BeOfType<BsonReaderAdapter>();
        }

        [Test]
        public void Read_should_return_false_when_wrapped_reader_state_is_Closed()
        {
            var subject = CreateSubject("");
            subject.Close();
            subject.WrappedReader.State.Should().Be(BsonReaderState.Closed);

            var result = subject.Read();

            result.Should().BeFalse();
        }

        [Test]
        public void Read_should_return_false_when_wrapped_reader_state_is_Done()
        {
            var document = new BsonDocument();
            var wrappedReader = new BsonDocumentReader(document);
            var subject = new BsonReaderAdapter(wrappedReader);
            subject.Read(); // StartObject
            subject.Read(); // EndObject
            wrappedReader.State.Should().Be(BsonReaderState.Done);

            var result = subject.Read();

            result.Should().BeFalse();
        }

        [TestCase("1")]
        public void Read_should_return_false_when_wrapped_reader_state_is_Initial_and_IsAtEndOfFile(string json)
        {
            var subject = CreateSubject(json);
            subject.Read();
            subject.WrappedReader.State.Should().Be(BsonReaderState.Initial);

            var result = subject.Read();

            result.Should().BeFalse();
        }

        [Test]
        public void Read_should_return_true_when_wrapped_reader_state_is_EndOfArray()
        {
            var subject = CreateSubject("[]");
            subject.Read(); // StartArray
            subject.WrappedReader.State.Should().Be(BsonReaderState.Type); // EndOfArray is a transient state that we can't assert on

            var result = subject.Read();

            result.Should().BeTrue();
            subject.TokenType.Should().Be(Newtonsoft.Json.JsonToken.EndArray);
            subject.Value.Should().BeNull();
            subject.BsonValue.Should().BeNull();
        }

        [Test]
        public void Read_should_return_true_when_wrapped_reader_state_is_EndOfDocument()
        {
            var subject = CreateSubject("{}");
            subject.Read(); // StartObject
            subject.WrappedReader.State.Should().Be(BsonReaderState.Type); // EndOfDocument is a transient state that we can't assert on

            var result = subject.Read();

            result.Should().BeTrue();
            subject.TokenType.Should().Be(Newtonsoft.Json.JsonToken.EndObject);
            subject.Value.Should().BeNull();
            subject.BsonValue.Should().BeNull();
        }

        [TestCase("1", Newtonsoft.Json.JsonToken.Integer, 1L, "1")]
        [TestCase("\"abc\"", Newtonsoft.Json.JsonToken.String, "abc", "\"abc\"")]
        [TestCase("[]", Newtonsoft.Json.JsonToken.StartArray, null, null)]
        [TestCase("{}", Newtonsoft.Json.JsonToken.StartObject, null, null)]
        public void Read_should_return_true_when_wrapped_reader_state_is_Initial(string json, Newtonsoft.Json.JsonToken expectedTokenType, object expectedValue, string expectedBsonValue)
        {
            var subject = CreateSubject(json);
            subject.WrappedReader.State.Should().Be(BsonReaderState.Initial);

            var result = subject.Read();

            result.Should().BeTrue();
            subject.TokenType.Should().Be(expectedTokenType);
            subject.Value.Should().Be(expectedValue);
            subject.BsonValue.Should().Be(expectedBsonValue);
        }

        [TestCase("{ x : 1 }")]
        public void Read_should_return_true_when_wrapped_reader_state_is_Name(string json)
        {
            var subject = CreateSubject(json);
            subject.Read(); // StartObject
            subject.WrappedReader.State.Should().Be(BsonReaderState.Type); // Name is a transitory state right after Type so we can't assert on it

            var result = subject.Read();

            result.Should().BeTrue();
            subject.TokenType.Should().Be(Newtonsoft.Json.JsonToken.PropertyName);
            subject.Value.Should().Be("x");
            subject.BsonValue.Should().BeNull();
        }

        [TestCase("{ x : 1 }")]
        public void Read_should_return_true_when_wrapped_reader_state_is_Type(string json)
        {
            var subject = CreateSubject(json);
            subject.Read(); // StartObject
            subject.WrappedReader.State.Should().Be(BsonReaderState.Type);

            var result = subject.Read();

            result.Should().BeTrue();
            subject.TokenType.Should().Be(Newtonsoft.Json.JsonToken.PropertyName);
            subject.Value.Should().Be("x");
            subject.BsonValue.Should().BeNull();
        }

        [TestCase("{ x : null }", Newtonsoft.Json.JsonToken.Null, null, "null")]
        [TestCase("{ x : undefined }", Newtonsoft.Json.JsonToken.Undefined, null, "undefined")]
        [TestCase("{ x : NumberLong(1) }", Newtonsoft.Json.JsonToken.Integer, 1L, "NumberLong(1)")]
        [TestCase("{ x : 1.0 }", Newtonsoft.Json.JsonToken.Float, 1.0, "1.0")]
        [TestCase("{ x : true }", Newtonsoft.Json.JsonToken.Boolean, true, "true")]
        [TestCase("{ x : \"abc\" }", Newtonsoft.Json.JsonToken.String, "abc", "\"abc\"")]
        [TestCase("{ x : [] }", Newtonsoft.Json.JsonToken.StartArray, null, null)]
        [TestCase("{ x : {} }", Newtonsoft.Json.JsonToken.StartObject, null, null)]
        [TestCase("{ x : 1 }", Newtonsoft.Json.JsonToken.Integer, 1L, "1")]
        [TestCase("{ x : { $binary : \"AQIDBAUGBwgJCgsMDQ4PEA==\", $type : \"00\" } }", Newtonsoft.Json.JsonToken.Bytes, "Hex:0102030405060708090a0b0c0d0e0f10", "{ $binary : \"AQIDBAUGBwgJCgsMDQ4PEA==\", $type : \"00\" }")]
        [TestCase("{ x : { $binary : \"AQIDBAUGBwgJCgsMDQ4PEA==\", $type : \"03\" } }", Newtonsoft.Json.JsonToken.Bytes, "Guid:04030201-0605-0807-090a-0b0c0d0e0f10", "{ $binary : \"AQIDBAUGBwgJCgsMDQ4PEA==\", $type : \"03\" }")]
        [TestCase("{ x : { $binary : \"AQIDBAUGBwgJCgsMDQ4PEA==\", $type : \"04\" } }", Newtonsoft.Json.JsonToken.Bytes, "Guid:01020304-0506-0708-090a-0b0c0d0e0f10", "{ $binary : \"AQIDBAUGBwgJCgsMDQ4PEA==\", $type : \"04\" }")]
        [TestCase("{ x : { $code : \"abc\" } }", Newtonsoft.Json.JsonToken.String, "abc", "{ $code : \"abc\" }")]
        [TestCase("{ x : { $code : \"abc\", $scope : { x : 1 } } }", Newtonsoft.Json.JsonToken.String, "abc", "{ $code : \"abc\", $scope : { x : 1 } }")]
        [TestCase("{ x : { $date : 0 } }", Newtonsoft.Json.JsonToken.Date, "DateTime:1970-01-01T00:00:00Z", "{ $date : 0 }")]
        [TestCase("{ x : { $date : 9223372036854775807 } }", Newtonsoft.Json.JsonToken.Date, 9223372036854775807L, "{ $date : 9223372036854775807 }")]
        [TestCase("{ x : { $date : -9223372036854775808 } }", Newtonsoft.Json.JsonToken.Date, -9223372036854775808L, "{ $date : -9223372036854775808 }")]
        [TestCase("{ x : { $maxKey : 1 } }", Newtonsoft.Json.JsonToken.Undefined, null, "{ $maxKey : 1 }")]
        [TestCase("{ x : { $minKey : 1 } }", Newtonsoft.Json.JsonToken.Undefined, null, "{ $minKey : 1 }")]
        [TestCase("{ x : { $oid : \"0102030405060708090a0b0c\" } }", Newtonsoft.Json.JsonToken.Bytes, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }, "{ $oid : \"0102030405060708090a0b0c\" }")]
        [TestCase("{ x : { $regex : \"abc\", $options : \"i\" } }", Newtonsoft.Json.JsonToken.String, "/abc/i", "{ $regex : \"abc\", $options : \"i\" }")]
        [TestCase("{ x : { $symbol : \"abc\" } }", Newtonsoft.Json.JsonToken.String, "abc", "{ $symbol : \"abc\" }")]
        [TestCase("{ x : { $timestamp : { t : 1, i : 2 } } }", Newtonsoft.Json.JsonToken.Integer, 0x100000002, "{ $timestamp : { t : 1, i : 2 } }")]
        public void Read_should_return_true_when_wrapped_reader_state_is_Value(string json, Newtonsoft.Json.JsonToken expectedTokenType, object expectedValue, string expectedBsonValue)
        {
            var subject = CreateSubject(json);
            subject.Read(); // StartObject
            subject.Read(); // PropertyName
            subject.WrappedReader.State.Should().Be(BsonReaderState.Value);

            var result = subject.Read();

            result.Should().BeTrue();
            subject.TokenType.Should().Be(expectedTokenType);
            if (subject.Value is byte[])
            {
                ((byte[])subject.Value).Should().Equal((byte[])ParseExpectedValue(expectedValue));
            }
            else
            {
                subject.Value.Should().Be(ParseExpectedValue(expectedValue));
            }
            subject.BsonValue.Should().Be(expectedBsonValue);
        }

        // private methods
        private BsonReaderAdapter CreateSubject(string json)
        {
            var wrappedReader = new JsonReader(json);
            return new BsonReaderAdapter(wrappedReader);
        }

        private object ParseExpectedValue(object value)
        {
            var stringValue = value as string;
            if (stringValue != null)
            {
                if (stringValue.StartsWith("DateTime:"))
                {
                    var styles = DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal;
                    return DateTime.Parse(stringValue.Substring(9), CultureInfo.InvariantCulture, styles);
                }

                if (stringValue.StartsWith("Guid:"))
                {
                    return Guid.Parse(stringValue.Substring(5));
                }

                if (stringValue.StartsWith("Hex:"))
                {
                    return BsonUtils.ParseHexString(stringValue.Substring(4));
                }
            }

            return value;
        }
    }
}
