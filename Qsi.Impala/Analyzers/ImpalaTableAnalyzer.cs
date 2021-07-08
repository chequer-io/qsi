using System.Threading.Tasks;
using Qsi.Analyzers.Table;
using Qsi.Analyzers.Table.Context;
using Qsi.Data;
using Qsi.Engines;
using Qsi.Impala.Tree;
using Qsi.Tree;

namespace Qsi.Impala.Analyzers
{
    public class ImpalaTableAnalyzer : QsiTableAnalyzer
    {
        public ImpalaTableAnalyzer(QsiEngine engine) : base(engine)
        {
        }

        public override ValueTask<QsiTableStructure> BuildTableStructure(TableCompileContext context, IQsiTableNode table)
        {
            if (table is ImpalaValuesTableNode valuesTableNode)
                return BuildImpalaValuesTableStructure(context,valuesTableNode);

            return base.BuildTableStructure(context, table);
        }

        private ValueTask<QsiTableStructure> BuildImpalaValuesTableStructure(TableCompileContext context, ImpalaValuesTableNode valuesTableNode)
        {
            throw new System.NotImplementedException();
        }
    }
}
