using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace NavProps;

public class ModelClassFinder
{
    private readonly GeneratorExecutionContext context;

    public ModelClassFinder(GeneratorExecutionContext context)
    {
        this.context = context;
    }

    public List<INamedTypeSymbol> GetEntityFrameworkModelClasses()
    {
        var dataContextSymbolVisitor = new DataContextSymbolVisitor();
        dataContextSymbolVisitor.Visit(context.Compilation.GlobalNamespace);
        var dataContextSymbols = dataContextSymbolVisitor.GetDataContextSymbols();

        return dataContextSymbols
            .SelectMany(dataContextSymbol =>
                dataContextSymbol
                    .GetPublicProperties()
                    .Where(property => property.Type is INamedTypeSymbol { IsGenericType: true, Name: "DbSet" })
                    .Select(property => (INamedTypeSymbol)((INamedTypeSymbol)property.Type).TypeArguments.Single())
            )
            .ToList();
    }
}
