using System.Collections.Generic;
using System.Linq;
using Qsi.Oracle.Internal;
using Qsi.Shared.Extensions;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Oracle.Tree.Visitors
{
    using static OracleParserInternal;

    internal static class ActionVisitor
    {
        public static IQsiTreeNode VisitCreate(CreateContext context)
        {
            switch (context.children[0])
            {
                case CreateViewContext createViewContext:
                    return VisitCreateView(createViewContext);

                default:
                    throw TreeHelper.NotSupportedTree(context.children[0]);
            }
        }

        public static IQsiTreeNode VisitDelete(DeleteContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleDataDeleteActionNode>(context);

            if (context.hint() is not null)
                node.Hint = context.hint().GetInputText();

            var targetNode = TableVisitor.VisitDmlTableExpressionClause(context.dmlTableExpressionClause());

            if (targetNode is not QsiTableReferenceNode referenceNode)
                throw TreeHelper.NotSupportedFeature("Expression Target in Update");

            if (context.whereClause() is not null)
            {
                var derivedTableNode = new OracleDerivedTableNode();

                derivedTableNode.Columns.Value = TreeHelper.CreateAllColumnsDeclaration();
                derivedTableNode.Source.Value = referenceNode;
                derivedTableNode.Where.Value = ExpressionVisitor.VisitWhereClause(context.whereClause());

                node.Target.Value = derivedTableNode;
            }
            else
            {
                node.Target.Value = referenceNode;
            }

            if (node.Target.Value is IOracleTableNode oracleTableNode)
                oracleTableNode.IsOnly = context.HasToken(ONLY);

            // tAlias ignored
            // returningClause, errorLoggingClause ignored

            return node;
        }

        public static IQsiTreeNode VisitUpdate(UpdateContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleDataUpdateActionNode>(context);

            if (context.hint() is not null)
                node.Hint = context.hint().GetInputText();

            var targetNode = TableVisitor.VisitDmlTableExpressionClause(context.dmlTableExpressionClause());

            if (targetNode is not QsiTableReferenceNode referenceNode)
                throw TreeHelper.NotSupportedFeature("Expression Target in Update");

            if (context.whereClause() is not null)
            {
                var derivedTableNode = new OracleDerivedTableNode();

                derivedTableNode.Columns.Value = TreeHelper.CreateAllColumnsDeclaration();
                derivedTableNode.Source.Value = referenceNode;
                derivedTableNode.Where.Value = ExpressionVisitor.VisitWhereClause(context.whereClause());

                node.Target.Value = derivedTableNode;
            }
            else
            {
                node.Target.Value = referenceNode;
            }

            if (node.Target.Value is IOracleTableNode oracleTableNode)
                oracleTableNode.IsOnly = context.HasToken(ONLY);

            node.SetValueExpressions.AddRange(ExpressionVisitor.VisitUpdateSetClause(context.updateSetClause()));

            // tAlias ignored
            // returningClause, errorLoggingClause ignored

            return node;
        }

        public static IQsiTreeNode VisitCreateView(CreateViewContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleViewDefinitionNode>(context);

            if (context.schema() is not null)
            {
                node.Identifier = IdentifierVisitor.CreateQualifiedIdentifier(
                    context.schema().identifier(),
                    context.view().identifier()
                );
            }
            else
            {
                node.Identifier = IdentifierVisitor.CreateQualifiedIdentifier(
                    context.view().identifier()
                );
            }

            node.Source.Value = TableVisitor.VisitSubquery(context.subquery());

            node.Replace = context.HasToken(REPLACE);
            node.Force = context.HasToken(FORCE) && !context.HasToken(NO);

            if (context.createViewEditionOption() is not null)
                node.EditionOption.Value = TreeHelper.Fragment(context.createViewEditionOption().GetInputText());

            if (context.createViewSharingOption() is not null)
                node.SharingOption.Value = TreeHelper.Fragment(context.createViewEditionOption().GetInputText());

            if (context.subqueryRestrictionClause() is not null)
                node.SubqueryRestriction.Value = TreeHelper.Fragment(context.subqueryRestrictionClause().GetInputText());

            if (context.createViewBequeathOption() is not null)
                node.Bequeath.Value = TreeHelper.Fragment(context.createViewEditionOption().GetInputText());

            if (context.containerOption is not null)
                node.ContainerOption.Value = TreeHelper.Fragment(context.containerOption.Text);

            if (context.defaultCollationOption() is not null)
                node.DefaultCollationName = context.defaultCollationOption().collationName().GetText();

            return node;
        }
    }
}
