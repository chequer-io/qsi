namespace Qsi.Data
{
    public sealed class QsiScriptPosition
    {
        public static QsiScriptPosition Start { get; } = new(0, 0);

        public int Line { get; }

        public int Column { get; }

        public QsiScriptPosition(int line, int column)
        {
            Line = line;
            Column = column;
        }

        public override string ToString()
        {
            return $"{Line}:{Column}";
        }
    }
}
