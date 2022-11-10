using System.Collections.Generic;
using System.Linq;
using Qsi.Data;
using Qsi.Tree;

namespace Qsi.PostgreSql.Tree;

public class PostgreSqlUndefinedExpressionNode : QsiExpressionNode
{
    public QsiQualifiedIdentifier Name { get; set; }
    
    public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();
}
