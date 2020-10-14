using Qsi.Diagnostics;

namespace Qsi.MongoDB.Diagnostics
{
    public class MongoDBRawTreeTerminalNode : IRawTreeTerminalNode
    {
        public string DisplayName { get; }

        public IRawTree[] Children { get; }

        internal MongoDBRawTreeTerminalNode(object value)
        {
            DisplayName = value.ToString();
        }
    }
}
