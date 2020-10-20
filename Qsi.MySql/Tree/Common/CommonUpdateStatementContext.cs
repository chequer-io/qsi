using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using static Qsi.MySql.Internal.MySqlParser;

namespace Qsi.MySql.Tree.Common
{
    internal readonly struct CommonUpdateStatementContext : ICommonContext
    {
        public IToken Start { get; }

        public IToken Stop { get; }

        public ParserRuleContext TableSource { get; }

        public UidContext Alias { get; }

        public UpdatedElementContext[] UpdatedElements { get; }

        public CommonWhereContext? Where { get; }

        public OrderByClauseContext Order { get; }

        public LimitClauseContext Limit { get; }

        public CommonUpdateStatementContext(SingleUpdateStatementContext context)
        {
            Start = context.Start;
            Stop = context.Stop;
            TableSource = context.tableName();
            Alias = context.uid();
            UpdatedElements = context.updatedElement();

            if (context.expression() != null)
                Where = new CommonWhereContext(context.expression(), context.WHERE().Symbol, context.expression().Stop);
            else
                Where = null;

            Order = context.orderByClause();
            Limit = context.limitClause();
        }

        public CommonUpdateStatementContext(MultipleUpdateStatementContext context)
        {
            Start = context.Start;
            Stop = context.Stop;
            TableSource = context.tableSources();
            Alias = null;
            UpdatedElements = context.updatedElement();

            if (context.expression() != null)
                Where = new CommonWhereContext(context.expression(), context.WHERE().Symbol, context.expression().Stop);
            else
                Where = null;

            Order = null;
            Limit = null;
        }
    }
}
