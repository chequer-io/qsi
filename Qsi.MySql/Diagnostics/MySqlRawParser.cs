using System;
using Antlr4.Runtime.Tree;
using Qsi.Diagnostics.Antlr;
using Qsi.MySql.Internal;

namespace Qsi.MySql.Diagnostics
{
    public class MySqlRawParser : AntlrRawParserBase
    {
        private readonly int _version;

        public MySqlRawParser(Version version)
        {
            _version = MySQLUtility.VersionToInt(version);
        }

        protected override (ITree Tree, string[] RuleNames) ParseAntlrTree(string input)
        {
            var parser = MySQLUtility.CreateParser(input, _version);
            return (parser.query(), parser.RuleNames);
        }
    }
}
