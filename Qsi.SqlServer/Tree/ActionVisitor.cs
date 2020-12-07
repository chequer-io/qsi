using Microsoft.SqlServer.TransactSql.ScriptDom;
using Qsi.Data;
using Qsi.Tree;

namespace Qsi.SqlServer.Tree
{
    internal sealed class ActionVisitor : VisitorBase
    {
        public ActionVisitor(IVisitorContext visitorContext) : base(visitorContext)
        {
        }

        public QsiChangeSearchPathActionNode VisitUseStatement(UseStatement useStatement)
        {
            return new QsiChangeSearchPathActionNode
            {
                Identifiers = new[]
                {
                    IdentifierVisitor.CreateIdentifier(useStatement.DatabaseName), 
                }
            };
        }
    }
}
