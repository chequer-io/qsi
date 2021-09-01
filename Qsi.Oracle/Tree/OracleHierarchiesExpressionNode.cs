using System.Collections.Generic;
using System.Linq;
using Qsi.Data;
using Qsi.Tree;

namespace Qsi.Oracle.Tree
{
    public class OracleHierarchiesExpressionNode : QsiExpressionNode
    {
        public List<QsiQualifiedIdentifier> Identifiers { get; } = new();

        public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();
    }
}
