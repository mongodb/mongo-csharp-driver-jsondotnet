+++
date = "2015-03-17T15:36:56Z"
draft = false
title = "Sample Program"
[menu.main]
  parent = "Getting Started"
  weight = 40
  identifier = "Sample Program"
  pre = "<i class='fa'></i>"
+++

## Sample program using Json.NET

This small sample program lets you see all the code needed to use Json.NET with MongoDB in one small file.

The only things noteworthy about this sample program are that the Employee class is annotated using Json.NET
attributes, and that a JsonDotNetSerializationProvider is registered so that Json.NET will be used to
serialize instances of the Employee class.

{{% note %}}
Note: In your application you would need to write an appropriate predicate that
returns true for all the classes that you want to serialize using Json.NET.
{{% /note %}}

Other than that, the rest of the sample code is no different than if you were not using Json.NET.

```csharp
using System;
using System.Linq;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Integrations.JsonDotNet;
using Newtonsoft.Json;

namespace TestJsonDotNetIntegration
{
    public class Employee
    {
        [JsonProperty("_id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public static class Program
    {
        public static void Main(string[] args)
        {
            // always configure serialization before using any other driver calls
            Func<Type, bool> predicate = t => t.Name == "Employee";
            var provider = new JsonDotNetSerializationProvider(predicate);
            BsonSerializer.RegisterSerializationProvider(provider);

            var client = new MongoClient("mongodb://localhost");
            var database = client.GetDatabase("test");
            var collection = database.GetCollection<Employee>("employees");

            database.DropCollection("employees");
            collection.InsertOne(new Employee { Id = 1, Name = "John Doe" });
            collection.InsertOne(new Employee { Id = 2, Name = "Jane Doe" });

            var employee = collection.AsQueryable().Where(e => e.Id == 2).Single();
            Console.WriteLine($"Id = {employee.Id} Name = {employee.Name}");
        }
    }
}
```
