using System;
using System.Collections.Generic;
using System.Linq;
using net.sf.jsqlparser.expression;
using net.sf.jsqlparser.schema;
using net.sf.jsqlparser.statement;
using net.sf.jsqlparser.statement.create.view;
using net.sf.jsqlparser.statement.@select;
using Qsi.JSql.Extensions;
using Qsi.Tree.Base;
using Qsi.Utilities;

namespace Qsi.JSql.Tree
{
    public class JSqlTableVisitor
    {
        protected JSqlIdentifierVisitor IdentifierVisitor => _identifierVisitor ??= CreateIdentifierVisitor();

        protected JSqlExpressionVisitor ExpressionVisitor => _expressionVIsitor ??= CreateExpressionVisitor();

        private JSqlIdentifierVisitor _identifierVisitor;
        private JSqlExpressionVisitor _expressionVIsitor;

        protected virtual JSqlIdentifierVisitor CreateIdentifierVisitor()
        {
            return new JSqlIdentifierVisitor();
        }

        protected virtual JSqlExpressionVisitor CreateExpressionVisitor()
        {
            return new JSqlExpressionVisitor();
        }

        public virtual QsiTableNode Visit(Statement statement)
        {
            switch (statement)
            {
                case Select select:
                    return VisitSelect(select);

                case CreateView createView:
                    return VisitCreateView(createView);
            }

            throw TreeHelper.NotSupportedTree(statement);
        }

        public virtual QsiDerivedTableNode VisitSelect(Select select)
        {
            var tableNode = VisitSelectBody(select.getSelectBody());

            if (select.getWithItemsList()?.size() > 0)
            {
                IEnumerable<WithItem> withItems = select.getWithItemsList().AsEnumerable<WithItem>();
                tableNode.Directives.SetValue(VisitWithItems(withItems));
            }

            return tableNode;
        }

        #region WithItem
        public virtual QsiTableDirectivesNode VisitWithItems(IEnumerable<WithItem> withItems)
        {
            return TreeHelper.Create<QsiTableDirectivesNode>(n =>
            {
                n.IsRecursive = withItems.First().isRecursive();
                n.Tables.AddRange(withItems.Select(VisitWithItem));
            });
        }

        public virtual QsiTableNode VisitWithItem(WithItem withItem)
        {
            return TreeHelper.Create<QsiDerivedTableNode>(n =>
            {
                if (withItem.getWithItemList()?.size() > 0)
                {
                    IEnumerable<Column> columns = withItem.getWithItemList()
                        .AsEnumerable<SelectExpressionItem>()
                        .Select(item => (Column)item.getExpression());

                    n.Columns.SetValue(CreateSequentialColumnNodes(columns));
                }
                else
                {
                    n.Columns.SetValue(TreeHelper.CreateAllColumnsDeclaration());
                }

                n.Source.SetValue(VisitSelectBody(withItem.getSelectBody()));

                n.Alias.SetValue(new QsiAliasNode
                {
                    Name = IdentifierVisitor.Create(withItem.getName())
                });
            });
        }

        public virtual QsiColumnsDeclarationNode CreateSequentialColumnNodes(IEnumerable<Column> columns)
        {
            return TreeHelper.Create<QsiColumnsDeclarationNode>(n =>
            {
                n.Columns.AddRange(columns
                    .Select((column, i) => TreeHelper.Create<QsiSequentialColumnNode>(cn =>
                    {
                        var identifier = IdentifierVisitor.VisitMultiPartName(column);

                        if (identifier.Level != 1)
                            throw new InvalidOperationException();

                        cn.Ordinal = i;

                        cn.Alias.SetValue(new QsiAliasNode
                        {
                            Name = identifier[0]
                        });
                    })));
            });
        }
        #endregion

        #region SelectBody
        public virtual QsiDerivedTableNode VisitSelectBody(SelectBody selectBody)
        {
            switch (selectBody)
            {
                case PlainSelect plainSelect:
                    return VisitPlainSelect(plainSelect);

                case SubSelect subSelect:
                    return VisitSubSelect(subSelect);

                default:
                    throw TreeHelper.NotSupportedTree(selectBody);
            }
        }

        public virtual QsiDerivedTableNode VisitPlainSelect(PlainSelect plainSelect)
        {
            return TreeHelper.Create<QsiDerivedTableNode>(n =>
            {
                if (plainSelect.getSelectItems()?.size() > 0)
                {
                    IEnumerable<SelectItem> selectItems = plainSelect.getSelectItems().AsEnumerable<SelectItem>();
                    n.Columns.SetValue(VisitSelectItems(selectItems));
                }

                if (plainSelect.getFromItem() != null)
                {
                    n.Source.SetValue(VisitFromItem(plainSelect.getFromItem()));
                }
            });
        }

        private QsiDerivedTableNode VisitSubSelect(SubSelect subSelect)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region SelectItem
        public virtual QsiColumnsDeclarationNode VisitSelectItems(IEnumerable<SelectItem> selectItems)
        {
            return TreeHelper.Create<QsiColumnsDeclarationNode>(n =>
            {
                n.Columns.AddRange(selectItems.Select(VisitSelectItem));
            });
        }

        public virtual QsiColumnNode VisitSelectItem(SelectItem selectItem)
        {
            switch (selectItem)
            {
                case AllColumns allColumns:
                    return VisitAllColumns(allColumns);

                case AllTableColumns allTableColumns:
                    return VisitAllTableColumns(allTableColumns);

                case SelectExpressionItem selectExpressionItem:
                    return VisitSelectExpressionItem(selectExpressionItem);

                default:
                    throw TreeHelper.NotSupportedTree(selectItem);
            }
        }

        public virtual QsiAllColumnNode VisitAllColumns(AllColumns columns)
        {
            return new QsiAllColumnNode();
        }

        public virtual QsiAllColumnNode VisitAllTableColumns(AllTableColumns columns)
        {
            return new QsiAllColumnNode
            {
                Path = IdentifierVisitor.VisitMultiPartName(columns.getTable())
            };
        }

        public virtual QsiColumnNode VisitSelectExpressionItem(SelectExpressionItem selectExpression)
        {
            var alias = selectExpression.getAlias();
            var expression = selectExpression.getExpression();

            if (expression is Column column)
            {
                var columnNode = VisitColumn(column);

                if (alias == null)
                    return columnNode;

                return TreeHelper.Create<QsiDerivedColumnNode>(n =>
                {
                    n.Column.SetValue(columnNode);
                    n.Alias.SetValue(VisitAlias(alias));
                });
            }

            return TreeHelper.Create<QsiDerivedColumnNode>(n =>
            {
                n.Expression.SetValue(ExpressionVisitor.Visit(expression));

                if (alias != null)
                    n.Alias.SetValue(VisitAlias(alias));
            });
        }

        public virtual QsiDeclaredColumnNode VisitColumn(Column column)
        {
            return TreeHelper.Create<QsiDeclaredColumnNode>(n =>
            {
                n.Name = IdentifierVisitor.VisitMultiPartName(column);
            });
        }
        #endregion

        #region FromItem
        public virtual QsiTableNode VisitFromItem(FromItem item)
        {
            switch (item)
            {
                case Table table:
                    return VisitTable(table);

                case Join join:
                    return VisitJoin(join);

                case SubJoin subJoin:
                    return VisitSubJoin(subJoin);

                case SubSelect subSelect:
                    return VisitSubSelect(subSelect);

                case TableFunction tableFunction:
                    return VisitTableFunction(tableFunction);

                case SpecialSubSelect specialSubSelect:
                    return VisitSpecialSubSelect(specialSubSelect);

                case ValuesList valuesList:
                    return VisitValuesList(valuesList);

                case ParenthesisFromItem parenthesisFromItem:
                    return VisitParenthesisFromItem(parenthesisFromItem);
            }

            throw TreeHelper.NotSupportedTree(item);
        }

        private QsiTableNode VisitTable(Table table)
        {
            var source = TreeHelper.Create<QsiTableAccessNode>(n =>
            {
                n.Identifier = IdentifierVisitor.VisitMultiPartName(table);
            });

            if (table.getAlias() == null)
                return source;

            return TreeHelper.Create<QsiDerivedTableNode>(n =>
            {
                n.Columns.SetValue(TreeHelper.CreateAllColumnsDeclaration());
                n.Source.SetValue(source);
                n.Alias.SetValue(VisitAlias(table.getAlias()));
            });
        }

        private QsiTableNode VisitJoin(Join join)
        {
            throw new NotImplementedException();
        }

        private QsiTableNode VisitSubJoin(SubJoin subJoin)
        {
            throw new NotImplementedException();
        }

        private QsiTableNode VisitTableFunction(TableFunction tableFunction)
        {
            throw new NotImplementedException();
        }

        private QsiTableNode VisitSpecialSubSelect(SpecialSubSelect specialSubSelect)
        {
            throw new NotImplementedException();
        }

        private QsiTableNode VisitValuesList(ValuesList valuesList)
        {
            throw new NotImplementedException();
        }

        private QsiTableNode VisitParenthesisFromItem(ParenthesisFromItem parenthesisFromItem)
        {
            var alias = parenthesisFromItem.getAlias();
            var source = VisitFromItem(parenthesisFromItem.getFromItem());

            if (alias == null)
                return source;

            return TreeHelper.Create<QsiDerivedTableNode>(n =>
            {
                n.Columns.SetValue(TreeHelper.CreateAllColumnsDeclaration());
                n.Source.SetValue(source);
                n.Alias.SetValue(VisitAlias(alias));
            });
        }
        #endregion

        public virtual QsiAliasNode VisitAlias(Alias alias)
        {
            return new QsiAliasNode
            {
                Name = IdentifierVisitor.VisitAlias(alias)
            };
        }

        public virtual QsiTableNode VisitCreateView(CreateView createView)
        {
            throw new NotImplementedException();
        }
    }
}
