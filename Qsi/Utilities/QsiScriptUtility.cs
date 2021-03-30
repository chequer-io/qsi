using Qsi.Data;

namespace Qsi.Utilities
{
    internal static class QsiScriptUtility
    {
        public static int GetEndLine(in string script)
        {
            int count = 1;
            int index = -1;

            while ((index = script.IndexOf('\n', index + 1)) >= 0)
                count++;

            return count;
        }

        public static int GetEndColumn(in string script)
        {
            int index = script.LastIndexOf('\n');

            if (index == -1)
                index = script.Length - 1;

            return script.Length - (index + 1);
        }

        public static QsiScriptPosition GetEndPosition(in string script)
        {
            return new(GetEndLine(script), GetEndColumn(script), script.Length - 1);
        }
    }
}
