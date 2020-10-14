using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Qsi.Analyzers.Context;
using Qsi.Data;
using Qsi.Tree;

namespace Qsi.Analyzers
{
    public abstract class QsiAnalyzerBase
    {
        protected IEqualityComparer<QsiIdentifier> IdentifierComparer => _identifierComparer.Value;

        private readonly QsiEngine _engine;
        private readonly Lazy<IEqualityComparer<QsiIdentifier>> _identifierComparer;

        protected QsiAnalyzerBase(QsiEngine engine)
        {
            _engine = engine;
            _identifierComparer = new Lazy<IEqualityComparer<QsiIdentifier>>(() => new InternalIdentifierComparer(Match));
        }

        public ValueTask<IQsiAnalysisResult> Execute(
            QsiScript script,
            IQsiTreeNode tree,
            QsiAnalyzerOptions options,
            CancellationToken cancellationToken = default)
        {
            if (!CanExecute(script, tree))
                throw new QsiException(QsiError.NotSupportedScript, script.ScriptType);

            return OnExecute(new AnalyzerContext(_engine, script, tree, options, cancellationToken));
        }

        public abstract bool CanExecute(QsiScript script, IQsiTreeNode tree);

        protected abstract ValueTask<IQsiAnalysisResult> OnExecute(IAnalyzerContext context);

        #region Utilities
        protected bool Match(QsiIdentifier a, QsiIdentifier b)
        {
            return _engine.LanguageService.MatchIdentifier(a, b);
        }

        protected bool Match(QsiQualifiedIdentifier a, QsiQualifiedIdentifier b)
        {
            if (a.Level != b.Level)
                return false;

            for (int i = 0; i < a.Level; i++)
            {
                if (!Match(a[i], b[i]))
                    return false;
            }

            return true;
        }
        #endregion

        private readonly struct InternalIdentifierComparer : IEqualityComparer<QsiIdentifier>
        {
            private readonly Func<QsiIdentifier, QsiIdentifier, bool> _equalsDelegate;

            public InternalIdentifierComparer(Func<QsiIdentifier, QsiIdentifier, bool> equalsDelegate)
            {
                _equalsDelegate = equalsDelegate;
            }

            public bool Equals(QsiIdentifier x, QsiIdentifier y)
            {
                return _equalsDelegate(x, y);
            }

            public int GetHashCode(QsiIdentifier obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
