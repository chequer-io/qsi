using System.Linq;
using Qsi.Data;
using Qsi.PostgreSql.Internal.PG10.Types;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.PostgreSql.Tree.PG10
{
    internal class PgActionVisitor : PgVisitorBase
    {
        public PgActionVisitor(IPgVisitorSet set) : base(set)
        {
        }

        public QsiTreeNode Visit(IPg10Node node)
        {
            switch (node)
            {
                case RawStmt rawStmt:
                    return VisitRawStmt(rawStmt);

                case VariableSetStmt variableSetStmt:
                    return VisitVariableSetStmt(variableSetStmt);
            }

            throw TreeHelper.NotSupportedTree(node);
        }

        public QsiTreeNode VisitRawStmt(RawStmt rawStmt)
        {
            return Visit(rawStmt.stmt[0]);
        }

        public QsiTreeNode VisitVariableSetStmt(VariableSetStmt variableSetStmt)
        {
            if (variableSetStmt.kind == VariableSetKind.VAR_SET_VALUE &&
                variableSetStmt.name == "search_path")
            {
                var node = new QsiChangeSearchPathActionNode
                {
                    Identifiers = variableSetStmt.args
                        .Cast<A_Const>()
                        .Select(x => new QsiQualifiedIdentifier(new QsiIdentifier(x.val.str, false)))
                        .ToArray()
                };

                return node;
            }

            throw TreeHelper.NotSupportedTree(variableSetStmt);
        }
    }
}
