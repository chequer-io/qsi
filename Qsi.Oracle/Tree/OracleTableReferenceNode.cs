﻿using Qsi.Tree;

namespace Qsi.Oracle.Tree
{
    public class OracleTableReferenceNode : QsiTableReferenceNode, IOracleTableNode
    {
        public bool IsOnly { get; set; }

        public QsiTreeNodeProperty<OraclePartitionExpressionNode> Partition { get; }

        public QsiTreeNodeProperty<OracleHierarchiesExpressionNode> Hierarchies { get; }

        public OracleTableReferenceNode()
        {
            Partition = new QsiTreeNodeProperty<OraclePartitionExpressionNode>(this);
            Hierarchies = new QsiTreeNodeProperty<OracleHierarchiesExpressionNode>(this);
        }
    }
}
