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
        public static IEnumerable<QsiTableNode> Visit(IPg10Node node)
        {
            switch (node)
            {
                case RawStmt rawStmt:
                    return VisitRawStmt(rawStmt);

                case SelectStmt selectStmt:
                    return new[] { VisitSelectStmt(selectStmt) };
            }

            return Enumerable.Empty<QsiTableNode>();
        }

        public static IEnumerable<QsiTableNode> VisitRawStmt(RawStmt rawStmt)
        {
            return rawStmt.stmt.SelectMany(Visit);
        }

        public static QsiTableNode VisitSelectStmt(SelectStmt selectStmt)
        {
            return TreeHelper.Create<QsiDerivedTableNode>(n =>
            {
                if (selectStmt.targetList != null)
                {
                    ResTarget[] targets = selectStmt.targetList
                        .Cast<ResTarget>()
                        .ToArray();

                    n.Columns.SetValue(VisitResTargets(targets));
                }

                if (selectStmt.fromClause != null)
                {
                    RangeVar[] vars = selectStmt.fromClause
                        .Cast<RangeVar>()
                        .ToArray();

                    n.Source.SetValue(VisitRangeVars(vars));
                }
            });
        }

        public static QsiColumnsDeclarationNode VisitResTargets(ResTarget[] targets)
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

        public static QsiTableNode VisitRangeVars(RangeVar[] vars)
        {
            throw new NotImplementedException();
        }
    }
}
