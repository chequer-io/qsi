using System.Collections.Generic;
using Qsi.Collections;
using Qsi.Compiler;
using Qsi.Data;
using Qsi.Parsing;

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
            return QsiIdentifierEqualityComparer.Default;
        }
    }
}
