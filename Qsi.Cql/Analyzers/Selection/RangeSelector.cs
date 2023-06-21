using System;
using Newtonsoft.Json.Linq;

namespace Qsi.Cql.Analyzers.Selection;

internal class RangeSelector : ISelector
{
    public Range Range { get; }

    public RangeSelector(Range range)
    {
        Range = range;
    }

    public JToken Run(JToken value)
    {
        if (value is JArray array)
            return array[Range];

        throw new QsiException(QsiError.NotSupportedFeature, $"Range access to {value.Type}");
    }
}