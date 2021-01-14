using Qsi.Data;

namespace Qsi.SqlServer.Data
{
    public class SqlServerAlterUserAction : IQsiAction
    {
        public QsiIdentifier TargetUser { get; set; }

        public QsiIdentifier DefaultSchema { get; set; }

        public QsiIdentifier NewUserName { get; set; }
    }
}
