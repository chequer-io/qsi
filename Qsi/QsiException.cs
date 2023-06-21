using System;
using System.Text;

namespace Qsi;

public class QsiException : Exception
{
    public QsiError Error { get; }

    public QsiException(QsiError error) : this(error, null)
    {
    }

    public QsiException(QsiError error, params object[] args) : base(CreateMessage(error, args))
    {
        Error = error;
    }

    // QSI-C9A8: Unknow table 'actor'
    private static string CreateMessage(QsiError error, object[] args)
    {
        var builder = new StringBuilder();

        // Message code
        builder.Append($"QSI-{(int)error:X4}");

        var template = SR.GetResource(error);

        // Description
        if (!string.IsNullOrEmpty(template))
        {
            builder.Append(": ");

            if (args?.Length > 0)
                builder.AppendFormat(template, args);
            else
                builder.Append(template);
        }

        return builder.ToString();
    }
}