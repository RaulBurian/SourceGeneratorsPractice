namespace SourceGenerators.Generator
{
    public static class StringExtensions
    {
        public static string ToCamelCase(this string self)
        {
            return $"{self[0].ToString().ToLowerInvariant()}{self.Substring(1)}";
        }

        public static string SpaceX(int x)
        {
            return new string(' ', x);
        }
    }
}
