using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Qsi.Analyzers.Table;
using Qsi.Analyzers.Table.Context;
using Qsi.Data;
using Qsi.SqlServer.Data;
using Qsi.SqlServer.Tree;
using Qsi.Tree;

namespace Qsi.SqlServer.Analyzers
{
    public class SqlServerTableAnalyzer : QsiTableAnalyzer
    {
        public SqlServerTableAnalyzer(QsiEngine engine) : base(engine)
        {
        }

        public override async ValueTask<QsiTableStructure> BuildTableStructure(TableCompileContext context, IQsiTableNode table)
        {
            switch (table)
            {
                case ISqlServerBinaryTableNode binaryTableNode:
                    return await BuildBinaryTableStructure(context, binaryTableNode);
            }

            return await base.BuildTableStructure(context, table);
        }

        private async Task<QsiTableStructure> BuildBinaryTableStructure(TableCompileContext context, ISqlServerBinaryTableNode table)
        {
            if (table.Left == null || table.Right == null)
                throw new QsiException(QsiError.Syntax);

            var binaryTable = new QsiTableStructure
            {
                Type = table.BinaryTableType switch
                {
                    SqlServerBinaryTableType.Except => QsiTableType.Except,
                    SqlServerBinaryTableType.Intersect => QsiTableType.Intersect,
                    SqlServerBinaryTableType.Union => QsiTableType.Union,
                    _ => throw new NotSupportedException()
                }
            };

            QsiTableStructure left;
            QsiTableStructure right;

            if (table.Left is IQsiJoinedTableNode leftNode)
            {
                left = await BuildJoinedTableStructure(context, leftNode);
            }
            else
            {
                using var leftContext = new TableCompileContext(context);
                left = await BuildTableStructure(leftContext, table.Left);
                context.SourceTables.Add(left);
            }

            if (table.Right is IQsiJoinedTableNode rightNode)
            {
                right = await BuildJoinedTableStructure(context, rightNode);
            }
            else
            {
                using var rightContext = new TableCompileContext(context);
                right = await BuildTableStructure(rightContext, table.Right);
                context.SourceTables.Add(right);
            }

            binaryTable.References.Add(left);
            binaryTable.References.Add(right);

            HashSet<QsiTableColumn> leftColumns = left.Columns.ToHashSet();

            foreach (var leftColumn in leftColumns)
            {
                var column = binaryTable.NewColumn();
                column.Name = leftColumn.Name;
                column.References.Add(leftColumn);
            }

            return binaryTable;
        }
    }
}
