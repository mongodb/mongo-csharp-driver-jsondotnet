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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FluentAssertions;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using NSubstitute;
using NUnit.Framework;

namespace MongoDB.Integrations.JsonDotNet.Tests
{
    [TestFixture]
    public class JsonReaderBaseTests
    {
        [TestCase("{ x : [1, { $maxKey : 1 }] }", new string[] { null, null, null, "1", "{ $maxKey : 1 }", null, null })]
        public void BsonValue_should_return_expected_result(string json, string[] expectedResults)
        {
            var subject = CreateSubject(json);

            var results = new List<BsonValue>();
            while (subject.Read())
            {
                var result = subject.BsonValue;
                results.Add(result);
            }

            results.Should().Equal(expectedResults.Select(r => r == null ? null : BsonSerializer.Deserialize<BsonValue>(r)));
        }

        [Test]
        public void constructor_should_initialize_instance()
        {
            var wrappedReader = Substitute.For<IBsonReader>();

            var result = new BsonReaderAdapter(wrappedReader);

            result.BsonValue.Should().BeNull();
            result.Depth.Should().Be(0);
            result.TokenType.Should().Be(Newtonsoft.Json.JsonToken.None);
            result.Value.Should().BeNull();
            result.BsonValue.Should().BeNull();
        }

        [TestCase("{ x : { y : 2 } }", new [] { 0, 1, 1, 2, 2, 2, 1, 0 })]
        public void Depth_should_return_expected_result(string json, int[] expectedResults)
        {
            var subject = CreateSubject(json);

            var results = new List<int>();
            do
            {
                var result = subject.Depth;
                results.Add(result);
            }
            while (subject.Read());

            results.Should().Equal(expectedResults);
        }

        [TestCase("", null)]
        [TestCase("null", null)]
        [TestCase("\"\"", new byte[0])]
        [TestCase("\"AA==\"", new byte[] { 0 })]
        [TestCase("\"\"", new byte[0])]
        [TestCase("[]", new byte[0])]
        [TestCase("[0, 1]", new byte[] { 0, 1 })]
        [TestCase("{ $binary : \"AA==\", $type : \"00\" }", new byte[] { 0 })]
        [TestCase("{ $type : \"System.Byte[]\", $value : [0] }", new byte[] { 0 })]
        public void ReadAsBytes_should_return_expected_result(string json, byte[] expectedResult)
        {
            var subject = CreateSubject(json);

            var result = subject.ReadAsBytes();

            result.Should().Equal(expectedResult);
        }

        [TestCase("[\"AA==\"]")]
        public void ReadAsBytes_should_return_null_at_end_of_array(string json)
        {
            var subject = CreateSubject(json);
            subject.Read();
            subject.ReadAsBytes(); // "AA==\"

            var result = subject.ReadAsBytes();

            result.Should().BeNull();
        }

        [TestCase("undefined")]
        [TestCase("[undefined]")]
        [TestCase("{ $type : undefined }")]
        [TestCase("{ $type : \"System.Byte[]\", $value : undefined }")]
        public void ReadAsBytes_should_throw_when_token_is_invalid(string json)
        {
            var subject = CreateSubject(json);

            Action action = () => { var result = subject.ReadAsBytes(); };

            action.ShouldThrow<Newtonsoft.Json.JsonReaderException>();
        }

        [TestCase("", null)]
        [TestCase("null", null)]
        [TestCase("\"\"", null)]
        [TestCase("\"1970-01-01T00:00:00Z\"", "1970-01-01T00:00:00Z")]
        [TestCase("{ $date : 0 }", "1970-01-01T00:00:00Z")]
        public void ReadAsDateTime_should_return_expected_result(string json, string expectedResultString)
        {
            var subject = CreateSubject(json);

            var result = subject.ReadAsDateTime();

            var styles = DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal;
            var expectedResult = expectedResultString == null ? null : (DateTime?)DateTime.Parse(expectedResultString, null, styles);
            result.Should().Be(expectedResult);
        }

        [TestCase("[{ $date : 0 }]")]
        public void ReadAsDateTime_should_return_null_at_end_of_array(string json)
        {
            var subject = CreateSubject(json);
            subject.Read(); // StartArray
            subject.ReadAsDateTime(); // 1

            var result = subject.ReadAsDateTime();

            result.Should().Be(null);
        }

        [TestCase("undefined")]
        [TestCase("\"abc\"")]
        public void ReadAsDateTime_should_throw_when_token_is_invalid(string json)
        {
            var subject = CreateSubject(json);

            Action action = () => { var result = subject.ReadAsDateTime(); };

            action.ShouldThrow<Newtonsoft.Json.JsonReaderException>();
        }

        [TestCase("", null)]
        [TestCase("null", null)]
        [TestCase("\"\"", null)]
        [TestCase("\"1970-01-01T00:00:00Z\"", "1970-01-01T00:00:00Z")]
        [TestCase("{ $date : 0 }", "1970-01-01T00:00:00Z")]
        public void ReadAsDateTimeOffset_should_return_expected_result(string json, string expectedResultString)
        {
            var subject = CreateSubject(json);

            var result = subject.ReadAsDateTimeOffset();

            var styles = DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal;
            var expectedResult = expectedResultString == null ? null : (DateTimeOffset?)DateTimeOffset.Parse(expectedResultString, null, styles);
            result.Should().Be(expectedResult);
        }

        [TestCase("[{ $date : 0 }]")]
        public void ReadAsDateTimeOffset_should_return_null_at_end_of_array(string json)
        {
            var subject = CreateSubject(json);
            subject.Read(); // StartArray
            subject.ReadAsDateTimeOffset(); // 0

            var result = subject.ReadAsDateTimeOffset();

            result.Should().Be(null);
        }

        [TestCase("undefined")]
        [TestCase("\"abc\"")]
        public void ReadAsDateTimeOffset_should_throw_when_token_is_invalid(string json)
        {
            var subject = CreateSubject(json);

            Action action = () => { var result = subject.ReadAsDateTimeOffset(); };

            action.ShouldThrow<Newtonsoft.Json.JsonReaderException>();
        }

        [TestCase("", null)]
        [TestCase("null", null)]
        [TestCase("1", 1.0)]
        [TestCase("1.0", 1.0)]
        [TestCase("NumberLong(1)", 1.0)]
        [TestCase("\"\"", null)]
        [TestCase("\"1\"", 1.0)]
        public void ReadAsDecimal_should_return_expected_result(string json, double? expectedResult)
        {
            var subject = CreateSubject(json);

            var result = subject.ReadAsDecimal();

            result.Should().Be((decimal?)expectedResult);
        }

        [TestCase("[1.0]")]
        public void ReadAsDecimal_should_return_null_at_end_of_array(string json)
        {
            var subject = CreateSubject(json);
            subject.Read(); // StartArray
            subject.ReadAsDecimal(); // 1.0

            var result = subject.ReadAsDecimal();

            result.Should().Be(null);
        }

        [TestCase("\"abc\"")]
        [TestCase("undefined")]
        public void ReadAsDecimalshould_throw_when_token_is_invalid(string json)
        {
            var subject = CreateSubject(json);

            Action action = () => { var result = subject.ReadAsDecimal(); };

            action.ShouldThrow<Newtonsoft.Json.JsonReaderException>();
        }

        [TestCase("", null)]
        [TestCase("null", null)]
        [TestCase("1", 1)]
        [TestCase("1.0", 1)]
        [TestCase("NumberLong(1)", 1)]
        [TestCase("\"\"", null)]
        [TestCase("\"1\"", 1)]
        public void ReadAsInt32_should_return_expected_result(string json, int? expectedResult)
        {
            var subject = CreateSubject(json);

            var result = subject.ReadAsInt32();

            result.Should().Be(expectedResult);
        }

        [TestCase("[1]")]
        public void ReadAsInt32_should_return_null_at_end_of_array(string json)
        {
            var subject = CreateSubject(json);
            subject.Read(); // StartArray
            subject.ReadAsInt32(); // 1

            var result = subject.ReadAsInt32();

            result.Should().Be(null);
        }

        [TestCase("\"abc\"")]
        [TestCase("undefined")]
        public void ReadAsInt32_should_throw_when_token_is_invalid(string json)
        {
            var subject = CreateSubject(json);

            Action action = () => { var result = subject.ReadAsInt32(); };

            action.ShouldThrow<Newtonsoft.Json.JsonReaderException>();
        }

        [TestCase("", null)]
        [TestCase("null", null)]
        [TestCase("\"abc\"", "abc")]
        [TestCase("1", "1")]
        [TestCase("1.0", "1")]
        [TestCase("NumberLong(1)", "1")]
        [TestCase("true", "True")]
        public void ReadAsString_should_return_expected_result(string json, string expectedResult)
        {
            var subject = CreateSubject(json);

            var result = subject.ReadAsString();

            result.Should().Be(expectedResult);
        }

        [TestCase("[\"abc\"]")]
        public void ReadAsString_should_return_null_at_end_of_array(string json)
        {
            var subject = CreateSubject(json);
            subject.Read(); // StartArray
            subject.ReadAsString(); // "abc"

            var result = subject.ReadAsString();

            result.Should().Be(null);
        }

        [TestCase("undefined")]
        public void ReadAsString_should_throw_when_token_is_invalid(string json)
        {
            var subject = CreateSubject(json);

            Action action = () => { var result = subject.ReadAsString(); };

            action.ShouldThrow<Newtonsoft.Json.JsonReaderException>();
        }

        [TestCase(
            "{ x : 1 }",
            new Newtonsoft.Json.JsonToken[]
            {
                Newtonsoft.Json.JsonToken.StartObject,
                Newtonsoft.Json.JsonToken.PropertyName,
                Newtonsoft.Json.JsonToken.Integer,
                Newtonsoft.Json.JsonToken.EndObject
            })]
        public void TokenType_should_return_expected_result(string json, Newtonsoft.Json.JsonToken[] expectedResults)
        {
            var subject = CreateSubject(json);

            var results = new List<Newtonsoft.Json.JsonToken>();
            while (subject.Read())
            {
                var result = subject.TokenType;
                results.Add(result);
            }

            results.Should().Equal(expectedResults);
        }

        [TestCase("{ x : [1, \"abc\"] }", new object[] { null, "x", null, 1L, "abc", null, null })]
        public void Value_should_return_expected_result(string json, object[] expectedResults)
        {
            var subject = CreateSubject(json);

            var results = new List<object>();
            while (subject.Read())
            {
                var result = subject.Value;
                results.Add(result);
            }

            results.Should().Equal(expectedResults);
        }

        [TestCase("{ x : [1, \"abc\"] }", new Type[] { null, typeof(string), null, typeof(long), typeof(string), null, null })]
        public void ValueType_should_return_expected_result(string json, Type[] expectedResults)
        {
            var subject = CreateSubject(json);

            var results = new List<Type>();
            while (subject.Read())
            {
                var result = subject.ValueType;
                results.Add(result);
            }

            results.Should().Equal(expectedResults);
        }

        // private methods
        private JsonReaderBase CreateSubject(string json)
        {
            var wrappedReader = new JsonReader(json);
            return new BsonReaderAdapter(wrappedReader);
        }
    }
}
