using System;
using System.Diagnostics.CodeAnalysis;
using PgQuery;
using Qsi.PostgreSql.Data;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.PostgreSql.NewTree;

internal static partial class PgNodeVisitor
{
    [return: NotNullIfNotNull("node")]
    public static IQsiTreeNode? Visit(Node node)
    {
        if (node is not { } || node.Get() is not { })
            return null;

        return node.Get() switch
        {
            IPgStatementNode statement => VisitStatement(statement),
            IPgExpressionNode expression => VisitExpression(expression),
            IPgConstNode constNode => VisitConst(constNode),
            IPgClauseNode clause => VisitClause(clause),
            ResTarget resTarget => Visit(resTarget),
            RangeVar rangeVar => Visit(rangeVar),
            A_Indices aIndices => Visit(aIndices),
            MultiAssignRef multiAssignRef => Visit(multiAssignRef),
            RangeSubselect rangeSubselect => Visit(rangeSubselect),
            SortBy sortBy => Visit(sortBy),
            WindowDef windowDef => Visit(windowDef),
            GroupingSet groupingSet => Visit(groupingSet),
            RangeFunction rangeFunction => Visit(rangeFunction),
            DefElem defElem => Visit(defElem),
            _ => throw TreeHelper.NotSupportedTree(node.Get())
        };
    }

    [return: NotNullIfNotNull("node")]
    public static IQsiTreeNode? VisitStatement(IPgStatementNode? node)
    {
        return node switch
        {
            null => null,
            InsertStmt insert => Visit(insert),
            DeleteStmt delete => Visit(delete),
            UpdateStmt update => Visit(update),
            MergeStmt merge => Visit(merge),
            SelectStmt select => Visit(select),
            CreateStmt create => Visit(create),
            CreateTableAsStmt createTableAs => Visit(createTableAs),
            VariableSetStmt variableSetStmt => Visit(variableSetStmt),
            ViewStmt viewStmt => Visit(viewStmt),
            _ => throw TreeHelper.NotSupportedTree(node)
        };
    }

    [return: NotNullIfNotNull("node")]
    public static IQsiTreeNode? VisitClause(IPgClauseNode node)
    {
        return node switch
        {
            null => null,
            WithClause with => Visit(with),
            _ => throw new NotSupportedException($"Not supported clause node: '{node.GetType().Name}'")
        };
    }

    [return: NotNullIfNotNull("node")]
    public static T? Visit<T>(Node? node) where T : IQsiTreeNode
    {
        if (node is null)
            return default;

        var treeNode = Visit(node);

        return treeNode is not T tTreeNode
            ? throw new Exception($"Exptected node: {typeof(T).Name} but received: {treeNode.GetType().Name}")
            : tTreeNode;
    }

    public static QsiException CreateInternalException(string message)
    {
        return new QsiException(QsiError.Internal, message);
    }

    public static QsiException NotSupportedOption<T>(T value) where T : Enum
    {
        return new QsiException(QsiError.Internal, $"Not supported Option {typeof(T).Name}.{value}");
    }

    private static Relpersistence ToRelpersistence(this string value)
    {
        return value switch
        {
            "p" => Relpersistence.Permanent,
            "u" => Relpersistence.Unlogged,
            "t" => Relpersistence.Temp,
            _ => Relpersistence.Unknown
        };
    }
}
