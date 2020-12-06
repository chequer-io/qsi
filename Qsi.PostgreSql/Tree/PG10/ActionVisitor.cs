using System.Linq;
using Qsi.Data;
using Qsi.PostgreSql.Internal.PG10.Types;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.PostgreSql.Tree.PG10
{
    internal static class ActionVisitor
    {
        public static QsiTreeNode Visit(IPg10Node node)
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

        public static QsiTreeNode VisitRawStmt(RawStmt rawStmt)
        {
            return Visit(rawStmt.stmt[0]);
        }

        public static QsiTreeNode VisitVariableSetStmt(VariableSetStmt variableSetStmt)
        {
            if (variableSetStmt.kind == VariableSetKind.VAR_SET_VALUE &&
                variableSetStmt.name == "search_path")
            {
                var node = new QsiChangeSearchPathActionNode
                {
                    Identifiers = variableSetStmt.args
                        .Cast<A_Const>()
                        .Select(x => new QsiIdentifier(x.val.str, false))
                        .ToArray()
                };

                return node;
            }

            throw TreeHelper.NotSupportedTree(variableSetStmt);
        }
    }
}
