using System.Text;

namespace Qsi.Parsing.Common
{
    public class ScriptWriter
    {
        private readonly StringBuilder _builder;

        private bool _space;

        public ScriptWriter()
        {
            _builder = new StringBuilder();
        }

        public ScriptWriter WriteSpace()
        {
            if (!_space)
            {
                _builder.Append(' ');
                _space = true;
            }

            return this;
        }

        public ScriptWriter Write(object value)
        {
            _space = false;
            _builder.Append(value);
            return this;
        }

        public ScriptWriter Write(char value)
        {
            _space = false;
            _builder.Append(value);
            return this;
        }

        public ScriptWriter Write(string value)
        {
            _space = false;
            _builder.Append(value);
            return this;
        }

        public override string ToString()
        {
            return _builder.ToString();
        }
    }
}
