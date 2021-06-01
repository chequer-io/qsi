namespace Qsi.Data
{
    public sealed class QsiParameter
    {
        public QsiParameterType Type { get; }

        public string Name { get; }

        public object Value { get; }

        public QsiParameter(QsiParameterType type, string name, object value)
        {
            Type = type;
            Name = name;
            Value = value;
        }
    }
}
