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

using System.IO;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Integrations.JsonDotNet.Converters;
using NUnit.Framework;

namespace MongoDB.Integrations.JsonDotNet.Tests.Converters
{
    public abstract class JsonConverterTestsBase
    {
        // protected methods
        protected Newtonsoft.Json.JsonSerializer CreatedConfiguredNewtonsoftJsonSerializer()
        {
            var serializer = new Newtonsoft.Json.JsonSerializer();
            serializer.Converters.Add(BsonValueConverter.Instance);
            serializer.Converters.Add(ObjectIdConverter.Instance);
            return serializer;
        }

        protected T ReadJson<T>(Newtonsoft.Json.JsonConverter converter, Newtonsoft.Json.JsonReader reader, bool mustBeNested = false)
        {
            if (mustBeNested)
            {
                reader.Read();
                if (reader.TokenType != Newtonsoft.Json.JsonToken.StartObject)
                {
                    Assert.Fail("Expected StartObject token.");
                }
                reader.Read();
                if (reader.TokenType != Newtonsoft.Json.JsonToken.PropertyName)
                {
                    Assert.Fail("Expected PropertyName token.");
                }
            }

            // note: Json.NET calls Read before calling a converter
            if (!reader.Read())
            {
                Assert.Fail("Unexpected end of stream.");
            }
            var serializer = CreatedConfiguredNewtonsoftJsonSerializer();
            var result = (T)converter.ReadJson(reader, typeof(T), null, serializer);

            if (mustBeNested)
            {
                reader.Read();
                if (reader.TokenType != Newtonsoft.Json.JsonToken.EndObject)
                {
                    Assert.Fail("Expected EndObject token.");
                }
            }

            return result;
        }

        protected T ReadJsonUsingNativeBsonReader<T>(Newtonsoft.Json.JsonConverter converter, byte[] bson, bool mustBeNested = false)
        {
            using (var stream = new MemoryStream(bson))
            using (var reader = new Newtonsoft.Json.Bson.BsonReader(stream))
            {
                return ReadJson<T>(converter, reader, mustBeNested);
            }
        }

        protected T ReadJsonUsingNativeJsonReader<T>(Newtonsoft.Json.JsonConverter converter, string json, bool mustBeNested = false)
        {
            using (var stringReader = new StringReader(json))
            using (var reader = new Newtonsoft.Json.JsonTextReader(stringReader))
            {
                return ReadJson<T>(converter, reader, mustBeNested);
            }
        }

        protected T ReadJsonUsingWrappedBsonReader<T>(Newtonsoft.Json.JsonConverter converter, byte[] bson, bool mustBeNested = false, GuidRepresentation guidRepresentation = GuidRepresentation.CSharpLegacy)
        {
            var readerSettings = new BsonBinaryReaderSettings { GuidRepresentation = guidRepresentation };
            using (var stream = new MemoryStream(bson))
            using (var wrappedReader = new BsonBinaryReader(stream, readerSettings))
            using (var reader = new BsonReaderAdapter(wrappedReader))
            {
                return ReadJson<T>(converter, reader, mustBeNested);
            }
        }

        protected T ReadJsonUsingWrappedJsonReader<T>(Newtonsoft.Json.JsonConverter converter, string json, bool mustBeNested = false)
        {
            using (var wrappedReader = new JsonReader(json))
            using (var reader = new BsonReaderAdapter(wrappedReader))
            {
                return ReadJson<T>(converter, reader, mustBeNested);
            }
        }

        protected byte[] ToBson(string json, GuidRepresentation guidRepresentation = GuidRepresentation.Unspecified)
        {
            var writerSettings = new BsonBinaryWriterSettings { GuidRepresentation = guidRepresentation };
            return UnquoteExtendedJson(BsonDocument.Parse(json)).ToBson(writerSettings: writerSettings);
        }

        protected void WriteJson(Newtonsoft.Json.JsonConverter converter, object value, Newtonsoft.Json.JsonWriter writer, bool mustBeNested = false)
        {
            if (mustBeNested)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("x");
            }

            var serializer = CreatedConfiguredNewtonsoftJsonSerializer();
            converter.WriteJson(writer, value, serializer);

            if (mustBeNested)
            {
                writer.WriteEndObject();
            }
        }

        protected byte[] WriteJsonUsingNativeBsonWriter(Newtonsoft.Json.JsonConverter converter, object value, bool mustBeNested = false)
        {
            using (var stream = new MemoryStream())
            using (var writer = new Newtonsoft.Json.Bson.BsonWriter(stream))
            {
                WriteJson(converter, value, writer, mustBeNested);
                return stream.ToArray();
            }
        }

        protected string WriteJsonUsingNativeJsonWriter(Newtonsoft.Json.JsonConverter converter, object value)
        {
            using (var stringWriter = new StringWriter())
            using (var writer = new Newtonsoft.Json.JsonTextWriter(stringWriter))
            {
                WriteJson(converter, value, writer);
                return stringWriter.ToString();
            }
        }

        protected byte[] WriteJsonUsingWrappedBsonWriter(Newtonsoft.Json.JsonConverter converter, object value, bool mustBeNested = false, GuidRepresentation guidRepresentation = GuidRepresentation.CSharpLegacy)
        {
            var wrappedWriterSettings = new BsonBinaryWriterSettings { GuidRepresentation = guidRepresentation };

            using (var stream = new MemoryStream())
            using (var wrappedWriter = new BsonBinaryWriter(stream, wrappedWriterSettings))
            using (var writer = new BsonWriterAdapter(wrappedWriter))
            {
                WriteJson(converter, value, writer, mustBeNested);
                return stream.ToArray();
            }
        }

        protected string WriteJsonUsingWrappedJsonWriter(Newtonsoft.Json.JsonConverter converter, object value)
        {
            using (var stringWriter = new StringWriter())
            using (var wrappedWriter = new JsonWriter(stringWriter))
            using (var writer = new BsonWriterAdapter(wrappedWriter))
            {
                WriteJson(converter, value, writer);
                return stringWriter.ToString();
            }
        }

        // private methods
        private BsonDocument UnquoteExtendedJson(BsonDocument document)
        {
            return new BsonDocument(document.Select(e => new BsonElement(UnquoteExtendedJson(e.Name), UnquoteExtendedJson(e.Value))));
        }

        private BsonValue UnquoteExtendedJson(BsonValue value)
        {
            switch (value.BsonType)
            {
                case BsonType.Document: return UnquoteExtendedJson((BsonDocument)value);
                default: return value;
            }
        }

        private string UnquoteExtendedJson(string elementName)
        {
            if (elementName.StartsWith("$$"))
            {
                return elementName.Substring(1);
            }
            else
            {
                return elementName;
            }
        }
    }
}
