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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using MongoDB.Bson;
using NSubstitute;
using NUnit.Framework;

namespace MongoDB.Integrations.JsonDotNet.Tests
{
    [TestFixture]
    public class JsonDotNetSerializationProviderTests
    {
        [Test]
        public void constructor_should_initialize_instance()
        {
            var result = new JsonDotNetSerializationProvider();

            result.Predicate.Should().NotBeNull();
            result.WrappedSerializer.Should().NotBeNull();
        }

        [Test]
        public void constructor_with_predicate_should_initialize_instance()
        {
            Func<Type, bool> predicate = t => true;

            var result = new JsonDotNetSerializationProvider(predicate: predicate);

            result.Predicate.Should().BeSameAs(predicate);
            result.WrappedSerializer.Should().NotBeNull();
        }

        [Test]
        public void constructor_with_wrappedSerializer_and_predicate__should_initialize_instance()
        {
            Func<Type, bool> predicate = t => true;
            var wrappedSerializer = Substitute.For<Newtonsoft.Json.JsonSerializer>();

            var result = new JsonDotNetSerializationProvider(wrappedSerializer: wrappedSerializer, predicate: predicate);

            result.Predicate.Should().BeSameAs(predicate);
            result.WrappedSerializer.Should().BeSameAs(wrappedSerializer);
        }

        [Test]
        public void constructor_with_wrappedSerializer_should_initialize_instance()
        {
            var wrappedSerializer = Substitute.For<Newtonsoft.Json.JsonSerializer>();

            var result = new JsonDotNetSerializationProvider(wrappedSerializer: wrappedSerializer);

            result.Predicate.Should().NotBeNull();
            result.WrappedSerializer.Should().BeSameAs(wrappedSerializer);
        }

        [Test]
        public void GetSerializer_should_return_null_when_predicate_is_false()
        {
            Func<Type, bool> predicate = t => false;
            var subject = new JsonDotNetSerializationProvider(predicate: predicate);

            var result = subject.GetSerializer(typeof(BsonValue));

            result.Should().BeNull();
        }

        [Test]
        public void GetSerializer_should_return_null_when_type_is_assignable_to_BsonValue()
        {
            Func<Type, bool> predicate = t => true;
            var subject = new JsonDotNetSerializationProvider(predicate: predicate);

            var result = subject.GetSerializer(typeof(BsonValue));

            result.Should().BeNull();
        }

        [Test]
        public void GetSerializer_should_return_serializer_when_predicate_is_true()
        {
            Func<Type, bool> predicate = t => true;
            var subject = new JsonDotNetSerializationProvider(predicate: predicate);

            var result = subject.GetSerializer(typeof(Version));

            result.Should().NotBeNull();
        }

        [Test]
        public void Predicate_get_should_return_expected_result()
        {
            Func<Type, bool> predicate = t => true;
            var subject = new JsonDotNetSerializationProvider(predicate: predicate);

            var result = subject.Predicate;

            result.Should().BeSameAs(predicate);
        }

        [Test]
        public void WrappedSerializer_get_should_return_expected_result()
        {
            var wrappedSerializer = Substitute.For<Newtonsoft.Json.JsonSerializer>();
            var subject = new JsonDotNetSerializationProvider(wrappedSerializer: wrappedSerializer);

            var result = subject.WrappedSerializer;

            result.Should().BeSameAs(wrappedSerializer);
        }
    }
}
