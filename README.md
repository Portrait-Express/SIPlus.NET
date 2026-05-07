# SIPlus.NET

C# for [SIPlus templating library](https://github.com/Portrait-Express/SIPlus).

## Installation
- Install from NuGet

## Usage
```cs
using SIPlus;

var parser = new Parser();
var template = parser.GetInterpolation("Hello, { .foo }!");

var context = parser.Context().Builder().Default(new { foo = "world" }).Build();
var result = template.Construct(context);  // Hello, world!
```
