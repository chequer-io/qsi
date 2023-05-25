using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Qsi.Data;
using Qsi.MongoDB.Acorn;
using Qsi.MongoDB.Internal.Nodes.Location;
using Qsi.Parsing.Common;

namespace Qsi.MongoDB;

public class MongoDBScriptParser : CommonScriptParser
{
    public override IEnumerable<QsiScript> Parse(string input, CancellationToken cancellationToken = default)
    {
        return MongoDBScript.Parse(input)
            .Statements
            .Select(s => new QsiScript(
                input[s.Range],
                QsiScriptType.Unknown,
                ConvertToPosition(s.Start, s.Range.Start.Value),
                ConvertToPosition(s.End, s.Range.End.Value - 1)
            ));
    }

    private static QsiScriptPosition ConvertToPosition(Location location, int index)
    {
        return new(location.Line, location.Column + 1, index);
    }
}
