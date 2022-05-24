using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Qsi.Analyzers.Table.Context;
using Qsi.Data;
using Qsi.Engines;
using Qsi.PostgreSql.Tree;
using Qsi.Tree;

namespace Qsi.PostgreSql.Analyzers;

// TODO: Realized that Redshift analyzer extends PG one.
//       Can I change PgTableAnalyzer directly?
//       Or create pg base analyzer and extend it?
public class PostgreSqlTableAnalyzer : PgTableAnalyzer
{
    public PostgreSqlTableAnalyzer(QsiEngine engine) : base(engine)
    {
    }

    protected override IEnumerable<QsiTableColumn> ResolveColumnsInExpression(TableCompileContext context, IQsiExpressionNode expression)
    {
        switch (expression)
        {
            // TODO: Remove
            // case PostgreSqlUndefinedExpressionNode undefinedExpression:
            //     yield return GetColumn(context, undefinedExpression);
            //     break;
            
            case PostgreSqlSubscriptExpressionNode:
                break;
            
            default:
                foreach (var tableColumn in base.ResolveColumnsInExpression(context, expression))
                    yield return tableColumn;

                break;
        }
    }

    protected override async ValueTask<QsiTableStructure> BuildInlineDerivedTableStructure(TableCompileContext context, IQsiInlineDerivedTableNode table)
    {
        var structure = await base.BuildInlineDerivedTableStructure(context, table);

        for (var i = 0; i < structure.Columns.Count; i++)
        {
            var column = structure.Columns[i];
            column.Name = new QsiIdentifier($"column{i + 1}", false);
        }

        return structure;
    }

    protected override QsiIdentifier ResolveDerivedColumnName(TableCompileContext context, IQsiDerivedTableNode table, IQsiDerivedColumnNode column)
    {
        if (column.Alias is not null || !column.IsExpression)
        {
            return base.ResolveDerivedColumnName(context, table, column);
        }

        var identifier = column.Expression switch
        {
            IQsiFunctionExpressionNode functionExpression => GetFunctionColumnName(functionExpression),
            IQsiInvokeExpressionNode invokeExpression => GetFunctionColumnName(invokeExpression.Member),
            IQsiLiteralExpressionNode literalExpression => GetLiteralColumnName(literalExpression),
            IQsiMultipleExpressionNode multipleExpression => GetRowColumnName(multipleExpression),
            IQsiTableExpressionNode tableExpression => GetTableColumnName(context, tableExpression, column),
            IQsiMemberAccessExpressionNode memberAccessExpression => GetMemberAccessColumnName(context, memberAccessExpression),
            // TODO: Implement cases such as "1::int2", "actor_id::int2".
            IQsiBinaryExpressionNode => new QsiIdentifier("?column?", false),
            _ => base.ResolveDerivedColumnName(context, table, column)
        };

        return identifier;
    }

    private QsiIdentifier GetFunctionColumnName(IQsiFunctionExpressionNode node)
    {
        var name = node.Identifier[^1].ToString().ToLower();
        var identifier = new QsiIdentifier(name, false);

        return identifier;
    }

    private QsiIdentifier GetLiteralColumnName(IQsiLiteralExpressionNode node)
    {
        var name = node.Type switch
        {
            QsiDataType.Boolean => "bool",
            _ => "?column?"
        };

        var identifier = new QsiIdentifier(name, false);

        return identifier;
    }

    private QsiIdentifier GetRowColumnName(IQsiMultipleExpressionNode node)
    {
        return new QsiIdentifier("row", false);
    }

    private QsiIdentifier GetTableColumnName(TableCompileContext context, IQsiTableExpressionNode tableExpression, IQsiDerivedColumnNode derivedColumn)
    {
        var innerTable = tableExpression.Table;

        switch (innerTable)
        {
            case QsiInlineDerivedTableNode inlineTable:
            {
                using var scopedContext = new TableCompileContext(context);
                
                var scopedTable = BuildInlineDerivedTableStructure(scopedContext, inlineTable).AsTask().Result;
                    
                IList<QsiTableColumn> columns = scopedTable.Columns;
                    
                if (columns.Count != 1)
                    throw new QsiException(QsiError.Syntax);

                return columns[0].Name;
            }

            case IQsiDerivedTableNode derivedTable:
            {
                using var scopedContext = new TableCompileContext(context);

                var scopedTable = BuildDerivedTableStructure(scopedContext, derivedTable).AsTask().Result;
            
                IList<QsiTableColumn> columns = scopedTable.Columns;
            
                if (columns.Count != 1)
                    throw new QsiException(QsiError.Syntax);

                return columns[0].Name;
            }

            default:
                throw new NotSupportedException("Not supported node");
        }
    }

    private QsiIdentifier GetMemberAccessColumnName(TableCompileContext context, IQsiMemberAccessExpressionNode node)
    {
        switch (node.Member)
        {
            case IQsiFieldExpressionNode fieldExpression:
                return fieldExpression.Identifier[^1];

            case PostgreSqlSubscriptExpressionNode:
                var columns = ResolveColumnsInExpression(context, node.Target);

                if (columns.Count() != 1)
                    throw new Exception("Member should not have multiple columns");

                return columns.First().Name;
                
            default:
                throw new NotSupportedException($"Node {node.Member.GetType()} is not supported");
        }
    }
}
