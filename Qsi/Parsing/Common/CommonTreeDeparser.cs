using System;
using System.Collections.Generic;
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
        protected bool IsAliasedTableReferenceNode(IQsiDerivedTableNode node)
        {
            return
                node.Source is IQsiTableReferenceNode &&
                node.Alias is not null &&
                node.Directives is null &&
                node.Where is null &&
                node.Grouping is null &&
                node.Order is null &&
                node.Limit is null &&
                node.Columns is not null &&
                IsWildcard(node.Columns);
        }

        protected bool IsAliasedDerivedTableNode(IQsiDerivedTableNode node)
        {
            return
                node.Source is IQsiDerivedTableNode &&
                node.Alias is not null &&
                node.Directives is null &&
                node.Where is null &&
                node.Grouping is null &&
                node.Order is null &&
                node.Limit is null &&
                node.Columns is not null &&
                IsWildcard(node.Columns);
        }

        protected bool IsWildcard(IQsiColumnsDeclarationNode node)
        {
            if (node.Count != 1)
                return false;

            return
                node.Columns[0] is IQsiAllColumnNode { Path: null };
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

                if (table.Columns is not null && !IsWildcard(table.Columns))
                {
                    w.WriteSpace();
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

            JoinElements(writer, ", ", node, script);
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

                case IQsiColumnReferenceNode columnReferenceNode:
                    DeparseColumnReferenceNode(writer, columnReferenceNode, script);
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

            if (node.Alias is null)
                return;

            writer.WriteSpace();
            DeparseTreeNode(writer, node.Alias, script);
        }

        protected virtual void DeparseAllColumnNode(ScriptWriter writer, IQsiAllColumnNode node, QsiScript script)
        {
            if (node.Path is not null)
                writer.Write(node.Path).Write('.');

            writer.Write('*');
        }

        protected virtual void DeparseColumnReferenceNode(ScriptWriter writer, IQsiColumnReferenceNode node, QsiScript script)
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

                case IQsiTableFunctionNode tableFunctionNode:
                    DeparseTableFunctionNode(writer, tableFunctionNode, script);
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

                case IQsiTableReferenceNode tableReferenceNode:
                    DeparseTableReferenceNode(writer, tableReferenceNode, script);
                    break;

                case IQsiTableDirectivesNode tableDirectivesNode:
                    DeparseTableDirectivesNode(writer, tableDirectivesNode, script);
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        protected virtual void DeparseCompositeTableNode(ScriptWriter writer, IQsiCompositeTableNode node, QsiScript script)
        {
            bool parenthesis = node.Order is not null || node.Limit is not null;

            if (parenthesis)
                writer.Write('(');

            JoinElements(writer, $" {node.CompositeType ?? "UNION"} ", node.Sources, script);

            if (parenthesis)
                writer.Write(')');

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

        protected virtual void DeparseTableFunctionNode(ScriptWriter writer, IQsiTableFunctionNode node, QsiScript script)
        {
            writer.Write(node.Member.Identifier);
            writer.Write('(');
            DeparseExpressionNode(writer, node.Parameters, script);
            writer.Write(')');
        }

        protected virtual void DeparseDerivedTableNode(ScriptWriter writer, IQsiDerivedTableNode node, QsiScript script)
        {
            if (IsAliasedTableReferenceNode(node))
            {
                // IQsiTableReferenceNode
                DeparseTreeNode(writer, node.Source, script);
                writer.WriteSpace();
                DeparseTreeNode(writer, node.Alias, script);
                return;
            }

            if (IsAliasedDerivedTableNode(node))
            {
                DeparseTreeNodeWithParenthesis(writer, node.Source, script);
                writer.WriteSpace();
                DeparseTreeNode(writer, node.Alias, script);
                return;
            }

            if (node.Directives is not null)
            {
                DeparseTreeNode(writer, node.Directives, script);
                writer.WriteSpace();
            }

            writer.Write("SELECT ");

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

        protected virtual void DeparseInlineDerivedTableNode(ScriptWriter writer, IQsiInlineDerivedTableNode node, QsiScript script)
        {
            throw new NotImplementedException();
        }

        protected virtual void DeparseJoinedTableNode(ScriptWriter writer, IQsiJoinedTableNode node, QsiScript script)
        {
            string joinType = node.IsComma ? ", " : $" {node.JoinType?.Trim()} ";

            DeparseTreeNode(writer, node.Left, script);
            writer.Write(joinType);
            DeparseTreeNode(writer, node.Right, script);

            if (node.PivotColumns is not null)
            {
                writer.Write(" USING ");
                DeparseTreeNodeWithParenthesis(writer, node.PivotColumns, script);
            }
        }

        protected virtual void DeparseTableReferenceNode(ScriptWriter writer, IQsiTableReferenceNode node, QsiScript script)
        {
            writer.Write(node.Identifier);
        }
        #endregion

        #region IQsiExpressionNode
        protected virtual void DeparseExpressionNode(ScriptWriter writer, IQsiExpressionNode node, QsiScript script)
        {
            switch (node)
            {
                case QsiExpressionFragmentNode fragmentNode:
                    writer.Write(fragmentNode.Text);
                    break;

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

                case IQsiBindParameterExpressionNode bindParameterExpressionNode:
                    DeparseBindParameterExpressionNode(writer, bindParameterExpressionNode, script);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        protected virtual void DeparseWhereExpressionNode(ScriptWriter writer, IQsiWhereExpressionNode node, QsiScript script)
        {
            writer.Write("WHERE ");
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

            if (node.Offset is not null)
                DeparseTreeNode(writer, node.Offset, script);

            if (node.Limit is not null)
            {
                if (node.Offset is not null)
                    writer.Write(", ");

                DeparseTreeNode(writer, node.Limit, script);
            }
        }

        protected virtual void DeparseGroupingExpressionNode(ScriptWriter writer, IQsiGroupingExpressionNode node, QsiScript script)
        {
            writer.Write("GROUP BY ");
            writer.WriteJoin(", ", node.Items, (_, item) => DeparseTreeNode(writer, item, script));

            if (node.Having is not null)
            {
                writer.Write("HAVING ");
                DeparseTreeNode(writer, node.Having, script);
            }
        }

        protected virtual void DeparseBindParameterExpressionNode(ScriptWriter writer, IQsiBindParameterExpressionNode node, QsiScript script)
        {
            writer.Write(node.Prefix);

            if (!node.NoSuffix)
                writer.Write(node.Type == QsiParameterType.Index ? node.Index : node.Name);
        }

        protected virtual void DeparseLiteralExpressionNode(ScriptWriter writer, IQsiLiteralExpressionNode node, QsiScript script)
        {
            if (node.Value is null)
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
