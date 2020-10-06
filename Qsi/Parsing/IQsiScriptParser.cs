using System.Collections.Generic;
using System.Threading;
using Qsi.Data;

namespace Qsi.Parsing
{
    public interface IQsiScriptParser
    {
        IEnumerable<QsiScript> Parse(string input, CancellationToken cancellationToken = default);

        QsiScriptType GetSuitableType(string input);
    }
}
