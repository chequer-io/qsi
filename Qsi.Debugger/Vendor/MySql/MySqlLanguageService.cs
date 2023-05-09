using System;
using Qsi.Data;
using Qsi.MySql;
using Qsi.Services;
using Qsi.Tree;

namespace Qsi.Debugger.Vendor.MySql
{
    internal class MySqlLanguageService : MySqlLanguageServiceBase
    {
        public override Version Version { get; }

        public override bool MariaDBCompatibility { get; }

        public MySqlLanguageService(Version version, bool mariaDbCompatibility)
        {
            Version = version;
            MariaDBCompatibility = mariaDbCompatibility;
        }

        public override IQsiRepositoryProvider CreateRepositoryProvider()
        {
            return new MySqlRepositoryProvider();
        }

        public override QsiParameter FindParameter(QsiParameter[] parameters, IQsiBindParameterExpressionNode node)
        {
            return VendorDebugger.HookFindParameter(parameters, node);
        }
    }
}
