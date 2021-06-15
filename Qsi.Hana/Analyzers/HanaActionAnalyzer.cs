using Qsi.Analyzers.Action;
using Qsi.Engines;
using Qsi.Hana.Tree;
using Qsi.Tree;

namespace Qsi.Hana.Analyzers
{
    public class HanaActionAnalyzer : QsiActionAnalyzer
    {
        public HanaActionAnalyzer(QsiEngine engine) : base(engine)
        {
        }

        protected override IQsiTableNode ReassembleCommonTableNode(IQsiTableNode node)
        {
            switch (node)
            {
                case IHanaDerivedTableNode derivedTableNode:
                    return new ImmutableHanaDerivedTableNode(
                        derivedTableNode.Parent,
                        derivedTableNode.Directives,
                        derivedTableNode.Columns,
                        derivedTableNode.Source,
                        null,
                        derivedTableNode.Where,
                        derivedTableNode.Grouping,
                        derivedTableNode.Order,
                        derivedTableNode.Limit,
                        derivedTableNode.Top,
                        derivedTableNode.Operation,
                        derivedTableNode.Sampling,
                        derivedTableNode.Behavior,
                        derivedTableNode.TimeTravel,
                        derivedTableNode.Hint,
                        null
                    );
            }

            return base.ReassembleCommonTableNode(node);
        }
    }
}
