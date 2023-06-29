using System;
using System.Collections.Generic;
using System.Linq;
using Qsi.Data;
using Qsi.MySql.Tree.Common;
using Qsi.Shared.Extensions;
using Qsi.Tree;
using Qsi.Utilities;
using static Qsi.MySql.Internal.MySqlParserInternal;

namespace Qsi.MySql.Tree
{
    internal static class ActionVisitor
    {
        public static QsiActionNode VisitDeleteStatement(DeleteStatementContext context)
        {
            var derivedNode = new QsiDerivedTableNode();

            var withClause = context.withClause();
            var whereClause = context.whereClause();
            var tableAliasRefList = context.tableAliasRefList();

            if (withClause != null)
                derivedNode.Directives.SetValue(TableVisitor.VisitWithClause(withClause));

            if (whereClause != null)
                derivedNode.Where.SetValue(ExpressionVisitor.VisitWhereClause(whereClause));

            if (tableAliasRefList != null)
            {
                var tableReferenceList = context.tableReferenceList();

                (QsiQualifiedIdentifier Identifier, bool Wildcard)[] aliases = tableAliasRefList.tableRefWithWildcard()
                    .Select(IdentifierVisitor.VisitTableRefWithWildcard)
                    .Select(i =>
                    {
                        if (i[^1] == QsiIdentifier.Wildcard)
                            return (i.SubIdentifier(..^1), true);

                        return (i, false);
                    })
                    .ToArray();

                var columns = new QsiColumnsDeclarationNode();
                var isUsing = context.HasToken(USING_SYMBOL);

                foreach (var (identifier, wildcard) in aliases)
                {
                    if (isUsing || wildcard)
                    {
                        columns.Columns.Add(new QsiAllColumnNode
                        {
                            Path = identifier
                        });
                    }
                    else
                    {
                        columns.Columns.Add(new QsiColumnReferenceNode
                        {
                            Name = identifier
                        });
                    }
                }

                derivedNode.Columns.SetValue(columns);
                derivedNode.Source.SetValue(TableVisitor.VisitTableReferenceList(tableReferenceList));
            }
            else
            {
                var tableRef = context.tableRef();
                var partitionDelete = context.partitionDelete();
                var orderClause = context.orderClause();
                var simpleLimitClause = context.simpleLimitClause();

                derivedNode.Columns.SetValue(TreeHelper.CreateAllColumnsDeclaration());
                derivedNode.Source.SetValue(TableVisitor.VisitTableRef(new CommonTableRefContext(tableRef, partitionDelete)));

                if (orderClause != null)
                    derivedNode.Order.SetValue(ExpressionVisitor.VisitOrderClause(orderClause));

                if (simpleLimitClause != null)
                    derivedNode.Limit.SetValue(ExpressionVisitor.VisitSimpleLimitClause(simpleLimitClause));
            }

            var node = new QsiDataDeleteActionNode
            {
                Target = { Value = derivedNode }
            };

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiActionNode VisitReplaceStatement(ReplaceStatementContext context)
        {
            return VisitCommonInsert(new CommonInsertContext(context));
        }

        public static QsiActionNode VisitInsertStatement(InsertStatementContext context)
        {
            return VisitCommonInsert(new CommonInsertContext(context));
        }

        public static QsiActionNode VisitCommonInsert(CommonInsertContext context)
        {
            var node = new QsiDataInsertActionNode
            {
                ConflictBehavior = context.ConflictBehavior
            };

            node.Target.SetValue(TableVisitor.VisitTableRef(new CommonTableRefContext(context.TableRef, context.UsePartition)));

            if (context.InsertFromConstructor != null)
            {
                node.Columns = context.InsertFromConstructor
                    .fields()?
                    .insertIdentifier()?
                    .Select(IdentifierVisitor.VisitInsertIdentifier)
                    .ToArray();

                node.Values.AddRange(ExpressionVisitor.VisitInsertValues(context.InsertFromConstructor.insertValues()));

                // TODO: implement context.ValuesReference
            }
            else if (context.UpdateList != null)
            {
                node.SetValues.AddRange(ExpressionVisitor.VisitUpdateList(context.UpdateList));

                // TODO: implement context.ValuesReference
            }
            else
            {
                node.Columns = context.InsertQueryExpression
                    .fields()?
                    .insertIdentifier()?
                    .Select(IdentifierVisitor.VisitInsertIdentifier)
                    .ToArray();

                node.ValueTable.SetValue(
                    TableVisitor.VisitQueryExpressionOrParens(context.InsertQueryExpression.queryExpressionOrParens()));
            }

            if (context.InsertUpdateList != null)
                node.ConflictAction.SetValue(VisitInsertUpdateList(context.InsertUpdateList));

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiDataConflictActionNode VisitInsertUpdateList(InsertUpdateListContext context)
        {
            var node = new QsiDataConflictActionNode();

            node.SetValues.AddRange(ExpressionVisitor.VisitUpdateList(context.updateList()));

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiActionNode VisitUpdateStatement(UpdateStatementContext context)
        {
            var tableNode = TableVisitor.VisitTableReferenceList(context.tableReferenceList());
            var withClause = context.withClause();
            var whereClause = context.whereClause();
            var orderClause = context.orderClause();
            var simpleLimitClause = context.simpleLimitClause();

            if (withClause != null || whereClause != null || orderClause != null || simpleLimitClause != null)
            {
                if (tableNode is not QsiDerivedTableNode derivedTableNode || !derivedTableNode.Alias.IsEmpty)
                {
                    derivedTableNode = new QsiDerivedTableNode();
                    derivedTableNode.Columns.SetValue(TreeHelper.CreateAllColumnsDeclaration());
                    derivedTableNode.Source.SetValue(tableNode);
                }

                if (withClause != null)
                    derivedTableNode.Directives.SetValue(TableVisitor.VisitWithClause(withClause));

                if (whereClause != null)
                    derivedTableNode.Where.SetValue(ExpressionVisitor.VisitWhereClause(whereClause));

                if (orderClause != null)
                    derivedTableNode.Order.SetValue(ExpressionVisitor.VisitOrderClause(orderClause));

                if (simpleLimitClause != null)
                    derivedTableNode.Limit.SetValue(ExpressionVisitor.VisitSimpleLimitClause(simpleLimitClause));

                tableNode = derivedTableNode;
            }

            var node = new QsiDataUpdateActionNode();

            node.Target.SetValue(tableNode);
            node.SetValues.AddRange(ExpressionVisitor.VisitUpdateList(context.updateList()));

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiChangeSearchPathActionNode VisitUseCommand(UseCommandContext context)
        {
            var node = new QsiChangeSearchPathActionNode
            {
                Identifiers = new[]
                {
                    new QsiQualifiedIdentifier(IdentifierVisitor.VisitIdentifier(context.identifier()))
                }
            };

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiCreateUserActionNode VisitCreateUser(CreateUserContext context)
        {
            return new QsiCreateUserActionNode
            {
                ConflictBehavior = context.HasRule<IfNotExistsContext>() ? QsiDataConflictBehavior.Ignore : QsiDataConflictBehavior.None,
                Users = { context.createUserList().createUserEntry().Select(VisitCreateUserEntry) }
            }.WithContextSpan(context);
        }

        public static QsiAlterUserActionNode VisitAlterUser(AlterUserContext context)
        {
            var node = new QsiAlterUserActionNode
            {
                ConflictBehavior = context.HasRule<IfExistsContext>() ? QsiDataConflictBehavior.Ignore : QsiDataConflictBehavior.None
            }.WithContextSpan(context);

            if (context.alterUserTail() is not { } alterUserTail)
                return node;

            if (alterUserTail.createUserTail() is { })
            {
                // ( createUserList | alterUserList ) createUserTail

                if (alterUserTail.createUserList() is { } createUserList)
                {
                    node.Users.AddRange(createUserList.createUserEntry().Select(VisitCreateUserEntry));
                }
                else if (alterUserTail.alterUserList() is { } alterUserList)
                {
                    node.Users.AddRange(alterUserList.alterUserEntry().Select(VisitAlterUserEntry));
                }
            }
            else if (alterUserTail.textString() is { } textString)
            {
                // user IDENTIFIED BY textString replacePassword

                node.Users.Add(new QsiUserNode
                {
                    Username = GetUsername(alterUserTail.user()),
                    Password =
                    {
                        Value = new QsiLiteralExpressionNode
                        {
                            Value = textString.GetText(),
                            Type = QsiDataType.String
                        }.WithContextSpan(textString)
                    }
                });

                // replacePassword ignored
            }
            else if (alterUserTail.discardOldPassword() is { })
            {
                // user DISCARD OLD PASSWORD

                // ignored
                node.Users.Add(new QsiUserNode
                {
                    Username = GetUsername(alterUserTail.user())
                });
            }
            else if (context.HasToken(DEFAULT_SYMBOL) && context.HasToken(ROLE_SYMBOL))
            {
                // user DEFAULT ROLE (ALL | NONE | roleList)

                node.Users.Add(new QsiUserNode
                {
                    Username = GetUsername(alterUserTail.user())
                });
            }
            else if (context.HasToken(RANDOM_SYMBOL) && context.HasToken(PASSWORD_SYMBOL))
            {
                // user IDENTIFIER (WITH textOrIdentifier)? BY RANDOM PASSWORD retainCurrentPassword?
                // RANDOM PASSWORD ignored

                node.Users.Add(new QsiUserNode
                {
                    Username = GetUsername(alterUserTail.user())
                });
            }
            else if (context.HasToken(FAILED_LOGIN_ATTEMPTS_SYMBOL))
            {
                // FAILED_LOGIN_ATTEMPTS real_ulong_number
            }
            else if (context.HasToken(PASSWORD_LOCK_TIME_SYMBOL))
            {
                // PASSWORD_LOCK_TIME (real_ulong_number | UNBOUNDED)
            }

            return node;
        }

        public static QsiGrantUserActionNode VisitGrant(GrantContext context)
        {
            var node = new QsiGrantUserActionNode().WithContextSpan(context);

            if (context.roleOrPrivilegesList() is { } roleOrPrivilegesList)
            {
                node.Roles = roleOrPrivilegesList.roleOrPrivilege().Select(p => p.GetInputText()).ToArray();
            }

            if (context.HasToken(ALL_SYMBOL))
                node.AllPrivileges = true;

            if (context.userList() is { } userList)
                node.Users.AddRange(VisitUserList(userList));

            if (context.grantTargetList() is { } grantTargetList)
            {
                if (grantTargetList.userList() is { } grantTargetuserList)
                    node.Users.AddRange(VisitUserList(grantTargetuserList));

                if (grantTargetList.createUserList() is { } createUserList)
                    node.Users.AddRange(createUserList.createUserEntry().Select(VisitCreateUserEntry));
            }

            return node;
        }

        private static IEnumerable<QsiUserNode> VisitUserList(UserListContext context)
        {
            return context.user()
                .Select(u => u.userIdentifierOrText()?.GetText())
                .Where(i => i is not null)
                .Select(i => new QsiUserNode
                {
                    Username = i
                });
        }

        private static QsiUserNode VisitCreateUserEntry(CreateUserEntryContext context)
        {
            var user = new QsiUserNode
            {
                Username = GetUsername(context.user())
            }.WithContextSpan(context);

            if (context.IDENTIFIED_SYMBOL() is not null)
            {
                if (context.HasToken(RANDOM_SYMBOL) && context.HasToken(PASSWORD_SYMBOL))
                {
                    // NOTE: RANDOM PASSWORD
                }
                else
                {
                    QsiLiteralExpressionNode password = null;

                    if (context.textString() is { } textString)
                    {
                        password = new QsiLiteralExpressionNode().WithContextSpan(textString);

                        password.Value = textString.GetText();
                        password.Type = QsiDataType.String;
                    }
                    else if (context.textStringHash() is { } textStringHash)
                    {
                        password = new QsiLiteralExpressionNode().WithContextSpan(textStringHash);

                        password.Value = textStringHash.GetText();
                        password.Type = QsiDataType.String;
                    }

                    if (password is { })
                        user.Password.Value = password;

                    // WITH <auth_plugin> ignored
                }
            }

            return user;
        }

        public static QsiUserNode VisitAlterUserEntry(AlterUserEntryContext context)
        {
            var user = new QsiUserNode
            {
                Username = GetUsername(context.user())
            }.WithContextSpan(context);

            if (context.IDENTIFIED_SYMBOL() is not null)
            {
                QsiLiteralExpressionNode password = null;

                if (context.textString(0) is { } textString)
                {
                    password = new QsiLiteralExpressionNode().WithContextSpan(textString);

                    password.Value = textString.GetText();
                    password.Type = QsiDataType.String;
                }
                else if (context.textStringHash() is { } textStringHash)
                {
                    password = new QsiLiteralExpressionNode().WithContextSpan(textStringHash);

                    password.Value = textStringHash.GetText();
                    password.Type = QsiDataType.String;
                }

                if (password is { })
                    user.Password.Value = password;

                // WITH <auth_plugin> ignored
                // retainCurrentPassword ignored
            }

            // discardOldPassword ignored

            return user;
        }

        public static QsiVariableSetActionNode VisitSetStatement(SetStatementContext context)
        {
            return context switch
            {
                SetContext ctx => VisitSet(ctx),
                SetStatementForContext ctx => VisitSetStatementFor(ctx),
                _ => throw TreeHelper.NotSupportedTree(context)
            };
        }

        private static QsiVariableSetActionNode VisitSet(SetContext context)
        {
            var options = context.startOptionValueList();

            return new QsiVariableSetActionNode
            {
                SetItems =
                {
                    VisitStartOptionValueList(options)
                }
            };
        }

        private static QsiVariableSetActionNode VisitSetStatementFor(SetStatementForContext context)
        {
            var options = context.startOptionValueList();
            var node = new QsiVariableSetActionNode();

            try
            {
                node.SetItems.AddRange(VisitStartOptionValueList(options));
            }
            catch (QsiException e) when (e.Error is QsiError.NotSupportedTree)
            {
                // ignore
            }

            if (context.simpleStatement() is { } simpleStatement)
                node.Target = MySqlParser.Parse(simpleStatement.children[0]);

            return node;
        }

        private static IEnumerable<QsiVariableSetItemNode> VisitStartOptionValueList(StartOptionValueListContext context)
        {
            if (context.HasToken(PASSWORD_SYMBOL) && context.HasRule<EqualContext>())
            {
                // PASSWORD (FOR user) = textString
                yield return new QsiVariableSetItemNode
                {
                    Name = new QsiIdentifier("PASSWORD", false),
                    Expression =
                    {
                        Value = new QsiLiteralExpressionNode
                        {
                            Value = context.textString().GetText(),
                            Type = QsiDataType.String
                        }.WithContextSpan(context.textString())
                    }
                };
            }
            else
            {
                throw TreeHelper.NotSupportedTree(context);
            }
        }

        private static string GetUsername(UserContext context)
        {
            return context.GetText();
        }
    }
}
