using System.Collections.Generic;
using System.Threading.Tasks;
using Qsi.Analyzers.Table;
using Qsi.Analyzers.Table.Context;
using Qsi.Data;
using Qsi.Engines;
using Qsi.Oracle.Internal;
using Qsi.Oracle.Tree;
using Qsi.Tree;

namespace Qsi.Oracle.Analyzers
{
    public sealed class OracleTableAnalyzer : QsiTableAnalyzer
    {
        // TODO: Build Sequence: => seq.NEXTVAL, seq.CURRVAL

        public OracleTableAnalyzer(QsiEngine engine) : base(engine)
        {
        }

        protected override ValueTask<QsiTableStructure> BuildDerivedTableStructure(TableCompileContext context, IQsiDerivedTableNode table)
        {
            if (table.Source == null)
                throw new QsiException(QsiError.NoFromClause);

            return base.BuildDerivedTableStructure(context, table);
        }

        protected override IEnumerable<QsiTableColumn> ResolveColumnsInExpression(TableCompileContext context, IQsiExpressionNode expression)
        {
            switch (expression)
            {
                case OracleAggregateFunctionExpressionNode e:
                    return base.ResolveColumnsInExpression(context, e.Function.Value);

                case OracleAnalyticFunctionExpressionNode e:
                    return base.ResolveColumnsInExpression(context, e.Function.Value);

                default:
                    return base.ResolveColumnsInExpression(context, expression);
            }
        }

        protected override QsiTableColumn ResolveColumnReference(TableCompileContext context, IQsiColumnReferenceNode column)
        {
            try
            {
                return base.ResolveColumnReference(context, column);
            }
            catch (QsiException e) when (e.Error is QsiError.UnknownColumn or QsiError.UnknownColumnIn)
            {
                if (OraclePseudoColumn.TryGetColumn(column.Name[0].Value, out var tableColumn))
                    return tableColumn;

                throw;
            }
        }
    }
}
