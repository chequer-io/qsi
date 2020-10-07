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

        internal MySqlString(string value) : this(value, null, null)
        {
        }

        internal MySqlString(string value, string charSet, string collate)
        {
            Kind = MySqlStringKind.Default;
            Value = value;
            CharacterSet = charSet;
            CollateName = collate;
        }

        internal MySqlString(string value, string collate) : this(MySqlStringKind.Default, value, collate)
        {
        }

        internal MySqlString(MySqlStringKind kind, string value, string collate)
        {
            Kind = kind;
            Value = value;
            CharacterSet = null;
            CollateName = collate;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            switch (Kind)
            {
                case MySqlStringKind.National:
                    builder.Append('N');
                    break;

                case MySqlStringKind.Bit:
                    builder.Append('B');
                    break;

                case MySqlStringKind.Hexa:
                    builder.Append('X');
                    break;

                default:
                    if (!string.IsNullOrWhiteSpace(CharacterSet))
                    {
                        builder.Append(CharacterSet);
                    }

                    break;
            }

            builder.AppendFormat("'{0}'", Value.Replace("'", "''"));

            if (!string.IsNullOrWhiteSpace(CollateName))
            {
                builder.Append(" COLLATE ").Append(CollateName);
            }

            return builder.ToString();
        }
    }
}
