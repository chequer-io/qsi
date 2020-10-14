using System.Text;
using Qsi.Data;

namespace Qsi.MySql.Data
{
    public readonly struct MySqlString : IDataProvider<string>
    {
        public QsiDataType Type => QsiDataType.String;

        public MySqlStringKind Kind { get; }

        public string Value { get; }

        public string CharacterSet { get; }

        public string CollateName { get; }

        internal MySqlString(MySqlStringKind kind, string value, string charSet, string collate)
        {
            Kind = kind;
            Value = value;
            CharacterSet = charSet;
            CollateName = collate;
        }

        internal MySqlString WithCollate(string collate)
        {
            return new MySqlString(Kind, Value, CharacterSet, collate);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            var escape = true;

            if (!string.IsNullOrWhiteSpace(CharacterSet))
            {
                builder.Append(CharacterSet);
                builder.Append(' ');
            }

            switch (Kind)
            {
                case MySqlStringKind.National:
                    builder.Append('N');
                    break;

                case MySqlStringKind.Bit:
                    builder.Append('B');
                    break;

                case MySqlStringKind.HexaString:
                    builder.Append('X');
                    break;

                case MySqlStringKind.Hexa:
                    builder.Append("0x");
                    escape = false;
                    break;
            }

            if (escape)
            {
                builder.AppendFormat("'{0}'", Value.Replace("'", "''"));
            }
            else
            {
                builder.Append(Value);
            }

            if (!string.IsNullOrWhiteSpace(CollateName))
            {
                builder.Append(" COLLATE ").Append(CollateName);
            }

            return builder.ToString();
        }
    }
}
