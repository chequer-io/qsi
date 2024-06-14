using System;
using Antlr4.Runtime;
using Qsi.Shared;
using static Qsi.SingleStore.Internal.SingleStoreParserInternal;

namespace Qsi.SingleStore.Tree.Common;

internal readonly struct CommonQueryContext : IParserRuleContext
{
    public IToken Start { get; }

    public IToken Stop { get; }

    public WithClauseContext WithClause { get; }

    public QueryExpressionBodyContext QueryExpressionBody { get; }

    public QueryExpressionParensContext QueryExpressionParens { get; }

    public OrderClauseContext OrderClause { get; }

    public LimitClauseContext LimitClause { get; }

    public ProcedureAnalyseClauseContext ProcedureAnalyseClause { get; }

    public LockingClauseListContext LockingClauseList { get; }

    public CommonQueryContext(QueryExpressionContext queryExpression, LockingClauseListContext lockingClauseList)
    {
        if (queryExpression == null)
            throw new ArgumentNullException(nameof(queryExpression));

        WithClause = queryExpression.withClause();
        QueryExpressionBody = queryExpression.queryExpressionBody();
        QueryExpressionParens = queryExpression.queryExpressionParens();
        OrderClause = queryExpression.orderClause();
        LimitClause = queryExpression.limitClause();
        ProcedureAnalyseClause = queryExpression.procedureAnalyseClause();
        LockingClauseList = lockingClauseList;

        Start = queryExpression.Start;
        Stop = lockingClauseList?.Stop ?? queryExpression.Stop;
    }
}
