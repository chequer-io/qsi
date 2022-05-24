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

        var allColumnDeclaration = new QsiColumnsDeclarationNode();
        allColumnDeclaration.Columns.Add(new QsiAllColumnNode());
        
        var derived = new QsiDerivedTableNode
        {
            Columns = { Value = allColumnDeclaration }
        };

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

        var source = TableVisitor.VisitTableReference(table);
        derived.Source.SetValue(TableVisitor.VisitTableReference(table));

        if (nowith.fromItemList() != null)
        {
            QsiAliasNode aliasNode = null;
            
            if (alias != null)
            {
                aliasNode = TableVisitor.VisitAliasClause(nowith.aliasClause());

                var aliasDeclaration = new QsiColumnsDeclarationNode
                {
                    Columns = { new QsiAllColumnNode() }
                };
                
                var aliasTableNode = new QsiDerivedTableNode
                {
                    Columns = { Value = aliasDeclaration },
                    Source = { Value = derived.Source.Value },
                    Alias = { Value = aliasNode }
                };
            
                PostgreSqlTree.PutContextSpan(aliasTableNode, nowith.tableName().Start, nowith.aliasClause().Stop);
            
                derived.Source.Value = aliasTableNode;
            }
            
            derived.Source.SetValue(VisitFromItemListClause(nowith.fromItemList(), derived.Source.Value));

            var newAllColumn = new QsiAllColumnNode
            {
                Path = aliasNode != null ?
                    new QsiQualifiedIdentifier(aliasNode.Name) :
                    source.Identifier
            };
            
            var newColumnsDeclaration = new QsiColumnsDeclarationNode();
            newColumnsDeclaration.Columns.Add(newAllColumn);
            derived.Columns.Value = newColumnsDeclaration;
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
            var tableNode = new QsiDerivedTableNode
            {
                Columns = { Value = new QsiColumnsDeclarationNode { Columns = { new QsiAllColumnNode() } } },
                Source = { Value = TableVisitor.VisitTableReference(nowith.tableName()) },
                Alias = { Value = new QsiAliasNode { Name = IdentifierVisitor.VisitIdentifier(nowith.columnIdentifier()) } }
            };

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

        var node = new QsiSetColumnExpressionNode
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

        var node = new QsiDataUpdateActionNode();
        
        if (nowith.returningClause() != null)
        {
            var returning = VisitReturningClause(nowith.returningClause());

            node = new PostgreSqlDataUpdateActionNode
            {
                Returning = { Value = returning }
            };
        }
        
        node.SetValues.AddRange(ExpressionVisitor.VisitUpdateSetList(nowith.updateSetList()));
        PostgreSqlTree.PutContextSpan(node, context);
        
        var source = TableVisitor.VisitTableReference(nowith.tableName());
        
        var withClause = context.withClause();
        var fromClause = nowith.fromClause();
        var aliasClause = nowith.aliasClause();
        var whereClause = nowith.whereClause();
        
        if (withClause == null &&
            fromClause == null &&
            whereClause == null)
        {
            node.Target.SetValue(source);
            return node;
        }

        var columnsDeclaration = new QsiColumnsDeclarationNode { Columns = { new QsiAllColumnNode() } };
        
        var derived = new QsiDerivedTableNode
        {
            Columns = { Value = columnsDeclaration},
            Source = { Value = source }
        };

        if (fromClause != null)
        {
            QsiAliasNode alias = null;
            
            if (aliasClause != null)
            {
                alias = TableVisitor.VisitAliasClause(aliasClause);

                var aliasTable = new QsiDerivedTableNode
                {
                    Columns = { Value = new QsiColumnsDeclarationNode { Columns = { new QsiAllColumnNode() } } },
                    Source = { Value = derived.Source.Value },
                    Alias = { Value = alias }
                };
            
                PostgreSqlTree.PutContextSpan(aliasTable, nowith.tableName().Start, nowith.aliasClause().Stop);

                derived.Source.Value = aliasTable;
            }
            
            var table = VisitFromItemListClause(fromClause.fromItemList(), derived.Source.Value);
            derived.Source.Value = table;
            
            var newAllColumn = new QsiAllColumnNode
            {
                Path = alias != null ?
                    new QsiQualifiedIdentifier(alias.Name) :
                    source.Identifier
            };
            
            var newColumnsDeclaration = new QsiColumnsDeclarationNode();
            newColumnsDeclaration.Columns.Add(newAllColumn);
            derived.Columns.Value = newColumnsDeclaration;
        }

        if (withClause != null)
        {
            derived.Directives.SetValue(TableVisitor.VisitWithClause(withClause));
        }

        if (whereClause != null)
        {
            derived.Where.SetValue(ExpressionVisitor.VisitWhereClause(whereClause));
        }

        node.Target.Value = derived;
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

    public static QsiTableNode VisitFromItemListClause(FromItemListContext context, QsiTableNode origin)
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
