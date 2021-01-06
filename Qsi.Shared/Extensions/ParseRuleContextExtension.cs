using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace Qsi.Shared.Extensions
{
    internal static class ParseRuleContextExtension
    {
        public static bool HasToken(this ParserRuleContext context, int type)
        {
            return context.children
                .OfType<ITerminalNode>()
                .Any(n => n.Symbol.Type == type);
        }

        public static bool TryGetToken(this ParserRuleContext context, int type, out IToken token)
        {
            foreach (var terminalNode in context.children.OfType<ITerminalNode>())
            {
                if (terminalNode.Symbol.Type != type)
                    continue;

                token = terminalNode.Symbol;
                return true;
            }

            token = null;
            return false;
        }
    }
}
