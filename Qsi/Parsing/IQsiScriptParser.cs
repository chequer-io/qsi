using System.Collections.Generic;
using System.Threading;
using Qsi.Data;

namespace Qsi.Parsing
{
    public interface IQsiScriptParser
    {
        IEnumerable<QsiScript> Parse(in string input, CancellationToken cancellationToken);
    }
}
