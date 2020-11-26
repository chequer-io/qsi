using System.Threading.Tasks;
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

        protected override async ValueTask<QsiDataTable> GetDataTableByCommonTableNode(IAnalyzerContext context, IQsiTableNode commonTableNode)
        {
            var table = await base.GetDataTableByCommonTableNode(context, commonTableNode);

            return table;
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
