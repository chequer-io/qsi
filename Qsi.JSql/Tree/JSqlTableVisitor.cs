using System;
using System.Collections.Generic;
using System.Linq;
using net.sf.jsqlparser.expression;
using net.sf.jsqlparser.schema;
using net.sf.jsqlparser.statement;
using net.sf.jsqlparser.statement.create.view;
using net.sf.jsqlparser.statement.@select;
using net.sf.jsqlparser.statement.values;
using Qsi.Data;
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

        public virtual QsiTableNode VisitSelect(Select select)
        {
            var tableNode = VisitSelectBody(select.getSelectBody());
            IEnumerable<WithItem> withItems = select.getWithItemsList()?.AsEnumerable<WithItem>();

            if (withItems == null || !withItems.Any())
                return tableNode;

            QsiDerivedTableNode derivedNode;

            if (tableNode is QsiDerivedTableNode derivedTableNode && derivedTableNode.Directives.IsEmpty)
            {
                derivedNode = derivedTableNode;
            }
            else
            {
                derivedNode = TreeHelper.Create<QsiDerivedTableNode>(n =>
                {
                    n.Columns.SetValue(TreeHelper.CreateAllColumnsDeclaration());
                    n.Source.SetValue(tableNode);
                });
            }

            derivedNode.Directives.SetValue(VisitWithItems(withItems));

            return derivedNode;
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
        public virtual QsiTableNode VisitSelectBody(SelectBody selectBody)
        {
            switch (selectBody)
            {
                case PlainSelect plainSelect:
                    return VisitPlainSelect(plainSelect);

                case SetOperationList setOperationList:
                    return VisitSetOperationList(setOperationList);

                case WithItem withItem:
                    return VisitWithItem(withItem);

                case ValuesStatement valuesStatement:
                    return VisitValuesStatement(valuesStatement);

                default:
                    throw TreeHelper.NotSupportedTree(selectBody);
            }
        }

        public virtual QsiDerivedTableNode VisitPlainSelect(PlainSelect plainSelect)
        {
            if (plainSelect.getIntoTables()?.size() > 0)
                throw TreeHelper.NotSupportedFeature("Into clause");

            return TreeHelper.Create<QsiDerivedTableNode>(n =>
            {
                IList<SelectItem> selectItems = plainSelect.getSelectItems().AsList<SelectItem>();
                var fromItem = plainSelect.getFromItem();

                if (!ListUtility.IsNullOrEmpty(selectItems))
                    n.Columns.SetValue(VisitSelectItems(selectItems));

                if (fromItem == null)
                    return;

                IList<Join> joins = plainSelect.getJoins().AsList<Join>();

                n.Source.SetValue(
                    ListUtility.IsNullOrEmpty(joins) ?
                        VisitFromItem(fromItem) :
                        CreateJoins(fromItem, joins));
            });
        }

        public virtual QsiTableNode CreateJoins(FromItem left, IEnumerable<Join> joins)
        {
            var anchor = VisitFromItem(left);

            foreach (var join in joins)
            {
                var rightSource = VisitFromItem(join.getRightItem());
                IList<Column> usingColumns = join.getUsingColumns().AsList<Column>();

                var joinNode = new QsiJoinedTableNode
                {
                    JoinType = GetJoinType(join)
                };

                joinNode.Left.SetValue(anchor);
                joinNode.Right.SetValue(rightSource);

                if (!ListUtility.IsNullOrEmpty(usingColumns))
                {
                    var pivotColumnsDeclaration = TreeHelper.Create<QsiColumnsDeclarationNode>(n =>
                    {
                        n.Columns.AddRange(usingColumns.Select(VisitColumn));
                    });

                    joinNode.PivotColumns.SetValue(pivotColumnsDeclaration);
                }

                anchor = joinNode;
            }

            return anchor;
        }

        protected virtual QsiJoinType GetJoinType(Join join)
        {
            if (join.isRight())
                return QsiJoinType.Right;

            if (join.isLeft())
                return QsiJoinType.Left;

            if (join.isInner())
                return QsiJoinType.Inner;

            if (join.isSemi())
                return QsiJoinType.Semi;

            if (join.isCross())
                return QsiJoinType.Cross;

            return QsiJoinType.Full;
        }

        public virtual QsiCompositeTableNode VisitSetOperationList(SetOperationList setOperationList)
        {
            return TreeHelper.Create<QsiCompositeTableNode>(n =>
            {
                n.Sources.AddRange(setOperationList.getSelects()
                    .AsEnumerable<SelectBody>()
                    .Select(VisitSelectBody));
            });
        }

        public virtual QsiDerivedTableNode VisitWithItem(WithItem withItem)
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

        private QsiDerivedTableNode VisitValuesStatement(ValuesStatement valuesStatement)
        {
            throw TreeHelper.NotSupportedFeature("Values statement");
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

                // case Join join:
                //     process on VisitPlainSelect

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

        private QsiTableNode WrapFromItem(FromItem fromItem, QsiTableNode tableNode)
        {
            // TODO: Pivot, UnPivot

            if (fromItem.getAlias() == null)
                return tableNode;

            QsiDerivedTableNode derivedNode;

            if (tableNode is QsiDerivedTableNode derivedTableNode && derivedTableNode.Alias.IsEmpty)
            {
                derivedNode = derivedTableNode;
            }
            else
            {
                derivedNode = TreeHelper.Create<QsiDerivedTableNode>(n =>
                {
                    n.Columns.SetValue(TreeHelper.CreateAllColumnsDeclaration());
                    n.Source.SetValue(tableNode);
                });
            }

            derivedNode.Alias.SetValue(VisitAlias(fromItem.getAlias()));

            return derivedNode;
        }

        public virtual QsiTableNode VisitTable(Table table)
        {
            return WrapFromItem(
                table,
                TreeHelper.Create<QsiTableAccessNode>(n =>
                {
                    n.Identifier = IdentifierVisitor.VisitMultiPartName(table);
                }));
        }

        public virtual QsiTableNode VisitSubJoin(SubJoin subJoin)
        {
            return WrapFromItem(
                subJoin,
                CreateJoins(subJoin.getLeft(), subJoin.getJoinList().AsEnumerable<Join>()));
        }

        public virtual QsiDerivedTableNode VisitSubSelect(SubSelect subSelect)
        {
            throw new NotImplementedException();
        }

        public virtual QsiTableNode VisitTableFunction(TableFunction tableFunction)
        {
            // TODO: Implement table function
            throw TreeHelper.NotSupportedFeature("Table function");
        }

        public virtual QsiTableNode VisitSpecialSubSelect(SpecialSubSelect specialSubSelect)
        {
            throw new NotImplementedException();
        }

        public virtual QsiTableNode VisitValuesList(ValuesList valuesList)
        {
            throw new NotImplementedException();
        }

        public virtual QsiTableNode VisitParenthesisFromItem(ParenthesisFromItem parenthesisFromItem)
        {
            return WrapFromItem(
                parenthesisFromItem,
                VisitFromItem(parenthesisFromItem.getFromItem()));
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
