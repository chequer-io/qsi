using System;
using Qsi.Diagnostics;
using Qsi.MongoDB.Acorn;

namespace Qsi.MongoDB.Diagnostics
{
    public class MongoDBRawParser : IRawTreeParser
    {
        public IRawTree Parse(string input)
        {
            try
            {
                return new MongoDBRawTree(AcornParser.GetAstNode(input));
            }
            catch (Exception e)
            {
                return new MongoDBRawTreeTerminalNode(input);
            }
        }
    }
}
