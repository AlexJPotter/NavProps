using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace NavProps;

public static class NamedTypeSymbolExtensions
{
    public static IEnumerable<IPropertySymbol> GetPublicProperties(this INamedTypeSymbol targetClass) =>
        targetClass.GetMembers()
            .Where(member => member.Kind == SymbolKind.Property)
            .Where(member => member.DeclaredAccessibility == Accessibility.Public)
            .Select(member => (IPropertySymbol) member);

    public static IEnumerable<IPropertySymbol> GetPropertiesWithTypeInList(this INamedTypeSymbol targetClass, List<string> typeNames) =>
        targetClass.GetPublicProperties()
            .Where(property => property.Type is INamedTypeSymbol)
            .Where(property =>
                property.Type is INamedTypeSymbol namedTypeSymbol &&
                (typeNames.Contains(namedTypeSymbol.Name) ||
                 (
                     IsCollectionLike(namedTypeSymbol) &&
                     typeNames.Contains(namedTypeSymbol.TypeArguments.Single().Name)
                 ))
            );

    public static bool IsCollectionLike(this INamedTypeSymbol namedTypeSymbol)
    {
        var collectionTypeNames = new[]
        {
            "ICollection",
            "IList",
            "List",
            // TODO - Full list
        };

        return collectionTypeNames.Contains(namedTypeSymbol.Name);
    }
}
