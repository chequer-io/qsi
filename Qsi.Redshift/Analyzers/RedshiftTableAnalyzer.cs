using System;
using System.Collections.Generic;
using Qsi.Analyzers.Table.Context;
using Qsi.Data;
using Qsi.Engines;
using Qsi.PostgreSql.Analyzers;
using Qsi.Tree;

namespace Qsi.Redshift.Analyzers;

public class RedshiftTableAnalyzer : PgTableAnalyzer
{
    // As defined in https://docs.aws.amazon.com/en_us/redshift/latest/dg/r_Dateparts_for_datetime_functions.html
    private readonly HashSet<string> _dateFuncParameter = new()
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

    private readonly HashSet<string> _withoutBracketFunctions = new()
    {
        "localtime",
        "localtimestamp",
        "sysdate",
        "current_date",
        "current_time",
        "current_timestamp",
        "user",
        "current_user_id",
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
        catch (QsiException e) when (
            column.Name.Level == 1 &&
            e.Error is QsiError.UnknownColumn or QsiError.UnknownColumnIn
        )
        {
            if (IsEnumParameterInFunction(column) || IsBuiltInFunction(column))
                return Array.Empty<QsiTableColumn>();

            throw;
        }
    }

    private bool IsBuiltInFunction(IQsiColumnReferenceNode column)
    {
        return _withoutBracketFunctions.Contains(column.Name[0].Value.ToLowerInvariant());
    }

    private bool IsEnumParameterInFunction(IQsiColumnReferenceNode column)
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
        string parameter = column.Name.ToString().ToLowerInvariant();

        switch (functionName)
        {
            case "dateadd":
            case "datediff":
            case "date_part":
            case "date_trunc":
            case "extract":
            {
                if (_dateFuncParameter.Contains(parameter))
                    return true;

                break;
            }
        }

        return false;
    }
}
