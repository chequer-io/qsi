using System;
using System.Collections.Generic;

namespace Qsi.Cql.Internal
{
    internal partial class CqlLexerInternal
    {
        private Stack<int> _saveStack;

        public void Save()
        {
            _saveStack ??= new Stack<int>();
            _saveStack.Push(InputStream.Index);
        }

        public void Restore()
        {
            if (_saveStack == null || !_saveStack.TryPeek(out int index))
                throw new StackOverflowException();

            InputStream.Seek(index);
        }
    }
}
