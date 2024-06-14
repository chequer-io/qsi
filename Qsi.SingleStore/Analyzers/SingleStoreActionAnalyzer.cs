using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Qsi.Analyzers;
using Qsi.Analyzers.Action;
using Qsi.Analyzers.Context;
using Qsi.Data;
using Qsi.Engines;
using Qsi.Extensions;
using Qsi.SingleStore.Data;
using Qsi.SingleStore.Tree;
using Qsi.Tree;
using static Qsi.SingleStore.Tree.SingleStoreProperties;

namespace Qsi.SingleStore.Analyzers;

public class SingleStoreActionAnalyzer : QsiActionAnalyzer
{
    public SingleStoreActionAnalyzer(QsiEngine engine) : base(engine)
    {
    }

    protected override QsiDataValue ResolveColumnValue(IAnalyzerContext context, IQsiExpressionNode expression)
    {
        if (expression is IQsiLiteralExpressionNode { Value: SingleStoreString singleStoreString })
        {
            return new QsiDataValue(singleStoreString.ToString(), QsiDataType.String);
        }

        return base.ResolveColumnValue(context, expression);
    }
}
