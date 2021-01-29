using Qsi.SqlServer.Data;
using Qsi.Tree;

namespace Qsi.SqlServer.Tree
{
    public interface ISqlServerBinaryTableNode : IQsiTableNode
    {
        public IQsiTableNode Left { get; }
        
        public SqlServerBinaryTableType BinaryTableType { get; }

        public IQsiTableNode Right { get; }
    }
}
