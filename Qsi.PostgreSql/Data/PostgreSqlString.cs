using System.Linq;
using System.Text;
using Qsi.Data;
using Qsi.Utilities;

namespace Qsi.PostgreSql.Data;

public readonly struct PostgreSqlString : IDataProvider<string>
{
    public QsiDataType Type => QsiDataType.String;

    public PostgreSqlStringKind Kind { get; }
    
    public string Value { get; }

    internal PostgreSqlString(PostgreSqlStringKind kind, string value)
    {
        Kind = kind;
        Value = value;
    }

    internal PostgreSqlString(string unescapedValue)
    {
        if (unescapedValue.StartsWith("n", true, null))
        {
            Kind = unescapedValue.StartsWith("nchar", true, null) ?
                PostgreSqlStringKind.NCharString :
                PostgreSqlStringKind.National;
        }
        else if (unescapedValue.StartsWith("char", true, null))
        {
            Kind = PostgreSqlStringKind.CharString;
        }
        else if (unescapedValue.StartsWith("bpchar", true, null))
        {
            Kind = PostgreSqlStringKind.BpCharString;
        }
        else if (unescapedValue.StartsWith("varchar", true, null))
        {
            Kind = PostgreSqlStringKind.VarcharString;
        }
        else if (unescapedValue.StartsWith("x", true, null))
        {
            Kind = PostgreSqlStringKind.Hex;
        }
        else if (unescapedValue.StartsWith("b", true, null))
        {
            Kind = PostgreSqlStringKind.Bin;
        }
        else
        {
            throw new QsiException(QsiError.Internal);
        }

        var noPrefix = string.Concat(unescapedValue
            .SkipWhile(c => c != '\''));
        
        var value = IdentifierUtility.Unescape(noPrefix);

        Value = value;
    }

    public override string ToString()
    {
        var builder = new StringBuilder();

        switch (Kind)
        {
            case PostgreSqlStringKind.National:
                builder.Append('N');
                break;
            
            case PostgreSqlStringKind.CharString:
                builder.Append("CHAR");
                break;
            
            case PostgreSqlStringKind.NCharString:
                builder.Append("NCHAR");
                break;
            
            case PostgreSqlStringKind.BpCharString:
                builder.Append("BPCHAR");
                break;
            
            case PostgreSqlStringKind.VarcharString:
                builder.Append("VARCHAR");
                break;
            
            case PostgreSqlStringKind.Bin:
                builder.Append('B');
                break;
            
            case PostgreSqlStringKind.Hex:
                builder.Append('X');
                break;
        }

        builder.Append($"'{Value.Replace("'", "''")}'");

        return builder.ToString();
    }
}
