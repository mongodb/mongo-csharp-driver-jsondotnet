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
using MongoDB.Integrations.JsonDotNet.Converters;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson;
using System.Reflection;
using Newtonsoft.Json.Serialization;
using System.Linq;
using System.Collections.Generic;

namespace MongoDB.Integrations.JsonDotNet
{
    public static class JsonSerializerAdapter
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

    public class JsonSerializerAdapter<TValue> : SerializerBase<TValue>, IBsonArraySerializer, IBsonDocumentSerializer
    {
        // private fields
        private readonly Newtonsoft.Json.JsonSerializer _wrappedSerializer;

        // constructors
        public JsonSerializerAdapter()
            : this(JsonSerializerAdapter.CreateWrappedSerializer())
        {
        }

        public JsonSerializerAdapter(Newtonsoft.Json.JsonSerializer wrappedSerializer)
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
            var readerAdapter = new BsonReaderAdapter(context.Reader);
            return (TValue)_wrappedSerializer.Deserialize(readerAdapter, args.NominalType);
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, TValue value)
        {
            var writerAdapter = new BsonWriterAdapter(context.Writer);
            _wrappedSerializer.Serialize(writerAdapter, value, args.NominalType);
        }

        public bool TryGetItemSerializationInfo(out BsonSerializationInfo serializationInfo)
        {
            var valueType = typeof(TValue);

            var contract = _wrappedSerializer.ContractResolver.ResolveContract(valueType);
            var arrayContract = contract as JsonArrayContract;
            if (arrayContract == null)
            {
                throw new BsonSerializationException($"The Json.NET contract for type \"{valueType.Name}\" is not a JsonArrayContract.");
            }
            if (arrayContract.Converter != null)
            {
                throw new BsonSerializationException($"The Json.NET contract for type \"{valueType.Name}\" has a Converter and JsonConverters are opaque.");
            }
            if (arrayContract.IsReference ?? false)
            {
                throw new BsonSerializationException($"The Json.NET contract for type \"{valueType.Name}\" is serialized as a reference.");
            }
            if (arrayContract.ItemConverter != null)
            {
                throw new BsonSerializationException($"The Json.NET contract for type \"{valueType.Name}\" has an ItemConverter and JsonConverters are opaque.");
            }

            var itemType = arrayContract.CollectionItemType;
            var itemSerializerType = typeof(JsonSerializerAdapter<>).MakeGenericType(itemType);
            var itemSerializer = (IBsonSerializer)Activator.CreateInstance(itemSerializerType, _wrappedSerializer);

            serializationInfo = new BsonSerializationInfo(null, itemSerializer, nominalType: itemType);
            return true;
        }

        public bool TryGetMemberSerializationInfo(string memberName, out BsonSerializationInfo serializationInfo)
        {
            serializationInfo = null;

            var valueType = typeof(TValue);
            var contract = _wrappedSerializer.ContractResolver.ResolveContract(valueType);
            var objectContract = contract as JsonObjectContract;
            if (objectContract == null)
            {
                throw new BsonSerializationException($"The Json.NET contract for type \"{valueType.Name}\" is not a JsonObjectContract.");
            }
            if (objectContract.Converter != null)
            {
                throw new BsonSerializationException($"The Json.NET contract for type \"{valueType.Name}\" has a JsonConverter and JsonConverters are opaque.");
            }
            if (objectContract.IsReference ?? false)
            {
                throw new BsonSerializationException($"The Json.NET contract for type \"{valueType.Name}\" is serialized as a reference.");
            }

            var property = objectContract.Properties.FirstOrDefault(p => p.UnderlyingName == memberName);
            if (property == null)
            {
                return false;
            }
            var elementName = property.PropertyName;

            Type memberType;
            if (!TryGetMemberType(valueType, memberName, out memberType))
            {
                return false;
            }

            var memberSerializerType = typeof(JsonSerializerAdapter<>).MakeGenericType(memberType);
            var memberSerializer = (IBsonSerializer)Activator.CreateInstance(memberSerializerType, _wrappedSerializer);

            serializationInfo = new BsonSerializationInfo(elementName, memberSerializer, nominalType: memberType);
            return true;
        }

        private bool TryGetMemberType(Type type, string memberName, out Type memberType)
        {
            memberType = null;

            var memberInfos = type.GetMember(memberName);
            if (memberInfos.Length != 1)
            {
                return false;
            }
            var memberInfo = memberInfos[0];

            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field: memberType = ((FieldInfo)memberInfo).FieldType; break;
                case MemberTypes.Property: memberType = ((PropertyInfo)memberInfo).PropertyType; break;
                default: throw new BsonSerializationException($"Unsupported member type \"{memberInfo.MemberType}\" for member: {memberName}.");
            }

            return true;
        }
    }
}
