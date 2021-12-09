using System.Collections.Generic;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.PostgreSql.Tree.PG10.Nodes
{
    public class PgRoutineNode : QsiTableNode, IQsiInlineDerivedTableNode

    {
        public QsiTreeNodeProperty<QsiFunctionExpressionNode> Member { get; }

        public QsiTreeNodeList<QsiExpressionNode> Parameters { get; }

        public override IEnumerable<IQsiTreeNode> Children
            => TreeHelper.YieldChildren(Member?.Value, _parameters);

        private readonly QsiParametersExpressionNode _parameters;

        public IQsiAliasNode Alias { get; set; }

        public IQsiColumnsDeclarationNode Columns { get; set; }

        public IQsiRowValueExpressionNode[] Rows { get; set; }

        public PgRoutineNode()
        {
            Member = new QsiTreeNodeProperty<QsiFunctionExpressionNode>(this);

            _parameters = new QsiParametersExpressionNode
            {
                Parent = this
            };

            Parameters = _parameters.Expressions;
        }
    }
}
