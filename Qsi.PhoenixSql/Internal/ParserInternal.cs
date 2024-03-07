using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using PhoenixSql;
using Qsi.Parsing;
using PParser = PhoenixSql.PhoenixSqlParser;

namespace Qsi.PhoenixSql.Internal;

internal static class ParserInternal
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IBindableStatement Parse(string sql)
    {
        try
        {
            return PParser.Parse(sql);
        }
        catch (PhoenixSqlSyntaxException e)
        {
            throw new QsiSyntaxErrorException(-1, -1, e.Message);
        }
        catch (PhoenixSqlException e)
        {
            throw new QsiException(QsiError.Internal, e.Message);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<IBindableStatement> ParseAsync(string sql)
    {
        try
        {
            return await PParser.ParseAsync(sql);
        }
        catch (PhoenixSqlSyntaxException e)
        {
            throw new QsiSyntaxErrorException(-1, -1, e.Message);
        }
        catch (PhoenixSqlException e)
        {
            throw new QsiException(QsiError.Internal, e.Message);
        }
    }
}