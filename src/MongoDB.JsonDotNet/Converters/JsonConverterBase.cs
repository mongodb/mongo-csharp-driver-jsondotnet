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

namespace MongoDB.JsonDotNet.Converters
{
    public abstract class JsonConverterBase<T> : Newtonsoft.Json.JsonConverter
    {
        // public methods
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(T);
        }

        // protected methods
        protected void ReadEndObject(Newtonsoft.Json.JsonReader reader)
        {
            ReadExpectedToken(reader, Newtonsoft.Json.JsonToken.EndObject);
        }

        protected void ReadExpectedPropertyName(Newtonsoft.Json.JsonReader reader, string expectedPropertyName)
        {
            var propertyName = ReadPropertyName(reader);
            if (propertyName != expectedPropertyName)
            {
                var message = string.Format("Expected property named '{0}', but got '{1}.", expectedPropertyName, propertyName);
                throw new Newtonsoft.Json.JsonReaderException(message);
            }
        }

        protected void ReadExpectedToken(Newtonsoft.Json.JsonReader reader, Newtonsoft.Json.JsonToken expectedTokenType)
        {
            ReadToken(reader);
            if (reader.TokenType != expectedTokenType)
            {
                var message = string.Format("Expected token {0}, but got {1}.", expectedTokenType, reader.TokenType);
                throw new Newtonsoft.Json.JsonReaderException(message);
            }
        }

        protected string ReadPropertyName(Newtonsoft.Json.JsonReader reader)
        {
            ReadExpectedToken(reader, Newtonsoft.Json.JsonToken.PropertyName);
            return (string)reader.Value;
        }

        protected void ReadStartObject(Newtonsoft.Json.JsonReader reader)
        {
            ReadExpectedToken(reader, Newtonsoft.Json.JsonToken.StartObject);
        }

        protected string ReadStringValue(Newtonsoft.Json.JsonReader reader)
        {
            ReadExpectedToken(reader, Newtonsoft.Json.JsonToken.String);
            return (string)reader.Value;
        }

        protected void ReadToken(Newtonsoft.Json.JsonReader reader)
        {
            if (!reader.Read())
            {
                var message = "Expected token but none available.";
                throw new Newtonsoft.Json.JsonSerializationException(message);
            }
        }
    }
}
