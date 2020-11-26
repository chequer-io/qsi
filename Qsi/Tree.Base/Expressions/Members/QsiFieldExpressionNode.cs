using System.Collections.Generic;
using System.Linq;
using Qsi.Data;

namespace Qsi.Tree
{
    public class QsiFieldExpressionNode : QsiExpressionNode, IQsiFieldExpressionNode, IQsiTerminalNode
    {
        public QsiQualifiedIdentifier Identifier
        {
            get => _identifier;
            set
            {
                if (value != null && value.Level != 1)
                    throw new QsiException(QsiError.Internal, "Field identifier more than one.");

                _identifier = value;
            }
        }

        public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();

        private QsiQualifiedIdentifier _identifier;
    }
}
