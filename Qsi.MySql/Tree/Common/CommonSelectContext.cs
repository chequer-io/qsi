using Antlr4.Runtime;
using static Qsi.MySql.Internal.MySqlParser;

namespace Qsi.MySql.Tree.Common
{
    internal readonly struct CommonSelectContext
    {
        public ParserRuleContext Context { get; }

        public SelectElementsContext SelectElements { get; }

        public FromClauseContext FromClause { get; }

        public CommonSelectContext(QuerySpecificationContext context)
        {
            Context = context;
            SelectElements = context.selectElements();
            FromClause = context.fromClause();
        }

        public CommonSelectContext(QuerySpecificationNointoContext context)
        {
            Context = context;
            SelectElements = context.selectElements();
            FromClause = context.fromClause();
        }
    }
}
