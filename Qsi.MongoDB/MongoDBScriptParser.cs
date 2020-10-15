using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Qsi.Data;
using Qsi.MongoDB.Acorn;
using Qsi.MongoDB.Internal.Nodes.Location;
using Qsi.Parsing.Common;

namespace Qsi.MongoDB
{
    public class MongoDBScriptParser : CommonScriptParser
    {
        public override IEnumerable<QsiScript> Parse(string input, CancellationToken cancellationToken = default)
        {
            return MongoDBScript.Parse(input)
                .JavascriptStatements
                .Select(s => new QsiScript(
                    input[s.Range],
                    QsiScriptType.Unknown,
                    ConvertToPosition(s.Start),
                    ConvertToPosition(s.End)
                ));
        }

        private QsiScriptPosition ConvertToPosition(Location location)
        {
            return new QsiScriptPosition(location.Line, location.Column + 1);
        }
    }
}
