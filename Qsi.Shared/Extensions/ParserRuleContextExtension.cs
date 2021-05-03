using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace Qsi.Shared.Extensions
{
    internal static class ParserRuleContextExtension
    {
        public static string GetInputText(this ParserRuleContext context)
        {
            var startIndex = context.Start.StartIndex;
            var stopIndex = context.Stop.StopIndex;
            var interval = new Interval(startIndex, stopIndex);
            return context.Start.InputStream.GetText(interval);
        }

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

        public static int GetTokenIndex(this ParserRuleContext context, int type)
        {
            for (int i = 0; i < context.ChildCount; i++)
            {
                if (context.children[i] is ITerminalNode t && t.Symbol.Type == type)
                {
                    return i;
                }
            }

            return -1;
        }

        public static bool TryGetTokenIndex(this ParserRuleContext context, int type, out int index)
        {
            index = GetTokenIndex(context, type);
            return index >= 0;
        }
    }
}
