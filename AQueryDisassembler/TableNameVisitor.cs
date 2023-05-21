using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace AQueryDisassembler;

internal class TableNameVisitor : TSqlFragmentVisitor
{
    public List<string> TableNames { get; } = new List<string>();

    public override void Visit(NamedTableReference node)
    {
        if (node.SchemaObject != null)
        {
            TableNames.Add(node.SchemaObject.BaseIdentifier.Value);
        }

        base.Visit(node);
    }
}
