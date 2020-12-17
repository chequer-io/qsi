using System;
using Antlr4.Runtime;
using static Qsi.MySql.Internal.MySqlParserInternal;

namespace Qsi.MySql.Tree.Common
{
    internal readonly struct CommonQueryContext : IParserRuleContext
    {
        public IToken Start { get; }

        public IToken Stop { get; }

        public WithClauseContext WithClause { get; }

        public SelectOptionContext[] SelectOptions { get; }

        public SelectItemListContext SelectItemList { get; }

        public IntoClauseContext IntoClause { get; }

        // QueryExpressionBodyContext
        // QueryExpressionParensContext
        // FromClauseContext
        public ParserRuleContext Source { get; }

        public WhereClauseContext WhereClause { get; }

        public GroupByClauseContext GroupByClause { get; }

        public HavingClauseContext HavingClause { get; }

        public WindowClauseContext WindowClause { get; }

        public OrderClauseContext OrderClause { get; }

        public LimitClauseContext LimitClause { get; }

        public ProcedureAnalyseClauseContext ProcedureAnalyseClause { get; }

        public LockingClauseListContext LockingClauseList { get; }

        public CommonQueryContext(QueryExpressionContext queryExpression, LockingClauseListContext lockingClauseList)
        {
            if (queryExpression == null)
                throw new ArgumentNullException(nameof(queryExpression));

            WithClause = queryExpression.withClause();
            Source = (ParserRuleContext)queryExpression.queryExpressionBody() ?? queryExpression.queryExpressionParens();
            OrderClause = queryExpression.orderClause();
            LimitClause = queryExpression.limitClause();
            ProcedureAnalyseClause = queryExpression.procedureAnalyseClause();
            LockingClauseList = lockingClauseList;

            SelectOptions = null;
            SelectItemList = null;
            IntoClause = null;
            WhereClause = null;
            GroupByClause = null;
            HavingClause = null;
            WindowClause = null;

            Start = queryExpression.Start;
            Stop = lockingClauseList?.Stop ?? queryExpression.Stop;
        }

        public CommonQueryContext(QuerySpecificationContext querySpecification)
        {
            SelectOptions = querySpecification.selectOption();
            SelectItemList = querySpecification.selectItemList();
            IntoClause = querySpecification.intoClause();
            Source = querySpecification.fromClause();
            WhereClause = querySpecification.whereClause();
            GroupByClause = querySpecification.groupByClause();
            HavingClause = querySpecification.havingClause();
            WindowClause = querySpecification.windowClause();

            WithClause = null;
            OrderClause = null;
            LimitClause = null;
            ProcedureAnalyseClause = null;
            LockingClauseList = null;

            Start = querySpecification.Start;
            Stop = querySpecification.Stop;
        }
    }
}
