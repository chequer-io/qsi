using System;
using System.Collections.Generic;
using System.Threading;
using Qsi.Data;
using Qsi.Parsing;

namespace Qsi.MongoDB
{
    public class MongoDBScriptParser : IQsiScriptParser
    {
        public IEnumerable<QsiScript> Parse(string input, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public QsiScriptType GetSuitableType(string input)
        {
            throw new NotImplementedException();
        }
    }
}
