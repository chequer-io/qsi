using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Qsi.Analyzers;
using Qsi.Analyzers.Action;
using Qsi.Analyzers.Context;
using Qsi.Data;
using Qsi.Engines;
using Qsi.Extensions;
using Qsi.MySql.Tree;
using Qsi.Tree;
using static Qsi.MySql.Tree.MySqlProperties;

namespace Qsi.MySql.Analyzers;

public class MySqlActionAnalyzer : QsiActionAnalyzer
{
    public MySqlActionAnalyzer(QsiEngine engine) : base(engine)
    {
    }

    protected override async ValueTask<IQsiAnalysisResult[]> ExecuteDataUpdateAction(IAnalyzerContext context, IQsiDataUpdateActionNode action)
    {
        IQsiAnalysisResult[] result = await base.ExecuteDataUpdateAction(context, action);

        if (result.FirstOrDefault() is { } firstResult &&
            action.Target is IQsiDerivedTableNode { Where: { } whereExpr })
        {
            IEnumerable<IQsiExpressionNode> nodes = action.SetValues
                .Select(values => values.Value)
                .Append(whereExpr.Expression);

            AddSensitiveData(firstResult.SensitiveDataCollection, nodes);
        }

        return result;
    }

    protected override async ValueTask<IQsiAnalysisResult[]> ExecuteDataDeleteAction(IAnalyzerContext context, IQsiDataDeleteActionNode action)
    {
        IQsiAnalysisResult[] result = await base.ExecuteDataDeleteAction(context, action);

        if (result.FirstOrDefault() is { } firstResult &&
            action.Target is IQsiDerivedTableNode { Where: { } whereExpr })
        {
            AddSensitiveData(firstResult.SensitiveDataCollection, whereExpr.Expression);
        }

        return result;
    }

    protected override QsiVariableSetActionResult ResolveVariableSet(IAnalyzerContext context, IQsiVariableSetItemNode node)
    {
        var result = base.ResolveVariableSet(context, node);

        if (result.Name.Value is "PASSWORD")
            result.SensitiveDataCollection.Add(CreateSensitiveData(QsiSensitiveDataType.Password, node.Expression));

        return result;
    }

    protected override QsiUserInfo ResolveUser(IAnalyzerContext context, IQsiUserNode node, QsiSensitiveDataCollection dataCollection)
    {
        var user = base.ResolveUser(context, node, dataCollection);

        if (node.Password is not null)
        {
            dataCollection.Add(CreateSensitiveData(QsiSensitiveDataType.Password, node.Password));
            user.Properties[User.IsRandomPassword] = false;
        }
        else
        {
            user.Properties[User.IsRandomPassword] = node is MySqlUserNode { IsRandomPassword: true };
        }

        return user;
    }

    private void AddSensitiveData(QsiSensitiveDataCollection collection, IEnumerable<IQsiExpressionNode> nodes)
    {
        AddSensitiveData(collection, nodes.ToArray());
    }

    private void AddSensitiveData(QsiSensitiveDataCollection collection, params IQsiExpressionNode[] nodes)
    {
        IEnumerable<QsiSensitiveData> sensitiveDatas = nodes
            .SelectMany(n => n.Flatten())
            .Select(e => (e, MySqlTree.SensitiveType[e]))
            .Where(e => e.Item2 is not QsiSensitiveDataType.None)
            .Select(e => CreateSensitiveData(e.Item2, e.e));

        collection.AddRange(sensitiveDatas);
    }

    protected override QsiSensitiveData CreateSensitiveData(QsiSensitiveDataType dataType, IQsiTreeNode node)
    {
        return new QsiSensitiveData(dataType, MySqlTree.Span[node]);
    }
}
