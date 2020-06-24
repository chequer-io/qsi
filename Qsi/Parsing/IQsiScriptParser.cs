using System.Collections.Generic;
using Qsi.Data;

namespace Qsi.Parsing
{
    public interface IQsiScriptParser
    {
        IEnumerable<QsiScript> Parse(in string input);
    }
}
