using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace NavProps;

public class NavigationPropertyFinder
{
    private readonly ModelClass modelClass;

    public NavigationPropertyFinder(ModelClass modelClass)
    {
        this.modelClass = modelClass;
    }

    public List<NavigationProperty> FindNavigationProperties(List<string> modelClassTypeNames)
    {
        var propertySymbols =
            modelClass.NamedTypeSymbol.GetPropertiesWithTypeInList(modelClassTypeNames)
                .Where(property => property.SetMethod != null);

        return
            propertySymbols
                .Select(propertySymbol =>
                    new NavigationProperty(
                        containingClass: modelClass,
                        propertySymbol: propertySymbol,
                        keyPropertySymbol: GetKeyProperty(propertySymbol)
                    )
                )
                .ToList();
    }

    private IPropertySymbol? GetKeyProperty(IPropertySymbol propertySymbol)
    {
        if (propertySymbol.IsCollectionLike()) return null;

        var publicProperties = modelClass.NamedTypeSymbol.GetPublicProperties();
        var keyPropertyName = propertySymbol.Name + "Id";
        var idProperty = publicProperties.SingleOrDefault(p => p.Name == keyPropertyName);
        return idProperty;
    }
}
