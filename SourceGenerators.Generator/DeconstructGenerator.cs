using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace SourceGenerators.Generator
{
    [Generator]
    public class DeconstructGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
// #if DEBUG
//             if (!Debugger.IsAttached)
//             {
//                 Debugger.Launch();
//             }
// #endif
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var syntaxTrees = context.Compilation.SyntaxTrees;
            var treeRootsWithDeconstructAttribute = syntaxTrees.Where(tree => tree.GetRoot().DescendantNodes().OfType<AttributeSyntax>().Any(IsDeconstructAttribute)).Select(tree=>tree.GetRoot());

            foreach (var root in treeRootsWithDeconstructAttribute)
            {
                var usingDirectives = root.DescendantNodes().OfType<UsingDirectiveSyntax>();
                var usingDirectivesAsText = string.Join(System.Environment.NewLine, usingDirectives);

                var namespaceDeclaration = root.DescendantNodes().OfType<FileScopedNamespaceDeclarationSyntax>().First();
                var namespaceDeclarationAsText = namespaceDeclaration.Name.ToString();

                var classesWithDeconstructAttribute = root.DescendantNodes().OfType<ClassDeclarationSyntax>()
                    .Where(classDeclaration => classDeclaration.AttributeLists.Any(attr => attr.Attributes.Any(IsDeconstructAttribute)));

                foreach (var classDeclaration in classesWithDeconstructAttribute)
                {
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
{usingDirectivesAsText}

namespace {namespaceDeclarationAsText};

{modifiers} class {classDeclaration.Identifier.Text}
{{
    public void Deconstruct({string.Join(", ", deconstructParamsBuilder)})
    {{
        {string.Join(System.Environment.NewLine, deconstructBodyBuilder)}
    }}
}}"
                    );

                    context.AddSource($"DeconstructGenerator_{classDeclaration.Identifier.Text}", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
                }
            }
        }

        private static bool IsDeconstructAttribute(AttributeSyntax attributeSyntax) => attributeSyntax.Name.ToString() is "Deconstruct";

        private static bool IsDeconstructIgnoreAttribute(AttributeSyntax attributeSyntax) => attributeSyntax.Name.ToString() is "DeconstructIgnore";
    }
}
