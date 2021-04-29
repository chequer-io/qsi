using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

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
    }
}
