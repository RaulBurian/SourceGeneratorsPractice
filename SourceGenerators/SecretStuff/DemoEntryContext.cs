using System.Text.Json.Serialization;

namespace SourceGenerators.SecretStuff;

[JsonSerializable(typeof(DemoEntry))]
[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Serialization, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, WriteIndented = true)]
public partial class DemoEntryContext : JsonSerializerContext
{
}
