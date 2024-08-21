using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace SourceGenerators.Generator
{
    [Generator]
    public class DeconstructGenerator : IIncrementalGenerator
    {
        private static bool IsDeconstructAttribute(AttributeSyntax attributeSyntax) => attributeSyntax.Name.ToString() is "Deconstruct";

        private static bool IsDeconstructIgnoreAttribute(AttributeSyntax attributeSyntax) => attributeSyntax.Name.ToString() is "DeconstructIgnore";

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            IncrementalValuesProvider<ClassDeclarationSyntax> classDecl = context.SyntaxProvider.CreateSyntaxProvider(
                predicate: (s, _) => IsSyntaxTargetForGeneration(s),
                transform: (ctx, _) => GetSemanticTargetForGeneration(ctx)
            ).Where(item => item != null);

            IncrementalValueProvider<(Compilation, ImmutableArray<ClassDeclarationSyntax>)> compilationAndClasses
                = context.CompilationProvider.Combine(classDecl.Collect());

            context.RegisterSourceOutput(compilationAndClasses, (spc, source) => Execute(source.Item1, source.Item2, spc));
        }

        private static bool IsSyntaxTargetForGeneration(SyntaxNode node)
            => node is ClassDeclarationSyntax cdcl && cdcl.AttributeLists.Count > 0;

        private static ClassDeclarationSyntax GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
        {
            var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;

            return classDeclarationSyntax.AttributeLists.Any(attr => attr.Attributes.Any(IsDeconstructAttribute))
                ? classDeclarationSyntax
                : null;
        }

        private static void Execute(Compilation compilation, ImmutableArray<ClassDeclarationSyntax> classes, SourceProductionContext context)
        {
            foreach (var classDeclaration in classes)
            {
                // var namespaceDeclaration = classDeclaration.pare.OfType<FileScopedNamespaceDeclarationSyntax>().First();
                var namespaceDeclaration = classDeclaration.Parent as FileScopedNamespaceDeclarationSyntax;
                var namespaceDeclarationAsText = namespaceDeclaration.Name.ToString();

                var modifiers = classDeclaration.Modifiers.ToString();

                var properties = classDeclaration.Members.OfType<PropertyDeclarationSyntax>()
                    .Where(prop => !prop.AttributeLists.Any(attributeList => attributeList.Attributes.Any(IsDeconstructIgnoreAttribute)))
                    .Where(prop => prop.Modifiers.All(modifier => modifier.Text is "public"));

                var propertiesDetails = properties.Select(propertyDeclaration =>
                {
                    var identifier = propertyDeclaration.Identifier.Text;
                    var type = propertyDeclaration.Type.GetText().ToString();

                    return (identifier, type);
                }).ToList();

                var deconstructParamsBuilder = new List<string>();
                var deconstructBodyBuilder = new List<string>();

                for (var index = 0; index < propertiesDetails.Count; index++)
                {
                    var (identifier, type) = propertiesDetails[index];
                    var camelCaseIdentifier = identifier.ToCamelCase();

                    deconstructParamsBuilder.Add($"out {type}{camelCaseIdentifier}");
                    deconstructBodyBuilder.Add($"{(index > 0 ? StringExtensions.SpaceX(8) : string.Empty)}{camelCaseIdentifier} = {identifier};");
                }


                var sourceBuilder = new StringBuilder();

                sourceBuilder.Append($@"
namespace {namespaceDeclarationAsText};

{modifiers} class {classDeclaration.Identifier.Text}
{{
    public void Deconstruct({string.Join(", ", deconstructParamsBuilder)})
    {{
        {string.Join("\r\n", deconstructBodyBuilder)}
    }}
}}"
                );

                context.AddSource($"DeconstructGenerator_{classDeclaration.Identifier.Text}", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
            }
        }
    }
}
