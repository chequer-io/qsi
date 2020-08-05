using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Qsi.PostgreSql.Internal.Postgres.Converters
{
    internal static class PgNodeContract
    {
        private static readonly Dictionary<string, Type> _nodeTypes;

        static PgNodeContract()
        {
            _nodeTypes = typeof(PgNodeContract).Assembly.DefinedTypes
                .Select(t => (t, t.GetCustomAttribute<PgNodeAttribute>()))
                .Where(x => x.Item2 != null)
                .ToDictionary(x => x.Item2.Name, x => x.t.AsType());
        }

        public static bool TryGetNodeType(string nodeName, out Type nodeType)
        {
            return _nodeTypes.TryGetValue(nodeName, out nodeType);
        }
    }
}
