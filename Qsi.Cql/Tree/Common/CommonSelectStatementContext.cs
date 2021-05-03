using Antlr4.Runtime;
using Qsi.Cql.Internal;
using Qsi.Shared;

namespace Qsi.Cql.Tree.Common
{
    internal readonly struct CommonSelectStatementContext : IParserRuleContext
    {
        public IToken Start { get; }

        public IToken Stop { get; }

        public bool IsJson { get; }

        public bool IsDistinct { get; }

        public bool AllowFiltering { get; }

        public CqlParserInternal.SelectorsContext Selectors { get; }

        public CqlParserInternal.ColumnFamilyNameContext FromSource { get; }

        public IToken WhereStart { get; }

        public CqlParserInternal.WhereClauseContext WhereClause { get; }

        public IToken GroupByStart { get; }

        public CqlParserInternal.GroupByClauseContext[] GroupByClauses { get; }

        public IToken OrderByStart { get; }

        public CqlParserInternal.OrderByClauseContext[] OrderByClauses { get; }

        public CqlParserInternal.IntValueContext PerLimit { get; }

        public IToken LimitStart { get; }

        public CqlParserInternal.IntValueContext Limit { get; }

        public CommonSelectStatementContext(CqlParserInternal.SelectStatementContext context)
        {
            Start = context.Start;
            Stop = context.Stop;
            IsJson = context.json;
            IsDistinct = context.distinct;
            AllowFiltering = context.allowFiltering;
            Selectors = context.selectors();
            FromSource = context.columnFamilyName();
            WhereStart = context.w;
            WhereClause = context.whereClause();
            GroupByStart = context.g;
            GroupByClauses = context.groupByClause();
            OrderByStart = context.o;
            OrderByClauses = context.orderByClause();
            PerLimit = context.perLimit;
            LimitStart = context.l;
            Limit = context.limit;
        }

        public CommonSelectStatementContext(CqlParserInternal.CreateMaterializedViewStatementContext context)
        {
            Start = context.K_SELECT().Symbol;
            Stop = context.wclause?.Stop ?? context.basecf.Stop;
            IsJson = false;
            IsDistinct = false;
            AllowFiltering = false;
            Selectors = context.sclause;
            FromSource = context.basecf;
            WhereStart = context.w;
            WhereClause = context.wclause;
            GroupByStart = null;
            GroupByClauses = null;
            OrderByStart = null;
            OrderByClauses = null;
            PerLimit = null;
            LimitStart = null;
            Limit = null;
        }
    }
}
