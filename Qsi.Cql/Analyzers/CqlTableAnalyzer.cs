using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Qsi.Analyzers;
using Qsi.Analyzers.Context;
using Qsi.Analyzers.Table;
using Qsi.Analyzers.Table.Context;
using Qsi.Cql.Tree;
using Qsi.Data;
using Qsi.Engines;
using Qsi.Shared.Extensions;
using Qsi.Tree;

namespace Qsi.Cql.Analyzers
{
    public class CqlTableAnalyzer : QsiTableAnalyzer
    {
        public CqlTableAnalyzer(QsiEngine engine) : base(engine)
        {
        }

        protected override async ValueTask<IQsiAnalysisResult[]> OnExecute(IAnalyzerContext context)
        {
            if (context.Tree is CqlDerivedTableNode { IsJson: true } cqlTableNode)
            {
                using var scope = new TableCompileContext(context);
                var table = await BuildTableStructure(scope, cqlTableNode);

                var jsonTable = new QsiTableStructure
                {
                    Identifier = table.Identifier,
                    Type = table.Type,
                    IsSystem = table.IsSystem
                };

                jsonTable.References.AddRange(table.References);

                var jsonColumn = jsonTable.NewColumn();

                jsonColumn.Name = new QsiIdentifier("[json]", false);
                jsonColumn.References.AddRange(table.Columns);

                return new CqlJsonTableResult(jsonTable).ToSingleArray();
            }

            return await base.OnExecute(context);
        }

        protected override IEnumerable<QsiTableColumn> ResolveColumnsInExpression(TableCompileContext context, IQsiExpressionNode expression)
        {
            switch (expression)
            {
                case CqlIndexExpressionNode _:
                case CqlMultipleUsingExpressionNode _:
                case CqlUsingExpressionNode _:
                    break;

                case CqlIndexerExpressionNode _:
                case CqlRangeExpressionNode _:
                case CqlSetColumnExpressionNode _:
                {
                    return expression.Children
                        .Cast<IQsiExpressionNode>()
                        .SelectMany(n => ResolveColumnsInExpression(context, n));
                }
            }

            return base.ResolveColumnsInExpression(context, expression);
        }
    }
}
