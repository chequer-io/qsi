namespace Qsi.PostgreSql.Data;

public interface ISubscriptable
{
    public PostgreSqlSubscriptExpressionNode Subscript { get; }
}
