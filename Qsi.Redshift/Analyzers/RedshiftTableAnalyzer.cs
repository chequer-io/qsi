using System;
using System.Linq;
using Qsi.Analyzers.Table.Context;
using Qsi.Data;
using Qsi.Engines;
using Qsi.PostgreSql.Analyzers;
using Qsi.Redshift.Internal;
using Qsi.Tree;

namespace Qsi.Redshift.Analyzers;

public class RedshiftTableAnalyzer : PgTableAnalyzer
{
    private readonly string[] _redshiftTimeFuncParameter =
    {
        "millennium", "millennia", "mil", "mils",
        "century", "centuries c", "cent", "cents",
        "decade", "decades", "dec", "decs", "Epoch",
        "year", "years", "y", "yr", "yrs",
        "quarter", "quarters", "qtr", "qtrs",
        "month", "months", "mon", "mons",
        "week", "weeks", "w", "dayofweek", "dow", "dw",
        "weekday", "day_of_year", "dayofyear", "doy", "dy",
        "yearday", "day", "days", "d",
        "hour", "hours", "h", "hr", "hrs",
        "minute", "minutes", "m", "min", "mins",
        "second", "seconds", "s", "sec", "secs",
        "millisecond", "milliseconds", "ms", "msec",
        "msecs", "msecond", "mseconds", "millisec", "millisecs", "millisecon",
        "microsecond", "microseconds", "microsec", "microsecs", "microsecond",
        "usecond", "useconds", "us", "usec", "usecs",
        "timezone", "timezone_hour", "timezone_minute"
    };

    public RedshiftTableAnalyzer(QsiEngine engine) : base(engine)
    {
    }

    protected override QsiTableColumn[] ResolveColumnReference(TableCompileContext context, IQsiColumnReferenceNode column, out QsiQualifiedIdentifier implicitTableWildcardTarget)
    {
        implicitTableWildcardTarget = default;

        try
        {
            return base.ResolveColumnReference(context, column, out implicitTableWildcardTarget);
        }
        catch (QsiException e) when (e.Error is QsiError.UnknownColumn or QsiError.UnknownColumnIn)
        {
            if (RedshiftPseudoColumn.TryGetColumn(column.Name[^1].Value, out var tableColumn))
                return new[] { tableColumn };

            if (IsEnumInFunction(column))
                return Array.Empty<QsiTableColumn>();

            throw;
        }
    }

    private bool IsEnumInFunction(IQsiColumnReferenceNode column)
    {
        if (column.Parent is not IQsiColumnExpressionNode
            {
                Parent: IQsiParametersExpressionNode
                {
                    Parent: IQsiInvokeExpressionNode funcNode
                }
            })
        {
            return false;
        }

        string functionName = funcNode.Member.Identifier.ToString().ToLowerInvariant();
        string parameterName = column.Name.ToString();

        switch (functionName)
        {
            case "datediff":
            {
                if (_redshiftTimeFuncParameter.Contains(parameterName))
                    return true;

                break;
            }
        }

        return false;
    }
}
