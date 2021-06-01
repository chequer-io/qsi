using System.Collections.Generic;
using Antlr4.Runtime;

namespace Qsi.Cql.Internal
{
    internal partial class CqlParserInternal
    {
        private readonly List<ErrorListener> _errorListeners = new();

        private readonly HashSet<string> _reservedTypeNames = new()
        {
            "byte",
            "complex",
            "enum",
            "date",
            "interval",
            "macaddr",
            "bitstring"
        };

        private int _bindParamIndex;

        protected int NextBindParameterIndex()
        {
            return _bindParamIndex++;
        }

        public override void AddErrorListener(IAntlrErrorListener<IToken> listener)
        {
            if (listener is ErrorListener errorListener)
                _errorListeners.Add(errorListener);

            base.AddErrorListener(listener);
        }

        public override void RemoveErrorListener(IAntlrErrorListener<IToken> listener)
        {
            if (listener is ErrorListener errorListener)
                _errorListeners.Remove(errorListener);

            base.RemoveErrorListener(listener);
        }

        public override void RemoveErrorListeners()
        {
            _errorListeners.Clear();
            base.RemoveErrorListeners();
        }

        protected void AddRecognitionError(string message)
        {
            foreach (var errorListener in _errorListeners)
            {
                errorListener.SyntaxError(this, message);
            }
        }

        protected void VerifyReservedTypeName(string typeName)
        {
            if (_reservedTypeNames.Contains(typeName))
                AddRecognitionError($"Invalid (reserved) user type name {typeName}");
        }
    }
}
