using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace NavProps;

public class ModelClass
{
    public INamedTypeSymbol NamedTypeSymbol { get; }
    public string ClassName { get; }
    public string ClassTypeName { get; }

    public ModelClass(INamedTypeSymbol namedTypeSymbol)
    {
        NamedTypeSymbol = namedTypeSymbol;
        ClassName = namedTypeSymbol.Name;
        ClassTypeName = namedTypeSymbol.ToDisplayString();
    }

    public string GetConstructorDeclaration(List<NavigationProperty> navigationProperties)
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.AppendLine($"public {ClassName}NavProps({ClassTypeName} {ClassName.LowerFirstLetter()})");
        stringBuilder.AppendLine("{");

        foreach (var property in navigationProperties)
        {
            if (property.PrivateKeyPropertyName != null && property.KeyPropertyName != null)
            {
                stringBuilder.AppendLine(
                    $"{Constants.Indent}{property.PrivateKeyPropertyName} = {ClassName.LowerFirstLetter()}.{property.KeyPropertyName};"
                );
            }

            stringBuilder.AppendLine(
                $"{Constants.Indent}{property.PrivatePropertyName} = {ClassName.LowerFirstLetter()}.{property.PropertyName};"
            );
        }

        stringBuilder.AppendLine("}");

        return stringBuilder.ToString();
    }
}
