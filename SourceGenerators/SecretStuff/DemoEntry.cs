using SourceGenerators.Attributes;

namespace SourceGenerators.SecretStuff;

[Deconstruct]
public partial class DemoEntry
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public DateTime EffectiveDate { get; set; }

    public int Grade { get; set; }

    public IList<string> AList { get; set; }

    public ComplexType ComplexObject { get; set; }
}

[Deconstruct]
public partial class ComplexType
{
    public string OneProp { get; set; }

    public string AnotherProp { get; set; }
}
