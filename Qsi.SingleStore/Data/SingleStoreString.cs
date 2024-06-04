using System.Text;
using Qsi.Data;

namespace Qsi.SingleStore.Data;

public readonly struct SingleStoreString : IDataProvider<string>
{
    public QsiDataType Type => QsiDataType.String;

    public SingleStoreStringKind Kind { get; }

    public string Value { get; }

    public string CharacterSet { get; }

    public string CollateName { get; }

    internal SingleStoreString(SingleStoreStringKind kind, string value, string charSet, string collate)
    {
        Kind = kind;
        Value = value;
        CharacterSet = charSet;
        CollateName = collate;
    }

    internal SingleStoreString WithCollate(string collate)
    {
        return new(Kind, Value, CharacterSet, collate);
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
            case SingleStoreStringKind.National:
                builder.Append('N');
                break;

            case SingleStoreStringKind.Bit:
                builder.Append('B');
                break;

            case SingleStoreStringKind.BitString:
                builder.Append("0b");
                escape = false;
                break;

            case SingleStoreStringKind.HexaString:
                builder.Append('X');
                break;

            case SingleStoreStringKind.Hexa:
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
