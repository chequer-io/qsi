using System.Threading.Tasks;
using Qsi.Analyzers.Table;
using Qsi.Analyzers.Table.Context;
using Qsi.Data;
using Qsi.Tree;

namespace Qsi.Oracle
{
    public sealed class OracleTableAnalyzer : QsiTableAnalyzer
    {
        private QsiTableStructure _pseudoTable;

        public OracleTableAnalyzer(QsiEngine engine) : base(engine)
        {
        }

        protected override ValueTask<QsiTableStructure> BuildDerivedTableStructure(TableCompileContext context, IQsiDerivedTableNode table)
        {
            if (table.Source == null)
                throw new QsiException(QsiError.NoFromClause);

            return base.BuildDerivedTableStructure(context, table);
        }

        protected override QsiTableColumn ResolveDeclaredColumn(TableCompileContext context, IQsiDeclaredColumnNode column)
        {
            try
            {
                return base.ResolveDeclaredColumn(context, column);
            }
            catch (QsiException e) when (e.Error == QsiError.UnknownColumn || e.Error == QsiError.UnknownColumnIn)
            {
                if (OraclePseudoColumn.Contains(column.Name[0].Value))
                {
                    _pseudoTable ??= CreatePseudoTable();
                    return _pseudoTable.Columns[0];
                }

                throw;
            }
        }

        private QsiTableStructure CreatePseudoTable()
        {
            var table = new QsiTableStructure
            {
                Identifier = new QsiQualifiedIdentifier(new QsiIdentifier("PseudoColumns", false))
            };

            foreach (var name in OraclePseudoColumn.Names)
            {
                var c = table.NewColumn();
                c.Name = new QsiIdentifier(name, false);
            }

            return table;
        }
    }
}
