using System.Collections.Generic;
using System.Linq;
using Qsi.Tree;

namespace Qsi.Trino.Tree
{
    public class TrinoDateTimeFunctionExpressionNode : QsiExpressionNode
    {
        public string Name { get; set; }
        
        public long Precision { get; set; }
        
        public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>(); 
    }
}
