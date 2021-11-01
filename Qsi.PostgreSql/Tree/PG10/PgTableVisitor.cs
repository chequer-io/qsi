using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Qsi.Data;
using Qsi.PostgreSql.Internal;
using Qsi.PostgreSql.Internal.PG10.Types;
using Qsi.PostgreSql.Tree.PG10.Nodes;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.PostgreSql.Tree.PG10
{
    internal class PgTableVisitor : PgVisitorBase
    {
        public PgTableVisitor(IPgVisitorSet set) : base(set)
        {
        }

        public QsiTableNode Visit(IPg10Node node)
        {
            switch (node)
            {
                case RawStmt rawStmt:
                    return VisitRawStmt(rawStmt);

                case SelectStmt selectStmt:
                    return VisitSelectStmt(selectStmt);
            }

            throw TreeHelper.NotSupportedTree(node);
        }

        public QsiTableNode VisitRawStmt(RawStmt rawStmt)
        {
            return Visit(rawStmt.stmt[0]);
        }

        public QsiTableNode VisitSelectStmt(SelectStmt selectStmt)
        {
            QsiTableNode tableNode;

            if (!ListUtility.IsNullOrEmpty(selectStmt.intoClause))
                throw TreeHelper.NotSupportedFeature("Into clause");

            switch (selectStmt.op)
            {
                case SetOperation.SETOP_NONE:
                    tableNode = VisitSelectStmtNone(selectStmt);
                    break;

                case SetOperation.SETOP_UNION:
                case SetOperation.SETOP_EXCEPT:
                case SetOperation.SETOP_INTERSECT:
                    tableNode = VisitSelectStmtComposite(selectStmt);
                    break;

                default:
                    throw TreeHelper.NotSupportedTree($"{selectStmt.GetType().Name}({selectStmt.op})");
            }

            if (ListUtility.IsNullOrEmpty(selectStmt.withClause))
                return tableNode;

            QsiDerivedTableNode derivedTableNode;

            if (tableNode is QsiDerivedTableNode derivedTable && derivedTable.Directives.IsEmpty)
            {
                derivedTableNode = derivedTable;
            }
            else
            {
                derivedTableNode = TreeHelper.Create<QsiDerivedTableNode>(n =>
                {
                    n.Columns.SetValue(TreeHelper.CreateAllColumnsDeclaration());
                    n.Source.SetValue(tableNode);
                });
            }

            derivedTableNode.Directives.SetValue(VisitWithClause(selectStmt.withClause[0]));

            return derivedTableNode;
        }

        private QsiTableNode VisitSelectStmtNone(SelectStmt stmt)
        {
            if (!ListUtility.IsNullOrEmpty(stmt.valuesLists))
                return VisitValueList(stmt.valuesLists);

            return TreeHelper.Create<QsiDerivedTableNode>(n =>
            {
                if (!ListUtility.IsNullOrEmpty(stmt.targetList))
                {
                    IEnumerable<ResTarget> targets = stmt.targetList.Cast<ResTarget>();
                    n.Columns.SetValue(VisitResTargets(targets));
                }

                if (!ListUtility.IsNullOrEmpty(stmt.fromClause))
                {
                    n.Source.SetValue(VisitFromClauses(n, stmt.fromClause));
                }
            });
        }

        private QsiInlineDerivedTableNode VisitValueList(IPg10Node[][] valueList)
        {
            return TreeHelper.Create<QsiInlineDerivedTableNode>(n =>
            {
                foreach (IPg10Node[] value in valueList)
                {
                    n.Rows.Add(TreeHelper.Create<QsiRowValueExpressionNode>(rn =>
                    {
                        rn.ColumnValues.AddRange(value.Select(ExpressionVisitor.Visit));
                    }));
                }
            });
        }

        private QsiCompositeTableNode VisitSelectStmtComposite(SelectStmt stmt)
        {
            return TreeHelper.Create<QsiCompositeTableNode>(n =>
            {
                n.Sources.Add(VisitSelectStmt(stmt.larg[0]));
                n.Sources.Add(VisitSelectStmt(stmt.rarg[0]));

                if (!stmt.op.HasValue)
                {
                    n.CompositeType = "UNION";
                }
                else
                {
                    n.CompositeType = stmt.op switch
                    {
                        SetOperation.SETOP_UNION => "UNION",
                        SetOperation.SETOP_EXCEPT => "EXCEPT",
                        SetOperation.SETOP_INTERSECT => "INTERSET",
                        _ => throw TreeHelper.NotSupportedFeature($"Composite Type: {stmt.op.Value}")
                    };
                }
            });
        }

        public QsiTableDirectivesNode VisitWithClause(WithClause withClause)
        {
            return TreeHelper.Create<QsiTableDirectivesNode>(n =>
            {
                n.IsRecursive = withClause.recursive ?? false;

                n.Tables.AddRange(withClause.ctes
                    .Cast<CommonTableExpr>()
                    .Select(VisitCommonTableExpression));
            });
        }

        private QsiDerivedTableNode VisitCommonTableExpression(CommonTableExpr cte)
        {
            return TreeHelper.Create<QsiDerivedTableNode>(n =>
            {
                var columnsDeclaration = new QsiColumnsDeclarationNode();

                if (ListUtility.IsNullOrEmpty(cte.aliascolnames))
                {
                    columnsDeclaration.Columns.Add(new QsiAllColumnNode());
                }
                else
                {
                    columnsDeclaration.Columns.AddRange(CreateSequentialColumnNodes(cte.aliascolnames.Cast<PgString>()));
                }

                n.Columns.SetValue(columnsDeclaration);
                n.Source.SetValue(Visit(cte.ctequery[0]));

                n.Alias.SetValue(new QsiAliasNode
                {
                    Name = new QsiIdentifier(cte.ctename, false)
                });
            });
        }

        public IEnumerable<QsiSequentialColumnNode> CreateSequentialColumnNodes(IEnumerable<PgString> uids)
        {
            return IdentifierVisitor.VisitStrings(uids)
                .Select(identifier => TreeHelper.Create<QsiSequentialColumnNode>(cn =>
                {
                    cn.ColumnType = QsiSequentialColumnType.Overwrite;

                    cn.Alias.SetValue(new QsiAliasNode
                    {
                        Name = identifier
                    });
                }));
        }

        public QsiColumnsDeclarationNode VisitResTargets(IEnumerable<ResTarget> targets)
        {
            return TreeHelper.Create<QsiColumnsDeclarationNode>(dn =>
            {
                dn.Columns.AddRange(targets.Select(VisitResTarget));
            });
        }

        public QsiSetColumnExpressionNode VisitSetColumn(ResTarget target)
        {
            return TreeHelper.Create<QsiSetColumnExpressionNode>(n =>
            {
                n.Target = new QsiQualifiedIdentifier(new QsiIdentifier(target.name, false));
                n.Value.Value = ExpressionVisitor.Visit(target.val[0]);
            });
        }

        public QsiColumnNode VisitResTarget(ResTarget target)
        {
            Debug.Assert(target.val.Length == 1);

            var value = target.val[0];
            QsiColumnNode columnNode;

            switch (value.Type)
            {
                case NodeTag.T_ColumnRef:
                    columnNode = VisitColumnRef((ColumnRef)value);
                    break;

                case NodeTag.T_A_Expr:
                case NodeTag.T_A_ArrayExpr:
                case NodeTag.T_A_Const:
                case NodeTag.T_FuncCall:
                case NodeTag.T_SubLink:
                case NodeTag.T_TypeCast:
                case NodeTag.T_TypeName:
                case NodeTag.T_Value:
                case NodeTag.T_CaseExpr:
                case NodeTag.T_BoolExpr:
                case NodeTag.T_RowExpr:
                case NodeTag.T_NullTest:
                case NodeTag.T_BooleanTest:
                case NodeTag.T_CoalesceExpr:
                case NodeTag.T_ParamRef:
                    columnNode = TreeHelper.Create<QsiDerivedColumnNode>(n =>
                    {
                        n.Expression.SetValue(ExpressionVisitor.Visit(value));
                    });

                    break;

                case NodeTag.T_ResTarget:
                    return TreeHelper.Create<QsiColumnReferenceNode>(n =>
                    {
                        n.Name = new QsiQualifiedIdentifier(new QsiIdentifier(target.name, false));
                    });

                default:
                    throw TreeHelper.NotSupportedTree(value);
            }

            if (string.IsNullOrEmpty(target.name))
                return columnNode;

            if (columnNode is not QsiDerivedColumnNode derivedColumnNode)
            {
                derivedColumnNode = new QsiDerivedColumnNode();
                derivedColumnNode.Column.SetValue(columnNode);
            }

            derivedColumnNode.Alias.SetValue(new QsiAliasNode
            {
                Name = new QsiIdentifier(target.name, false)
            });

            return derivedColumnNode;
        }

        public QsiColumnNode VisitColumnRef(ColumnRef columnRef)
        {
            var isAll = columnRef.fields[^1].Type == NodeTag.T_A_Star;

            if (isAll)
            {
                return TreeHelper.Create<QsiAllColumnNode>(n =>
                {
                    if (columnRef.fields.Length == 1)
                        return;

                    IEnumerable<PgString> pathFields = columnRef.fields
                        .SkipLast(1)
                        .Cast<PgString>();

                    n.Path = IdentifierVisitor.VisitStrings(pathFields);
                });
            }

            if (columnRef.fields.All(f => f.Type == NodeTag.T_String))
            {
                return new QsiColumnReferenceNode
                {
                    Name = IdentifierVisitor.VisitStrings(columnRef.fields.Cast<PgString>())
                };
            }

            throw TreeHelper.NotSupportedTree(columnRef);
        }

        public QsiTableNode VisitFromClauses(QsiDerivedTableNode parentNode, IEnumerable<IPg10Node> fromClauses)
        {
            QsiTableNode[] sources = fromClauses
                .Select(VisitFromClause)
                .ToArray();

            if (sources.Length == 1)
            {
                return sources[0];
            }

            if (sources.Length > 1)
            {
                // comma join

                var anchor = sources[0];

                foreach (var source in sources.Skip(1))
                {
                    var nextJoin = new QsiJoinedTableNode
                    {
                        IsComma = true
                    };

                    nextJoin.Left.SetValue(anchor);
                    nextJoin.Right.SetValue(source);
                    anchor = nextJoin;
                }

                return anchor;
            }

            return null;
        }

        public QsiTableNode VisitFromClause(IPg10Node fromClause)
        {
            return fromClause switch
            {
                RangeVar var => VisitRangeVar(var),
                RangeSubselect subselect => VisitRangeSubselect(subselect),
                RangeFunction function => VisitRangeFunction(function),
                JoinExpr joinExpr => VisitJoinExpression(joinExpr),
                _ => throw TreeHelper.NotSupportedTree(fromClause)
            };
        }

        // TODO: alias에 정의된 컬럼 + 정의되지 않은 컬럼 컴파일
        public QsiTableNode VisitRangeVar(RangeVar var)
        {
            var tableNode = new QsiTableReferenceNode
            {
                Identifier = IdentifierVisitor.VisitRangeVar(var)
            };

            if (ListUtility.IsNullOrEmpty(var.alias))
                return tableNode;

            return TreeHelper.Create<QsiDerivedTableNode>(n =>
            {
                Debug.Assert(var.alias.Length == 1);

                var alias = new QsiAliasNode
                {
                    Name = new QsiIdentifier(var.alias[0].aliasname, false)
                };

                QsiColumnsDeclarationNode columsDeclaration;

                PgString[] columns = var.alias[0].colnames?
                    .Cast<PgString>()
                    .ToArray();

                if (ListUtility.IsNullOrEmpty(columns))
                {
                    columsDeclaration = new QsiColumnsDeclarationNode();

                    columsDeclaration.Columns.Add(new QsiAllColumnNode
                    {
                        IncludeInvisibleColumns = true
                    });
                }
                else
                {
                    columsDeclaration = TreeHelper.Create<QsiColumnsDeclarationNode>(cn =>
                    {
                        cn.Columns.AddRange(CreateSequentialColumnNodes(columns));
                    });
                }

                n.Columns.SetValue(columsDeclaration);
                n.Source.SetValue(tableNode);
                n.Alias.SetValue(alias);
            });
        }

        private QsiTableNode VisitRangeSubselect(RangeSubselect subselect)
        {
            if (ListUtility.IsNullOrEmpty(subselect.subquery))
                return null;

            if (ListUtility.IsNullOrEmpty(subselect.alias))
                throw new QsiException(QsiError.NoAlias);

            if (subselect.alias.Length != 1)
                throw TreeHelper.NotSupportedTree(subselect);

            var source = VisitSelectStmt((SelectStmt)subselect.subquery[0]);

            var alias = new QsiAliasNode
            {
                Name = new QsiIdentifier(subselect.alias[0].aliasname, false)
            };

            QsiColumnsDeclarationNode columsDeclaration;

            PgString[] columns = subselect.alias[0].colnames?
                .Cast<PgString>()
                .ToArray();

            if (ListUtility.IsNullOrEmpty(columns))
            {
                columsDeclaration = TreeHelper.CreateAllColumnsDeclaration();
            }
            else
            {
                columsDeclaration = TreeHelper.Create<QsiColumnsDeclarationNode>(cn =>
                {
                    cn.Columns.AddRange(CreateSequentialColumnNodes(columns));
                });
            }

            if (source is QsiInlineDerivedTableNode inline)
            {
                inline.Alias.SetValue(alias);
                inline.Columns.SetValue(columsDeclaration);
                return source;
            }

            return TreeHelper.Create<QsiDerivedTableNode>(n =>
            {
                n.Columns.SetValue(columsDeclaration);
                n.Source.SetValue(source);
                n.Alias.SetValue(alias);
            });
        }

        private QsiTableNode VisitRangeFunction(RangeFunction rangeFunction)
        {
            if (ListUtility.IsNullOrEmpty(rangeFunction.alias))
                throw new QsiException(QsiError.NoAlias);

            if (rangeFunction.alias.Length != 1)
                throw TreeHelper.NotSupportedTree(rangeFunction);

            var func = rangeFunction.functions[0].Cast<FuncCall>().First();
            var source = ExpressionVisitor.VisitFunctionCall(func);

            var alias = new QsiAliasNode
            {
                Name = new QsiIdentifier(rangeFunction.alias[0].aliasname, false)
            };

            QsiColumnsDeclarationNode columsDeclaration;

            PgString[] columns = rangeFunction.alias[0].colnames?
                .Cast<PgString>()
                .ToArray();

            if (ListUtility.IsNullOrEmpty(columns))
            {
                columsDeclaration = TreeHelper.CreateAllColumnsDeclaration();
            }
            else
            {
                columsDeclaration = TreeHelper.Create<QsiColumnsDeclarationNode>(cn =>
                {
                    cn.Columns.AddRange(CreateSequentialColumnNodes(columns));
                });
            }

            return TreeHelper.Create<PgRangeFunctionNode>(n =>
            {
                n.Columns.Value = columsDeclaration;
                n.Source.Value = source;
                n.Alias.Value = alias;
            });
        }

        public QsiJoinedTableNode VisitJoinExpression(JoinExpr joinExpr)
        {
            return TreeHelper.Create<QsiJoinedTableNode>(n =>
            {
                Debug.Assert(!ListUtility.IsNullOrEmpty(joinExpr.larg));
                Debug.Assert(!ListUtility.IsNullOrEmpty(joinExpr.rarg));

                n.Left.SetValue(VisitFromClause(joinExpr.larg[0]));
                n.Right.SetValue(VisitFromClause(joinExpr.rarg[0]));

                n.JoinType = joinExpr.jointype switch
                {
                    JoinType.JOIN_INNER => "INNER JOIN",
                    JoinType.JOIN_LEFT => "LEFT JOIN",
                    JoinType.JOIN_RIGHT => "RIGHT JOIN",
                    JoinType.JOIN_FULL => "FULL JOIN",
                    _ => throw new QsiException(QsiError.Syntax)
                };

                if (joinExpr.isNatural ?? false)
                {
                    n.IsNatural = true;
                    n.JoinType = $"NATURAL {n.JoinType}";
                }

                if (!ListUtility.IsNullOrEmpty(joinExpr.usingClause))
                {
                    n.PivotColumns.SetValue(TreeHelper.Create<QsiColumnsDeclarationNode>(dn =>
                    {
                        foreach (var pgString in joinExpr.usingClause.Cast<PgString>())
                        {
                            dn.Columns.Add(new QsiColumnReferenceNode
                            {
                                Name = new QsiQualifiedIdentifier(new QsiIdentifier(pgString.str, false))
                            });
                        }
                    }));
                }
            });
        }

        private string ToString(IEnumerable<PgString> pgStrings)
        {
            return string.Join(".", pgStrings.Select(s => s.str));
        }
    }
}
