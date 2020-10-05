using System;
using System.Collections.Generic;
using System.Linq;
using net.sf.jsqlparser.expression;
using net.sf.jsqlparser.expression.operators.relational;
using net.sf.jsqlparser.schema;
using net.sf.jsqlparser.statement;
using net.sf.jsqlparser.statement.create.view;
using net.sf.jsqlparser.statement.@select;
using net.sf.jsqlparser.statement.values;
using Qsi.Data;
using Qsi.JSql.Extensions;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.JSql.Tree
{
    public class JSqlTableVisitor : JSqlVisitorBase
    {
        public JSqlTableVisitor(IJSqlVisitorContext context) : base(context)
        {
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

        public virtual QsiColumnsDeclarationNode CreateSequentialColumnNodes(IEnumerable<string> columns)
        {
            return CreateSequentialColumnNodes(columns.Select(c => (MultiPartName)new FakeMultiPartName(c)));
        }

        public virtual QsiColumnsDeclarationNode CreateSequentialColumnNodes(IEnumerable<Alias.AliasColumn> columns)
        {
            return CreateSequentialColumnNodes(columns.Select(c => (MultiPartName)new FakeMultiPartName(c.name)));
        }

        public virtual QsiColumnsDeclarationNode CreateSequentialColumnNodes(IEnumerable<Column> columns)
        {
            return CreateSequentialColumnNodes(columns.Cast<MultiPartName>());
        }

        public virtual QsiColumnsDeclarationNode CreateSequentialColumnNodes(IEnumerable<MultiPartName> columns)
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

        public virtual QsiDerivedTableNode VisitValuesStatement(ValuesStatement valuesStatement)
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
            QsiColumnNode columnNode;

            switch (expression)
            {
                case Column column:
                    columnNode = VisitColumn(column);
                    break;

                case NumericBind numericBind:
                    columnNode = VisitNumericBind(numericBind);
                    break;

                case JdbcParameter jdbcParameter:
                    columnNode = VisitJdbcParameter(jdbcParameter);
                    break;

                case JdbcNamedParameter jdbcNamedParameter:
                    columnNode = VisitJdbcNamedParameter(jdbcNamedParameter);
                    break;

                default:
                    return TreeHelper.Create<QsiDerivedColumnNode>(n =>
                    {
                        n.Expression.SetValue(ExpressionVisitor.Visit(expression));

                        if (alias != null)
                            n.Alias.SetValue(VisitAlias(alias));
                    });
            }

            if (alias == null)
                return columnNode;

            return TreeHelper.Create<QsiDerivedColumnNode>(n =>
            {
                n.Column.SetValue(columnNode);
                n.Alias.SetValue(VisitAlias(alias));
            });
        }

        public virtual QsiColumnNode VisitColumn(Column column)
        {
            return TreeHelper.Create<QsiDeclaredColumnNode>(n =>
            {
                n.Name = IdentifierVisitor.VisitMultiPartName(column);
            });
        }

        public virtual QsiBindingColumnNode VisitNumericBind(NumericBind expression)
        {
            return new QsiBindingColumnNode
            {
                Id = expression.getBindId().ToString()
            };
        }

        public virtual QsiBindingColumnNode VisitJdbcParameter(JdbcParameter expression)
        {
            return new QsiBindingColumnNode
            {
                Id = expression.toString()
            };
        }

        public virtual QsiBindingColumnNode VisitJdbcNamedParameter(JdbcNamedParameter expression)
        {
            return new QsiBindingColumnNode
            {
                Id = expression.getName()
            };
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

                case LateralSubSelect lateralSubSelect:
                    return VisitLateralSubSelect(lateralSubSelect);

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
            var alias = fromItem.getAlias();

            if (alias == null)
                return tableNode;

            QsiDerivedTableNode derivedNode;
            IList<Alias.AliasColumn> aliasColumns = alias.getAliasColumns().AsList<Alias.AliasColumn>();

            if (tableNode is QsiDerivedTableNode derivedTableNode &&
                derivedTableNode.Alias.IsEmpty &&
                ListUtility.IsNullOrEmpty(aliasColumns))
            {
                derivedNode = derivedTableNode;
            }
            else
            {
                derivedNode = TreeHelper.Create<QsiDerivedTableNode>(n =>
                {
                    n.Columns.SetValue(ListUtility.IsNullOrEmpty(aliasColumns)
                        ? TreeHelper.CreateAllColumnsDeclaration() :
                        CreateSequentialColumnNodes(aliasColumns));

                    n.Source.SetValue(tableNode);
                });
            }

            derivedNode.Alias.SetValue(VisitAlias(alias));

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

        public virtual QsiTableNode VisitSubSelect(SubSelect subSelect)
        {
            return WrapFromItem(
                subSelect,
                VisitSelectBody(subSelect.getSelectBody()));
        }

        public virtual QsiTableNode VisitTableFunction(TableFunction tableFunction)
        {
            // TODO: Implement table function
            throw TreeHelper.NotSupportedFeature("Table function");
        }

        public virtual QsiTableNode VisitLateralSubSelect(LateralSubSelect lateralSubSelect)
        {
            return WrapFromItem(
                lateralSubSelect,
                VisitSubSelect(lateralSubSelect.getSubSelect()));
        }

        public virtual QsiTableNode VisitValuesList(ValuesList valuesList)
        {
            return TreeHelper.Create<QsiInlineDerivedTableNode>(n =>
            {
                IEnumerable<QsiRowValueExpressionNode> rows = valuesList.getMultiExpressionList().getExpressionLists()
                    .AsEnumerable<ExpressionList>()
                    .Select(e => TreeHelper.Create<QsiRowValueExpressionNode>(rn =>
                    {
                        rn.ColumnValues.AddRange(e.getExpressions()
                            .AsEnumerable<Expression>()
                            .Select(ExpressionVisitor.Visit));
                    }));

                n.Rows.AddRange(rows);

                var alias = valuesList.getAlias();

                if (alias != null)
                {
                    n.Alias.SetValue(VisitAlias(alias));

                    QsiColumnsDeclarationNode columns = null;

                    if (alias.getAliasColumns() != null)
                    {
                        columns = CreateSequentialColumnNodes(alias.getAliasColumns().AsEnumerable<Alias.AliasColumn>());
                    }

                    if (IsEmpty(columns) && valuesList.getColumnNames() != null)
                    {
                        columns = CreateSequentialColumnNodes(valuesList.getColumnNames().AsEnumerable<string>());
                    }

                    if (IsEmpty(columns))
                    {
                        columns = TreeHelper.CreateAllColumnsDeclaration();
                    }

                    n.Columns.SetValue(columns);
                }
            });

            static bool IsEmpty(QsiColumnsDeclarationNode c)
            {
                return c == null || c.Columns.Count == 0;
            }
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
            return TreeHelper.Create<QsiDerivedTableNode>(n =>
            {
                IList<string> columns = createView.getColumnNames().AsList<string>();

                n.Columns.SetValue(
                    ListUtility.IsNullOrEmpty(columns) ?
                        TreeHelper.CreateAllColumnsDeclaration() :
                        CreateSequentialColumnNodes(columns.Select(c => (MultiPartName)new FakeMultiPartName(c))));

                n.Source.SetValue(VisitSelect(createView.getSelect()));

                n.Alias.SetValue(new QsiAliasNode
                {
                    Name = IdentifierVisitor.VisitMultiPartName(createView.getView())[^1]
                });
            });
        }

        private readonly struct FakeMultiPartName : MultiPartName
        {
            private readonly string _qualifiedName;

            public FakeMultiPartName(string qualifiedName)
            {
                _qualifiedName = qualifiedName;
            }

            string MultiPartName.getFullyQualifiedName()
            {
                return _qualifiedName;
            }
        }
    }
}
