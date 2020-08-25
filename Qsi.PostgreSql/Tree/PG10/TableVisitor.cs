using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Qsi.Data;
using Qsi.PostgreSql.Internal;
using Qsi.PostgreSql.Internal.PG10.Types;
using Qsi.Tree.Base;
using Qsi.Utilities;

namespace Qsi.PostgreSql.Tree.PG10
{
    internal static class TableVisitor
    {
        public static QsiTableNode Visit(IPg10Node node)
        {
            switch (node)
            {
                case RawStmt rawStmt:
                    return VisitRawStmt(rawStmt);

                case SelectStmt selectStmt:
                    return VisitSelectStmt(selectStmt);

                case ViewStmt viewStmt:
                    return VisitViewStmt(viewStmt);
            }

            throw TreeHelper.NotSupportedTree(node);
        }

        public static QsiTableNode VisitRawStmt(RawStmt rawStmt)
        {
            return Visit(rawStmt.stmt[0]);
        }

        public static QsiDerivedTableNode VisitSelectStmt(SelectStmt selectStmt)
        {
            return TreeHelper.Create<QsiDerivedTableNode>(n =>
            {
                if (!ListUtility.IsNullOrEmpty(selectStmt.targetList))
                {
                    IEnumerable<ResTarget> targets = selectStmt.targetList.Cast<ResTarget>();
                    n.Columns.SetValue(VisitResTargets(targets));
                }

                if (!ListUtility.IsNullOrEmpty(selectStmt.fromClause))
                {
                    n.Source.SetValue(VisitFromClauses(n, selectStmt.fromClause));
                }

                if (!ListUtility.IsNullOrEmpty(selectStmt.withClause))
                {
                    n.Directives.SetValue(VisitWithClause(selectStmt.withClause[0]));
                }
            });
        }

        public static QsiDerivedTableNode VisitViewStmt(ViewStmt viewStmt)
        {
            return TreeHelper.Create<QsiDerivedTableNode>(n =>
            {
                var viewAccessNode = IdentifierVisitor.VisitRangeVar(viewStmt.view[0]);
                var columnsDeclaration = new QsiColumnsDeclarationNode();

                if (ListUtility.IsNullOrEmpty(viewStmt.aliases))
                {
                    columnsDeclaration.Columns.Add(new QsiAllColumnNode());
                }
                else
                {
                    columnsDeclaration.Columns.AddRange(CreateSequentialColumnNodes(viewStmt.aliases.Cast<PgString>()));
                }

                n.Columns.SetValue(columnsDeclaration);
                n.Source.SetValue(Visit(viewStmt.query[0]));

                n.Alias.SetValue(new QsiAliasNode
                {
                    Name = viewAccessNode[^1]
                });
            });
        }

        public static QsiTableDirectivesNode VisitWithClause(WithClause withClause)
        {
            return TreeHelper.Create<QsiTableDirectivesNode>(n =>
            {
                if (withClause.recursive ?? false)
                    throw TreeHelper.NotSupportedFeature("Recursive CTE");

                n.Tables.AddRange(withClause.ctes
                    .Cast<CommonTableExpr>()
                    .Select(VisitCommonTableExpression));
            });
        }

        private static QsiTableNode VisitCommonTableExpression(CommonTableExpr cte)
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

        public static IEnumerable<QsiSequentialColumnNode> CreateSequentialColumnNodes(IEnumerable<PgString> uids)
        {
            return IdentifierVisitor.VisitStrings(uids)
                .Select((identifier, i) => TreeHelper.Create<QsiSequentialColumnNode>(cn =>
                {
                    cn.Ordinal = i;

                    cn.Alias.SetValue(new QsiAliasNode
                    {
                        Name = identifier
                    });
                }));
        }

        public static QsiColumnsDeclarationNode VisitResTargets(IEnumerable<ResTarget> targets)
        {
            return TreeHelper.Create<QsiColumnsDeclarationNode>(dn =>
            {
                dn.Columns.AddRange(targets.Select(VisitResTarget));
            });
        }

        public static QsiColumnNode VisitResTarget(ResTarget target)
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
                    columnNode = TreeHelper.Create<QsiDerivedColumnNode>(n =>
                    {
                        n.Expression.SetValue(ExpressionVisitor.Visit(value));
                    });

                    break;

                default:
                    throw TreeHelper.NotSupportedTree(value);
            }

            if (string.IsNullOrEmpty(target.name))
                return columnNode;

            if (!(columnNode is QsiDerivedColumnNode derivedColumnNode))
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

        public static QsiColumnNode VisitColumnRef(ColumnRef columnRef)
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
                return new QsiDeclaredColumnNode
                {
                    Name = IdentifierVisitor.VisitColumnRef(columnRef)
                };
            }

            throw TreeHelper.NotSupportedTree(columnRef);
        }

        public static QsiTableNode VisitFromClauses(QsiDerivedTableNode parentNode, IEnumerable<IPg10Node> fromClauses)
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
                        JoinType = QsiJoinType.Full
                    };

                    nextJoin.Left.SetValue(anchor);
                    nextJoin.Right.SetValue(source);
                    anchor = nextJoin;
                }

                return anchor;
            }

            return null;
        }

        public static QsiTableNode VisitFromClause(IPg10Node fromClause)
        {
            return fromClause switch
            {
                RangeVar var => VisitRangeVar(var),
                RangeSubselect subselect => VisitRangeSubselect(subselect),
                RangeFunction function => VisitRangeFunction(function),
                JoinExpr joinExpr => ViseitJoinExpression(joinExpr),
                _ => throw TreeHelper.NotSupportedTree(fromClause)
            };
        }

        public static QsiTableNode VisitRangeVar(RangeVar var)
        {
            var tableNode = new QsiTableAccessNode
            {
                Identifier = IdentifierVisitor.VisitRangeVar(var)
            };

            if (ListUtility.IsNullOrEmpty(var.alias))
                return tableNode;

            return TreeHelper.Create<QsiDerivedTableNode>(n =>
            {
                Debug.Assert(var.alias.Length == 1);

                var allDeclaration = new QsiColumnsDeclarationNode();
                allDeclaration.Columns.Add(new QsiAllColumnNode());

                n.Columns.SetValue(allDeclaration);
                n.Source.SetValue(tableNode);

                n.Alias.SetValue(new QsiAliasNode
                {
                    Name = new QsiIdentifier(var.alias[0].aliasname, false)
                });
            });
        }

        private static QsiTableNode VisitRangeSubselect(RangeSubselect subselect)
        {
            if (ListUtility.IsNullOrEmpty(subselect.subquery))
                return null;

            if (ListUtility.IsNullOrEmpty(subselect.alias))
                throw new Exception("Every derived table must have its own alias");

            Debug.Assert(subselect.alias.Length == 1);
            Debug.Assert((subselect.alias[0].colnames?.Length ?? 0) == 0);

            return TreeHelper.Create<QsiDerivedTableNode>(n =>
            {
                var allDeclaration = new QsiColumnsDeclarationNode();
                allDeclaration.Columns.Add(new QsiAllColumnNode());

                n.Columns.SetValue(allDeclaration);
                n.Source.SetValue(VisitSelectStmt((SelectStmt)subselect.subquery[0]));

                n.Alias.SetValue(new QsiAliasNode
                {
                    Name = new QsiIdentifier(subselect.alias[0].aliasname, false)
                });
            });
        }

        private static QsiTableNode VisitRangeFunction(RangeFunction function)
        {
            // TODO: Implement table function
            throw TreeHelper.NotSupportedFeature("Table function");
        }

        public static QsiJoinedTableNode ViseitJoinExpression(JoinExpr joinExpr)
        {
            return TreeHelper.Create<QsiJoinedTableNode>(n =>
            {
                Debug.Assert(!ListUtility.IsNullOrEmpty(joinExpr.larg));
                Debug.Assert(!ListUtility.IsNullOrEmpty(joinExpr.rarg));

                n.Left.SetValue(VisitFromClause(joinExpr.larg[0]));
                n.Right.SetValue(VisitFromClause(joinExpr.rarg[0]));

                n.JoinType = joinExpr.jointype switch
                {
                    JoinType.JOIN_INNER => QsiJoinType.Inner,
                    JoinType.JOIN_LEFT => QsiJoinType.Left,
                    JoinType.JOIN_FULL => QsiJoinType.Full,
                    JoinType.JOIN_RIGHT => QsiJoinType.Right,
                    JoinType.JOIN_SEMI => QsiJoinType.Semi,
                    JoinType.JOIN_ANTI => QsiJoinType.Anti,
                    JoinType.JOIN_UNIQUE_OUTER => QsiJoinType.UniqueOuter,
                    JoinType.JOIN_UNIQUE_INNER => QsiJoinType.UniqueInner,
                    _ => throw new InvalidOperationException()
                };

                if (!ListUtility.IsNullOrEmpty(joinExpr.usingClause))
                {
                    n.PivotColumns.SetValue(TreeHelper.Create<QsiColumnsDeclarationNode>(dn =>
                    {
                        foreach (var pgString in joinExpr.usingClause.Cast<PgString>())
                        {
                            dn.Columns.Add(new QsiDeclaredColumnNode
                            {
                                Name = new QsiQualifiedIdentifier(new QsiIdentifier(pgString.str, false))
                            });
                        }
                    }));
                }
            });
        }

        private static string ToString(IEnumerable<PgString> pgStrings)
        {
            return string.Join(".", pgStrings.Select(s => s.str));
        }
    }
}
