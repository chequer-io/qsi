using System.Threading;
using Qsi.Data;
using Qsi.Oracle.Internal;
using Qsi.Oracle.Tree.Visitors;
using Qsi.Parsing;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Oracle;

using static OracleParserInternal;

public sealed class OracleParser : IQsiTreeParser
{
    public IQsiTreeNode Parse(QsiScript script, CancellationToken cancellationToken = default)
    {
        var parser = OracleUtility.CreateParser(script.Script);

        var block = parser.root();

        if (block.block() is not null)
        {
            switch (block.block(0).children[0])
            {
                case OracleStatementContext oracleStatement:
                    return ParseOracleStatement(oracleStatement);

                default:
                    throw TreeHelper.NotSupportedTree(block.children[0]);
            }
        }

        return null;
    }

    private static IQsiTreeNode ParseOracleStatement(OracleStatementContext oracleStatement)
    {
        switch (oracleStatement.children[0])
        {
            case SelectContext select:
                return TableVisitor.VisitSelect(select);

            case CreateContext create:
                return ActionVisitor.VisitCreate(create);

            case UpdateContext update:
                return ActionVisitor.VisitUpdate(update);

            case DeleteContext delete:
                return ActionVisitor.VisitDelete(delete);

            case InsertContext insert:
                return ActionVisitor.VisitInsert(insert);

            case MergeContext merge:
                return ActionVisitor.VisitMerge(merge);

            case AlterContext alter:
                return ActionVisitor.VisitAlter(alter);

            default:
                throw TreeHelper.NotSupportedTree(oracleStatement.children[0]);
        }
    }
}