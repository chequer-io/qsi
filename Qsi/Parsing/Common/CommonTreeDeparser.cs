using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qsi.Data;
using Qsi.Tree;

namespace Qsi.Parsing.Common
{
    public class CommonTreeDeparser : IQsiTreeDeparser
    {
        string IQsiTreeDeparser.Deparse(IQsiTreeNode node, QsiScript script)
        {
            var writer = new ScriptWriter();
            DeparseTreeNode(writer, node, script);

            return writer.ToString();
        }

        #region Template
        protected bool IsAliasedTableAccessNode(IQsiDerivedTableNode node)
        {
            return
                node.Source is IQsiTableAccessNode &&
                node.Alias != null &&
                node.Directives == null &&
                node.WhereExpression == null &&
                node.OrderExpression == null &&
                node.LimitExpression == null &&
                node.Columns != null &&
                IsWildcard(node.Columns);
        }

        protected bool IsWildcard(IQsiColumnsDeclarationNode node)
        {
            if (node.Count != 0)
                return false;

            return
                node.Columns[0] is IQsiAllColumnNode allColumnNode &&
                allColumnNode.Path == null;
        }

        protected void DeparseTreeNodeWithParenthesis(ScriptWriter writer, IQsiTreeNode node, QsiScript script)
        {
            writer.Write('(');
            DeparseTreeNode(writer, node, script);
            writer.Write(')');
        }

        protected void JoinElements<T>(ScriptWriter writer, string delimiter, IEnumerable<T> elements, QsiScript script) where T : IQsiTreeNode
        {
            writer.WriteJoin(delimiter, elements, (w, n) => DeparseTreeNode(w, n, script));
        }
        #endregion

        #region IQsiTreeNode
        protected virtual void DeparseTreeNode(ScriptWriter writer, IQsiTreeNode node, QsiScript script)
        {
            switch (node)
            {
                case IQsiTableNode tableNode:
                    DeparseTableNode(writer, tableNode, script);
                    break;

                case IQsiColumnNode columnNode:
                    DeparseColumnNode(writer, columnNode, script);
                    break;

                case IQsiTableDirectivesNode tableDirectivesNode:
                    DeparseTableDirectivesNode(writer, tableDirectivesNode, script);
                    break;

                case IQsiColumnsDeclarationNode columnsDeclarationNode:
                    DeparseColumnsDeclarationNode(writer, columnsDeclarationNode, script);
                    break;

                case IQsiAliasNode aliasNode:
                    DeparseAliasNode(writer, aliasNode, script);
                    break;

                case IQsiExpressionNode expressionNode:
                    DeparseExpressionNode(writer, expressionNode, script);
                    break;

                case IQsiActionNode actionNode:
                    throw new NotImplementedException();

                default:
                    throw new NotSupportedException();
            }
        }

        protected virtual void DeparseTableDirectivesNode(ScriptWriter writer, IQsiTableDirectivesNode node, QsiScript script)
        {
            writer.Write("WITH ");

            if (node.IsRecursive)
                writer.Write("RECURSIVE ");

            writer.WriteJoin(", ", node.Tables, (w, table) =>
            {
                w.Write(table.Alias.Name);

                if (table.Columns != null && IsWildcard(table.Columns))
                {
                    DeparseTreeNodeWithParenthesis(w, table.Columns, script);
                }

                w.Write(" AS ");
                DeparseTreeNodeWithParenthesis(w, table.Source, script);
            });
        }

        protected virtual void DeparseColumnsDeclarationNode(ScriptWriter writer, IQsiColumnsDeclarationNode node, QsiScript script)
        {
            if (node.IsEmpty)
                return;

            JoinElements(writer, ", ", node.Columns, script);
        }

        protected virtual void DeparseAliasNode(ScriptWriter writer, IQsiAliasNode node, QsiScript script)
        {
            writer.Write("AS ").Write(node.Name);
        }
        #endregion

        #region IQsiColumnNode
        protected virtual void DeparseColumnNode(ScriptWriter writer, IQsiColumnNode node, QsiScript script)
        {
            switch (node)
            {
                case IQsiDerivedColumnNode derivedColumnNode:
                    DeparseDerivedColumnNode(writer, derivedColumnNode, script);
                    break;

                case IQsiAllColumnNode allColumnNode:
                    DeparseAllColumnNode(writer, allColumnNode, script);
                    break;

                case IQsiBindingColumnNode bindingColumnNode:
                    DeparseBindingColumnNode(writer, bindingColumnNode, script);
                    break;

                case IQsiDeclaredColumnNode declaredColumnNode:
                    DeparseDeclaredColumnNode(writer, declaredColumnNode, script);
                    break;

                case IQsiSequentialColumnNode sequentialColumnNode:
                    DeparseSequentialColumnNode(writer, sequentialColumnNode, script);
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        protected virtual void DeparseDerivedColumnNode(ScriptWriter writer, IQsiDerivedColumnNode node, QsiScript script)
        {
            if (node.IsColumn)
                DeparseTreeNode(writer, node.Column, script);
            else if (node.IsExpression)
                DeparseTreeNode(writer, node.Expression, script);

            if (node.Alias == null)
                return;

            writer.WriteSpace();
            DeparseTreeNode(writer, node.Alias, script);
        }

        protected virtual void DeparseAllColumnNode(ScriptWriter writer, IQsiAllColumnNode node, QsiScript script)
        {
            if (node.Path != null)
                writer.Write(node.Path).Write('.');

            writer.Write('*');
        }

        protected virtual void DeparseBindingColumnNode(ScriptWriter writer, IQsiBindingColumnNode node, QsiScript script)
        {
            throw new NotImplementedException();
        }

        protected virtual void DeparseDeclaredColumnNode(ScriptWriter writer, IQsiDeclaredColumnNode node, QsiScript script)
        {
            writer.Write(node.Name);
        }

        protected virtual void DeparseSequentialColumnNode(ScriptWriter writer, IQsiSequentialColumnNode node, QsiScript script)
        {
            writer.Write(node.Alias.Name);
        }
        #endregion

        #region IQsiTableNode
        protected virtual void DeparseTableNode(ScriptWriter writer, IQsiTableNode node, QsiScript script)
        {
            switch (node)
            {
                case IQsiCompositeTableNode compositeTableNode:
                    DeparseCompositeTableNode(writer, compositeTableNode, script);
                    break;

                case IQsiDerivedTableNode derivedTableNode:
                    DeparseDerivedTableNode(writer, derivedTableNode, script);
                    break;

                case IQsiInlineDerivedTableNode inlineDerivedTableNode:
                    DeparseInlineDerivedTableNode(writer, inlineDerivedTableNode, script);
                    break;

                case IQsiJoinedTableNode joinedTableNode:
                    DeparseJoinedTableNode(writer, joinedTableNode, script);
                    break;

                case IQsiTableAccessNode tableAccessNode:
                    DeparseTableAccessNode(writer, tableAccessNode, script);
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        protected virtual void DeparseCompositeTableNode(ScriptWriter writer, IQsiCompositeTableNode node, QsiScript script)
        {
            bool parenthesis = node.OrderExpression != null || node.LimitExpression != null;

            if (parenthesis)
                writer.Write('(');

            JoinElements(writer, " UNION ", node.Sources, script);

            if (parenthesis)
                writer.Write(')');

            if (node.OrderExpression != null)
            {
                writer.WriteSpace();
                DeparseTreeNode(writer, node.OrderExpression, script);
            }

            if (node.LimitExpression != null)
            {
                writer.WriteSpace();
                DeparseTreeNode(writer, node.LimitExpression, script);
            }
        }

        protected virtual void DeparseDerivedTableNode(ScriptWriter writer, IQsiDerivedTableNode node, QsiScript script)
        {
            if (IsAliasedTableAccessNode(node))
            {
                // IQsiTableAccessNode
                DeparseTreeNode(writer, node.Source, script);
                writer.WriteSpace();
                DeparseTreeNode(writer, node.Alias, script);
                return;
            }

            if (node.Alias != null)
                throw new NotSupportedException();

            writer.Write("SELECT ");

            if (node.Columns != null)
                DeparseTreeNode(writer, node.Columns, script);

            if (node.Source != null)
            {
                writer.WriteSpace();
                writer.Write("FROM ");

                if (node.Source is IQsiDerivedTableNode leftSource && !IsAliasedTableAccessNode(leftSource) ||
                    node.Source is IQsiCompositeTableNode)
                {
                    DeparseTreeNodeWithParenthesis(writer, node.Source, script);
                }
                else
                {
                    DeparseTreeNode(writer, node.Source, script);
                }
            }

            if (node.WhereExpression != null)
            {
                writer.WriteSpace();
                DeparseTreeNode(writer, node.WhereExpression, script);
            }

            if (node.GroupingExpression != null)
            {
                writer.WriteSpace();
                DeparseTreeNode(writer, node.GroupingExpression, script);
            }

            if (node.OrderExpression != null)
            {
                writer.WriteSpace();
                DeparseTreeNode(writer, node.OrderExpression, script);
            }

            if (node.LimitExpression != null)
            {
                writer.WriteSpace();
                DeparseTreeNode(writer, node.LimitExpression, script);
            }
        }

        protected virtual void DeparseInlineDerivedTableNode(ScriptWriter writer, IQsiInlineDerivedTableNode node, QsiScript script)
        {
            throw new NotImplementedException();
        }

        protected virtual void DeparseJoinedTableNode(ScriptWriter writer, IQsiJoinedTableNode node, QsiScript script)
        {
            string joinToken;

            switch (node.JoinType)
            {
                case QsiJoinType.Comma:
                    joinToken = ", ";
                    break;

                case QsiJoinType.Inner:
                case QsiJoinType.Left:
                case QsiJoinType.Right:
                case QsiJoinType.Full:
                case QsiJoinType.Semi:
                case QsiJoinType.Anti:
                case QsiJoinType.Cross:
                    joinToken = $" {node.JoinType.ToString().ToUpper()} JOIN ";
                    break;

                case QsiJoinType.Straight:
                    joinToken = " STRAIGHT_JOIN ";
                    break;

                case QsiJoinType.LeftOuter:
                    joinToken = " LEFT OUTER ";
                    break;

                case QsiJoinType.RightOuter:
                    joinToken = " RIGHT OUTER ";
                    break;

                case QsiJoinType.NaturalLeft:
                    joinToken = " NATURAL LEFT ";
                    break;

                case QsiJoinType.NaturalRight:
                    joinToken = " NATURAL RIGHT ";
                    break;

                case QsiJoinType.NaturalLeftOuter:
                    joinToken = " NATURAL LEFT OUTER ";
                    break;

                case QsiJoinType.NaturalRightOuter:
                    joinToken = " NATURAL RIGHT OUTER ";
                    break;

                default:
                    throw new NotSupportedException(node.JoinType.ToString());
            }

            DeparseTreeNode(writer, node.Left, script);
            writer.Write(joinToken);
            DeparseTreeNode(writer, node.Right, script);

            if (node.PivotColumns != null)
            {
                writer.Write(" USING ");
                DeparseTreeNodeWithParenthesis(writer, node.PivotColumns, script);
            }
        }

        protected virtual void DeparseTableAccessNode(ScriptWriter writer, IQsiTableAccessNode node, QsiScript script)
        {
            writer.Write(node.Identifier);
        }
        #endregion

        #region IQsiExpressionNode
        protected virtual void DeparseExpressionNode(ScriptWriter writer, IQsiExpressionNode node, QsiScript script)
        {
            switch (node)
            {
                case IQsiLiteralExpressionNode literalExpressionNode:
                    DeparseLiteralExpressionNode(writer, literalExpressionNode, script);
                    break;

                case IQsiWhereExpressionNode whereExpressionNode:
                    DeparseWhereExpressionNode(writer, whereExpressionNode, script);
                    break;

                case IQsiMultipleOrderExpressionNode multipleOrderExpressionNode:
                    DeparseMultipleOrderExpressionNode(writer, multipleOrderExpressionNode, script);
                    break;

                case IQsiOrderExpressionNode orderExpressionNode:
                    DeparseOrderExpressionNode(writer, orderExpressionNode, script);
                    break;

                case IQsiLimitExpressionNode limitExpressionNode:
                    DeparseLimitExpressionNode(writer, limitExpressionNode, script);
                    break;

                case IQsiGroupingExpressionNode groupingExpressionNode:
                    DeparseGroupingExpressionNode(writer, groupingExpressionNode, script);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        protected virtual void DeparseWhereExpressionNode(ScriptWriter writer, IQsiWhereExpressionNode node, QsiScript script)
        {
            writer.Write("FROM ");
            DeparseTreeNode(writer, node.Expression, script);
        }

        protected virtual void DeparseMultipleOrderExpressionNode(ScriptWriter writer, IQsiMultipleOrderExpressionNode node, QsiScript script)
        {
            writer.Write("ORDER BY ");
            JoinElements(writer, ", ", node.Orders, script);
        }

        protected virtual void DeparseOrderExpressionNode(ScriptWriter writer, IQsiOrderExpressionNode node, QsiScript script)
        {
            DeparseTreeNode(writer, node.Expression, script);
            writer.WriteSpace();
            writer.Write(node.Order == QsiSortOrder.Ascending ? "ASC" : "DESC");
        }

        protected virtual void DeparseLimitExpressionNode(ScriptWriter writer, IQsiLimitExpressionNode node, QsiScript script)
        {
            writer.Write("LIMIT ");

            if (node.Offset != null)
                DeparseTreeNode(writer, node.Offset, script);

            if (node.Limit != null)
            {
                if (node.Offset != null)
                    writer.Write(", ");

                DeparseTreeNode(writer, node.Limit, script);
            }
        }

        private void DeparseGroupingExpressionNode(ScriptWriter writer, IQsiGroupingExpressionNode node, QsiScript script)
        {
            writer.Write("GROUP BY ");
            writer.WriteJoin(", ", node.Items, (_, item) => DeparseTreeNode(writer, item, script));

            if (node.Having != null)
            {
                writer.Write("HAVING ");
                DeparseTreeNode(writer, node.Having, script);
            }
        }

        protected virtual void DeparseLiteralExpressionNode(ScriptWriter writer, IQsiLiteralExpressionNode node, QsiScript script)
        {
            if (node.Value == null)
                writer.Write("NULL");
            else
                writer.Write(node.Value);
        }
        #endregion

        #region IQsiActionNode
        // not implemented
        #endregion
    }
}
