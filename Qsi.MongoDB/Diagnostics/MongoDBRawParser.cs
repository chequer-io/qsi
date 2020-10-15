using System;
using Qsi.Diagnostics;
using Qsi.MongoDB.Acorn;

namespace Qsi.MongoDB.Diagnostics
{
    public class MongoDBRawParser : IRawTreeParser
    {
        public IRawTree Parse(string input)
        {
            return new MongoDBRawTree(AcornParser.GetAstNode(input));
        }
    }
}
