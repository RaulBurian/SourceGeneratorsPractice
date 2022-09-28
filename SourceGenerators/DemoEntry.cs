using SourceGenerators.Attributes;

namespace SourceGenerators;

[Deconstruct]
public partial class DemoEntry
{
    public Guid Id { get; init; }

    public string Name { get; init; }

    public DateTime EffectiveDate { get; init; }

    public int Grade { get; init; }

    public IList<string> AList { get; init; }

    public ComplexType ComplexObject { get; init; }
}

[Deconstruct]
public partial class ComplexType
{
    public string OneProp { get; init; }

    public string AnotherProp { get; init; }
}
