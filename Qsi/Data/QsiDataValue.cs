namespace Qsi.Data
{
    public sealed class QsiDataValue
    {
        public object Value { get; }

        public QsiDataType Type { get; }

        public QsiDataValue(object value, QsiDataType type)
        {
            Value = value;
            Type = type;
        }
    }
}
