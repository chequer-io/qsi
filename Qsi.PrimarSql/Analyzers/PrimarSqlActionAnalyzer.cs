using System.Threading.Tasks;
using Qsi.Analyzers;
using Qsi.Analyzers.Action;
using Qsi.Analyzers.Action.Models;
using Qsi.Analyzers.Context;
using Qsi.Analyzers.Table;
using Qsi.Analyzers.Table.Context;
using Qsi.Data;
using Qsi.PrimarSql.Tree;
using Qsi.Tree;

namespace Qsi.PrimarSql.Analyzers
{
    public class PrimarSqlActionAnalyzer : QsiActionAnalyzer
    {
        public PrimarSqlActionAnalyzer(QsiEngine engine) : base(engine)
        {
        }

        #region Delete
        protected override async ValueTask<IQsiAnalysisResult> ExecuteDataDeleteAction(IAnalyzerContext context, IQsiDataDeleteActionNode action)
        {
            var tableAnalyzer = context.Engine.GetAnalyzer<QsiTableAnalyzer>();
            QsiTableStructure table;

            using (var tableContext = new TableCompileContext(context))
            {
                table = (await tableAnalyzer.BuildTableStructure(tableContext, action.Target)).References[0];
            }

            var commonTableNode = ReassembleCommonTableNode(action.Target);
            var dataTable = await GetDataTableByCommonTableNode(context, commonTableNode);

            var documentColumn = table.NewColumn();
            documentColumn.Name = new QsiIdentifier("Document", false);

            var documentColumnPivot = new DataManipulationTargetColumnPivot(0, documentColumn, 0, documentColumn);
            var target = new DataManipulationTarget(table, new[] { documentColumnPivot });

            foreach (var row in dataTable.Rows)
            {
                var targetRow = target.DeleteRows.NewRow();
                targetRow.Items[0] = row.Items[0];
            }

            var dataAction = new QsiDataAction
            {
                Table = target.Table,
                DeleteRows = target.DeleteRows.Count == 0 ? null : target.DeleteRows
            };

            return new QsiActionAnalysisResult(new QsiActionSet<QsiDataAction>(new[] { dataAction }));
        }
        #endregion

        protected override IQsiTableNode ReassembleCommonTableNode(IQsiTableNode node)
        {
            if (node is PrimarSqlDerivedTableNode primarSqlNode)
            {
                var ctn = new PrimarSqlDerivedTableNode
                {
                    Parent = primarSqlNode.Parent,
                    SelectSpec = primarSqlNode.SelectSpec
                };

                if (!primarSqlNode.Columns.IsEmpty)
                    ctn.Columns.SetValue(primarSqlNode.Columns.Value);

                if (!primarSqlNode.Source.IsEmpty)
                    ctn.Source.SetValue(primarSqlNode.Source.Value);

                if (!primarSqlNode.Where.IsEmpty)
                    ctn.Where.SetValue(primarSqlNode.Where.Value);

                if (!primarSqlNode.Order.IsEmpty)
                    ctn.Order.SetValue(primarSqlNode.Order.Value);

                if (!primarSqlNode.Limit.IsEmpty)
                    ctn.Limit.SetValue(primarSqlNode.Limit.Value);

                if (!primarSqlNode.StartKey.IsEmpty)
                    ctn.StartKey.SetValue(primarSqlNode.StartKey.Value);

                return ctn;
            }

            return base.ReassembleCommonTableNode(node);
        }
    }
}
