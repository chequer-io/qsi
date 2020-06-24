using System;
using System.Collections.Generic;
using Qsi.Data;

namespace Qsi.Parsing
{
    public class CommonScriptParser : IQsiScriptParser
    {
        public IEnumerable<QsiScript> Parse(in string input)
        {
            throw new NotImplementedException();
        }

        protected QsiScript CreateScript(in string script, QsiScriptPosition start, QsiScriptPosition end)
        {
            throw new NotImplementedException();
        }
    }
}
