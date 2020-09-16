using System;
using System.Threading.Tasks;
using Qsi.Compiler;
using Qsi.Data;
using Qsi.Runtime.Internal;
using Qsi.Services;
using Qsi.Tree;

namespace Qsi.Oracle.Compiler
{
    public sealed class OracleTableCompiler : QsiTableCompiler
    {
        private QsiDataTable _pseudoTable;

        public OracleTableCompiler(IQsiLanguageService languageService) : base(languageService)
        {
        }

        protected override ValueTask<QsiDataTable> BuildDerivedTableStructure(CompileContext context, IQsiDerivedTableNode table)
        {
            if (table.Source == null)
                throw new QsiException(QsiError.NoFromClause);

            return base.BuildDerivedTableStructure(context, table);
        }

        protected override QsiDataColumn ResolveDeclaredColumn(CompileContext context, IQsiDeclaredColumnNode columnn)
        {
            try
            {
                return base.ResolveDeclaredColumn(context, columnn);
            }
            catch (QsiException e) when (e.Error == QsiError.UnknownColumn || e.Error == QsiError.UnknownColumnIn)
            {
                if (OraclePseudoColumn.Contains(columnn.Name[0].Value))
                {
                    _pseudoTable ??= CreatePseudoTable();
                    return _pseudoTable.Columns[0];
                }

                throw;
            }
        }

        private QsiDataTable CreatePseudoTable()
        {
            var table = new QsiDataTable
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
