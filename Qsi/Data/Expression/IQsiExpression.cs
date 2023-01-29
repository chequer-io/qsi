namespace Qsi.Data;

public interface IQsiExpression
{
    QsiExpressionType ExpressionType { get; }

    int StartIndex { get; set; }

    int EndIndex { get; set; }
}
