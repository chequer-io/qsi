﻿using System.Collections.Generic;

namespace Qsi.MongoDB.Internal.Nodes;

internal class ForInStatement : BaseNode, IForXStatementNode
{
    // VariableDeclarationNode, PatternNode
    public INode Left { get; set; }

    public IExpressionNode Right { get; set; }

    public IStatementNode Body { get; set; }

    public override IEnumerable<INode> Children
    {
        get
        {
            yield return Left;
            yield return Right;
            yield return Body;
        }
    }
}
