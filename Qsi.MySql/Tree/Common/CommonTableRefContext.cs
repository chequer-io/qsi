using Antlr4.Runtime;
using Qsi.Shared;
using static Qsi.MySql.Internal.MySqlParserInternal;

namespace Qsi.MySql.Tree.Common
{
    internal readonly struct CommonTableRefContext : IParserRuleContext
    {
        public IToken Start { get; }

        public IToken Stop { get; }

        public TableRefContext TableRef { get; }

        public IdentifierListContext Partitions { get; }

        public CommonTableRefContext(TableRefContext tableRef) : this(tableRef, (UsePartitionContext)null)
        {
        }

        public CommonTableRefContext(TableRefContext tableRef, UsePartitionContext usePartition)
        {
            TableRef = tableRef;
            Partitions = usePartition?.identifierListWithParentheses()?.identifierList();

            Start = tableRef.Start;
            Stop = usePartition?.Stop ?? tableRef.Stop;
        }

        public CommonTableRefContext(TableRefContext tableRef, PartitionDeleteContext partitionDelete)
        {
            TableRef = tableRef;
            Partitions = partitionDelete?.identifierList();

            Start = tableRef.Start;
            Stop = partitionDelete?.Stop ?? tableRef.Stop;
        }
    }
}
