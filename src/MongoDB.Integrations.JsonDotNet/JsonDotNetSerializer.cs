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
using MongoDB.Integrations.JsonDotNet.Converters;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace MongoDB.Integrations.JsonDotNet
{
    public static class JsonDotNetSerializer
    {
        // private static methods
        public static Newtonsoft.Json.JsonSerializer CreateWrappedSerializer()
        {
            var serializer = new Newtonsoft.Json.JsonSerializer();
            serializer.Converters.Add(BsonValueConverter.Instance);
            serializer.Converters.Add(ObjectIdConverter.Instance);
            return serializer;
        }
    }

    public class JsonDotNetSerializer<TValue> : SerializerBase<TValue>
    {
        // private fields
        private readonly Newtonsoft.Json.JsonSerializer _wrappedSerializer;

        // constructors
        public JsonDotNetSerializer()
            : this(JsonDotNetSerializer.CreateWrappedSerializer())
        {
        }

        public JsonDotNetSerializer(Newtonsoft.Json.JsonSerializer wrappedSerializer)
        {
            if (wrappedSerializer == null)
            {
                throw new ArgumentNullException("wrappedSerializer");
            }

            _wrappedSerializer = wrappedSerializer;
        }

        // public methods
        public override TValue Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var readerAdapter = new JsonReaderAdapter(context.Reader);
            return (TValue)_wrappedSerializer.Deserialize(readerAdapter, args.NominalType);
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, TValue value)
        {
            var writerAdapter = new JsonWriterAdapter(context.Writer);
            _wrappedSerializer.Serialize(writerAdapter, value, args.NominalType);
        }
    }
}
