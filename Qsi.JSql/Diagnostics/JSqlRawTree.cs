using System.Collections.Generic;
using System.Linq;
using ikvm.extensions;
using net.sf.jsqlparser.parser;
using Qsi.Diagnostics;
using Qsi.JSql.Extensions;

namespace Qsi.JSql.Diagnostics
{
    internal sealed class JSqlRawTree : IRawTree
    {
        public string DisplayName { get; }

        public IRawTree[] Children { get; }

        public JSqlRawTree(Node node)
        {
            DisplayName = node.toString();

            if (node.jjtGetNumChildren() == 0)
            {
                if (node is SimpleNode simpleNode)
                {
                    Children = new IRawTree[]
                    {
                        new JSqlRawTreeTerminalNode(simpleNode.jjtGetFirstToken())
                    };
                }
            }
            else
            {
                Children = node.GetChildren()
                    .Select(c => (IRawTree)new JSqlRawTree(c))
                    .ToArray();
            }
        }
    }
}
