using Qsi.Analyzers.Action;
using Qsi.PrimarSql.Tree;
using Qsi.Tree;

namespace Qsi.PrimarSql.Analyzers
{
    public class PrimarSqlActionAnalyzer : QsiActionAnalyzer
    {
        public PrimarSqlActionAnalyzer(QsiEngine engine) : base(engine)
        {
        }

        protected override IQsiTableNode ReassembleCommonTableNode(IQsiTableNode node)
        {
            if (node is PrimarSqlDerivedTableNode primarSqlNode)
            {
                var ctn = new PrimarSqlDerivedTableNode
                {
                    Parent = primarSqlNode.Parent,
                    SelectSpec = primarSqlNode.SelectSpec
                };

                if (!primarSqlNode.Columns.IsEmpty)
                    ctn.Columns.SetValue(primarSqlNode.Columns.Value);

                if (!primarSqlNode.Source.IsEmpty)
                    ctn.Source.SetValue(primarSqlNode.Source.Value);

                if (!primarSqlNode.Where.IsEmpty)
                    ctn.Where.SetValue(primarSqlNode.Where.Value);

                if (!primarSqlNode.Order.IsEmpty)
                    ctn.Order.SetValue(primarSqlNode.Order.Value);

                if (!primarSqlNode.Limit.IsEmpty)
                    ctn.Limit.SetValue(primarSqlNode.Limit.Value);

                if (!primarSqlNode.StartKey.IsEmpty)
                    ctn.StartKey.SetValue(primarSqlNode.StartKey.Value);

                return ctn;
            }

            return base.ReassembleCommonTableNode(node);
        }
    }
}
