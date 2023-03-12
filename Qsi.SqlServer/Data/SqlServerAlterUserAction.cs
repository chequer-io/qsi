using Qsi.Analyzers;
using Qsi.Data;

namespace Qsi.SqlServer.Data
{
    public class SqlServerAlterUserAction : IQsiAnalysisResult
    {
        public QsiIdentifier TargetUser { get; set; }

        public QsiIdentifier DefaultSchema { get; set; }

        public QsiIdentifier NewUserName { get; set; }

        public QsiSensitiveDataHolder SensitiveDataHolder => null;
    }
}
