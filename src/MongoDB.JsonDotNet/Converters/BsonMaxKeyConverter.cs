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
using MongoDB.Bson;

namespace MongoDB.JsonDotNet.Converters
{
    public class BsonMaxKeyConverter : JsonConverterBase<BsonMaxKey>
    {
        #region static
        private static readonly BsonMaxKeyConverter __instance = new BsonMaxKeyConverter();

        public static BsonMaxKeyConverter Instance
        {
            get { return __instance; }
        }
        #endregion

        // public methods
        public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var adapter = reader as JsonReaderAdapter;
            if (adapter != null && adapter.BsonValue != null && adapter.BsonValue.BsonType == BsonType.MaxKey)
            {
                return (BsonMaxKey)adapter.BsonValue;
            }

            switch (reader.TokenType)
            {
                case Newtonsoft.Json.JsonToken.Null:
                    return null;

                case Newtonsoft.Json.JsonToken.StartObject:
                    return ReadExtendedJson(reader);

                default:
                    var message = string.Format("Error reading BsonMaxKey. Unexpected token: {0}.", reader.TokenType);
                    throw new Newtonsoft.Json.JsonReaderException(message);
            }
        }

        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                var adapter = writer as JsonWriterAdapter;
                if (adapter != null)
                {
                    adapter.WriteMaxKey();
                }
                else
                {
                    WriteExtendedJson(writer);
                }
            }
        }

        // private methods
        private BsonMaxKey ReadExtendedJson(Newtonsoft.Json.JsonReader reader)
        {
            ReadExpectedPropertyName(reader, "$maxKey");
            reader.Skip();
            ReadEndObject(reader);

            return BsonMaxKey.Value;
        }

        private void WriteExtendedJson(Newtonsoft.Json.JsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("$maxKey");
            writer.WriteValue(1);
            writer.WriteEndObject();
        }
    }
}
