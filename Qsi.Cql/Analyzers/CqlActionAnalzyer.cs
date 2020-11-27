using System.Collections.Generic;
using System.Threading.Tasks;
using Qsi.Analyzers;
using Qsi.Analyzers.Action;
using Qsi.Analyzers.Context;
using Qsi.Cql.Tree;
using Qsi.Data;
using Qsi.Tree;

namespace Qsi.Cql.Analyzers
{
    public class CqlActionAnalzyer : QsiActionAnalyzer
    {
        public CqlActionAnalzyer(QsiEngine engine) : base(engine)
        {
        }

        protected override async ValueTask<IQsiAnalysisResult> ExecuteDataDeleteAction(IAnalyzerContext context, IQsiDataDeleteActionNode action)
        {
            var tableNode = (CqlDerivedTableNode)action.Target;
            var columns = tableNode.Columns.Value;
            var normalizedColumns = new List<QsiIdentifier>();

            foreach (var column in columns)
            {
                QsiIdentifier identifier;

                switch (column)
                {
                    case IQsiDeclaredColumnNode declaredColumn:
                    {
                        identifier = declaredColumn.Name[^1];
                        break;
                    }

                    case IQsiDerivedColumnNode derivedColumn when
                        derivedColumn.IsExpression &&
                        derivedColumn.Expression is IQsiMemberAccessExpressionNode memberAccess:
                    {
                        while (memberAccess.Target is IQsiMemberAccessExpressionNode prevMemberAccess)
                            memberAccess = prevMemberAccess;

                        var columnExpression = (IQsiColumnExpressionNode)memberAccess.Target;
                        var declaredColumn = (IQsiDeclaredColumnNode)columnExpression.Column;

                        identifier = declaredColumn.Name[^1];
                        break;
                    }

                    default:
                        throw new QsiException(QsiError.Syntax);
                }

                normalizedColumns.Add(identifier);
            }

            var result = new QsiDataAction();

            return new QsiActionAnalysisResult(result);
        }

        protected override IQsiTableNode ReassembleCommonTableNode(IQsiTableNode node)
        {
            if (node is CqlDerivedTableNode cqlNode)
            {
                var ctn = new CqlDerivedTableNode
                {
                    Parent = cqlNode.Parent,
                    IsJson = cqlNode.IsJson,
                    IsDistinct = cqlNode.IsDistinct,
                    AllowFiltering = cqlNode.AllowFiltering
                };

                if (!cqlNode.Columns.IsEmpty)
                    ctn.Columns.SetValue(cqlNode.Columns.Value);

                if (!cqlNode.Source.IsEmpty)
                    ctn.Source.SetValue(cqlNode.Source.Value);

                if (!cqlNode.Where.IsEmpty)
                    ctn.Where.SetValue(cqlNode.Where.Value);

                if (!cqlNode.Grouping.IsEmpty)
                    ctn.Grouping.SetValue(cqlNode.Grouping.Value);

                if (!cqlNode.Order.IsEmpty)
                    ctn.Order.SetValue(cqlNode.Order.Value);

                if (!cqlNode.Limit.IsEmpty)
                    ctn.Limit.SetValue(cqlNode.Limit.Value);

                if (!cqlNode.PerPartitionLimit.IsEmpty)
                    ctn.PerPartitionLimit.SetValue(cqlNode.PerPartitionLimit.Value);

                return ctn;
            }

            return base.ReassembleCommonTableNode(node);
        }
    }
}
