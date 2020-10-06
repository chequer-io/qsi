using static Qsi.MySql.Internal.MySqlParser;

namespace Qsi.MySql.Tree.Common
{
    internal readonly struct CommonSelectContext
    {
        public SelectElementsContext SelectElements { get; }

        public FromClauseContext FromClause { get; }

        public CommonSelectContext(QuerySpecificationContext context)
        {
            SelectElements = context.selectElements();
            FromClause = context.fromClause();
        }

        public CommonSelectContext(QuerySpecificationNointoContext context)
        {
            SelectElements = context.selectElements();
            FromClause = context.fromClause();
        }
    }
}
