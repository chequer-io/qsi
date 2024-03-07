using System;
using System.Collections.Generic;
using System.Linq;
using PgQuery;
using Qsi.Data;
using Qsi.Tree;

namespace Qsi.PostgreSql.Tree;

internal static partial class PgNodeVisitor
{
    private static QsiAliasNode CreateAliasNode(string name)
    {
        return new QsiAliasNode
        {
            Name = new QsiIdentifier(name, false)
        };
    }

    private static QsiIdentifier CreateIdentifier(Node strNode)
    {
        if (strNode is not { nodeCase: Node.nodeOneofCase.String })
            throw CreateInternalException("CreateIdentifier(Node) only support String node");

        return new QsiIdentifier(strNode.String.Sval, false);
    }

    private static QsiIdentifier CreateIdentifier(string name)
    {
        return new QsiIdentifier(name, false);
    }

    private static QsiQualifiedIdentifier CreateQualifiedIdentifier(RangeVar relation)
    {
        return CreateQualifiedIdentifier(relation.Catalogname, relation.Schemaname, relation.Relname);
    }

    private static QsiQualifiedIdentifier CreateQualifiedIdentifier(params string?[] names)
    {
        return new QsiQualifiedIdentifier(names.Where(n => !string.IsNullOrWhiteSpace(n)).Select(CreateIdentifier!));
    }

    private static QsiQualifiedIdentifier CreateQualifiedIdentifier(IEnumerable<string?> names)
    {
        return new QsiQualifiedIdentifier(names.Where(n => !string.IsNullOrWhiteSpace(n)).Select(CreateIdentifier!));
    }

    private static QsiQualifiedIdentifier CreateQualifiedIdentifier(params Node[] nodes)
    {
        return CreateQualifiedIdentifier((IEnumerable<Node>)nodes);
    }

    private static QsiQualifiedIdentifier CreateQualifiedIdentifier(IEnumerable<Node> nodes)
    {
        return new QsiQualifiedIdentifier(nodes.Select(n => new QsiIdentifier(n.String.Sval, false)));
    }
}
