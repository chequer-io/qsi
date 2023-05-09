using System;
using Antlr4.Runtime.Tree;
using Qsi.Diagnostics.Antlr;
using Qsi.MySql.Internal;

namespace Qsi.MySql.Diagnostics
{
    public class MySqlRawParser : AntlrRawParserBase
    {
        private readonly int _version;
        private readonly bool _mariaDbCompatibility;

        public MySqlRawParser(Version version, bool mariaDBCompatibility)
        {
            _version = MySQLUtility.VersionToInt(version);
            _mariaDbCompatibility = mariaDBCompatibility;
        }

        protected override (ITree Tree, string[] RuleNames) ParseAntlrTree(string input)
        {
            var parser = MySQLUtility.CreateParser(input, _version, _mariaDbCompatibility);
            return (parser.query(), parser.RuleNames);
        }
    }
}
