using System;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Qsi.Data;
using Qsi.SqlServer.Data;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.SqlServer.Tree
{
    internal sealed class ActionVisitor : VisitorBase
    {
        public ActionVisitor(IVisitorContext visitorContext) : base(visitorContext)
        {
        }

        public QsiChangeSearchPathActionNode VisitUseStatement(UseStatement useStatement)
        {
            var node = new QsiChangeSearchPathActionNode
            {
                Identifiers = new[]
                {
                    IdentifierVisitor.CreateIdentifier(useStatement.DatabaseName),
                }
            };

            SqlServerTree.PutFragmentSpan(node, useStatement);

            return node;
        }

        #region Alter User
        public SqlServerAlterUserActionNode VisitAlterUser(AlterUserStatement alterUserStatement)
        {
            var node = new SqlServerAlterUserActionNode
            {
                TargetUser = IdentifierVisitor.CreateIdentifier(alterUserStatement.Name)
            };

            foreach (var userOption in alterUserStatement.UserOptions)
            {
                if (userOption is IdentifierPrincipalOption identifierPrincipalOption)
                {
                    switch (userOption.OptionKind)
                    {
                        case PrincipalOptionKind.Name:
                            node.NewUserName = IdentifierVisitor.CreateIdentifier(identifierPrincipalOption.Identifier);
                            break;

                        case PrincipalOptionKind.DefaultSchema:
                            node.DefaultSchema = IdentifierVisitor.CreateIdentifier(identifierPrincipalOption.Identifier);
                            break;
                    }
                }
            }

            SqlServerTree.PutFragmentSpan(node, alterUserStatement);

            return node;
        }
        #endregion

        #region Insert
        public QsiActionNode VisitInsertStatement(InsertStatement insertStatement)
        {
            return VisitInsertSpecificiation(insertStatement.InsertSpecification);
        }

        public QsiActionNode VisitInsertSpecificiation(InsertSpecification insertSpecification)
        {
            var node = new QsiDataInsertActionNode();
            var tableNode = TableVisitor.VisitTableReference(insertSpecification.Target);

            if (tableNode is not QsiTableReferenceNode tableReferenceNode)
                throw new QsiException(QsiError.Syntax);

            node.Target.SetValue(tableReferenceNode);

            if (!ListUtility.IsNullOrEmpty(insertSpecification.Columns))
            {
                node.Columns = insertSpecification.Columns
                    .Select(ExpressionVisitor.VisitColumnReferenceExpression)
                    .Select(c => c.Column.Value switch
                    {
                        QsiColumnReferenceNode columnReferenceNode => columnReferenceNode.Name,
                        QsiAllColumnNode allColumnNode => allColumnNode.Path,
                        _ => throw new QsiException(QsiError.Syntax)
                    })
                    .ToArray();
            }

            switch (insertSpecification.InsertSource)
            {
                case ExecuteInsertSource _:
                    throw TreeHelper.NotSupportedFeature("Execute Insert");

                case SelectInsertSource selectInsertSource:
                {
                    node.ValueTable.SetValue(TableVisitor.VisitQueryExpression(selectInsertSource.Select));
                    break;
                }

                case ValuesInsertSource valuesInsertSource:
                {
                    node.Values.AddRange(valuesInsertSource.RowValues.Select(ExpressionVisitor.VisitRowValue));
                    break;
                }

                default:
                    throw TreeHelper.NotSupportedTree(insertSpecification.InsertSource);
            }

            SqlServerTree.PutFragmentSpan(node, insertSpecification);

            return node;
        }
        #endregion

        #region Delete
        public QsiActionNode VisitDeleteStatement(DeleteStatement deleteStatement)
        {
            return VisitDeleteSpecification(deleteStatement.DeleteSpecification);
        }

        public QsiActionNode VisitDeleteSpecification(DeleteSpecification deleteSpecification)
        {
            var table = TableVisitor.VisitTableReference(deleteSpecification.Target);
            var where = deleteSpecification.WhereClause;
            var topRowFilter = deleteSpecification.TopRowFilter;

            if (where != null || topRowFilter != null)
            {
                var derivedTable = new QsiDerivedTableNode();

                derivedTable.Columns.SetValue(TreeHelper.CreateAllColumnsDeclaration());
                derivedTable.Source.SetValue(table);

                if (where != null)
                {
                    derivedTable.Where.SetValue(TableVisitor.VisitWhereClause(where));
                }

                if (topRowFilter != null)
                {
                    derivedTable.Limit.SetValue(TableVisitor.VisitLimitOffset(topRowFilter, null));
                }

                table = derivedTable;
            }

            var node = new QsiDataDeleteActionNode
            {
                Target = { Value = table }
            };

            SqlServerTree.PutFragmentSpan(node, deleteSpecification);

            return node;
        }
        #endregion

        #region Update
        public QsiDataUpdateActionNode VisitUpdateStatement(UpdateStatement updateStatement)
        {
            return VisitUpdateSpecification(updateStatement.UpdateSpecification);
        }

        public QsiDataUpdateActionNode VisitUpdateSpecification(UpdateSpecification updateSpecification)
        {
            QsiTableNode table = TableVisitor.VisitTableReference(updateSpecification.Target);

            var where = updateSpecification.WhereClause;
            var topRowFilter = updateSpecification.TopRowFilter;

            if (where != null || topRowFilter != null)
            {
                var derivedTable = new QsiDerivedTableNode();

                derivedTable.Columns.SetValue(TreeHelper.CreateAllColumnsDeclaration());
                derivedTable.Source.SetValue(table);

                if (where != null)
                {
                    derivedTable.Where.SetValue(TableVisitor.VisitWhereClause(where));
                }

                if (topRowFilter != null)
                {
                    derivedTable.Limit.SetValue(TableVisitor.VisitLimitOffset(topRowFilter, null));
                }

                table = derivedTable;
            }

            var node = new QsiDataUpdateActionNode();

            node.Target.SetValue(table);
            node.SetValues.AddRange(updateSpecification.SetClauses.Select(ExpressionVisitor.VisitSetClause));

            SqlServerTree.PutFragmentSpan(node, updateSpecification);

            return node;
        }
        #endregion

        #region Merge
        public SqlServerMergeActionNode VisitMergeStatement(MergeStatement mergeStatement)
        {
            return VisitMergeSpecification(mergeStatement.MergeSpecification, mergeStatement.WithCtesAndXmlNamespaces);
        }

        public SqlServerMergeActionNode VisitMergeSpecification(MergeSpecification mergeSpecification, WithCtesAndXmlNamespaces withCtesAndXmlNamespaces)
        {
            var targetTable = TableVisitor.VisitTableReference(mergeSpecification.Target);
            QsiTableDirectivesNode directiveNode = null;

            if (withCtesAndXmlNamespaces != null)
                directiveNode = TableVisitor.VisitWithCtesAndXmlNamespaces(withCtesAndXmlNamespaces);

            if (targetTable is not QsiTableReferenceNode accessNode)
                throw new NotSupportedException("Merge target table is not Table Reference");

            var leftTable = targetTable;
            var rightTable = TableVisitor.VisitTableReference(mergeSpecification.TableReference);

            var identifier = accessNode.Identifier;

            if (mergeSpecification.TableAlias != null)
            {
                leftTable = TreeHelper.Create<QsiDerivedTableNode>(n =>
                {
                    var alias = IdentifierVisitor.CreateIdentifier(mergeSpecification.TableAlias);

                    n.Columns.SetValue(TreeHelper.CreateAllColumnsDeclaration());

                    n.Alias.SetValue(new QsiAliasNode
                    {
                        Name = alias
                    });

                    identifier = new QsiQualifiedIdentifier(alias);
                    n.Source.SetValue(targetTable);
                });
            }

            var joinedTable = TreeHelper.Create<SqlServerJoinedTableNode>(n =>
            {
                n.Left.SetValue(leftTable);
                n.Right.SetValue(rightTable);
                n.JoinType = "JOIN";
                n.Expression.SetValue(ExpressionVisitor.VisitBooleanExpression(mergeSpecification.SearchCondition));
            });

            var leftTableColumnDeclaration = TreeHelper.Create<QsiColumnsDeclarationNode>(n =>
            {
                n.Columns.Add(new QsiAllColumnNode
                {
                    Path = identifier
                });
            });

            return TreeHelper.Create<SqlServerMergeActionNode>(n =>
            {
                foreach (var actionClause in mergeSpecification.ActionClauses)
                {
                    QsiTableNode target = null;

                    switch (actionClause.Condition)
                    {
                        // [ WHEN MATCHED [ AND <clause_search_condition> ] THEN <merge_matched> ] [ ..n ]
                        case MergeCondition.Matched:
                        {
                            target = TreeHelper.Create<QsiDerivedTableNode>(dtn =>
                            {
                                dtn.Source.SetValue(joinedTable);
                                dtn.Columns.SetValue(leftTableColumnDeclaration);

                                if (actionClause.SearchCondition != null)
                                {
                                    dtn.Where.SetValue(TreeHelper.Create<QsiWhereExpressionNode>(wn =>
                                    {
                                        wn.Expression.SetValue(ExpressionVisitor.VisitBooleanExpression(actionClause.SearchCondition));
                                    }));
                                }
                            });

                            break;
                        }

                        // InsertMergeAction: not need to handle
                        case MergeCondition.NotMatched:
                        case MergeCondition.NotMatchedByTarget:
                            break;

                        case MergeCondition.NotMatchedBySource:
                        {
                            var exceptTable = TreeHelper.Create<SqlServerBinaryTableNode>(btn =>
                            {
                                btn.Left.SetValue(TreeHelper.Create<QsiDerivedTableNode>(ln =>
                                {
                                    ln.Columns.SetValue(TreeHelper.CreateAllColumnsDeclaration());
                                    ln.Source.SetValue(accessNode);
                                }));

                                btn.Right.SetValue(TreeHelper.Create<QsiDerivedTableNode>(dtn =>
                                {
                                    dtn.Columns.SetValue(leftTableColumnDeclaration);
                                    dtn.Source.SetValue(joinedTable);
                                }));

                                btn.BinaryTableType = SqlServerBinaryTableType.Except;
                            });

                            if (actionClause.SearchCondition != null)
                            {
                                target = TreeHelper.Create<QsiDerivedTableNode>(dtn =>
                                {
                                    dtn.Columns.SetValue(TreeHelper.CreateAllColumnsDeclaration());

                                    dtn.Source.SetValue(TreeHelper.Create<QsiDerivedTableNode>(dtn2 =>
                                    {
                                        dtn2.Source.SetValue(exceptTable);
                                        dtn2.Columns.SetValue(TreeHelper.CreateAllColumnsDeclaration());

                                        dtn2.Alias.SetValue(new QsiAliasNode
                                        {
                                            Name = accessNode.Identifier[^1]
                                        });
                                    }));

                                    dtn.Where.SetValue(TreeHelper.Create<QsiWhereExpressionNode>(wn =>
                                    {
                                        wn.Expression.SetValue(ExpressionVisitor.VisitBooleanExpression(actionClause.SearchCondition));
                                    }));
                                });
                            }
                            else
                            {
                                target = exceptTable;
                            }

                            break;
                        }

                        case MergeCondition.NotSpecified:
                            throw TreeHelper.NotSupportedFeature("Merge NOT SPECIFIED");
                    }

                    switch (actionClause.Action)
                    {
                        case DeleteMergeAction _:
                            n.ActionNodes.Add(TreeHelper.Create<QsiDataDeleteActionNode>(dn =>
                            {
                                dn.Target.SetValue(target);
                            }));

                            break;

                        case InsertMergeAction insertMergeAction:
                            n.ActionNodes.Add(TreeHelper.Create<QsiDataInsertActionNode>(dn =>
                            {
                                if (directiveNode != null)
                                    dn.Directives.SetValue(directiveNode);

                                dn.Target.SetValue(accessNode);

                                dn.Columns = insertMergeAction.Columns.Select(ExpressionVisitor.VisitColumnReferenceExpression)
                                    .Select(c =>
                                    {
                                        return c.Column.Value switch
                                        {
                                            QsiColumnReferenceNode columnReference => columnReference.Name,
                                            QsiAllColumnNode allColumn => allColumn.Path,
                                            _ => throw new NotSupportedException(c.Column.Value.GetType().ToString())
                                        };
                                    })
                                    .ToArray();

                                dn.Values.AddRange(insertMergeAction.Source.RowValues.Select(ExpressionVisitor.VisitRowValue));
                            }));

                            break;

                        case UpdateMergeAction updateMergeAction:
                            n.ActionNodes.Add(TreeHelper.Create<QsiDataUpdateActionNode>(dn =>
                            {
                                if (directiveNode != null)
                                    dn.Directives.SetValue(directiveNode);

                                dn.Target.SetValue(target);
                                dn.SetValues.AddRange(updateMergeAction.SetClauses.Select(ExpressionVisitor.VisitSetClause));
                            }));

                            break;
                    }
                }
            });
        }
        #endregion
    }
}
