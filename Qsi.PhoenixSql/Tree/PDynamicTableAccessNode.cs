﻿using Qsi.Tree;

namespace Qsi.PhoenixSql.Tree
{
    internal sealed class PDynamicTableAccessNode : QsiTableAccessNode, IDynamicTableNode
    {
        public QsiColumnsDeclarationNode DynamicColumns { get; set; }
    }
}
