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

            AddSensitiveData(firstResult.SensitiveDataHolder, nodes);
        }

        return result;
    }

    protected override async ValueTask<IQsiAnalysisResult[]> ExecuteDataDeleteAction(IAnalyzerContext context, IQsiDataDeleteActionNode action)
    {
        IQsiAnalysisResult[] result = await base.ExecuteDataDeleteAction(context, action);

        if (result.FirstOrDefault() is { } firstResult &&
            action.Target is IQsiDerivedTableNode { Where: { } whereExpr })
        {
            AddSensitiveData(firstResult.SensitiveDataHolder, whereExpr.Expression);
        }

        return result;
    }

    protected override QsiVariableSetActionResult ResolveVariableSet(IAnalyzerContext context, IQsiVariableSetItemNode node)
    {
        var result = base.ResolveVariableSet(context, node);

        if (result.Name.Value is "PASSWORD")
            result.SensitiveDataHolder.Add(new QsiSensitiveData(QsiSensitiveDataType.Password, MySqlTree.Span[node.Expression]));

        return result;
    }

    protected override QsiUserInfo ResolveUser(IAnalyzerContext context, IQsiUserNode node, QsiSensitiveDataHolder dataHolder)
    {
        var result = base.ResolveUser(context, node, dataHolder);

        if (node.Password is { })
            dataHolder.Add(new QsiSensitiveData(QsiSensitiveDataType.Password, MySqlTree.Span[node.Password]));

        return result;
    }

    private void AddSensitiveData(QsiSensitiveDataHolder holder, IEnumerable<IQsiExpressionNode> nodes)
    {
        AddSensitiveData(holder, nodes.ToArray());
    }

    private void AddSensitiveData(QsiSensitiveDataHolder holder, params IQsiExpressionNode[] nodes)
    {
        IEnumerable<(IQsiTreeNode expr, QsiSensitiveDataType type)> sensitiveExpressions = nodes
            .SelectMany(n => n.Flatten())
            .Select(e => (e, MySqlTree.SensitiveType[e]))
            .Where(e => e.Item2 is not QsiSensitiveDataType.None);

        foreach (var sensitiveExpression in sensitiveExpressions)
        {
            holder.Add(
                new QsiSensitiveData(
                    sensitiveExpression.type,
                    MySqlTree.Span[sensitiveExpression.expr]
                )
            );
        }
    }
}
