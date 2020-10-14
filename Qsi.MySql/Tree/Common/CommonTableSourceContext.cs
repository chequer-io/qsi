using Antlr4.Runtime;
using static Qsi.MySql.Internal.MySqlParser;

namespace Qsi.MySql.Tree.Common
{
    internal readonly struct CommonTableSourceContext
    {
        public ParserRuleContext Context { get; }
        
        public TableSourceItemContext TableSourceItem { get; }

        public JoinPartContext[] JoinPart { get; }

        public CommonTableSourceContext(TableSourceBaseContext context)
        {
            Context = context;
            TableSourceItem = context.tableSourceItem();
            JoinPart = context.joinPart();
        }

        public CommonTableSourceContext(TableSourceNestedContext context)
        {
            Context = context;
            TableSourceItem = context.tableSourceItem();
            JoinPart = context.joinPart();
        }
    }
}
