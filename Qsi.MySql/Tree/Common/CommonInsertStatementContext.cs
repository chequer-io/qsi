using System.Collections.Generic;
using Antlr4.Runtime;
using Qsi.Data;
using Qsi.Utilities;
using static Qsi.MySql.Internal.MySqlParser;

namespace Qsi.MySql.Tree.Common
{
    internal readonly struct CommonInsertStatementContext
    {
        public ParserRuleContext Context { get; }

        public QsiDataConflictBehavior ConflictBehavior { get; }

        public TableNameContext TableName { get; }

        public UidContext[] Partitions { get; }

        public UidContext[] Columns { get; }

        public InsertStatementValueContext InsertStatementValue { get; }

        public UpdatedElementContext[] SetElements { get; }

        public UpdatedElementContext[] DuplicateSetElements { get; }

        public CommonInsertStatementContext(InsertStatementContext context)
        {
            Context = context;
            TableName = context.tableName();
            Partitions = context.partitions?.uid();
            Columns = context.columns?.uid();
            InsertStatementValue = context.insertStatementValue();
            SetElements = Combine(context.setFirst, context._setElements);
            DuplicateSetElements = Combine(context.duplicatedFirst, context._duplicatedElements);

            if (context.IGNORE() != null)
            {
                ConflictBehavior = QsiDataConflictBehavior.Ignore;
            }
            else if (!ListUtility.IsNullOrEmpty(DuplicateSetElements))
            {
                ConflictBehavior = QsiDataConflictBehavior.Update;
            }
            else
            {
                ConflictBehavior = QsiDataConflictBehavior.None;
            }
        }

        public CommonInsertStatementContext(ReplaceStatementContext context)
        {
            Context = context;
            TableName = context.tableName();
            ConflictBehavior = QsiDataConflictBehavior.Update;
            Partitions = context.partitions?.uid();
            Columns = context.columns?.uid();
            InsertStatementValue = context.insertStatementValue();
            SetElements = Combine(context.setFirst, context._setElements);
            DuplicateSetElements = null;
        }

        private static UpdatedElementContext[] Combine(UpdatedElementContext first, IList<UpdatedElementContext> elements)
        {
            if (first == null)
                return null;

            var buffer = new UpdatedElementContext[elements.Count + 1];
            buffer[0] = first;
            elements.CopyTo(buffer, 1);

            return buffer;
        }
    }
}
