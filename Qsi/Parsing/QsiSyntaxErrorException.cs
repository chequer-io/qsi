namespace Qsi.Parsing;

public sealed class QsiSyntaxErrorException : QsiException
{
    public int Line { get; }

    public int Column { get; }

    public QsiSyntaxErrorException(int line, int column, string message) : base(QsiError.SyntaxError, message)
    {
        Line = line;
        Column = column;
    }
}