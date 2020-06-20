namespace Qsi.Data
{
    public sealed class QsiScript
    {
        public string Script { get; }

        public QsiScriptType ScriptType { get; }

        public QsiScript(in string script, QsiScriptType type)
        {
            Script = script;
            ScriptType = type;
        }
    }
}
