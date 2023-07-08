using Microsoft.CodeAnalysis;

namespace NavProps;

public class NavigationProperty
{
    public ModelClass ContainingClass { get; }
    public IPropertySymbol PropertySymbol { get; }
    public IPropertySymbol? KeyPropertySymbol { get; }

    public string OriginalTypeName { get; } // e.g. `SomeThing?`
    public string MappedTypeName { get; } // e.g. `SomeThing` if non-nullable
    public string PropertyName { get; }
    public string PrivatePropertyName { get; }

    public string? KeyPropertyName { get; }
    public string? KeyPropertyFullTypeName { get; }
    public string? PrivateKeyPropertyName { get; }

    public bool IsNullable { get; }
    public bool IsCollection { get; }

    public NavigationProperty(
        ModelClass containingClass,
        IPropertySymbol propertySymbol,
        IPropertySymbol? keyPropertySymbol
    )
    {
        ContainingClass = containingClass;
        PropertySymbol = propertySymbol;
        KeyPropertySymbol = keyPropertySymbol;

        OriginalTypeName = propertySymbol.Type.ToDisplayString();
        PropertyName = propertySymbol.Name;
        PrivatePropertyName = $"_{PropertyName.LowerFirstLetter()}";

        KeyPropertyName = keyPropertySymbol?.Name;
        KeyPropertyFullTypeName = keyPropertySymbol?.Type.ToDisplayString();

        IsNullable = KeyPropertyFullTypeName != null && KeyPropertyFullTypeName.EndsWith("?");

        // We don't need this unless the property is nullable
        PrivateKeyPropertyName = KeyPropertyName == null || !IsNullable ? null : $"_{KeyPropertyName.LowerFirstLetter()}";

        MappedTypeName = IsNullable || !OriginalTypeName.EndsWith("?")
            ? OriginalTypeName
            : OriginalTypeName.Substring(0, OriginalTypeName.Length - 1);

        IsCollection = propertySymbol.IsCollectionLike();
    }

    public string PrivatePropertyDeclaration => $"private {OriginalTypeName} {PrivatePropertyName};";

    public string? PrivateKeyPropertyDeclaration =>
        KeyPropertyFullTypeName == null || PrivateKeyPropertyName == null
            ? null
            : $"private {KeyPropertyFullTypeName} {PrivateKeyPropertyName};";

    public string GetterDeclaration()
    {
        var propertyOrThrowString =
            $"{PrivatePropertyName} ?? {GetThrowExceptionString(PropertyName, ContainingClass.ClassName)}";

        var returnValueString =
            !IsNullable
                ? propertyOrThrowString
                : $"{PrivateKeyPropertyName} == null ? null : ({propertyOrThrowString})";

        return $"public {MappedTypeName} {PropertyName} =>{Constants.NewLine}{Constants.Indent}{returnValueString};";
    }

    private static string GetThrowExceptionString(string propertyName, string className) =>
        $"throw new NavigationPropertyException(\"{propertyName}\", \"{className}\")";
}
