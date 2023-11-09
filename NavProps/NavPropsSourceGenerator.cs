using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace NavProps
{
    [Generator]
    public class NavPropsSourceGenerator : IIncrementalGenerator
    {
        private List<string> _entityClassNames = new();

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.RegisterPostInitializationOutput(ctx =>
                ctx.AddSource(
                    "NavigationPropertyException.g.cs",
                    SourceText.From(NavigationPropertyExceptionSourceCode, Encoding.UTF8)
                )
            );

            var classDeclarations = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: (syntaxNode, _) => IsSyntaxTargetForGeneration(syntaxNode),
                    transform: (generatorSyntaxContext, _) => GetSemanticTargetForGeneration(generatorSyntaxContext)
                )
                .Where(syntax => syntax is not null);

            var compilationAndClasses = context.CompilationProvider.Combine(classDeclarations.Collect());

            context.RegisterSourceOutput(
                compilationAndClasses,
                (spc, source) => Execute(source.Item1, source.Item2, spc)
            );
        }



        private bool IsSyntaxTargetForGeneration(SyntaxNode syntaxNode)
        {
            // Return false if the syntax node is not a class
            if (syntaxNode is not ClassDeclarationSyntax classDeclarationSyntax)
                return false;

            // Return false if the class is static
            if (classDeclarationSyntax.IsStatic())
                return false;

            // Return true if the class inherits from DbContext
            if (classDeclarationSyntax.IsDataContextClass())
            {
                _entityClassNames.AddRange(classDeclarationSyntax.GetEntityClassNames());
                _entityClassNames = _entityClassNames.Distinct().ToList();
                return true;
            }

            // Return true if the class has any public properties that are not static
            return classDeclarationSyntax.Members
                .OfType<PropertyDeclarationSyntax>()
                .Any(property => property.IsPublicProperty() && !property.IsStatic());
        }

        private static ClassDeclarationSyntax GetSemanticTargetForGeneration(GeneratorSyntaxContext context) =>
            (ClassDeclarationSyntax) context.Node;

        private void Execute(
            Compilation compilation,
            ImmutableArray<ClassDeclarationSyntax> classes,
            SourceProductionContext context
        )
        {
            var dataContextClasses =
                classes
                    .Where(c => c.IsDataContextClass())
                    .ToList();

            var dataContextSymbols = dataContextClasses
                .Select(dataContextClass => compilation.GetSemanticModel(dataContextClass.SyntaxTree).GetDeclaredSymbol(dataContextClass));

            var entityFrameworkModelClasses =
                dataContextSymbols
                    .Where(dataContextSymbol => dataContextSymbol != null)
                    .SelectMany(dataContextSymbol =>
                        dataContextSymbol!
                            .GetPublicProperties()
                            .Where(property => property.Type is INamedTypeSymbol { IsGenericType: true, Name: "DbSet" })
                            .Select(property => (INamedTypeSymbol)((INamedTypeSymbol)property.Type).TypeArguments.Single())
                    )
                    .ToList();

            var databaseModelTypeNames =
                entityFrameworkModelClasses
                    .Select(modelClass => modelClass.Name)
                    .Distinct()
                    .ToList();

            foreach (var classDeclaration in classes)
            {
                if (classDeclaration.IsDataContextClass()) continue;

                var semanticModel = compilation.GetSemanticModel(classDeclaration.SyntaxTree);
                var classSymbol = semanticModel.GetDeclaredSymbol(classDeclaration);
                if (classSymbol is null || !databaseModelTypeNames.Contains(classSymbol.Name)) continue;

                ProcessDatabaseModelClass(classSymbol, databaseModelTypeNames, context);
            }
        }

        private static void ProcessDatabaseModelClass(
            INamedTypeSymbol databaseModelClass,
            List<string> databaseModelTypeNames,
            SourceProductionContext context
        )
        {
            var stringBuilder = new StringBuilder();

            var modelClass = new ModelClass(databaseModelClass);
            var navigationPropertyFinder = new NavigationPropertyFinder(modelClass);
            var navigationProperties = navigationPropertyFinder.FindNavigationProperties(databaseModelTypeNames);

            if (!navigationProperties.Any()) return;

            stringBuilder.AppendLine("// <auto-generated />");
            stringBuilder.AppendLine("#nullable enable");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine($"public class {modelClass.ClassName}NavProps");
            stringBuilder.AppendLine("{");

            foreach (var property in navigationProperties)
            {
                if (property.PrivateKeyPropertyDeclaration != null)
                    stringBuilder.AppendLine(Constants.Indent + property.PrivateKeyPropertyDeclaration);

                stringBuilder.AppendLine(Constants.Indent + property.PrivatePropertyDeclaration);
                stringBuilder.AppendLine();
            }

            stringBuilder.AppendLine(modelClass.GetConstructorDeclaration(navigationProperties).Indent(1));

            foreach (var property in navigationProperties)
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendLine(property.GetterDeclaration().Indent(1));
            }

            stringBuilder.AppendLine("}");

            var source = stringBuilder.ToString();
            context.AddSource($"{modelClass.ClassName}NavProps.g.cs", source);
            stringBuilder.Clear();

            stringBuilder.AppendLine("// <auto-generated />");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine($"public static class {modelClass.ClassName}Extensions");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine(
                Constants.Indent +
                $"public static {modelClass.ClassName}NavProps NavProps("
            );
            stringBuilder.AppendLine(
                Constants.Indent + Constants.Indent +
                $"this {modelClass.ClassTypeName} {modelClass.ClassName.LowerFirstLetter()}"
            );
            stringBuilder.AppendLine(Constants.Indent + ") =>");
            stringBuilder.AppendLine(
                Constants.Indent + Constants.Indent +
                $"new {modelClass.ClassName}NavProps" +
                $"({modelClass.ClassName.LowerFirstLetter()});"
            );
            stringBuilder.AppendLine("}");
            source = stringBuilder.ToString();
            context.AddSource($"{modelClass.ClassName}Extensions.g.cs", source);
            stringBuilder.Clear();
        }

        private static string NavigationPropertyExceptionSourceCode =>
@"// <auto-generated />
#nullable enable

using System;

public class NavigationPropertyException : Exception
{
    public NavigationPropertyException(string propertyName, string className)
        : base($""Navigation property '{propertyName}' on class '{className}' was not loaded"") { }
}
";
    }
}
