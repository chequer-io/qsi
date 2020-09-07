using System.Collections.Generic;
using net.sf.jsqlparser.parser;

namespace Qsi.JSql.Extensions
{
    public static class NodeExtension
    {
        public static IEnumerable<Node> GetChildren(this Node node)
        {
            var childCount = node.jjtGetNumChildren();

            for (int i = 0; i < childCount; i++)
            {
                yield return node.jjtGetChild(i);
            }
        }
    }
}
