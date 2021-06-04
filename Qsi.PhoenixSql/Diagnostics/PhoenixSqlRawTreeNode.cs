using System;
using System.Collections;
using System.Linq;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using PhoenixSql;
using Qsi.Diagnostics;

namespace Qsi.PhoenixSql.Diagnostics
{
    internal class PhoenixSqlRawTreeNode : IRawTree
    {
        public string DisplayName { get; }

        public IRawTree[] Children { get; }

        public PhoenixSqlRawTreeNode(IMessage message)
        {
            if (message is IPhoenixProxyNode<IPhoenixNode> proxyMessage)
            {
                message = (IMessage)proxyMessage.Message;
            }

            DisplayName = message.GetType().Name;

            Children = message.Descriptor.Fields.InFieldNumberOrder()
                .Select(field => GetChildren(message, field))
                .Where(n => n != null)
                .ToArray();
        }

        private PhoenixSqlRawTreeNode(string key, IList list)
        {
            DisplayName = key;

            Children = list.OfType<object>()
                .Select(e =>
                {
                    if (e is IMessage message)
                        return new PhoenixSqlRawTreeNode(message);

                    return (IRawTree)new PhoenixSqlRawTerminalNode(e);
                })
                .ToArray();
        }

        private PhoenixSqlRawTreeNode(string key, object value)
        {
            DisplayName = key;
            Children = new IRawTree[1];

            ref var child = ref Children[0];

            if (value is IMessage message)
                child = new PhoenixSqlRawTreeNode(message);
            else
                child = new PhoenixSqlRawTerminalNode(value);
        }

        private IRawTree GetChildren(IMessage message, FieldDescriptor field)
        {
            var value = field.Accessor.GetValue(message);

            switch (value)
            {
                case IMessage childMessage:
                    return new PhoenixSqlRawTreeNode(field.Name, childMessage);

                case IList list:
                    return new PhoenixSqlRawTreeNode(field.Name, list);

                case string str when string.IsNullOrEmpty(str):
                    return null;

                case null:
                    return null;
            }

            var valueType = value.GetType();

            if (valueType.IsValueType && !valueType.IsEnum &&
                Activator.CreateInstance(valueType).Equals(value))
            {
                return null;
            }

            return new PhoenixSqlRawTreeNode(field.Name, value);
        }
    }
}
