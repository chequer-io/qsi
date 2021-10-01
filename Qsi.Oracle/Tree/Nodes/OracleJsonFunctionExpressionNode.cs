using System.Collections.Generic;
using System.Linq;
using Qsi.Oracle.Common;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Oracle.Tree
{
    public class OracleJsonFunctionExpressionNode : OracleInvokeExpressionNode
    {
        public OracleNullBehavior NullBehavior { get; set; } = OracleNullBehavior.Absent;

        public string ReturnType { get; set; }

        public bool IsStrict { get; set; }

        public bool IsPretty { get; set; }

        public bool IsAscii { get; set; }

        public bool IsTruncate { get; set; }

        public OracleOnErrorBehavior OnErrorBehavior { get; set; }

        public string OnErrorDefault { get; set; }

        public OracleOnEmptyBehavior OnEmptyBehavior { get; set; }

        public string OnEmptyDefault { get; set; }
    }

    public class OracleJsonElementNode : QsiExpressionNode
    {
        public QsiTreeNodeProperty<QsiExpressionNode> Expression { get; }

        public bool IsFormatted { get; set; }

        public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Expression);

        public OracleJsonElementNode()
        {
            Expression = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        }
    }

    public class OracleJsonEntryNode : QsiExpressionNode
    {
        public QsiTreeNodeProperty<QsiExpressionNode> Key { get; }

        public QsiTreeNodeProperty<QsiExpressionNode> Value { get; }

        public bool IsFormatted { get; set; }

        public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Key, Value);

        public OracleJsonEntryNode()
        {
            Key = new QsiTreeNodeProperty<QsiExpressionNode>(this);
            Value = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        }
    }
}
