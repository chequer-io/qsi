using System;
using System.Linq;
using PgQuery;
using Qsi.Data;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.PostgreSql.NewTree;

internal static partial class PgNodeVisitor
{
    public static QsiDataInsertActionNode Visit(InsertStmt node)
    {
        var target = Visit(node.Relation);

        // TODO: check about spec (Example) (feature/pg-official-parser)
        // INSERT INTO distributors AS d (did, dname) VALUES (8, 'Anvil Distribution')
        if (target is not QsiTableReferenceNode targetRef)
            throw TreeHelper.NotSupportedFeature("Insert Target with alias");

        var insert = new QsiDataInsertActionNode
        {
            Target = { Value = targetRef }
        };

        if (node.Cols.Count > 0)
        {
            insert.Columns = node.Cols
                .Select(Visit)
                .OfType<QsiColumnReferenceNode>()
                .Select(c => c.Name)
                .ToArray();
        }

        if (node.SelectStmt is not null)
        {
            var subquery = Visit(node.SelectStmt);

            if (subquery is QsiInlineDerivedTableNode values)
            {
                insert.Values.AddRange(values.Rows);
            }
            else if (subquery is QsiTableNode table)
            {
                insert.ValueTable.Value = table;
            }
            else
            {
                throw CreateInternalException($"Not supported INSERT Selectstmt node: {subquery.GetType().Name}");
            }
        }

        if (node.OnConflictClause is { } onConflict)
        {
            insert.ConflictBehavior = onConflict.Action switch
            {
                OnConflictAction.OnconflictNone => QsiDataConflictBehavior.None,
                OnConflictAction.OnconflictNothing => QsiDataConflictBehavior.None,
                OnConflictAction.OnconflictUpdate => QsiDataConflictBehavior.Update,
                _ => throw CreateInternalException($"Not supported ConflictBehavior type: {onConflict.Action}")
            };

            if (insert.ConflictBehavior is QsiDataConflictBehavior.Update)
            {
                // TODO: ConflictClause Update (feature/pg-official-parser)
                throw TreeHelper.NotSupportedTree(onConflict);
            }
        }

        if (node.WithClause is not null)
            insert.Directives.Value = Visit(node.WithClause);

        return insert;
    }

    public static IQsiTreeNode Visit(MergeStmt node)
    {
        throw new NotImplementedException();
    }

    public static QsiDataUpdateActionNode Visit(UpdateStmt node)
    {
        var target = Visit(node.Relation);

        if (node.WhereClause is { })
        {
            target = new QsiDerivedTableNode
            {
                Source = { Value = target },
                Columns = { Value = TreeHelper.CreateAllColumnsDeclaration() },
                Where =
                {
                    Value = new QsiWhereExpressionNode
                    {
                        Expression = { Value = VisitExpression(node.WhereClause) }
                    }
                }
            };
        }

        var update = new QsiDataUpdateActionNode
        {
            Target = { Value = target }
        };

        if (node.WithClause is { })
            update.Directives.Value = Visit(node.WithClause);

        if (node.TargetList is { })
            update.SetValues.AddRange(node.TargetList.Select(t => VisitSetColumn(t.ResTarget)));

        return update;
    }

    public static QsiDataDeleteActionNode Visit(DeleteStmt node)
    {
        var target = Visit(node.Relation);

        if (node.WhereClause is { } || node.WithClause is { })
        {
            var derivedTable = new QsiDerivedTableNode
            {
                Source = { Value = target },
                Columns = { Value = TreeHelper.CreateAllColumnsDeclaration() }
            };

            if (node.WhereClause is { })
            {
                derivedTable.Where.Value = new QsiWhereExpressionNode
                {
                    Expression = { Value = VisitExpression(node.WhereClause) }
                };
            }

            if (node.WithClause is { })
                derivedTable.Directives.Value = Visit(node.WithClause);

            target = derivedTable;
        }

        // Ignored usingClause, returningList
        return new QsiDataDeleteActionNode
        {
            Target = { Value = target }
        };
    }
}
