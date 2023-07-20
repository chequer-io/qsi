using System;
using System.Threading;
using Antlr4.Runtime.Tree;
using Qsi.Data;
using Qsi.MySql.Internal;
using Qsi.MySql.Tree;
using Qsi.Parsing;
using Qsi.Tree;
using Qsi.Utilities;
using static Qsi.MySql.Internal.MySqlParserInternal;

namespace Qsi.MySql;

public sealed class MySqlParser : IQsiTreeParser
{
    private readonly int _version;
    private readonly bool _mariaDBCompatibility;

    public MySqlParser(Version version, bool mariaDBCompatibility)
    {
        _mariaDBCompatibility = mariaDBCompatibility;
        _version = MySQLUtility.VersionToInt(version);
    }

    public IQsiTreeNode Parse(QsiScript script, CancellationToken cancellationToken = default)
    {
        var parser = MySQLUtility.CreateParser(script.Script, _version, _mariaDBCompatibility);
        var query = parser.query();

        if (query.children[0] is not SimpleStatementContext simpleStatement)
            return null;

        return Parse(simpleStatement.children[0]);
    }

    internal static IQsiTreeNode Parse(IParseTree tree)
    {
        if (tree is QueryContext query)
            tree = query.children[0];

        if (tree is SimpleStatementContext simpleStatementContext)
            tree = simpleStatementContext.children[0];

        switch (tree)
        {
            case SelectStatementContext selectStatement:
                return TableVisitor.VisitSelectStatement(selectStatement);

            case CreateStatementContext createStatement:
                return DefinitionVisitor.VisitCreateStatement(createStatement);

            case DeleteStatementContext deleteStatement:
                return ActionVisitor.VisitDeleteStatement(deleteStatement);

            case ReplaceStatementContext replaceStatement:
                return ActionVisitor.VisitReplaceStatement(replaceStatement);

            case UpdateStatementContext updateStatement:
                return ActionVisitor.VisitUpdateStatement(updateStatement);

            case InsertStatementContext insertStatement:
                return ActionVisitor.VisitInsertStatement(insertStatement);

            case UtilityStatementContext utilityStatement:
                return ParseUtilityStatement(utilityStatement);

            case AccountManagementStatementContext accountManagementStatement:
                return ParseAccountManagementStatement(accountManagementStatement);

            case SetStatementContext setStatement:
                return ActionVisitor.VisitSetStatement(setStatement);

            default:
                throw TreeHelper.NotSupportedTree(tree);
        }
    }

    private static IQsiTreeNode ParseUtilityStatement(UtilityStatementContext context)
    {
        switch (context.children[0])
        {
            case UseCommandContext useCommand:
                return ActionVisitor.VisitUseCommand(useCommand);

            default:
                throw TreeHelper.NotSupportedTree(context.children[0]);
        }
    }

    private static IQsiTreeNode ParseAccountManagementStatement(AccountManagementStatementContext context)
    {
        switch (context.children[0])
        {
            case CreateUserContext createUser:
                return ActionVisitor.VisitCreateUser(createUser);

            case AlterUserContext alterUser:
                return ActionVisitor.VisitAlterUser(alterUser);

            case GrantContext grant:
                return ActionVisitor.VisitGrant(grant);

            default:
                throw TreeHelper.NotSupportedTree(context.children[0]);
        }
    }
}
