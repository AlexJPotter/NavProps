using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NavProps;

public static class SyntaxExtensions
{
    private static IEnumerable<PropertyDeclarationSyntax> GetDbSetProperties(this ClassDeclarationSyntax classDeclarationSyntax) =>
        classDeclarationSyntax.Members
            .OfType<PropertyDeclarationSyntax>()
            .Where(property => property.Type is GenericNameSyntax { Identifier.Text: "DbSet" });

    public static IEnumerable<string> GetEntityClassNames(this ClassDeclarationSyntax classDeclarationSyntax) =>
        classDeclarationSyntax.GetDbSetProperties()
            .Select(property => ((GenericNameSyntax)property.Type).TypeArgumentList.Arguments.First().ToString());

    // TODO: This is potentially a bit flaky - can we avoid using a magic string?
    public static bool IsDataContextClass(this ClassDeclarationSyntax? classDeclarationSyntax) =>
        classDeclarationSyntax?.BaseList?.Types.Any(type => type.Type.ToString() == "DbContext") ?? false;

    public static bool IsPublicProperty(this PropertyDeclarationSyntax propertyDeclarationSyntax) =>
        propertyDeclarationSyntax.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PublicKeyword));

    public static bool IsStatic(this PropertyDeclarationSyntax propertyDeclarationSyntax) =>
        propertyDeclarationSyntax.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.StaticKeyword));

    public static bool IsStatic(this ClassDeclarationSyntax classDeclarationSyntax) =>
        classDeclarationSyntax.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.StaticKeyword));
}
