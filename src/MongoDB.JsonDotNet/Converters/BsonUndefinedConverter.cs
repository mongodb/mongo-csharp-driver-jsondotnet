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
    public class BsonUndefinedConverter : JsonConverterBase<BsonUndefined>
    {
        #region static
        private static readonly BsonUndefinedConverter __instance = new BsonUndefinedConverter();

        public static BsonUndefinedConverter Instance
        {
            get { return __instance; }
        }
        #endregion

        // public methods
        public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case Newtonsoft.Json.JsonToken.Null:
                    return null;

                case Newtonsoft.Json.JsonToken.StartObject:
                    return ReadExtendedJson(reader);

                case Newtonsoft.Json.JsonToken.Undefined:
                    return BsonUndefined.Value;

                default:
                    var message = string.Format("Error reading BsonUndefined. Unexpected token: {0}.", reader.TokenType);
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
                writer.WriteUndefined();
            }
        }

        // private methods
        private BsonUndefined ReadExtendedJson(Newtonsoft.Json.JsonReader reader)
        {
            ReadExpectedPropertyName(reader, "$undefined");
            reader.Skip();
            ReadEndObject(reader);

            return BsonUndefined.Value;
        }
    }
}
