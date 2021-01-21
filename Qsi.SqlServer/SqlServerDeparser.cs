using System;
using Qsi.Data;
using Qsi.Parsing.Common;
using Qsi.SqlServer.Data;
using Qsi.SqlServer.Tree;
using Qsi.Tree;

namespace Qsi.SqlServer
{
    public sealed class SqlServerDeparser : CommonTreeDeparser
    {
        protected override void DeparseTreeNode(ScriptWriter writer, IQsiTreeNode node, QsiScript script)
        {
            var range = SqlServerTree.GetSpan(node);

            if (Equals(range, default(Range)))
            {
                base.DeparseTreeNode(writer, node, script);
                return;
            }

            writer.Write(script.Script[range]);
        }

        protected override void DeparseCompositeTableNode(ScriptWriter writer, IQsiCompositeTableNode node, QsiScript script)
        {
            switch (node)
            {
                case SqlServerBinaryTableNode sqlServerBinaryTableNode:
                {
                    string binaryTableType;

                    switch (sqlServerBinaryTableNode.BinaryTableType)
                    {
                        case SqlServerBinaryTableType.Except:
                            binaryTableType = " EXCEPT ";
                            break;

                        case SqlServerBinaryTableType.Intersect:
                            binaryTableType = " INTERSECT ";
                            break;

                        case SqlServerBinaryTableType.Union:
                            binaryTableType = " UNION ";
                            break;

                        default:
                            throw new NotSupportedException(sqlServerBinaryTableNode.BinaryTableType.ToString());
                    }

                    DeparseTreeNode(writer, sqlServerBinaryTableNode.Left.Value, script);
                    writer.Write(binaryTableType);
                    DeparseTreeNode(writer, sqlServerBinaryTableNode.Right.Value, script);
                    break;
                }

                default:
                    base.DeparseCompositeTableNode(writer, node, script);
                    break;
            }
        }

        protected override void DeparseJoinedTableNode(ScriptWriter writer, IQsiJoinedTableNode node, QsiScript script)
        {
            base.DeparseJoinedTableNode(writer, node, script);

            if (node is SqlServerJoinedTableNode sqlServerJoinedTableNode)
            {
                if (!sqlServerJoinedTableNode.Expression.IsEmpty)
                {
                    writer.Write(" ON ");
                    DeparseTreeNode(writer, sqlServerJoinedTableNode.Expression.Value, script);
                }
            }
        }
    }
}
