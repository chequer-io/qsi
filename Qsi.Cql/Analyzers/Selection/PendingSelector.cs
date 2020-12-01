using System;
using System.Collections;
using Newtonsoft.Json.Linq;

namespace Qsi.Cql.Analyzers.Selection
{
    internal class PendingSelector : ISelector
    {
        public string Sql { get; }

        public Type Type { get; }

        private ISelector _selector;

        public PendingSelector(string sql, Type type)
        {
            Sql = sql;
            Type = type;
        }

        public void Resolve(object value)
        {
            if (Type == typeof(ElementSelector))
            {
                var index = (int)Convert.ChangeType(value, TypeCode.Int32);
                _selector = new ElementSelector(index);
            }
            else if (Type == typeof(FieldSelector))
            {
                _selector = new FieldSelector(value.ToString());
            }
            else if (Type == typeof(RangeSelector))
            {
                var dictionary = (IDictionary)value;
                var start = (Index)Convert.ChangeType(dictionary['s'], TypeCode.Int32);
                var end = (Index)Convert.ChangeType(dictionary['e'], TypeCode.Int32);

                if (start.Value == -1)
                    start = Index.Start;

                if (end.Value == -1)
                    end = Index.End;

                _selector = new RangeSelector(start..end);
            }
            else
            {
                throw new NotSupportedException(Type.Name);
            }
        }

        public JToken Run(JToken value)
        {
            return _selector.Run(value);
        }
    }
}
