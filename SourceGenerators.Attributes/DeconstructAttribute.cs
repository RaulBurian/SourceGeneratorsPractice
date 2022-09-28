namespace SourceGenerators.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class DeconstructAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Property)]
public class DeconstructIgnoreAttribute : Attribute
{
}
