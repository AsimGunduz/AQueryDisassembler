using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace AQueryDisassembler;

internal class FieldNameVisitor : TSqlFragmentVisitor
{
    public List<string> FieldNames { get; } = new List<string>();

    public override void Visit(ColumnReferenceExpression node)
    {
        if (node.MultiPartIdentifier != null)
        {
            foreach (var identifier in node.MultiPartIdentifier.Identifiers)
            {
                FieldNames.Add(identifier.Value);
            }
        }

        base.Visit(node);
    }
}
