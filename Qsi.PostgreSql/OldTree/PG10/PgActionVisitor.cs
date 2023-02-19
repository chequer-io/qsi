using System.Linq;
using Qsi.Data;
using Qsi.PostgreSql.Internal.PG10.Types;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.PostgreSql.OldTree.PG10
{
    internal class PgActionVisitor : PgVisitorBase
    {
        public PgActionVisitor(IPgVisitorSet set) : base(set)
        {
        }

        public QsiTreeNode Visit(IPg10Node node)
        {
            switch (node)
            {
                case RawStmt rawStmt:
                    return VisitRawStmt(rawStmt);

                case VariableSetStmt variableSetStmt:
                    return VisitVariableSetStmt(variableSetStmt);

                case InsertStmt insertStmt:
                    return VisitInsertStmt(insertStmt);
            }

            throw TreeHelper.NotSupportedTree(node);
        }

        public QsiTreeNode VisitRawStmt(RawStmt rawStmt)
        {
            return Visit(rawStmt.stmt[0]);
        }

        public QsiTreeNode VisitVariableSetStmt(VariableSetStmt variableSetStmt)
        {
            if (variableSetStmt.kind == VariableSetKind.VAR_SET_VALUE &&
                variableSetStmt.name == "search_path")
            {
                var node = new QsiChangeSearchPathActionNode
                {
                    Identifiers = variableSetStmt.args
                        .Cast<A_Const>()
                        .Select(x => new QsiQualifiedIdentifier(new QsiIdentifier(x.val.str, false)))
                        .ToArray()
                };

                return node;
            }

            throw TreeHelper.NotSupportedTree(variableSetStmt);
        }

        public QsiDataInsertActionNode VisitInsertStmt(InsertStmt insertStmt)
        {
            QsiDataInsertActionNode actionNode = TreeHelper.Create<QsiDataInsertActionNode>(n =>
            {
                n.Target.Value = new QsiTableReferenceNode
                {
                    Identifier = IdentifierVisitor.VisitRangeVar(insertStmt.relation[0])
                };

                if (insertStmt.cols is not null)
                {
                    n.Columns = insertStmt.cols
                        .Cast<ResTarget>()
                        .Select(col => ((QsiColumnReferenceNode)TableVisitor.VisitResTarget(col)).Name)
                        .ToArray();
                }

                if (insertStmt.selectStmt is not null)
                {
                    var node = insertStmt.selectStmt.Cast<SelectStmt>().First();

                    if (node.valuesLists is not null)
                    {
                        foreach (IPg10Node[] valuesList in node.valuesLists)
                        {
                            var rowValueNode = new QsiRowValueExpressionNode();

                            rowValueNode.ColumnValues.AddRange(valuesList.Select(ExpressionVisitor.Visit));

                            n.Values.Add(rowValueNode);
                        }
                    }
                    else
                    {
                        n.ValueTable.Value = TableVisitor.Visit(node);
                    }
                }

                if (insertStmt.onConflictClause is not null)
                {
                    switch (insertStmt.onConflictClause[0].action)
                    {
                        case OnConflictAction.ONCONFLICT_NONE:
                        case OnConflictAction.ONCONFLICT_NOTHING:
                            n.ConflictBehavior = QsiDataConflictBehavior.None;
                            break;

                        case OnConflictAction.ONCONFLICT_UPDATE:
                            n.ConflictBehavior = QsiDataConflictBehavior.Update;
                            n.ConflictAction.Value = VisitConflictAction(insertStmt.onConflictClause[0]);
                            break;
                    }
                }
            });

            if (ListUtility.IsNullOrEmpty(insertStmt.withClause))
                return actionNode;

            actionNode.Directives.SetValue(TableVisitor.VisitWithClause(insertStmt.withClause[0]));
            return actionNode;
        }

        public QsiDataUpdateActionNode VisitUpdateStmt(UpdateStmt updateStmt)
        {
            QsiDataUpdateActionNode actionNode = TreeHelper.Create<QsiDataUpdateActionNode>(n =>
            {
                n.Target.Value = new QsiDerivedTableNode
                {
                    Source =
                    {
                        Value = new QsiTableReferenceNode
                        {
                            Identifier = IdentifierVisitor.VisitRangeVar(updateStmt.relation[0])
                        }
                    },
                    Columns =
                    {
                        Value = TreeHelper.CreateAllColumnsDeclaration()
                    }
                };

                if (updateStmt.whereClause is not null)
                {
                    ((QsiDerivedTableNode)n.Target.Value).Where.Value = new QsiWhereExpressionNode
                    {
                        Expression =
                        {
                            Value = ExpressionVisitor.Visit(updateStmt.whereClause[0])
                        }
                    };
                }

                if (updateStmt.targetList is not null)
                {
                    n.SetValues.AddRange(updateStmt.targetList
                        .Cast<ResTarget>()
                        .Select(col => TableVisitor.VisitSetColumn(col))
                        .ToArray()
                    );
                }
            });

            if (ListUtility.IsNullOrEmpty(updateStmt.withClause))
                return actionNode;

            actionNode.Directives.SetValue(TableVisitor.VisitWithClause(updateStmt.withClause[0]));

            return actionNode;
        }

        public QsiDataDeleteActionNode VisitDeleteStmt(DeleteStmt deleteStmt)
        {
            var actionNode = TreeHelper.Create<QsiDataDeleteActionNode>(n =>
            {
                n.Target.Value = new QsiDerivedTableNode
                {
                    Source =
                    {
                        Value = new QsiTableReferenceNode
                        {
                            Identifier = IdentifierVisitor.VisitRangeVar(deleteStmt.relation[0])
                        }
                    },
                    Columns =
                    {
                        Value = TreeHelper.CreateAllColumnsDeclaration()
                    }
                };

                if (deleteStmt.whereClause is not null)
                {
                    ((QsiDerivedTableNode)n.Target.Value).Where.Value = new QsiWhereExpressionNode
                    {
                        Expression =
                        {
                            Value = ExpressionVisitor.Visit(deleteStmt.whereClause[0])
                        }
                    };
                }
            });

            if (ListUtility.IsNullOrEmpty(deleteStmt.withClause))
                return actionNode;

            ((QsiDerivedTableNode)actionNode.Target.Value).Directives
                .SetValue(TableVisitor.VisitWithClause(deleteStmt.withClause[0]));

            // Ignored usingClause, returningList

            return actionNode;
        }

        public QsiDataConflictActionNode VisitConflictAction(OnConflictClause onConflictClause)
        {
            throw TreeHelper.NotSupportedTree(onConflictClause);
        }
    }
}
