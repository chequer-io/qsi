using System.Collections.Generic;
using System.Linq;
using Qsi.Compiler;
using Qsi.Data;
using Qsi.Parsing;
using Qsi.Utilities;

namespace Qsi.Services
{
    public abstract class QsiLanguageServiceBase : IQsiLanguageService
    {
        public abstract QsiTableCompileOptions CreateCompileOptions();

        public abstract IQsiTreeParser CreateTreeParser();

        public abstract IQsiScriptParser CreateScriptParser();

        public abstract IQsiReferenceResolver CreateResolver();

        private IEqualityComparer<QsiIdentifier> _comparer;

        public virtual bool MatchIdentifier(QsiIdentifier x, QsiIdentifier y)
        {
            _comparer ??= GetIdentifierComparer();
            return _comparer.Equals(x, y);
        }

        protected virtual IEqualityComparer<QsiIdentifier> GetIdentifierComparer()
        {
            return new DefaultIdentifierComparer();
        }

        private class DefaultIdentifierComparer : IEqualityComparer<QsiIdentifier>
        {
            public bool Equals(QsiIdentifier x, QsiIdentifier y)
            {
                if (x == null && y == null)
                    return true;

                if (x == null || y == null)
                    return false;

                string nX = x.IsEscaped ? IdentifierUtility.Unescape(x.Value) : x.Value;
                string nY = y.IsEscaped ? IdentifierUtility.Unescape(y.Value) : y.Value;

                return nX == nY;
            }

            public int GetHashCode(QsiIdentifier obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
