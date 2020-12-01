using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Qsi.Analyzers;
using Qsi.Analyzers.Context;
using Qsi.Analyzers.Table;
using Qsi.Analyzers.Table.Context;
using Qsi.Data;
using Qsi.Tree;

namespace Qsi.PrimarSql.Analyzers
{
    public class PrimarSqlTableAnalyzer : QsiTableAnalyzer
    {
        private IEnumerable<QsiIdentifier> _identifiers = Enumerable.Empty<QsiIdentifier>();
        
        public PrimarSqlTableAnalyzer(QsiEngine engine) : base(engine)
        {
        }
        
        protected override async ValueTask<IQsiAnalysisResult> OnExecute(IAnalyzerContext context)
        {
            var set = new HashSet<QsiIdentifier>(IdentifierComparer);
            QsiQualifiedIdentifier tableIdentifier = null;
            QsiTableNode node = (context.Tree as QsiDerivedTableNode);
            
            _identifiers = Enumerable.Empty<QsiIdentifier>();
            
            while (node is QsiDerivedTableNode tableNode)
            {
                IEnumerable<QsiQualifiedIdentifier> columns = tableNode
                    .Columns.Value
                    .OfType<QsiDeclaredColumnNode>()
                    .Select(c => c.Name);

                foreach (var column in columns)
                {
                    set.Add(column[^1]);
                }

                if (tableNode.Source.Value is QsiTableAccessNode tableAccessNode)
                    tableIdentifier = tableAccessNode.Identifier;
                
                node = tableNode.Source.Value;
            }

            if (set.Count == 0 && tableIdentifier != null)
            {
                var jsonTable = new QsiTableStructure
                {
                    Identifier = tableIdentifier,
                    Type = QsiTableType.Table,
                    IsSystem = false
                };

                var documentColumn = jsonTable.NewColumn();
                documentColumn.Name = new QsiIdentifier("Document", false);

                return new PrimarSqlJsonTableAnalysisResult(jsonTable);
            }
            
            _identifiers = set;
            
            return await base.OnExecute(context);
        }

        protected override async ValueTask<QsiTableStructure> BuildTableAccessStructure(TableCompileContext context, IQsiTableAccessNode table)
        {
            var tableStructure = await base.BuildTableAccessStructure(context, table);

            foreach (var identifier in _identifiers)
            {
                var c = tableStructure.NewColumn();
                c.Name = identifier;
            }

            return tableStructure;
        }
    }
}
