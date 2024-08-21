using System.Text.Json.Serialization;
using SourceGenerators.Attributes;

namespace SourceGenerators.SecretStuff;

[Deconstruct]
public partial class DemoEntry
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    [DeconstructIgnore]
    public DateTime EffectiveDate { get; set; }

    public int Grade { get; set; }

    public IList<string> AList { get; set; }

    public ComplexType ComplexObject { get; set; }
}

[Deconstruct]
public partial class ComplexType
{
    [JsonPropertyName("oneProp")]
    public string OneProp { get; set; }

    [JsonPropertyName("another")]
    public string AnotherProp { get; set; }
}
