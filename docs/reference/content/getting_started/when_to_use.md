+++
date = "2015-03-17T15:36:56Z"
draft = false
title = "When to use Json.NET"
[menu.main]
  parent = "Getting Started"
  weight = 30
  identifier = "When To Use Json.NET"
  pre = "<i class='fa'></i>"
+++

## When to use Json.NET

The .NET Driver includes full featured support for serialization, and most users will never need anything else beyond that.

However, some users may wish to use Json.NET as an alternative to the .NET Driver's built-in serialization support.

Reasons for wanting to use Json.NET could be:

1. You are already using Json.NET for other reasons and want to standardize on a single approach
2. Json.NET has some feature that you need that is not provided by the .NET Driver
3. Json.NET might perform better in some situations (be sure to verify using benchmarks)

Whatever your reason for wanting to use Json.NET, this NuGet package makes it possible for you to do so.
