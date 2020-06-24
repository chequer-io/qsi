using Qsi.Utilities;

namespace Qsi.Data
{
    public sealed class QsiScript
    {
        public string Script { get; }

        public QsiScriptType ScriptType { get; }

        public QsiScriptPosition Start { get; }

        public QsiScriptPosition End { get; }

        public QsiScript(in string script, QsiScriptType type) : 
            this(script, type, QsiScriptPosition.Start, QsiScriptUtility.GetEndPosition(script))
        {
        }

        public QsiScript(in string script, QsiScriptType type, QsiScriptPosition start, QsiScriptPosition end)
        {
            Script = script;
            ScriptType = type;
            Start = start;
            End = end;
        }
    }
}
