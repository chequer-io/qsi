﻿using System.Collections.Generic;
using System.Linq;
using Qsi.Data;
using Qsi.Tree.Data;

namespace Qsi.Tree.Immutable;

public readonly struct ImmutableBindParameterExpressionNode : IQsiBindParameterExpressionNode
{
    public IQsiTreeNode Parent { get; }

    public QsiParameterType Type { get; }

    public string Prefix { get; }

    public bool NoSuffix { get; }

    public string Name { get; }

    public int? Index { get; }

    public IUserDataHolder UserData { get; }

    public IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();

    public ImmutableBindParameterExpressionNode(
        IQsiTreeNode parent,
        QsiParameterType type,
        string prefix,
        bool noSuffix,
        string name,
        int? index,
        IUserDataHolder userData)
    {
        Parent = parent;
        Type = type;
        Prefix = prefix;
        NoSuffix = noSuffix;
        Name = name;
        Index = index;
        UserData = userData;
    }
}