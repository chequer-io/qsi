using Qsi.Tree;
using static Qsi.Cql.Internal.CqlParserInternal;

namespace Qsi.Cql.Tree
{
    internal static class ActionVisitor
    {
        #region InsertStatement
        public static QsiActionNode VisitInsertStatement(InsertStatementContext context)
        {
            var node = new CqlDataInsertActionNode();

            if (context.st1 != null)
            {
                
            }
            else
            {
                var jsonValue = context.st2.jsonValue();

                if (jsonValue.s == null)
                    throw new QsiException(QsiError.NotSupportedFeature, "BindParameter");

                node.Json = jsonValue.s.raw;
                node.DefaultValue = context.st2.defaultValue;
                node.IfNotExists = context.st2.ifNotExists;
                // TODO: usingClause
            }

            CqlTree.PutContextSpan(node, context);

            return node;
        }
        #endregion

        #region UpdateStatement
        public static QsiActionNode VisitUpdateStatement(UpdateStatementContext context)
        {
            throw new System.NotImplementedException();
        }
        #endregion

        #region DeleteStatement
        public static QsiActionNode VisitDeleteStatement(DeleteStatementContext context)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
