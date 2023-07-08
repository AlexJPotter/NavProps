using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace NavProps;

public class DataContextSymbolVisitor : SymbolVisitor
{
    private readonly IList<INamedTypeSymbol> dataContextSymbols = new List<INamedTypeSymbol>();

    public IReadOnlyCollection<INamedTypeSymbol> GetDataContextSymbols() => dataContextSymbols.ToList().AsReadOnly();

    public override void VisitNamespace(INamespaceSymbol symbol)
    {
        Parallel.ForEach(symbol.GetMembers(), s => s.Accept(this));
    }

    public override void VisitNamedType(INamedTypeSymbol symbol)
    {
        if (symbol.BaseType?.Name == "DbContext")
        {
            dataContextSymbols.Add(symbol);
        }

        foreach (var childSymbol in symbol.GetTypeMembers())
        {
            base.Visit(childSymbol);
        }
    }
}
