using Qsi.Shared.Extensions;
using Qsi.Tree;
using Qsi.Utilities;
using static Qsi.Hana.Internal.HanaParserInternal;

namespace Qsi.Hana.Tree.Visitors
{
    internal static class DefinitionVisitor
    {
        public static IQsiDefinitionNode VisitCreateViewStatement(CreateViewStatementContext context)
        {
            var node = new HanaViewDefinitionNode
            {
                Identifier = context.name,
                Comment = context.comment,
                StructuredPrivilegeCheck = context.structuredPrivilegeCheck,
                Force = context.force,
                CheckOption = context.checkOption,
                DdlOnly = context.ddlOnly,
                ReadOnly = context.readOnly
            };

            if (context.TryGetRuleContext<ColumnListClauseContext>(out var columnList))
                node.Columns.SetValue(TableVisitor.VisitColumnListClause(columnList, null));

            if (context.TryGetRuleContext<ParameterizedViewClauseContext>(out var parameterizedViewClause))
                node.Parameters.SetValue(TreeHelper.Fragment(parameterizedViewClause.GetInputText()));

            node.Source.SetValue(TableVisitor.VisitSubquery(context.subquery()));

            if (context.TryGetRuleContext<WithAssociationClauseContext>(out var withAssociationClause))
                node.Associations.SetValue(TreeHelper.Fragment(withAssociationClause.GetInputText()));

            if (context.TryGetRuleContext<WithMaskClauseContext>(out var withMaskClause))
                node.Masks.SetValue(TreeHelper.Fragment(withMaskClause.GetInputText()));

            if (context.TryGetRuleContext<WithExpressionMacroClauseContext>(out var withExpressionMacroClause))
                node.ExpressionMacros.SetValue(TreeHelper.Fragment(withExpressionMacroClause.GetInputText()));

            if (context.TryGetRuleContext<WithAnnotationClauseContext>(out var withAnnotationClause))
                node.Annotation.SetValue(TreeHelper.Fragment(withAnnotationClause.GetInputText()));

            if (context.TryGetRuleContext<WithCacheClauseContext>(out var withCacheClause))
                node.Cache.SetValue(TreeHelper.Fragment(withCacheClause.GetInputText()));

            if (context.TryGetRuleContext<WithAnonymizationClauseContext>(out var withAnonymizationClause))
                node.Anonymization.SetValue(TreeHelper.Fragment(withAnonymizationClause.GetInputText()));

            HanaTree.PutContextSpan(node, context);

            return node;
        }
    }
}
