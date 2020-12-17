using System;
using System.Collections.Generic;
using System.Text;

namespace Qsi.Parsing.Common
{
    public class ScriptWriter
    {
        private readonly StringBuilder _builder;

        public ScriptWriter()
        {
            _builder = new StringBuilder();
        }

        public ScriptWriter WriteSpace()
        {
            if (_builder.Length > 0 && _builder[^1] != ' ')
            {
                _builder.Append(' ');
            }

            return this;
        }

        public ScriptWriter Write(object value)
        {
            _builder.Append(value);
            return this;
        }

        public ScriptWriter Write(char value)
        {
            _builder.Append(value);
            return this;
        }

        public ScriptWriter Write(string value)
        {
            _builder.Append(value);
            return this;
        }

        public void WriteJoin<T>(string delimiter, IEnumerable<T> elements, Action<ScriptWriter, T> action = null)
        {
            using IEnumerator<T> enumerator = elements.GetEnumerator();

            if (!enumerator.MoveNext())
                return;

            action ??= (w, i) => w.Write(i);
            action(this, enumerator.Current);

            while (enumerator.MoveNext())
            {
                Write(delimiter);
                action(this, enumerator.Current);
            }
        }

        public override string ToString()
        {
            return _builder.ToString();
        }
    }
}
