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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace MongoDB.Integrations.JsonDotNet
{
    public class JsonDotNetSerializationProvider : IBsonSerializationProvider
    {
        // private fields
        private readonly Func<Type, bool> _predicate;
        private readonly Newtonsoft.Json.JsonSerializer _wrappedSerializer;

        // constructors
        public JsonDotNetSerializationProvider(Newtonsoft.Json.JsonSerializer wrappedSerializer = null, Func<Type, bool> predicate = null)
        {
            _wrappedSerializer = wrappedSerializer ?? JsonSerializerAdapter.CreateWrappedSerializer();
            _predicate = predicate ?? (t => true);
        }

        // public properties
        public Func<Type, bool> Predicate
        {
            get { return _predicate; }
        }
        
        public Newtonsoft.Json.JsonSerializer WrappedSerializer
        {
            get { return _wrappedSerializer; }
        }

        // public methods
        public IBsonSerializer GetSerializer(Type type)
        {
            if (!_predicate(type) || typeof(BsonValue).IsAssignableFrom(type))
            {
                return null;
            }

            var serializerType = typeof(JsonSerializerAdapter<>).MakeGenericType(type);
            var constructorInfo = serializerType.GetConstructor(new Type[] { typeof(Newtonsoft.Json.JsonSerializer) });
            var serializer = (IBsonSerializer)constructorInfo.Invoke(new object[] { _wrappedSerializer });
            return serializer;
        }
    }
}
