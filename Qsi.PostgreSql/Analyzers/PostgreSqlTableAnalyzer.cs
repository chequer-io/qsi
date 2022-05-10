using System.Collections.Generic;
using System.Linq;
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
            case PostgreSqlUndefinedExpressionNode e:
                var identifier = ResolveQualifiedIdentifier(context, e.Name);
                var lastName = identifier[^1];

                // TODO: Remove console logs used for debugging
                System.Console.WriteLine($"IDENTIFIER: {identifier}");
                System.Console.WriteLine($"IDENTIFIER_LAST: {lastName.Value}");
                
                PrintContext(context);

                // TODO: Make the logic strictly.
                //       It could be a value even if there is a column which has a same name as the identifier.
                var cursor = context;
                var tableStructures = new List<QsiTableStructure>();

                tableStructures.AddRange(cursor.GetAllSourceTables());
                
                // TODO: Remove nasty while codes; cleanup.
                while(cursor.Parent != null)
                {
                    tableStructures.AddRange(cursor.GetAllSourceTables());
                    cursor = cursor.Parent;
                }

                var columnTuples = tableStructures
                    .Where(t => t.HasIdentifier)
                    .SelectMany(t => t.Columns.Select(c => (table: t, column: c)));

                var tuple = columnTuples.Where(tuple => tuple.column.Name.Value == lastName.Value);

                if (tuple.Any())
                {
                    // TODO: Remove console logs used for debugging
                    System.Console.WriteLine("TUPLE_FIND");
                    System.Console.WriteLine(tuple
                        .Select(t => $"{t.table.Identifier}:${t.column.Name.Value}")
                        .Aggregate((a, b) => $"{a} {b}"));
                    
                    // TODO: Dividing resolved identifier is required due to an error while executing ResolveColumnReference.
                    //       If not divide, unknown column error occurs because it contains database name.
                    //       If divide and contain only column name, ambiguous error occurs because there's two table:
                    //       postgres.public.actor and a (table alias).
                    //       Need to consider resolving identifier without database name.
                    //       (public.actor not postgres.public.actor)
                    var columnNode = new QsiColumnReferenceNode
                    {
                        Name = new QsiQualifiedIdentifier(tuple.First().table.Identifier.Append(lastName))
                    }; // TODO: Bad approach.
                    
                    var tableColumn = ResolveColumnReference(context, columnNode, out _);

                    foreach (var column in tableColumn)
                    {
                        System.Console.WriteLine(column.Name.Value);
                        System.Console.WriteLine(column.Parent.Identifier.ToString());
                        yield return column;
                    }

                    break;
                }

                // TODO: Implement case of non-columns (such as row values)
                // throw new System.Exception("Error!");
                foreach (var column in base.ResolveColumnsInExpression(context, expression))
                {
                    yield return column;
                }
                
                break;
            
            default:
                foreach (var column in base.ResolveColumnsInExpression(context, expression))
                {
                    yield return column;
                }

                break;
        }
    }

    private void PrintContext(TableCompileContext context)
    {
        System.Console.WriteLine($"----CONTEXT INFORMATION FOR: {context}----");
        
        if (context.Directives?.Count() > 0)
        {
            System.Console.WriteLine("DIRECTIVES");
            System.Console.WriteLine(context.Directives
                .Select(d => d.HasIdentifier ?
                    d.Identifier.ToString() : 
                    d.Columns
                        .Select(c => c.Name.Value)
                        .Aggregate((a, b) => $"{a}, {b}"))
                .Aggregate((a, b) => $"{a} {b}"));
        }

        // TODO: Typo; should be 'JoinedSourceTables'.
        if (context.JoinedSouceTables?.Count > 0)
        {
            System.Console.WriteLine("JOINED_SOURCE_TABLES");
            System.Console.WriteLine(context.JoinedSouceTables?
                .Select(t => t.Identifier.ToString())
                .Aggregate((a, b) => $"{a} {b}"));   
        }

        if (context.GetAllSourceTables()?.Count() > 0)
        {
            System.Console.WriteLine("ALL_SOURCE_TABLES");

            System.Console.WriteLine(context.GetAllSourceTables()
                .Select(t => t.HasIdentifier ?
                    t.Identifier.ToString() :
                    t.Columns
                        .Select(c => c.Name.Value)
                        .Aggregate((a, b) => $"{a}, {b}"))
                .Aggregate((a, b) => $"{a} {b}"));
        }

        if (context.SourceTable != null)
        {
            System.Console.WriteLine($"SOURCE_TABLE_IDENTIFIER: {context.SourceTable.Identifier}");
            System.Console.WriteLine("SOURCE_TABLE_COLUMNS");
            System.Console.WriteLine(context.SourceTable.Columns
                .Select(c => c.Name.Value)
                .Aggregate((a, b) => $"{a} {b}"));
        }
        
        System.Console.WriteLine("---- END CONTEXT INFORMATION ----");
        
        if (context.Parent != null)
        {
            System.Console.WriteLine(">>>> PARENT CONTEXT >>>>");
            
            PrintContext(context.Parent);
        }
    }
}
