using System.Text.Json;
using SourceGenerators;
using SourceGenerators.SecretStuff;
using ComplexType = SourceGenerators.SecretStuff.ComplexType;
using DemoEntry = SourceGenerators.SecretStuff.DemoEntry;
using DemoEntryContext = SourceGenerators.SecretStuff.DemoEntryContext;

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

#region Json Source Generator

Console.WriteLine(JsonSerializer.Serialize(entry, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true }));

Console.WriteLine("--------------------------------------");
Console.WriteLine(JsonSerializer.Serialize(entry, DemoEntryContext.Default.DemoEntry));
Console.WriteLine("--------------------------------------");
Console.WriteLine(JsonSerializer.Serialize(entry.ComplexObject, DemoEntryContext.Default.ComplexType));
Console.WriteLine("--------------------------------------");

#endregion

#region Enums

var myEnum = MyEnum.Three;

Console.WriteLine(myEnum);
Console.WriteLine(myEnum.ToStringFast());

if (Enum.TryParse("Four", out MyEnum four))
{
    Console.WriteLine(four);
}

if (MyEnumExtensions.TryParse("Four", out MyEnum fourFast))
{
    Console.WriteLine(fourFast);
}

if (Enum.IsDefined(typeof(MyEnum), (MyEnum)2))
{
    Console.WriteLine(2);
}

if (MyEnumExtensions.IsDefined((MyEnum)2))
{
    Console.WriteLine("2 fast");
}

#endregion

#region Our own

var (id, name, effectiveDate, grade, aList, complexObject) = entry;

foreach (var i in ..5)
{
    Console.WriteLine(JsonSerializer.Serialize(new { id, name, effectiveDate, grade, aList, complexObject }));
}

#endregion
