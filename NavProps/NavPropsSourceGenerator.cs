﻿using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace NavProps
{
    [Generator]
    public class NavPropsSourceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context) {}

        public void Execute(GeneratorExecutionContext context)
        {
            context.AddSource("NavigationPropertyException.g.cs", NavigationPropertyExceptionSourceCode);

            var stringBuilder = new StringBuilder();

            var entityFrameworkModelFinder = new ModelClassFinder(context);
            var entityFrameworkModelClasses = entityFrameworkModelFinder.GetEntityFrameworkModelClasses();
            var databaseModelTypeNames = entityFrameworkModelClasses.Select(c => c.Name).ToList();

            foreach (var databaseModelClass in entityFrameworkModelClasses)
            {
                ProcessDatabaseModelClass(databaseModelClass);
            }

            void ProcessDatabaseModelClass(INamedTypeSymbol databaseModelClass)
            {
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