using System;
using System.Linq;
using Qsi.Data;
using Qsi.PostgreSql.Data;
using Qsi.Tree;
using Qsi.Utilities;
using static Qsi.PostgreSql.Internal.PostgreSqlParserInternal;

namespace Qsi.PostgreSql.Tree.Visitors;

internal static class ActionVisitor
{
    public static QsiActionNode VisitDeleteStatement(DeleteStatementContext context)
    {
        var nowith = context.deleteStatementNoWith();
        
        var derived = new QsiDerivedTableNode();

        var withClause = context.withClause();
        var whereClause = nowith.whereClause();
        
        // Cursor feature on DELETE statement is not supported.
        if (nowith.cursorName() != null)
        {
            throw TreeHelper.NotSupportedFeature("Cursor feature on DELETE statement is not supported.");
        }

        if (withClause != null)
        {
            derived.Directives.SetValue(TableVisitor.VisitWithClause(withClause));
        }

        if (whereClause != null)
        {
            derived.Where.SetValue(ExpressionVisitor.VisitWhereClause(whereClause));
        }
        
        var table = nowith.tableName();
        var alias = nowith.aliasClause();
        
        derived.Source.SetValue(TableVisitor.VisitTableReference(table));

        if (alias != null)
        {
            derived.Alias.SetValue(TableVisitor.VisitAliasClause(nowith.aliasClause()));
        }

        if (nowith.fromItemList() != null)
        {
            derived.Source.SetValue(VisitUsingFromItemListClause(nowith.fromItemList(), derived.Source.Value));
        }

        var node = new QsiDataDeleteActionNode();
        
        if (nowith.returningClause() != null)
        {
            var returning = VisitReturningClause(nowith.returningClause());

            node = new PostgreSqlDataDeleteActionNode
            {
                Returning = { Value = returning }
            };
        }

        node.Target.Value = derived;

        PostgreSqlTree.PutContextSpan(node, context);

        return node;
    }
    
    public static QsiActionNode VisitInsertStatement(InsertStatementContext context)
    {
        var nowith = context.insertStatementNoWith();

        var node = new PostgreSqlDataInsertActionNode();

        if (nowith.columnIdentifier() != null)
        {
            var tableNode = new QsiDerivedTableNode();
            tableNode.Source.SetValue(TableVisitor.VisitTableReference(nowith.tableName()));
            tableNode.Alias.SetValue(new QsiAliasNode
            {
                Name = IdentifierVisitor.VisitIdentifier(nowith.columnIdentifier())
            });
            
            PostgreSqlTree.PutContextSpan(tableNode, nowith.tableName().Start, nowith.columnIdentifier().Stop);
        }
        else
        {
            node.Target.SetValue(TableVisitor.VisitTableReference(nowith.tableName()));   
        }

        if (nowith.qualifiedIdentifierList() != null)
        {
            node.Columns = nowith
                .qualifiedIdentifierList()
                .qualifiedIdentifier()
                .Select(IdentifierVisitor.VisitQualifiedIdentifier)
                .ToArray();
        }

        if (nowith.selectStatement() != null)
        {
            var valueNode = TableVisitor.VisitSelectStatement(nowith.selectStatement());

            if (valueNode is QsiInlineDerivedTableNode inlineDerived)
            {
                node.Values.AddRange(inlineDerived.Rows);
            }
            else
            {
                node.ValueTable.Value = valueNode;
            }
        }
        else
        {
            node.IsDefaultValues = true;
        }

        if (nowith.overridingOption() != null)
        {
            node.OverridingOption = nowith.overridingOption().SYSTEM_P() != null ?
                OverridingOption.OverridingSystemValue :
                OverridingOption.OverridingUserValue;
        }
        
        if (nowith.onConflictClause() != null)
        {
            var conflictAction = nowith.onConflictClause().conflictAction();
            
            node.ConflictBehavior = GetConflictBehaviour(conflictAction);

            // TODO Later: Implement rest of update sets.
            node.ConflictAction.Value = VisitUpdateConflictAction(conflictAction.updateConflictAction());
        }

        if (nowith.returningClause() != null)
        {
            node.Returning.Value = VisitReturningClause(nowith.returningClause());
        }
        
        PostgreSqlTree.PutContextSpan(node, context);

        return node;
    }

    public static QsiDataConflictActionNode VisitUpdateConflictAction(UpdateConflictActionContext context)
    {
        var sets = context.updateSetList().updateSet().Select(VisitUpdateSet);

        var node = new QsiDataConflictActionNode();
        node.SetValues.AddRange(sets);
        
        if (context.whereClause() != null)
        {
            var condition = ExpressionVisitor.VisitWhereClause(context.whereClause());
            node.Condition.Value = condition;
        }
        
        PostgreSqlTree.PutContextSpan(node, context);

        return node;
    }

    public static QsiSetColumnExpressionNode VisitUpdateSet(UpdateSetContext context)
    {
        return context switch
        {
            ColumnUpdateSetContext column => VisitColumnUpdateSet(column),
            // TODO: Not supported currently, implement later.
            ColumnListUpdateSetContext columnList => // VisitColumnListUpdateSet(columnList)
                throw TreeHelper.NotSupportedFeature("Updating set with column list is not supported."),
            SubqueryUpdateSetContext subquery => // VisitSubqueryUpdateSet(subquery)
                throw TreeHelper.NotSupportedFeature("Updating set with subquery is not supported."),
            _ => throw TreeHelper.NotSupportedTree(context)
        };
    }

    public static QsiSetColumnExpressionNode VisitColumnUpdateSet(ColumnUpdateSetContext context)
    {
        if (context.expression() == null)
        {
            // TODO: Implement DEFAULT case on ColumnUpdateSetContext.
            throw new NotImplementedException();
        }
        
        var identifier = IdentifierVisitor.VisitIdentifier(context.columnIdentifier());
        var qualified = new QsiQualifiedIdentifier(identifier);

        var node = new QsiSetColumnExpressionNode()
        {
            Target = qualified,
            Value = { Value = ExpressionVisitor.VisitExpression(context.expression()) }
        };
            
        PostgreSqlTree.PutContextSpan(node, context);

        return node;
    }

    public static QsiActionNode VisitUpdateStatement(UpdateStatementContext context)
    {
        var nowith = context.updateStatementNoWith();
        
        var withClause = context.withClause();
        var aliasClause = nowith.aliasClause();
        var whereClause = nowith.whereClause();

        var source = TableVisitor.VisitTableReference(nowith.tableName());
        var derived = new QsiDerivedTableNode();
        derived.Source.SetValue(source);
        
        if (aliasClause != null)
        {
            derived.Alias.SetValue(TableVisitor.VisitAliasClause(aliasClause));
        }

        if (withClause != null)
        {
            derived.Directives.SetValue(TableVisitor.VisitWithClause(withClause));
        }

        if (whereClause != null)
        {
            derived.Where.SetValue(ExpressionVisitor.VisitWhereClause(whereClause));
        }
        
        var node = new QsiDataUpdateActionNode();

        if (nowith.returningClause() != null)
        {
            var returning = VisitReturningClause(nowith.returningClause());

            node = new PostgreSqlDataUpdateActionNode
            {
                Returning = { Value = returning }
            };
        }
        
        node.Target.Value = derived;
        node.SetValues.AddRange(ExpressionVisitor.VisitUpdateSetList(nowith.updateSetList()));

        PostgreSqlTree.PutContextSpan(node, context);

        return node;
    }

    public static QsiDataConflictBehavior GetConflictBehaviour(ConflictActionContext context)
    {
        return context.NOTHING() != null ? 
            QsiDataConflictBehavior.None :
            QsiDataConflictBehavior.Update;
    }

    public static QsiColumnsDeclarationNode VisitReturningClause(ReturningClauseContext context)
    {
        var selectItemList = context
            .returningItemList()
            .selectItemList();
            
        return TableVisitor.VisitSelectItemList(selectItemList);
    }

    public static QsiTableNode VisitUsingFromItemListClause(FromItemListContext context, QsiTableNode origin)
    {
        var tables = context.fromItem().Select(TableVisitor.VisitFromItem);

        var anchor = origin;
        
        foreach (var table in tables)
        {
            var joined = new QsiJoinedTableNode
            {
                Left = { Value = anchor },
                Right = { Value = table },
                IsComma = true
            };

            anchor = joined;
        }

        return anchor;
    }
}
