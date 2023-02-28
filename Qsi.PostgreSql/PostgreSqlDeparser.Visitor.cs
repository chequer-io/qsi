using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Google.Protobuf.Collections;
using PgQuery;
using Qsi.Data;
using Qsi.PostgreSql.Data;
using Qsi.PostgreSql.Extensions;
using Qsi.PostgreSql.Tree;
using Qsi.PostgreSql.Tree.Nodes;
using Qsi.Tree;
using Boolean = PgQuery.Boolean;
using String = PgQuery.String;

namespace Qsi.PostgreSql;

public partial class PostgreSqlDeparser
{
    private static class Visitor
    {
        public static InsertStmt Visit(PgDataInsertActionNode node)
        {
            SelectStmt? selectStmt;

            if (node.Values.Count > 0)
            {
                selectStmt = new SelectStmt
                {
                    ValuesLists = { node.Values.Select(Visit).ToNode() }
                };
            }
            else if (!node.ValueTable.IsEmpty)
            {
                var valueTable = Visit(node.ValueTable.Value);

                if (valueTable.NodeCase is not Node.NodeOneofCase.SelectStmt)
                    throw new QsiException(QsiError.Internal, $"Expected type SelectStmt but was: {valueTable.NodeCase}");

                selectStmt = valueTable.SelectStmt;
            }
            else
            {
                selectStmt = null;
            }

            var relation = Visit((PgTableReferenceNode)node.Target.Value);

            if (!node.Alias.IsEmpty)
            {
                relation.Alias = new Alias
                {
                    Aliasname = node.Alias.Value.Name.Value
                };
            }

            // ignored ReturningList
            return new InsertStmt
            {
                Relation = relation,
                Cols =
                {
                    node.Columns?.Select(c => new ResTarget { Name = c[0].Value }.ToNode()) ?? Enumerable.Empty<Node>()
                },
                SelectStmt = selectStmt?.ToNode(),
                OnConflictClause = node.Conflict.InvokeWhenNotNull(Visit),
                WithClause = node.Directives.InvokeWhenNotNull(Visit),
                Override = node.Override
            };
        }

        public static UpdateStmt Visit(PgDataUpdateActionNode node)
        {
            var target = node.Target.Value;
            Node? whereExpr = null;

            if (target is PgActionDerivedTableNode { Where.Value: { } whereValue } derivedTable)
            {
                target = derivedTable.Source.Value;
                whereExpr = Visit(whereValue.Expression);
            }

            if (Visit(target) is not { RangeVar: { } rangeVar })
                throw new InvalidOperationException("Target is not RangeVar");

            // ignored ReturningList
            return new UpdateStmt
            {
                Relation = rangeVar,
                WhereClause = whereExpr,
                WithClause = node.Directives.InvokeWhenNotNull(Visit),
                TargetList = { node.SetValues.Select(v => Visit(v).ToNode()) },
                FromClause = { node.FromSources.Select(Visit) }
            };
        }

        public static DeleteStmt Visit(QsiDataDeleteActionNode node)
        {
            var target = node.Target.Value;
            Node? whereExpr = null;
            WithClause? withClause = null;

            if (target is PgActionDerivedTableNode derivedTable)
            {
                target = derivedTable.Source.Value;

                if (derivedTable.Where.Value is { } whereValue)
                {
                    whereExpr = Visit(whereValue.Expression);
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

        public static VariableSetStmt Visit(PgVariableSetActionNode node)
        {
            return new VariableSetStmt
            {
                Name = node.Name?.Value ?? string.Empty,
                Args = { node.Arguments.Select(Visit) },
                IsLocal = node.IsLocal,
                Kind = node.Kind
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
                            Catalogname = GetCatalogName(node.Identifier),
                            Schemaname = GetSchemaName(node.Identifier),
                            Relname = GetRelName(node.Identifier),
                            Relpersistence = node.Relpersistence.FromRelpersistence(),
                            Inh = node.IsInherit
                        },
                        OnCommit = node.OnCommit,
                    },
                    Query = Visit(node.DataSource),
                    IfNotExists = node.ConflictBehavior is QsiDefinitionConflictBehavior.Ignore
                };

                AddAliasNamesIfNotEmpty(stmt.Into.ColNames, node.Columns.Value);
                return stmt;
            }

            // partbound, partspec ignored
            return new CreateStmt
            {
                Relation =
                {
                    Catalogname = GetCatalogName(node.Identifier),
                    Schemaname = GetSchemaName(node.Identifier),
                    Relname = GetRelName(node.Identifier),
                    Relpersistence = node.Relpersistence.FromRelpersistence()
                },
                AccessMethod = node.AccessMethod,
                TableElts = { node.TableElts.Select(Visit) },
                InhRelations = { node.InheritRelations.Select(Visit) },
                Constraints = { node.Constraints.Select(Visit) },
                Options = { node.Options.Select(Visit) },
                OfTypename = node.OfType.InvokeWhenNotNull(Visit),
                IfNotExists = node.ConflictBehavior is QsiDefinitionConflictBehavior.Ignore,
                Oncommit = node.OnCommit,
                Tablespacename = node.TablespaceName
            };
        }

        public static CommonTableExpr Visit(PgCommonTableNode node)
        {
            var expr = new CommonTableExpr
            {
                Ctequery = Visit(node.Source),
                Ctematerialized = node.Materialized,
                Ctename = node.Alias.IsEmpty ? string.Empty : node.Alias.Value.Name.Value
            };

            AddAliasNamesIfNotEmpty(expr.Aliascolnames, node.Columns.Value);

            return expr;
        }

        public static IPgNode Visit(PgAliasedTableNode node)
        {
            var alias = new Alias
            {
                Aliasname = node.Alias.Value.Name.Value
            };

            AddAliasNamesIfNotEmpty(alias.Colnames, node.Columns.Value);

            var table = Visit(node.Source);

            switch (table?.NodeCase)
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

                case Node.NodeOneofCase.RangeFunction:
                    table.RangeFunction.Alias = alias;
                    return table.RangeFunction;

                case Node.NodeOneofCase.RangeTableFunc:
                    table.RangeTableFunc.Alias = alias;
                    return table.RangeTableFunc;

                default:
                    throw new NotSupportedException($"Not supported Aliased table node: {table?.NodeCase.ToString() ?? "Unknown"}");
            }
        }

        public static SelectStmt Visit(PgDerivedTableNode node)
        {
            var stmt = Visit((IQsiDerivedTableNode)node);
            stmt.DistinctClause.AddRange(node.DisinictExpressions.Select(Visit));

            return stmt;
        }

        public static SelectStmt Visit(IQsiDerivedTableNode node)
        {
            var stmt = new SelectStmt
            {
                LimitOption = LimitOption.Default,
                Op = SetOperation.SetopNone,
                WithClause = node.Directives is null ? null : Visit(node.Directives) // WITH <with>
            };

            // <target>
            if (node.Columns is { } columnsDeclaration)
            {
                stmt.AddTargetList(columnsDeclaration.Columns.Select(c => c switch
                {
                    QsiAllColumnNode allColumn => new ResTarget { Val = Visit(allColumn).ToNode() },
                    QsiColumnReferenceNode cr => new ResTarget { Val = Visit(cr).ToNode() },
                    QsiDerivedColumnNode dc => Visit(dc),
                    _ => new ResTarget()
                }));
            }

            // FROM <Source>
            if (node.Source is { } sourceTable)
            {
                if (sourceTable is QsiJoinedTableNode { IsComma: true } joinedSourceTable)
                {
                    stmt.FromClause.Add(GetCommaJoinedSources(joinedSourceTable).Select(Visit));
                }
                else
                {
                    stmt.FromClause.Add(Visit(sourceTable));
                }
            }

            // WHERE <filter>
            if (node.Where is { } whereExpression)
            {
                stmt.WhereClause = Visit(whereExpression.Expression);
            }

            // LIMIT <limit> OFFSET <offset>
            if (node.Limit is PgLimitExpressionNode limit)
            {
                stmt.LimitOption = limit.Option;
                stmt.LimitCount = Visit(limit.Limit);
                stmt.LimitOffset = Visit(limit.Offset);
            }

            // GROUP BY ~~~ HAVING ~~~
            if (node.Grouping is { } grouping)
            {
                stmt.GroupClause.AddRange(grouping.Items.Select(Visit));

                if (grouping.Having is { } having)
                    stmt.HavingClause = Visit(having);
            }

            // ORDER BY sort_clause1, sort_clause2..
            if (node.Order is { } order)
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

        public static RangeTableFunc Visit(PgXmlTableNode node)
        {
            return new RangeTableFunc
            {
                Docexpr = Visit(node.DocExpr),
                Rowexpr = Visit(node.RowExpr),
                Lateral = node.Lateral,
                Namespaces = { node.Namespaces.Select(Visit) },
                Columns = { node.Columns.Value.Select(Visit) }
            };
        }

        public static RangeTableFuncCol Visit(PgXmlColumnNode node)
        {
            return new RangeTableFuncCol
            {
                Colname = node.Name.Value,
                TypeName = node.TypeName.InvokeWhenNotNull(Visit),
                Colexpr = Visit(node.ColumnExpression),
                Coldefexpr = Visit(node.ColumnDefExpression),
                ForOrdinality = node.ForOrdinality,
                IsNotNull = node.IsNotNull
            };
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

        public static WithClause Visit(IQsiTableDirectivesNode node)
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
                return VisitBuiltIn(node);

            return new FuncCall
            {
                Funcname = { node.Member.Value.Identifier.ToPgString().ToNode() },
                Funcformat = node.FunctionFormat,
                Args = { node.Parameters.Select(Visit) },
                AggStar = node.AggregateStar,
                AggDistinct = node.AggregateDistinct,
                AggOrder = { node.AggregateOrder.Select(Visit) },
                AggFilter = Visit(node.AggregateFilter.Value),
                AggWithinGroup = node.AggregateWithInGroup,
                FuncVariadic = node.FunctionVariadic,
                Over = node.Over.InvokeWhenNotNull(Visit)
            };
        }

        private static IPgNode VisitBuiltIn(PgInvokeExpressionNode node)
        {
            string funcName = node.Member.Value.Identifier[0].Value;

            return funcName switch
            {
                PgKnownFunction.Grouping => new GroupingFunc { Args = { node.Parameters.Select(Visit) } },
                PgKnownFunction.Coalesce => new CoalesceExpr { Args = { node.Parameters.Select(Visit) } },
                PgKnownFunction.Greatest => new MinMaxExpr
                {
                    Op = MinMaxOp.IsGreatest,
                    Args = { node.Parameters.Select(Visit) }
                },
                PgKnownFunction.Least => new MinMaxExpr
                {
                    Op = MinMaxOp.IsLeast,
                    Args = { node.Parameters.Select(Visit) }
                },
                _ => throw new NotSupportedException($"Not supported function to create PgNode: {funcName}")
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
                Collname = { node.Column.ToPgStringNode() },
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
                Names = { node.Identifier.ToPgStringNode() },
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
                OperName = { node.OperatorName.ToPgStringNode() },
                SubLinkType = node.SubLinkType
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
                IQsiTableDirectivesNode directives => Visit(directives),
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
                Name = PgKnownVariable.SearchPath,
                IsLocal = false,
                Kind = VariableSetKind.VarSetValue,
                Args = { node.Identifiers.Select(i => new A_Const { Sval = new String { Sval = i[0].Value } }.ToNode()) }
            };
        }

        public static RangeVar Visit(PgTableReferenceNode node)
        {
            return new RangeVar
            {
                Catalogname = GetCatalogName(node.Identifier),
                Schemaname = GetSchemaName(node.Identifier),
                Relname = GetRelName(node.Identifier),
                Inh = node.IsInherit,
                Relpersistence = node.Relpersistence.FromRelpersistence()
            };
        }

        public static ViewStmt Visit(PgViewDefinitionNode node)
        {
            var stmt = new ViewStmt
            {
                Query = Visit(node.Source.Value),
                View = new RangeVar
                {
                    Catalogname = GetCatalogName(node.Identifier),
                    Schemaname = GetSchemaName(node.Identifier),
                    Relname = GetRelName(node.Identifier),
                },
                Options =
                {
                    node.Options
                        .Where(o => o is not null)
                        .Select(o => Visit(o!).ToNode())
                },
                Replace = node.ConflictBehavior is QsiDefinitionConflictBehavior.Replace,
            };

            AddAliasNamesIfNotEmpty(stmt.Aliases, node.Columns.Value);

            return stmt;
        }

        public static ColumnDef Visit(PgColumnDefinitionNode node)
        {
            return new ColumnDef
            {
                Colname = node.Name.Value,
                TypeName = Visit(node.TypeName)?.TypeName,
                RawDefault = Visit(node.RawDefault),
                CollClause = node.CollClause.InvokeWhenNotNull(Visit),
                Constraints = { node.Constraints.Select(Visit) },
            };
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
                Op = node.CompositeType.ToSetOperation(),
                All = node.IsAll,
                Larg = Visit(node.Sources[0]).SelectStmt,
                Rarg = Visit(node.Sources[1]).SelectStmt,
            };
        }

        public static RangeFunction Visit(PgRoutineTableNode node)
        {
            return new RangeFunction
            {
                Functions = { node.Sources.Select(Visit) },
                IsRowsfrom = node.IsRowsfrom,
                Lateral = node.Lateral,
                Ordinality = node.Ordinality,
                Coldeflist = { node.ColumnDefinitions.WhereNotNull().Select(Visit).ToNode() }
            };
        }

        public static IPgNode Visit(PgTableFunctionNode node)
        {
            return new List
            {
                Items =
                {
                    Visit(node.Function), // Item
                    new Node() // TODO: Alias (not empty when has alias) (feature/pg-official-parser)
                }
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
                Node = Visit(node.Expression),
                SortbyNulls = node.SortByNulls,
                UseOp = { node.UsingOperator.Select(Visit) }
            };
        }

        public static JoinExpr Visit(PgJoinedTableNode node)
        {
            var join = new JoinExpr
            {
                Larg = Visit(node.Left),
                Rarg = Visit(node.Right),
                Jointype = node.JoinType.ToJoinType(),
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
                Arg = Visit(node.Expression)
            };
        }

        public static ResTarget Visit(QsiSetColumnExpressionNode node)
        {
            return new ResTarget
            {
                Name = node.Target[0].Value,
                Val = Visit(node.Value),
                Indirection = { node.Indirections.Select(Visit) }
            };
        }

        public static OnConflictClause Visit(PgOnConflictNode node)
        {
            return new OnConflictClause
            {
                Action = node.Action,
                WhereClause = Visit(node.Where),
                Infer = node.Infer.InvokeWhenNotNull(Visit),
                TargetList = { node.TargetList.Select(Visit) }
            };
        }

        public static InferClause Visit(PgInferExpressionNode node)
        {
            return new InferClause
            {
                Conname = node.Name,
                WhereClause = Visit(node.Where),
                IndexElems = { node.IndexElems.Select(Visit) }
            };
        }

        public static MultiAssignRef Visit(PgMultipleAssignExpressionNode node)
        {
            return new MultiAssignRef
            {
                Source = Visit(node.Value),
                Colno = node.ColumnNumber,
                Ncolumns = node.NColumns
            };
        }

        private static void AddAliasNamesIfNotEmpty(RepeatedField<Node?> source, QsiColumnsDeclarationNode? node)
        {
            if (node is not { } || node.Columns.Any(c => c is not QsiSequentialColumnNode))
                return;

            source.AddRange(node.Columns.Select(Visit));
        }

        private static string GetCatalogName(QsiQualifiedIdentifier identifier)
        {
            return identifier.Level >= 3 ? identifier[^3].Value : string.Empty;
        }

        private static string GetSchemaName(QsiQualifiedIdentifier identifier)
        {
            return identifier.Level >= 2 ? identifier[^2].Value : string.Empty;
        }

        private static string GetRelName(QsiQualifiedIdentifier identifier)
        {
            return identifier.Level >= 1 ? identifier[^1].Value : string.Empty;
        }

        public static Node? Visit<T>(IQsiTreeNodeProperty<T> node) where T : QsiTreeNode
        {
            return node.IsEmpty ? null : Visit(node.Value);
        }

        [return: NotNullIfNotNull("node")]
        public static Node? Visit(IQsiTreeNode? node)
        {
            if (node is null)
                return null;

            return (node switch
            {
                // Action Nodes
                PgDataInsertActionNode pgDataInsertAction => Visit(pgDataInsertAction),
                PgDataUpdateActionNode pgDataUpdateAction => Visit(pgDataUpdateAction),
                QsiDataDeleteActionNode qsiDataDeleteAction => Visit(qsiDataDeleteAction),
                PgVariableSetActionNode pgVariableSetAction => Visit(pgVariableSetAction),
                QsiChangeSearchPathActionNode qsiChangeSearchPathAction => Visit(qsiChangeSearchPathAction),

                // Table Nodes
                PgCommonTableNode pgCommonTable => Visit(pgCommonTable),
                PgDerivedTableNode pgDerivedTableNode => Visit(pgDerivedTableNode),
                PgAliasedTableNode pgAliasedTable => Visit(pgAliasedTable),
                PgJoinedTableNode pgJoinedTable => Visit(pgJoinedTable),
                QsiInlineDerivedTableNode qsiInlineDerived => Visit(qsiInlineDerived),
                PgCompositeTableNode pgCompositeTable => Visit(pgCompositeTable),
                PgRoutineTableNode routineTable => Visit(routineTable),
                PgTableFunctionNode pgTableFunction => Visit(pgTableFunction),
                IQsiDerivedTableNode qsiDerivedTable => Visit(qsiDerivedTable),
                PgXmlTableNode pgXmlTableNode => Visit(pgXmlTableNode),

                // Definition Nodes
                PgTableDefinitionNode pgTableDefinition => Visit(pgTableDefinition),
                PgViewDefinitionNode pgViewDefinition => Visit(pgViewDefinition),
                PgColumnDefinitionNode pgColumnDefinition => Visit(pgColumnDefinition),

                // Column Nodes
                QsiDerivedColumnNode qsiDerivedColumn => Visit(qsiDerivedColumn),
                QsiAllColumnNode qsiAllColumn => Visit(qsiAllColumn),
                QsiSequentialColumnNode qsiSequentialColumn => Visit(qsiSequentialColumn),
                PgXmlColumnNode pgXmlColumn => Visit(pgXmlColumn),

                // Expression Nodes
                QsiLiteralExpressionNode qsiLiteralExpression => Visit(qsiLiteralExpression),
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
                PgIndexExpressionNode pgIndexExpression => Visit(pgIndexExpression),
                PgSubLinkExpressionNode pgSubLinkExpression => Visit(pgSubLinkExpression),
                PgNullTestExpressionNode pgNullTestExpression => Visit(pgNullTestExpression),
                QsiMultipleExpressionNode qsiMultipleExpression => Visit(qsiMultipleExpression),
                QsiSwitchExpressionNode qsiSwitchExpression => Visit(qsiSwitchExpression),
                QsiSwitchCaseExpressionNode qsiSwitchCaseExpression => Visit(qsiSwitchCaseExpression),
                PgWindowDefExpressionNode pgWindowDefExpression => Visit(pgWindowDefExpression),
                QsiRowValueExpressionNode qsiRowValueExpression => Visit(qsiRowValueExpression),
                QsiColumnExpressionNode qsiColumnExpression => Visit(qsiColumnExpression),
                QsiTableExpressionNode qsiTableExpression => Visit(qsiTableExpression),
                PgTableReferenceNode pgTableReference => Visit(pgTableReference),
                QsiColumnReferenceNode qsiColumnReference => Visit(qsiColumnReference),
                IQsiTableDirectivesNode qsiTableDirectives => Visit(qsiTableDirectives),
                PgGroupingSetExpressionNode pgGroupingSet => Visit(pgGroupingSet),
                PgOrderExpressionNode pgOrder => Visit(pgOrder),
                PgDefinitionElementNode pgDefinitionElement => Visit(pgDefinitionElement),
                QsiSetColumnExpressionNode qsiSetColumnExpression => Visit(qsiSetColumnExpression),
                PgOnConflictNode pgOnConflict => Visit(pgOnConflict),
                PgInferExpressionNode pgInferExpression => Visit(pgInferExpression),
                PgDefaultExpressionNode => new SetToDefault(),
                PgMultipleAssignExpressionNode pgMultipleAssignExpression => Visit(pgMultipleAssignExpression),

                _ => throw new NotSupportedException($"Cannot Visit({node.GetType().Name})")
            }).ToNode();
        }
    }
}
