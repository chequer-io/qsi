using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using PgQuery;
using Qsi.Data;
using Qsi.PostgreSql.Data;
using Qsi.PostgreSql.Tree;
using Qsi.PostgreSql.Tree.Nodes;
using Qsi.PostgreSql.OldTree.PG10.Nodes;
using Qsi.Tree;
using Boolean = PgQuery.Boolean;
using String = PgQuery.String;

namespace Qsi.PostgreSql;

public partial class PostgreSqlDeparser
{
    private static class Visitor
    {
        public static IPgNode Visit(QsiDataInsertActionNode node)
        {
            return new InsertStmt
            {
                Relation = Visit((PgTableReferenceNode)node.Target.Value),
                Cols = { node.Columns.Select(c => new ResTarget { Name = c[0].Value }.ToNode()) }
            };
        }

        public static IPgNode Visit(QsiDataUpdateActionNode node)
        {
            var target = node.Target.Value;
            Node? whereExpr = null;

            if (target is QsiDerivedTableNode { Where.Value: { } whereValue } derivedTable)
            {
                target = derivedTable.Source.Value;
                whereExpr = Visit(whereValue.Expression.Value);
            }

            if (Visit(target) is not { RangeVar: { } rangeVar })
                throw new InvalidOperationException("Target is not RangeVar");

            return new UpdateStmt
            {
                Relation = rangeVar,
                WhereClause = whereExpr,
                WithClause = node.Directives.IsEmpty ? null : Visit(node.Directives.Value),
                TargetList = { node.SetValues.Select(Visit) },
                // TODO: not all implemented yet: FromClause (feature/pg-official-parser)
                // FromClause = {  },
                // ReturningList = {  }
            };
        }

        public static IPgNode Visit(QsiDataDeleteActionNode node)
        {
            var target = node.Target.Value;
            Node? whereExpr = null;
            WithClause? withClause = null;

            if (target is QsiDerivedTableNode derivedTable and ({ Where.IsEmpty: false } or { Directives.IsEmpty: false }))
            {
                target = derivedTable.Source.Value;

                if (derivedTable.Where.Value is { } whereValue)
                {
                    whereExpr = Visit(whereValue.Expression.Value);
                }

                if (derivedTable.Directives.Value is { } directivesValue)
                {
                    withClause = Visit(directivesValue);
                }
            }

            if (Visit(target) is not { RangeVar: { } rangeVar })
                throw new InvalidOperationException("Target is not RangeVar");

            // Ignored usingClause, returningList
            return new DeleteStmt
            {
                Relation = rangeVar,
                WhereClause = whereExpr,
                WithClause = withClause,
            };
        }

        public static IPgNode Visit(PgTableDefinitionNode node)
        {
            if (node.IsCreateTableAs)
            {
                var stmt = new CreateTableAsStmt
                {
                    Into = new IntoClause
                    {
                        AccessMethod = node.AccessMethod,
                        Rel = new RangeVar
                        {
                            Relpersistence = ((char)node.Relpersistence).ToString()
                        },
                        OnCommit = node.OnCommit,
                    },
                    Query = Visit(node.DataSource.Value),
                    IfNotExists = node.ConflictBehavior is QsiDefinitionConflictBehavior.Ignore
                };

                if (node.Columns.Value.Columns.All(c => c is QsiSequentialColumnNode))
                    stmt.Into.ColNames.AddRange(node.Columns.Value.Columns.Select(Visit));

                return stmt;
            }

            // TODO: not all implemented yet (feature/pg-official-parser)
            return new CreateStmt
            {
            };
        }

        public static CommonTableExpr Visit(PgCommonTableNode node)
        {
            var expr = new CommonTableExpr
            {
                Ctequery = Visit(node.Source.Value),
                Ctematerialized = node.Materialized,
                Ctename = node.Alias.IsEmpty ? string.Empty : node.Alias.Value.Name.Value
            };

            if (node.Columns.Value.Columns.All(c => c is QsiSequentialColumnNode))
                expr.Aliascolnames.AddRange(node.Columns.Value.Columns.Select(Visit));

            return expr;
        }

        public static IPgNode Visit(PgAliasedTableNode node)
        {
            var alias = new Alias
            {
                Aliasname = node.Alias.Value.Name.Value
            };

            if (node.Columns.Value.Columns.All(c => c is QsiSequentialColumnNode))
                alias.Colnames.AddRange(node.Columns.Value.Columns.Select(Visit));

            var table = Visit(node.Source.Value);

            switch (table.NodeCase)
            {
                case Node.NodeOneofCase.RangeVar:
                    table.RangeVar.Alias = alias;
                    return table.RangeVar;

                case Node.NodeOneofCase.RangeSubselect:
                    table.RangeSubselect.Alias = alias;
                    return table.RangeVar;

                case Node.NodeOneofCase.SelectStmt:
                    return new RangeSubselect
                    {
                        Alias = alias,
                        Subquery = table.SelectStmt.ToNode()
                    };

                case Node.NodeOneofCase.JoinExpr:
                    table.JoinExpr.Alias = alias;
                    return table.JoinExpr;

                default:
                    throw new NotSupportedException($"Not supported Aliased table node: {table.NodeCase.ToString()}");
            }
        }

        public static SelectStmt Visit(QsiDerivedTableNode node)
        {
            var stmt = new SelectStmt
            {
                LimitOption = LimitOption.Default,
                Op = SetOperation.SetopNone
            };

            // WITH <with>
            if (!node.Directives.IsEmpty)
                stmt.WithClause = Visit(node.Directives.Value);

            // <target>
            if (!node.Columns.IsEmpty)
            {
                stmt.AddTargetList(node.Columns.Value.Columns.Select(c => c switch
                {
                    QsiAllColumnNode allColumn => new ResTarget { Val = Visit(allColumn).ToNode() },
                    QsiColumnReferenceNode cr => new ResTarget { Val = Visit(cr).ToNode() },
                    QsiDerivedColumnNode dc => Visit(dc),
                    _ => new ResTarget()
                }));
            }

            // FROM <Source>
            if (!node.Source.IsEmpty)
            {
                var sourceTable = node.Source.Value;

                if (sourceTable is QsiJoinedTableNode { IsComma: true } joinedSourceTable)
                {
                    stmt.FromClause.Add(GetCommaJoinedSources(joinedSourceTable).Select(Visit));
                }
                else
                {
                    stmt.FromClause.Add(Visit(node.Source.Value));
                }
            }

            // WHERE <filter>
            if (node.Where.Value is { } whereExpression)
            {
                stmt.WhereClause = Visit(whereExpression.Expression.Value);
            }

            // LIMIT <limit> OFFSET <offset>
            if (node.Limit.Value is PgLimitExpressionNode limit)
            {
                stmt.LimitOption = limit.Option;
                stmt.LimitCount = Visit(limit.Limit.Value);
                stmt.LimitOffset = Visit(limit.Offset.Value);
            }

            // GROUP BY ~~~ HAVING ~~~
            if (node.Grouping.Value is { } grouping)
            {
                stmt.GroupClause.AddRange(grouping.Items.Select(Visit));

                if (grouping.Having.Value is { } having)
                    stmt.HavingClause = Visit(having);
            }

            // ORDER BY sort_clause1, sort_clause2..
            if (node.Order.Value is { } order)
            {
                stmt.SortClause.AddRange(order.Orders.Select(Visit));
            }

            return stmt;
        }

        private static IEnumerable<QsiTableNode> GetCommaJoinedSources(QsiJoinedTableNode joinedTable)
        {
            switch (joinedTable.Left.Value)
            {
                case QsiJoinedTableNode { IsComma: true } innerJoinedTable:
                {
                    foreach (var source in GetCommaJoinedSources(innerJoinedTable))
                        yield return source;

                    break;
                }

                default:
                    yield return joinedTable.Left.Value;

                    break;
            }

            yield return joinedTable.Right.Value;
        }

        public static A_Const Visit(QsiLiteralExpressionNode node)
        {
            return node.Value switch
            {
                int i => new A_Const { Ival = new Integer { Ival = i } },
                float f => new A_Const { Fval = new Float { Fval = f.ToString(CultureInfo.InvariantCulture) } },
                bool b => new A_Const { Boolval = new Boolean { Boolval = b } },
                string s => new A_Const { Sval = new String { Sval = s } },
                PgBinaryString bs => new A_Const { Bsval = new BitString { Bsval = bs.Value } },
                null => new A_Const { Isnull = true },
                _ => throw new NotSupportedException($"Not supported literal value: {node.Value.GetType().Name}")
            };
        }

        public static BoolExpr Visit(PgBooleanMultipleExpressionNode node)
        {
            return new BoolExpr
            {
                Args = { node.Elements.Select(Visit) },
                Boolop = node.Type
            };
        }

        public static A_ArrayExpr Visit(QsiMultipleExpressionNode node)
        {
            return new A_ArrayExpr
            {
                Elements = { node.Elements.Select(Visit) }
            };
        }

        public static String Visit(QsiSequentialColumnNode node)
        {
            return new String
            {
                Sval = node.Alias.Value.Name.Value
            };
        }

        public static WithClause Visit(QsiTableDirectivesNode node)
        {
            return new WithClause
            {
                Recursive = node.IsRecursive,
                Ctes =
                {
                    node.Tables.OfType<PgCommonTableNode>()
                        .Select(Visit)
                        .Select(i => i.ToNode())
                }
            };
        }

        public static IPgNode Visit(PgUnaryExpressionNode node)
        {
            if (node.BoolKind is not BoolExprType.Undefined)
            {
                return new BoolExpr
                {
                    Args = { Visit(node.Expression.Value) },
                    Boolop = node.BoolKind
                };
            }

            return new A_Expr
            {
                Rexpr = Visit(node.Expression.Value),
                Kind = node.ExprKind,
                Name = { node.Operator.Split('.').Select(s => new String { Sval = s }.ToNode()) }
            };
        }

        public static A_Expr Visit(PgBinaryExpressionNode node)
        {
            if (node.ExprKind is A_Expr_Kind.Undefined)
                throw new NotSupportedException("PgBinaryExpressionNode must include BoolType or ExprKind");

            return new A_Expr
            {
                Lexpr = Visit(node.Left.Value),
                Rexpr = Visit(node.Right.Value),
                Kind = node.ExprKind,
                Name = { node.Operator.Split('.').Select(s => new String { Sval = s }.ToNode()) }
            };
        }

        public static IPgNode Visit(PgInvokeExpressionNode node)
        {
            if (node.IsBuiltIn)
            {
                string funcName = node.Member.Value.Identifier[0].Value;

                return funcName switch
                {
                    "GROUPING" => new GroupingFunc { Args = { node.Parameters.Select(Visit) } },
                    "COALESCE" => new CoalesceExpr { Args = { node.Parameters.Select(Visit) } },
                    "GREATEST" => new MinMaxExpr
                    {
                        Op = MinMaxOp.IsGreatest,
                        Args = { node.Parameters.Select(Visit) }
                    },
                    "LEAST" => new MinMaxExpr
                    {
                        Op = MinMaxOp.IsLeast,
                        Args = { node.Parameters.Select(Visit) }
                    },
                    _ => throw new NotSupportedException($"Not supported function to create PgNode: {funcName}")
                };
            }

            return new FuncCall
            {
                Funcname = { ToStringNodes(node.Member.Value.Identifier) },
                Funcformat = node.FunctionFormat,
                Args = { node.Parameters.Select(Visit) },
                AggStar = node.AggregateStar,
                AggDistinct = node.AggregateDistinct,
                AggOrder = { node.AggregateOrder.Select(Visit) },
                AggFilter = Visit(node.AggregateFilter.Value),
                AggWithinGroup = node.AggregateWithInGroup,
                FuncVariadic = node.FunctionVariadic,
                Over = Visit(node.Over.Value)
            };
        }

        public static NamedArgExpr Visit(PgNamedParameterExpressionNode node)
        {
            return new NamedArgExpr
            {
                Name = node.Name,
                Arg = Visit(node.Expression.Value)
            };
        }

        public static IPgNode Visit(PgRowValueExpressionNode node)
        {
            return new RowExpr
            {
                Args = { node.ColumnValues.Select(Visit) },
                RowFormat = node.IsExplicit ? CoercionForm.CoerceExplicitCall : CoercionForm.CoerceImplicitCast
            };
        }

        public static IPgNode Visit(PgSqlValueInvokeExpressionNode node)
        {
            var function = new SQLValueFunction
            {
                Op = node.FunctionOp
            };

            if (node.Parameters.Count >= 1 &&
                node.Parameters[0] is QsiLiteralExpressionNode { Type: QsiDataType.Numeric } typmod)
            {
                function.Typmod = (int)typmod.Value;
            }
            else
            {
                function.Typmod = -1;
            }

            return function;
        }

        public static IPgNode Visit(PgBooleanTestExpressionNode node)
        {
            return new BooleanTest
            {
                Arg = Visit(node.Target.Value),
                Booltesttype = node.BoolTestType
            };
        }

        public static ParamRef Visit(QsiBindParameterExpressionNode node)
        {
            return new ParamRef
            {
                Number = (int)(node.Index.HasValue ? node.Index + 1 : 0)
            };
        }

        public static CollateClause Visit(PgCollateExpressionNode node)
        {
            return new CollateClause
            {
                Collname = { ToStringNodes(node.Column) },
                Arg = Visit(node.Expression.Value)
            };
        }

        public static IPgNode Visit(PgCastExpressionNode node)
        {
            return new TypeCast
            {
                Arg = Visit(node.Source.Value),
                TypeName = Visit((PgTypeExpressionNode)node.Type.Value),
            };
        }

        public static TypeName Visit(PgTypeExpressionNode node)
        {
            return new TypeName
            {
                Names = { ToStringNodes(node.Identifier) },
                PctType = node.PctType,
                Setof = node.Setof,
                Typmods = { node.TypMods.Select(Visit) },
                ArrayBounds = { node.ArrayBounds.Select(Visit) }
            };
        }

        public static IPgNode Visit(PgIndirectionExpressionNode node)
        {
            return new A_Indirection
            {
                Arg = Visit(node.Target.Value.Expression.Value),
                Indirection = { node.Indirections.Select(Visit) }
            };
        }

        public static ResTarget Visit(QsiDerivedColumnNode node)
        {
            var resTarget = new ResTarget();

            if (!node.Expression.IsEmpty)
            {
                resTarget.Val = Visit(node.Expression.Value);
            }

            if (!node.Column.IsEmpty)
            {
                var colRef = Visit(node.Column.Value);

                if (colRef.NodeCase is not Node.NodeOneofCase.ColumnRef)
                    throw new InvalidOperationException("Visit QsiDerivedColumnNode.Column result not ColumnRef");

                resTarget.Val = colRef;
            }

            if (!node.Alias.IsEmpty)
                resTarget.Name = node.Alias.Value.Name.ToString();

            return resTarget;
        }

        public static A_Indices Visit(PgIndexExpressionNode node)
        {
            var index = new A_Indices
            {
                Uidx = Visit(node.Index.Value)
            };

            if (!node.IndexEnd.IsEmpty)
            {
                index.IsSlice = true;
                index.Lidx = Visit(node.IndexEnd.Value);
            }

            return index;
        }

        public static SubLink Visit(PgSubLinkExpressionNode node)
        {
            return new SubLink
            {
                Testexpr = Visit(node.Expression.Value),
                Subselect = Visit(node.Table.Value),
                OperName = { ToStringNodes(node.OperatorName) },
                SubLinkType = node.SubLinkType switch
                {
                    "" => SubLinkType.ExprSublink,
                    "ALL" => SubLinkType.AllSublink,
                    "ANY" => SubLinkType.AnySublink,
                    "EXISTS" => SubLinkType.ExistsSublink,
                    "ARRAY" => SubLinkType.ArraySublink,
                    _ => throw new NotSupportedException($"Not supported SubLinkType: {node.SubLinkType}")
                }
            };
        }

        public static IPgNode Visit(PgNullTestExpressionNode node)
        {
            return new NullTest
            {
                Nulltesttype = node.IsNot ? NullTestType.IsNotNull : NullTestType.IsNull,
                Arg = Visit(node.Target.Value)
            };
        }

        public static ColumnRef Visit(QsiAllColumnNode node)
        {
            var colRef = new ColumnRef();

            if (node.Path is { } path)
            {
                for (int i = 0; i < path.Level; i++)
                {
                    colRef.Fields.Add(new Node
                    {
                        String = new String
                        {
                            Sval = path[i].Value
                        }
                    });
                }
            }

            colRef.Fields.Add(new A_Star().ToNode());

            return colRef;
        }

        public static CaseExpr Visit(QsiSwitchExpressionNode node)
        {
            var caseExpr = new CaseExpr();

            // CASE <Value>
            if (node.Value.Value is { } caseArg)
                caseExpr.Arg = Visit(caseArg);

            // WHEN <Expr> THEN <Result>
            foreach (var caseItem in node.Cases)
            {
                if (!caseItem.Condition.IsEmpty)
                {
                    caseExpr.Args.Add(Visit(caseItem).ToNode());
                }
                // ELSE <Result>
                else
                {
                    caseExpr.Defresult = Visit(caseItem.Consequent.Value);
                }
            }

            return caseExpr;
        }

        public static CaseWhen Visit(QsiSwitchCaseExpressionNode node)
        {
            return new CaseWhen
            {
                Expr = Visit(node.Condition.Value),
                Result = Visit(node.Consequent.Value)
            };
        }

        public static SetToDefault Visit(PgDefaultExpressionNode node)
        {
            return new SetToDefault();
        }

        public static WindowDef Visit(PgWindowDefExpressionNode node)
        {
            return new WindowDef
            {
                Name = node.Name?.Value ?? string.Empty,
                Refname = node.Refname?.Value ?? string.Empty,
                PartitionClause = { node.PartitionClause.Select(Visit) },
                OrderClause = { node.OrderClause.Select(Visit) },
                FrameOptions = (int)node.FrameOptions,
                StartOffset = Visit(node.StartOffset.Value),
                EndOffset = Visit(node.EndOffset.Value)
            };
        }

        public static List Visit(QsiRowValueExpressionNode node)
        {
            return new List
            {
                Items = { node.ColumnValues.Select(Visit) }
            };
        }

        public static ColumnRef Visit(QsiColumnExpressionNode node)
        {
            if (node.Column.Value is not QsiColumnReferenceNode columnReference)
                throw new NotSupportedException("Column Expression value is not QsiColumnReferenceNode");

            return Visit(columnReference);
        }

        public static IPgNode Visit(QsiTableExpressionNode node)
        {
            return node.Table.Value switch
            {
                PgCompositeTableNode composite => Visit(composite),
                QsiDerivedTableNode derived => Visit(derived),
                QsiInlineDerivedTableNode inlineDerived => Visit(inlineDerived),
                PgJoinedTableNode joined => Visit(joined),
                QsiTableDirectivesNode directives => Visit(directives),
                PgTableReferenceNode reference => Visit(reference),
                _ => throw new NotSupportedException($"not supported table expression node: {node.GetType().Name}")
            };
        }

        public static VariableSetStmt Visit(QsiChangeSearchPathActionNode node)
        {
            if (node.Identifiers is { Length: 0 })
                throw new QsiException(QsiError.Internal, "QsiChangeSearchPathActionNode identifier length is 0");

            return new VariableSetStmt
            {
                Name = "search_path",
                IsLocal = false,
                Kind = VariableSetKind.VarSetValue,
                Args = { node.Identifiers[0].Select(i => new A_Const { Sval = new String { Sval = i.Value } }.ToNode()) }
            };
        }

        public static RangeVar Visit(PgTableReferenceNode node)
        {
            var id = node.Identifier;

            return new RangeVar
            {
                Catalogname = id.Level >= 3 ? node.Identifier[^3].Value : string.Empty,
                Schemaname = id.Level >= 2 ? node.Identifier[^2].Value : string.Empty,
                Relname = id.Level >= 1 ? node.Identifier[^1].Value : string.Empty,
                Inh = node.IsInherit,
                Relpersistence = ((char)node.Relpersistence).ToString()
            };
        }

        public static ViewStmt Visit(PgViewDefinitionNode node)
        {
            var id = node.Identifier;

            var stmt = new ViewStmt
            {
                Query = Visit(node.Source.Value),
                View = new RangeVar
                {
                    Catalogname = id.Level >= 3 ? node.Identifier[^3].Value : string.Empty,
                    Schemaname = id.Level >= 2 ? node.Identifier[^2].Value : string.Empty,
                    Relname = id.Level >= 1 ? node.Identifier[^1].Value : string.Empty,
                },
                Options =
                {
                    node.Options
                        .Where(o => o is not null)
                        .Select(o => Visit(o!).ToNode())
                },
                Replace = node.ConflictBehavior is QsiDefinitionConflictBehavior.Replace,
            };

            if (node.Columns.Value.Columns.All(c => c is QsiSequentialColumnNode))
                stmt.Aliases.AddRange(node.Columns.Value.Columns.Select(Visit));

            return stmt;
        }

        public static SelectStmt Visit(QsiInlineDerivedTableNode node)
        {
            return new SelectStmt
            {
                LimitOption = LimitOption.Default,
                Op = SetOperation.SetopNone,
                ValuesLists = { node.Rows.Select(Visit).Select(n => n.ToNode()) }
            };
        }

        public static SelectStmt Visit(PgCompositeTableNode node)
        {
            return new SelectStmt
            {
                LimitOption = LimitOption.Default,
                Op = node.CompositeType switch
                {
                    "EXCEPT" => SetOperation.SetopExcept,
                    "INTERSECT" => SetOperation.SetopIntersect,
                    "UNION" => SetOperation.SetopUnion,
                    _ => throw new QsiException(QsiError.Syntax)
                },
                All = node.IsAll,
                Larg = Visit(node.Sources[0]).SelectStmt,
                Rarg = Visit(node.Sources[1]).SelectStmt,
            };
        }

        public static GroupingSet Visit(PgGroupingSetExpressionNode node)
        {
            return new GroupingSet
            {
                Kind = node.Kind,
                Content = { node.Expressions.Select(Visit) }
            };
        }

        public static ColumnRef Visit(QsiColumnReferenceNode node)
        {
            return new ColumnRef
            {
                Fields = { node.Name.Select(i => new String { Sval = i.Value }.ToNode()) }
            };
        }

        public static IPgNode Visit(PgOrderExpressionNode node)
        {
            return new SortBy
            {
                SortbyDir = node.Order switch
                {
                    QsiSortOrder.Ascending when node.SoryByUsing => SortByDir.SortbyUsing,
                    QsiSortOrder.Ascending when node.IsDefault => SortByDir.SortbyDefault,
                    QsiSortOrder.Ascending => SortByDir.SortbyAsc,
                    QsiSortOrder.Descending => SortByDir.SortbyDesc,
                    _ => throw new NotSupportedException(node.Order.ToString())
                },
                Node = Visit(node.Expression.Value),
                SortbyNulls = node.SortByNulls,
                UseOp = { node.UsingOperator.Select(Visit) }
            };
        }

        public static JoinExpr Visit(PgJoinedTableNode node)
        {
            var join = new JoinExpr
            {
                Larg = Visit(node.Left.Value),
                Rarg = Visit(node.Right.Value),
                Jointype = node.JoinType switch
                {
                    "CROSS JOIN" => JoinType.JoinInner,
                    "FULL JOIN" => JoinType.JoinFull,
                    "LEFT JOIN" => JoinType.JoinLeft,
                    "RIGHT JOIN" => JoinType.JoinRight,
                    _ => JoinType.Undefined
                },
                IsNatural = node.IsNatural
            };

            if (!node.PivotColumns.IsEmpty)
            {
                join.UsingClause.AddRange(
                    node.PivotColumns.Value.Columns
                        .Select(c => new String { Sval = ((QsiColumnReferenceNode)c).Name[0].Value }.ToNode())
                );

                if (node.JoinUsingAlias is { } joinAlias)
                    join.JoinUsingAlias = new Alias { Aliasname = joinAlias.Value };
            }

            if (node.PivotExpression.Value is { } onExpr)
                join.Quals = Visit(onExpr);

            return join;
        }

        public static DefElem Visit(PgDefinitionElementNode node)
        {
            return new DefElem
            {
                Defname = node.DefinitionName,
                Defnamespace = node.DefinitionNamespace,
                Defaction = node.Action,
                Arg = Visit(node.Expression.Value)
            };
        }

        private static IEnumerable<Node> ToStringNodes(QsiQualifiedIdentifier? identifier)
        {
            return identifier is not { }
                ? Enumerable.Empty<Node>()
                : identifier.Select(i => new String { Sval = i.Value }.ToNode());
        }

        [return: NotNullIfNotNull("node")]
        public static Node? Visit(IQsiTreeNode? node)
        {
            if (node is null)
                return null;

            return (node switch
            {
                QsiDataInsertActionNode qsiDataInsertAction => Visit(qsiDataInsertAction),
                PgCommonTableNode pgCommonTable => Visit(pgCommonTable),
                PgAliasedTableNode pgAliasedTable => Visit(pgAliasedTable),
                QsiDerivedTableNode qsiDerivedTable => Visit(qsiDerivedTable),
                QsiDataUpdateActionNode qsiDataUpdateAction => Visit(qsiDataUpdateAction),
                QsiDataDeleteActionNode qsiDataDeleteAction => Visit(qsiDataDeleteAction),
                QsiLiteralExpressionNode qsiLiteralExpression => Visit(qsiLiteralExpression),
                PgTableDefinitionNode pgTableDefinition => Visit(pgTableDefinition),
                PgUnaryExpressionNode pgUnaryExpression => Visit(pgUnaryExpression),
                PgBinaryExpressionNode qsiBinaryExpression => Visit(qsiBinaryExpression),
                PgBooleanMultipleExpressionNode pgBooleanMultiple => Visit(pgBooleanMultiple),
                PgSqlValueInvokeExpressionNode pgSqlValueInvokeExpression => Visit(pgSqlValueInvokeExpression),
                PgInvokeExpressionNode pgInvokeExpression => Visit(pgInvokeExpression),
                PgNamedParameterExpressionNode pgNamedParameterExpression => Visit(pgNamedParameterExpression),
                PgRowValueExpressionNode pgRowValueExpression => Visit(pgRowValueExpression),
                PgBooleanTestExpressionNode pgBooleanTestExpression => Visit(pgBooleanTestExpression),
                QsiBindParameterExpressionNode qsiBindParameterExpression => Visit(qsiBindParameterExpression),
                PgCollateExpressionNode pgCollateExpression => Visit(pgCollateExpression),
                PgCastExpressionNode pgCastExpression => Visit(pgCastExpression),
                PgTypeExpressionNode pgTypeExpression => Visit(pgTypeExpression),
                PgIndirectionExpressionNode pgIndirectionExpression => Visit(pgIndirectionExpression),
                QsiDerivedColumnNode qsiDerivedColumn => Visit(qsiDerivedColumn),
                PgIndexExpressionNode pgIndexExpression => Visit(pgIndexExpression),
                PgSubLinkExpressionNode pgSubLinkExpression => Visit(pgSubLinkExpression),
                PgNullTestExpressionNode pgNullTestExpression => Visit(pgNullTestExpression),
                QsiMultipleExpressionNode qsiMultipleExpression => Visit(qsiMultipleExpression),
                QsiAllColumnNode qsiAllColumn => Visit(qsiAllColumn),
                QsiSwitchExpressionNode qsiSwitchExpression => Visit(qsiSwitchExpression),
                QsiSwitchCaseExpressionNode qsiSwitchCaseExpression => Visit(qsiSwitchCaseExpression),
                PgDefaultExpressionNode pgDefaultExpression => Visit(pgDefaultExpression),
                PgWindowDefExpressionNode pgWindowDefExpression => Visit(pgWindowDefExpression),
                QsiRowValueExpressionNode qsiRowValueExpression => Visit(qsiRowValueExpression),
                QsiColumnExpressionNode qsiColumnExpression => Visit(qsiColumnExpression),
                QsiTableExpressionNode qsiTableExpression => Visit(qsiTableExpression),
                QsiChangeSearchPathActionNode qsiChangeSearchPathAction => Visit(qsiChangeSearchPathAction),
                PgTableReferenceNode pgTableReference => Visit(pgTableReference),
                PgViewDefinitionNode pgViewDefinition => Visit(pgViewDefinition),
                QsiColumnReferenceNode qsiColumnReference => Visit(qsiColumnReference),
                PgJoinedTableNode pgJoinedTable => Visit(pgJoinedTable),
                QsiTableDirectivesNode qsiTableDirectives => Visit(qsiTableDirectives),
                QsiSequentialColumnNode qsiSequentialColumn => Visit(qsiSequentialColumn),
                QsiInlineDerivedTableNode qsiInlineDerived => Visit(qsiInlineDerived),
                PgCompositeTableNode pgCompositeTable => Visit(pgCompositeTable),
                PgGroupingSetExpressionNode pgGroupingSet => Visit(pgGroupingSet),
                PgOrderExpressionNode pgOrder => Visit(pgOrder),
                PgDefinitionElementNode definitionElement => Visit(definitionElement),
                _ => throw new NotSupportedException($"Cannot Visit({node.GetType().Name})")
            }).ToNode();
        }
    }
}
