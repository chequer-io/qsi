﻿namespace Qsi.Athena.Internal;

internal sealed class ParsingException : QsiException
{
    private readonly string _message;

    public ParsingException(string message) : this(message, 1, 1)
    {
    }

    public ParsingException(string message, int line, int column) : base(QsiError.SyntaxError)
    {
        _message = message;
        Line = line;
        Column = column;
    }

    public override string Message => $"line {Line}:{Column}: {_message}";

    public int Line { get; }

    public int Column { get; }
}
