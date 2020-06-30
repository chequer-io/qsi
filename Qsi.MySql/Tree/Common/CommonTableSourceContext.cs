using static Qsi.MySql.Internal.MySqlParser;

namespace Qsi.MySql.Tree.Common
{
    internal sealed class CommonTableSourceContext
    {
        public TableSourceItemContext TableSourceItem { get; }

        public JoinPartContext[] JoinPart { get; }

        public CommonTableSourceContext(TableSourceBaseContext context)
        {
            TableSourceItem = context.tableSourceItem();
            JoinPart = context.joinPart();
        }

        public CommonTableSourceContext(TableSourceNestedContext context)
        {
            TableSourceItem = context.tableSourceItem();
            JoinPart = context.joinPart();
        }
    }
}
