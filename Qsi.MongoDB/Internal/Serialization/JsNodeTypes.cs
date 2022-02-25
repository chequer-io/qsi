using System;
using System.Collections.Generic;
using System.Linq;
using Qsi.MongoDB.Internal.Nodes;

namespace Qsi.MongoDB.Internal.Serialization;

internal static class JsNodeTypes
{
    private static readonly Dictionary<string, Type> _nodeTypes;

    static JsNodeTypes()
    {
        _nodeTypes = typeof(JsNodeTypes).Assembly.DefinedTypes
            .Where(t => typeof(INode).IsAssignableFrom(t) && t.IsClass)
            .ToDictionary(x => x.Name[..^4], x => x.AsType());
    }

    public static bool TryGetNodeType(string nodeName, out Type nodeType)
    {
        return _nodeTypes.TryGetValue(nodeName, out nodeType);
    }
}
