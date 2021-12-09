using System;
using Qsi.Data;
using Qsi.Oracle.Common;
using Qsi.Oracle.Tree;
using Qsi.Parsing.Common;
using Qsi.Tree;

namespace Qsi.Oracle
{
    public sealed class OracleDeparser : CommonTreeDeparser
    {
        protected override void DeparseTreeNode(ScriptWriter writer, IQsiTreeNode node, QsiScript script)
        {
            var range = OracleTree.Span[node];

            if (Equals(range, default(Range)))
            {
                base.DeparseTreeNode(writer, node, script);
                return;
            }

            writer.Write(script.Script[range]);
        }

        protected override void DeparseTableNode(ScriptWriter writer, IQsiTableNode node, QsiScript script)
        {
            switch (node)
            {
                case OracleLateralTableNode lateralNode:
                    DeparseLateralTableNode(writer, lateralNode, script);
                    break;

                default:
                    base.DeparseTableNode(writer, node, script);
                    break;
            }
        }

        private void DeparseLateralTableNode(ScriptWriter writer, OracleLateralTableNode lateralNode, QsiScript script)
        {
            writer.Write("LATERAL(");
            DeparseTreeNode(writer, lateralNode.Source.Value, script);
            writer.Write(")");
        }

        protected override void DeparseTableDirectivesNode(ScriptWriter writer, IQsiTableDirectivesNode node, QsiScript script)
        {
            writer.Write("WITH ");

            writer.WriteJoin(", ", node.Tables, (w, table) =>
            {
                w.Write(table.Alias.Name);

                if (table.Columns is not null && !IsWildcard(table.Columns))
                {
                    w.WriteSpace();
                    DeparseTreeNodeWithParenthesis(w, table.Columns, script);
                }

                w.Write(" AS ");
                DeparseTreeNodeWithParenthesis(w, table.Source, script);
            });
        }

        protected override void DeparseDerivedTableNode(ScriptWriter writer, IQsiDerivedTableNode node, QsiScript script)
        {
            var oracleNode = node as OracleDerivedTableNode;

            if (oracleNode is not null &&
                (!string.IsNullOrEmpty(oracleNode.Hint) ||
                 oracleNode.QueryBehavior is not null
                )
            )
            {
                if (node.Directives is not null)
                {
                    DeparseTreeNode(writer, node.Directives, script);
                    writer.WriteSpace();
                }

                writer.Write("SELECT ");

                if (!string.IsNullOrEmpty(oracleNode.Hint))
                {
                    writer.WriteSpace();
                    writer.Write(oracleNode.Hint);
                }

                if (oracleNode.QueryBehavior is not null)
                {
                    writer.WriteSpace();
                    writer.Write(oracleNode.QueryBehavior?.ToString().ToUpperInvariant());
                }

                if (node.Columns is not null)
                    DeparseTreeNode(writer, node.Columns, script);

                if (node.Source is not null)
                {
                    writer.WriteSpace();
                    writer.Write("FROM ");
                    DeparseTreeNode(writer, node.Source, script);
                }

                if (node.Where is not null)
                {
                    writer.WriteSpace();
                    DeparseTreeNode(writer, node.Where, script);
                }

                if (node.Grouping is not null)
                {
                    writer.WriteSpace();
                    DeparseTreeNode(writer, node.Grouping, script);
                }

                if (node.Order is not null)
                {
                    writer.WriteSpace();
                    DeparseTreeNode(writer, node.Order, script);
                }

                if (node.Limit is not null)
                {
                    writer.WriteSpace();
                    DeparseTreeNode(writer, node.Limit, script);
                }
            }
            else
            {
                if (node.Alias is not null &&
                    node.Directives is null &&
                    node.Where is null &&
                    node.Grouping is null &&
                    node.Order is null &&
                    node.Limit is null &&
                    node.Columns is not null &&
                    IsWildcard(node.Columns)
                )
                {
                    DeparseTreeNodeWithParenthesis(writer, node.Source, script);
                    writer.WriteSpace();
                    DeparseTreeNode(writer, node.Alias, script);
                    return;
                }

                base.DeparseDerivedTableNode(writer, node, script);
            }

            if (oracleNode is not null)
            {
                if (!oracleNode.FlashbackQueryClause.IsEmpty)
                {
                    writer.WriteSpace();
                    DeparseTreeNode(writer, oracleNode.FlashbackQueryClause.Value, script);
                }

                if (!oracleNode.TableClauses.IsEmpty)
                {
                    writer.WriteSpace();
                    DeparseTreeNode(writer, oracleNode.TableClauses.Value, script);
                }

                if (!oracleNode.Partition.IsEmpty)
                {
                    writer.WriteSpace();
                    DeparseTreeNode(writer, oracleNode.Partition.Value, script);
                }
            }
        }

        protected override void DeparseAliasNode(ScriptWriter writer, IQsiAliasNode node, QsiScript script)
        {
            writer.WriteSpace();
            writer.Write(node.Name);
        }

        protected override void DeparseTableReferenceNode(ScriptWriter writer, IQsiTableReferenceNode node, QsiScript script)
        {
            base.DeparseTableReferenceNode(writer, node, script);

            if (node is not OracleTableReferenceNode oracleNode)
                return;

            if (!oracleNode.Partition.IsEmpty)
            {
                writer.WriteSpace();
                DeparseTreeNode(writer, oracleNode.Partition.Value, script);
            }

            if (!oracleNode.Hierarchies.IsEmpty)
            {
                writer.WriteSpace();
                DeparseTreeNode(writer, oracleNode.Hierarchies.Value, script);
            }
        }

        protected override void DeparseJoinedTableNode(ScriptWriter writer, IQsiJoinedTableNode node, QsiScript script)
        {
            base.DeparseJoinedTableNode(writer, node, script);

            if (node is OracleJoinedTableNode oracleNode)
            {
                if (!oracleNode.OnCondition.IsEmpty)
                {
                    writer.Write(" ON ");
                    DeparseTreeNodeWithParenthesis(writer, oracleNode.OnCondition.Value, script);
                }
            }
        }

        protected override void DeparseExpressionNode(ScriptWriter writer, IQsiExpressionNode node, QsiScript script)
        {
            if (node is OracleColumnOuterJoinExpressionNode outerJoinExpressionNode)
            {
                DeparseTreeNode(writer, outerJoinExpressionNode.Column.Value, script);
                writer.Write("(+)");
            }
            else
            {
                base.DeparseExpressionNode(writer, node, script);
            }
        }

        protected override void DeparseLimitExpressionNode(ScriptWriter writer, IQsiLimitExpressionNode node, QsiScript script)
        {
            if (node is not OracleLimitExpressionNode oracleNode)
            {
                base.DeparseLimitExpressionNode(writer, node, script);
                return;
            }

            if (!oracleNode.Offset.IsEmpty)
            {
                writer.Write("OFFSET ");
                DeparseTreeNode(writer, oracleNode.Offset.Value, script);
                writer.Write("ROWS ");
            }

            if (!oracleNode.Limit.IsEmpty)
            {
                writer.Write("FETCH FIRST ");
                DeparseTreeNode(writer, oracleNode.Limit.Value, script);
                writer.Write("ROWS ONLY ");
            }

            if (!oracleNode.LimitPercent.IsEmpty)
            {
                writer.Write("FETCH FIRST ");
                DeparseTreeNode(writer, oracleNode.LimitPercent.Value, script);
                writer.Write("ROWS ONLY ");
            }
        }
    }
}
