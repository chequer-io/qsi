using System;
using Qsi.Utilities;

namespace Qsi.Data
{
    public sealed class QsiIdentifier
    {
        public string Value { get; }

        public bool IsEscaped { get; }

        internal string UnescapedValue => _unescapedValue ??= IsEscaped ? IdentifierUtility.Unescape(Value) : Value;

        private string _unescapedValue;

        public QsiIdentifier(string value, bool escaped)
        {
            Value = value;
            IsEscaped = escaped;
        }

        public override int GetHashCode()
        {
            return UnescapedValue.GetHashCode();
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
