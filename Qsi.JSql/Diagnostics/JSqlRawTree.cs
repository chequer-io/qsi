using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ikvm.extensions;
using java.util;
using net.sf.jsqlparser;
using net.sf.jsqlparser.expression;
using net.sf.jsqlparser.parser;
using Qsi.Diagnostics;
using Qsi.JSql.Extensions;

namespace Qsi.JSql.Diagnostics
{
    internal sealed class JSqlRawTree : IRawTree
    {
        public string DisplayName { get; }

        public IRawTree[] Children { get; }

        public JSqlRawTree(object value)
        {
            DisplayName = value.GetType().Name;

            Children = GetDeepFields(value)
                .Select(field => GetRawTreeFromObject(field.Name, field.GetValue(value)))
                .Where(t => t != null)
                .ToArray();

            if (Children.Length == 0 && value is ASTNodeAccess nodeAccess)
            {
                Children = new IRawTree[]
                {
                    new JSqlRawTreeTerminalNode(nodeAccess.toString()),
                };
            }
        }

        private JSqlRawTree(string key, List list)
        {
            DisplayName = key;

            Children = list
                .toArray()
                .Select((v, i) =>
                {
                    var value = GetRawTreeFromObject(null, v);

                    if (value != null)
                        return value;

                    return new JSqlRawTree($"[{i}] {v.toString()}");
                })
                .ToArray();
        }

        private JSqlRawTree(Node node)
        {
            DisplayName = node.toString();

            if (node.jjtGetNumChildren() == 0)
            {
                if (node is SimpleNode simpleNode)
                {
                    Children = new IRawTree[]
                    {
                        new JSqlRawTreeTerminalNode(simpleNode.jjtGetFirstToken())
                    };
                }
            }
            else
            {
                Children = node.GetChildren()
                    .Select(c => (IRawTree)new JSqlRawTree(c))
                    .ToArray();
            }
        }

        private JSqlRawTree(string displayName)
        {
            DisplayName = displayName;
        }

        private IEnumerable<FieldInfo> GetDeepFields(object value)
        {
            var t = value.GetType();

            while (t != null)
            {
                foreach (var field in t.GetRuntimeFields().Where(f => !f.IsStatic))
                {
                    var methodName = $"get{char.ToUpper(field.Name[0])}{field.Name[1..]}";

                    if (field.FieldType == typeof(List) || t.GetRuntimeMethod(methodName, Type.EmptyTypes) != null)
                    {
                        yield return field;
                    }
                }

                t = t.BaseType;
            }
        }

        private IRawTree GetRawTreeFromObject(string key, object value)
        {
            switch (value)
            {
                case Node node:
                    return new JSqlRawTree(node);

                case List list:
                    return new JSqlRawTree(key, list);

                case Model _:
                case ASTNodeAccess _:
                case java.lang.Object _:
                    return new JSqlRawTree(value);
            }

            return null;
        }
    }
}
