using Qsi.Data;
using Qsi.Tree;

namespace Qsi.Oracle.Tree;

public class OraclePartitionExpressionNode : QsiMultipleExpressionNode
{
    public bool IsSubpartition { get; set; }
        
    public QsiQualifiedIdentifier Identifier { get; set; }
}