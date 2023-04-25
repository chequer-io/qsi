using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Qsi.Analyzers.Context;
using Qsi.Data;
using Qsi.Engines;
using Qsi.Engines.SensitiveData;
using Qsi.Extensions;
using Qsi.MySql.Tree;
using Qsi.Tree;

namespace Qsi.MySql.Analyzers;

public class MySqlSensitiveDataAnalyzer : SensitiveDataAnalyzer
{
    public MySqlSensitiveDataAnalyzer(QsiEngine engine) : base(engine)
    {
    }

    protected override async ValueTask<IEnumerable<QsiSensitiveData>> Analyze(IAnalyzerContext context)
    {
        switch (context.Tree)
        {
            // DELETE ~~
            case IQsiDataDeleteActionNode dataDeleteAction:
                return await AnalyzeDataDeleteAction(context, dataDeleteAction);

            // UPDATE ~~
            case IQsiDataUpdateActionNode dataUpdateAction:
                return await AnalyzeDataUpdateAction(context, dataUpdateAction);

            // CREATE USER ~~
            case IQsiCreateUserActionNode createUserAction:
                return await AnalyzeCreateUserAction(context, createUserAction);

            // ALTER USER ~~
            case IQsiAlterUserActionNode alterUserAction:
                return await AnalyzeAlterUserAction(context, alterUserAction);

            // GRANT USER ~~
            case IQsiGrantUserActionNode grantUserAction:
                return await AnalyzeGrantUserAction(context, grantUserAction);

            // SET <name> = <value>
            case IQsiVariableSetActionNode variableSetAction:
                return await AnalyzeVariableSetAction(context, variableSetAction);

            default:
                return Enumerable.Empty<QsiSensitiveData>();
        }
    }

    protected virtual ValueTask<IEnumerable<QsiSensitiveData>> AnalyzeDataUpdateAction(IAnalyzerContext context, IQsiDataUpdateActionNode action)
    {
        IEnumerable<IQsiExpressionNode> target = action.SetValues.Select(setValue => setValue.Value);

        if (action.Target is IQsiDerivedTableNode { Where: { } whereExpr })
            target = target.Append(whereExpr);

        return ValueTask.FromResult(target.SelectMany(FlattenSensitiveData));
    }

    protected virtual ValueTask<IEnumerable<QsiSensitiveData>> AnalyzeDataDeleteAction(IAnalyzerContext context, IQsiDataDeleteActionNode action)
    {
        IEnumerable<QsiSensitiveData> result = action.Target is IQsiDerivedTableNode { Where: { } whereExpr }
            ? FlattenSensitiveData(whereExpr)
            : Enumerable.Empty<QsiSensitiveData>();

        return ValueTask.FromResult(result);
    }

    protected virtual ValueTask<IEnumerable<QsiSensitiveData>> AnalyzeCreateUserAction(IAnalyzerContext context, IQsiCreateUserActionNode node)
    {
        return ValueTask.FromResult(node.Users.SelectMany(AnalyzeUser));
    }

    protected virtual ValueTask<IEnumerable<QsiSensitiveData>> AnalyzeAlterUserAction(IAnalyzerContext context, IQsiAlterUserActionNode node)
    {
        return ValueTask.FromResult(node.Users.SelectMany(AnalyzeUser));
    }

    protected virtual ValueTask<IEnumerable<QsiSensitiveData>> AnalyzeGrantUserAction(IAnalyzerContext context, IQsiGrantUserActionNode node)
    {
        return ValueTask.FromResult(node.Users.SelectMany(AnalyzeUser));
    }

    protected virtual ValueTask<IEnumerable<QsiSensitiveData>> AnalyzeVariableSetAction(IAnalyzerContext context, IQsiVariableSetActionNode node)
    {
        return ValueTask.FromResult(node.SetItems.SelectMany(AnalyzeVariableSetItem));
    }

    private IEnumerable<QsiSensitiveData> AnalyzeUser(IQsiUserNode node)
    {
        if (node.Password is { })
        {
            yield return CreatePasswordSensitiveData(node.Password);
        }
    }

    private IEnumerable<QsiSensitiveData> AnalyzeVariableSetItem(IQsiVariableSetItemNode node)
    {
        if (node.Name.Value is MySqlKnownVariable.Password)
        {
            yield return CreatePasswordSensitiveData(node.Expression);
        }
    }

    private IEnumerable<QsiSensitiveData> FlattenSensitiveData(IQsiExpressionNode node)
    {
        return node
            .Flatten()
            .Select(e => (e, MySqlTree.SensitiveType[e]))
            .Where(e => e.Item2 is not QsiSensitiveDataType.None)
            .Select(e => CreateSensitiveData(e.Item2, e.e));
    }

    private static QsiSensitiveData CreatePasswordSensitiveData(IQsiTreeNode node)
    {
        return CreateSensitiveData(QsiSensitiveDataType.Password, node);
    }

    private static QsiSensitiveData CreateSensitiveData(QsiSensitiveDataType dataType, IQsiTreeNode node)
    {
        return new QsiSensitiveData(dataType, MySqlTree.Span[node]);
    }
}
