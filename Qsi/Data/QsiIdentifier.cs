namespace Qsi.Data
{
    public sealed class QsiIdentifier
    {
        public string Value { get; }

        public bool IsEscaped { get; }

        public QsiIdentifier(string value, bool escaped)
        {
            Value = value;
            IsEscaped = escaped;
        }
    }
}
