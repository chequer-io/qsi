using PgQuery;
using Qsi.PostgreSql.Data;
using Qsi.PostgreSql.NewTree.Nodes;
using Qsi.Tree;
using Qsi.Tree.Definition;

namespace Qsi.PostgreSql.Tree.PG10.Nodes
{
    public class PgViewDefinitionNode : QsiViewDefinitionNode
    {
        public string? CheckOptionOld { get; set; }

        public ViewCheckOption CheckOption { get; set; }

        public Relpersistence Relpersistence { get; set; }

        public QsiTreeNodeList<PgDefinitionElementNode?> Options { get; }

        public PgViewDefinitionNode()
        {
            Options = new QsiTreeNodeList<PgDefinitionElementNode?>(this);
        }
    }
}
