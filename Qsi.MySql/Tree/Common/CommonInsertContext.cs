using Antlr4.Runtime;
using Qsi.Data;
using Qsi.Shared;
using Qsi.Shared.Extensions;
using static Qsi.MySql.Internal.MySqlParserInternal;

namespace Qsi.MySql.Tree.Common
{
    internal readonly struct CommonInsertContext : IParserRuleContext
    {
        public IToken Start { get; }

        public IToken Stop { get; }

        public QsiDataConflictBehavior ConflictBehavior { get; }

        public TableRefContext TableRef { get; }

        public UsePartitionContext UsePartition { get; }

        public InsertFromConstructorContext InsertFromConstructor { get; }

        public UpdateListContext UpdateList { get; }

        public InsertQueryExpressionContext InsertQueryExpression { get; }

        public ValuesReferenceContext ValuesReference { get; }

        public InsertUpdateListContext InsertUpdateList { get; }

        public CommonInsertContext(InsertStatementContext context)
        {
            Start = context.Start;
            Stop = context.Stop;
            TableRef = context.tableRef();
            UsePartition = context.usePartition();
            InsertFromConstructor = context.insertFromConstructor();
            UpdateList = context.updateList();
            InsertQueryExpression = context.insertQueryExpression();
            ValuesReference = context.valuesReference();
            InsertUpdateList = context.insertUpdateList();

            if (context.HasToken(IGNORE_SYMBOL))
                ConflictBehavior = QsiDataConflictBehavior.Ignore;
            else if (InsertUpdateList != null)
                ConflictBehavior = QsiDataConflictBehavior.Update;
            else
                ConflictBehavior = QsiDataConflictBehavior.None;
        }

        public CommonInsertContext(ReplaceStatementContext context)
        {
            Start = context.Start;
            Stop = context.Stop;
            TableRef = context.tableRef();
            UsePartition = context.usePartition();
            InsertFromConstructor = context.insertFromConstructor();
            UpdateList = context.updateList();
            InsertQueryExpression = context.insertQueryExpression();
            ValuesReference = null;
            InsertUpdateList = null;
            ConflictBehavior = QsiDataConflictBehavior.Update;
        }
    }
}
