using Qsi.Shared.Extensions;
using Qsi.Tree;
using Qsi.Utilities;
using static Qsi.MySql.Internal.MySqlParserInternal;

namespace Qsi.MySql.Tree
{
    internal static class DefinitionVisitor
    {
        public static IQsiDefinitionNode VisitCreateStatement(CreateStatementContext context)
        {
            IQsiDefinitionNode node;

            switch (context.children[1])
            {
                case CreateViewContext createViewContext:
                    node = VisitCreateViewStatement(createViewContext);
                    break;

                default:
                    throw TreeHelper.NotSupportedTree(context.children[1]);
            }

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static IQsiDefinitionNode VisitCreateViewStatement(CreateViewContext context)
        {
            if (!context.TryGetTokenIndex(VIEW_SYMBOL, out var index))
                throw new QsiException(QsiError.Syntax);

            var viewName = (ViewNameContext)context.children[index + 1];
            var viewTail = (ViewTailContext)context.children[index + 2];
            var columnInternalRefList = viewTail.columnInternalRefList();

            var node = new MySqlViewDefinitionNode
            {
                Identifier = IdentifierVisitor.VisitViewName(viewName),
            };

            if (context.viewReplaceOrAlgorithm() != null)
            {
                var replaceOrAlgorithm = context.viewReplaceOrAlgorithm();

                node.Replace = replaceOrAlgorithm.HasToken(REPLACE_SYMBOL);

                if (replaceOrAlgorithm.viewAlgorithm() != null)
                    node.ViewAlgorithm.SetValue(TreeHelper.Fragment(replaceOrAlgorithm.viewAlgorithm().GetInputText()));
            }

            if (context.definerClause() != null)
                node.Definer = context.definerClause().user().GetInputText();

            if (context.viewSuid() != null)
                node.ViewSuid.SetValue(TreeHelper.Fragment(context.viewSuid().GetInputText()));

            node.Columns.SetValue(columnInternalRefList == null ?
                TreeHelper.CreateAllVisibleColumnsDeclaration() :
                TableVisitor.CreateSequentialColumns(columnInternalRefList));

            node.Source.SetValue(TableVisitor.VisitViewSelect(viewTail.viewSelect()));

            return node;
        }
    }
}
