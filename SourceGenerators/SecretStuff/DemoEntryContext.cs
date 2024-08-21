using System.Text.Json.Serialization;

namespace SourceGenerators.SecretStuff;

[JsonSerializable(typeof(DemoEntry))]
[JsonSourceGenerationOptions(WriteIndented = true)]
public partial class DemoEntryContext : JsonSerializerContext
{
}
