using Microsoft.CodeAnalysis;

namespace NavProps;

public static class PropertySymbolExtensions
{
    public static bool IsCollectionLike(this IPropertySymbol property) =>
        property.Type is INamedTypeSymbol namedTypeSymbol &&
        namedTypeSymbol.IsCollectionLike();
}
