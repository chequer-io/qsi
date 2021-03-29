namespace Qsi.Data
{
    public sealed class QsiScriptPosition
    {
        public static QsiScriptPosition Start { get; } = new(0, 0, 0);

        public int Line { get; }

        public int Column { get; }

        public int Index { get; }

        public QsiScriptPosition(int line, int column, int index)
        {
            Line = line;
            Column = column;
            Index = index;
        }

        public override string ToString()
        {
            return $"{Line}:{Column}";
        }
    }
}
