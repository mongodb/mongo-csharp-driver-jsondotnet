+++
date = "2015-03-17T15:36:56Z"
draft = false
title = "Quick Tour"
[menu.main]
  parent = "Getting Started"
  weight = 40
  identifier = "Quick Tour"
  pre = "<i class='fa'></i>"
+++

## .NET Driver Json.NET Integration Quick Tour

The .NET Driver Json.NET Integration package allows you to use Json.NET to serialize and deserialize
documents (or fields of documents).

To use Json.NET with the .NET Driver you need to:

- Add a reference to the `MongoDB.Integrations.JsonDotNet` NuGet package
- Configure serialization so that Json.NET based serializers are used where desired

There are several ways you could configure the .NET Driver to use Json.NET serializers. They
are described below.

## The simplest way to use Json.NET serializers

In most cases, the following will be sufficient to configure the .NET Driver to use Json.NET serialization:

```csharp
Func<Type, bool> predicate; // you need to write this predicate 
var serializationProvider = new JsonDotNetSerializationProvider(predicate);
BsonSerializer.RegisterSerializationProvider(serializationProvider);
```

You need to write the predicate, which should return true for any class that you want to serialize using
Json.NET, and false for any class you want serialized some other way.

For example, suppose you want to use Json.NET to serialize all classes that are in the "MyApplication" namespace.
The predicate for that would be:

```csharp
Func<Type, bool> predicate = t => t.FullName.StartsWith("MyApplication.");
```

It might help to have a mental model of how the .NET Driver chooses which serializer to use for a type.

When the driver needs to serializer for a type `T`, it consults the global serializer registry to see if it
already has a serializer registered for type `T`. If it does, it uses that one.

Otherwise it needs to create a serializer for `T`. It does that by consulting a list of serialization providers
one a time until one of the providers returns the required serializer. As soon as one of the serialization providers
returns the required serializer, no further serialization providers are consulted, and the returned
serializer is registered for future use, so any subsequent requests for a serializer for `T` will be 
satisfied from the global serializer registry.

What the above code is doing is registering an additional serialization provider that returns a Json.NET
based serializer for any type for which the predicate returns true. 

## Manually configuring a `JsonSerializerAdapter`

A Json.NET serializer is an instance of a `JsonSerializer`. The .NET Driver expects a serializer for type `T`
to implement the `IBsonSerializer<T>` interface, and of course a `JsonSerializer` does not implement that
interface. In order to use a `JsonSerializer` with the .NET Driver, it needs to be wrapped in an instance
of a `JsonSerializerAdapter<T>`. A `JsonSerializerAdapter<T>` implements the `IBsonSerializer<T>` interface
that is required by the .NET Driver, and handles adapting the two serialization machineries to each other.

To manually configure a Json.NET based serializer for a class called `MyClass` you write:

```csharp
var jsonSerializer = new JsonSerializer();
var myClassSerializer = new JsonSerializerAdapter<MyClass>(jsonSerializer);
BsonSerializer.RegisterSerializer<MyClass>(myClassSerializer);
```

In this case we are not using the `JsonDotNetSerializationProvider`; we are simply instantiating a serializer
directly and registering it.

The code above assumes that the `JsonSerializer` doesn't require any additional configuration, but sometimes
you do need some additional configuration. For example, Json.NET doesn't handle the .NET Driver's `ObjectId`
type natively, so if your classes have any `ObjectId(s)` you will need to configure the `JsonSerializer` to
handle `ObjectId(s)`, as follows:

```csharp
var jsonSerializer = new JsonSerializer();
jsonSerializer.Converters.Add(ObjectIdConverter.Instance);
var myClassSerializer = new JsonSerializerAdapter<MyClass>(jsonSerializer);
BsonSerializer.RegisterSerializer<MyClass>(myClassSerializer);
```

Note that while a .NET Driver serializer is for a specific type `T`, a `JsonSerializer` can actually be used
for any number of types. What this means is that you can use the same instance of a `JsonSerializer` with
multiple `JsonSerializerAdapters`. So if you have two classes, `MyClass1` and `MyClass2`, they can share
a single underlying `JsonSerializer`.

```csharp
var jsonSerializer = new JsonSerializer();
jsonSerializer.Converters.Add(ObjectIdConverter.Instance);
var myClass1Serializer = new JsonSerializerAdapter<MyClass1>(jsonSerializer);
var myClass2Serializer = new JsonSerializerAdapter<MyClass2>(jsonSerializer);
BsonSerializer.RegisterSerializer<MyClass1>(myClass1Serializer);
BsonSerializer.RegisterSerializer<MyClass2>(myClass2Serializer);
```

If your classes have any fields or properties of type BsonValue (or any subclass of BsonValue) you will need to
register a converter for BsonValue as well:

```csharp
var jsonSerializer = new JsonSerializer();
jsonSerializer.Converters.Add(ObjectIdConverter.Instance);
jsonSerializer.Converters.Add(BsonValueConverter.Instance);
```
