// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using SourceGenerators;

var entry = new DemoEntry
{
    Id = Guid.NewGuid(),
    Name = "Demo",
    EffectiveDate = DateTime.UtcNow,
    Grade = 10,
    AList = new List<string> { "Some Text" },
    ComplexObject = new ComplexType
    {
        OneProp = "Prop",
        AnotherProp = "Another",
    },
};

var (id, name, effectiveDate, grade, aList, complexObject) = entry;

Console.WriteLine(JsonSerializer.Serialize(new { id, name, effectiveDate, grade, aList, complexObject }, new JsonSerializerOptions { WriteIndented = true }));
