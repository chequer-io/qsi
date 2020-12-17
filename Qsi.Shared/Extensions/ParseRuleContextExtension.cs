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
    }
}
