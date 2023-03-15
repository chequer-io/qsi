using System;

namespace Qsi.Data;

public class QsiSensitiveData
{
    public QsiSensitiveDataType DataType { get; }

    public Range SpanRange { get; }

    public QsiSensitiveData(QsiSensitiveDataType dataType, Range spanRange)
    {
        DataType = dataType;
        SpanRange = spanRange;
    }
}
