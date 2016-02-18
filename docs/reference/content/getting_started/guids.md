+++
date = "2015-03-17T15:36:56Z"
draft = false
title = "Using Guids"
[menu.main]
  parent = "Getting Started"
  weight = 50
  identifier = "Using Guids"
  pre = "<i class='fa'></i>"
+++

## Notes on Using Guids with Json.NET

Json.NET does not handle BSON encoded Guids correctly by itself, but if you are using the
.NET Driver Json.NET Integration Guids are handled correctly.

Historically drivers encoded Guids as binary BSON values with a binary sub type of 3. However, 
different drivers accidentally ended up writing the bytes of the Guid in different orders. When
this was discovered, a new binary sub type of 4 was introduced with a well-defined byte order
that all drivers would agree on.

Assuming the following Guid value:

```csharp
var value = Guid.Parse("01020304-0506-0708-090a-0b0c0d0e0f10");
```

The actual binary value written out could be any of the following depending on which driver
was used and how it was configured:

| Length | Sub type | Bytes                               | GuidRepresentation |
| ------ | -------- | ----------------------------------- | ------------------ |
| 16     | 3        | 04030201 06050807 090a0b0c 0d0e0f10 | CSharpLegacy       |
| 16     | 3        | 08070605 04030201 100f0e0d 0c0b0a09 | JavaLegacy         |
| 16     | 3        | 01020304 05060708 090a0b0c 0d0e0f10 | PythonLegacy       |
| 16     | 4        | 01020304 05060708 090a0b0c 0d0e0f10 | Standard           |

Note that when sub type 3 is used the actual byte order is not defined by the BSON spec,
but rather by the context in which the data appears (i.e. which driver was used to write the
data and how that driver was configured).

Json.NET does not provide any way to configure how Guids are represented, and therefore when
used by itself Json.NET is not suitable for reading or writing BSON documents that contain Guids.

However, when Json.NET is used via a JsonSerializerAdapter, the representation for Guids can
be determined from the settings of the Bson reader/writer used. When reading or writing documents from a
MongoDB collection, the GuidRepresentation property of the reader/writer settings is automatically set
from the GuidRepresentation property of the MongoCollectionSettings.

If you are serializing directly, you can configure the GuidRepresentation like this:

```csharp
public class C
{
    public Guid G;
}

public static class Program
{
    public static void Main(string[] args)
    {
        var c = new C { G = Guid.Parse("01020304-0506-0708-090a-0b0c0d0e0f10") };

        var jsonSerializer = new JsonSerializer();
        var serializer = new JsonSerializerAdapter<C>(jsonSerializer);
        var bsonWriterSettings = new BsonBinaryWriterSettings
        {
            GuidRepresentation = GuidRepresentation.CSharpLegacy
        };
        using (var memoryStream = new MemoryStream())
        using (var bsonWriter = new BsonBinaryWriter(memoryStream, bsonWriterSettings))
        {
            var context = BsonSerializationContext.CreateRoot(bsonWriter);
            serializer.Serialize(context, c);
            var bson = memoryStream.ToArray();
        }
    }
}
```
