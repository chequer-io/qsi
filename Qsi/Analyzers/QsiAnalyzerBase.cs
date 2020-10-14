using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Qsi.Data;
using Qsi.Tree;

namespace Qsi.Analyzers
{
    public abstract class QsiAnalyzerBase
    {
        public QsiEngine Engine { get; }

        protected IEqualityComparer<QsiIdentifier> IdentifierComparer => _identifierComparer.Value;

        private readonly Lazy<IEqualityComparer<QsiIdentifier>> _identifierComparer;

        protected QsiAnalyzerBase(QsiEngine engine)
        {
            Engine = engine;
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

            return OnExecute(new ExecutionContext(script, tree, options, cancellationToken));
        }

        public abstract bool CanExecute(QsiScript script, IQsiTreeNode tree);

        protected abstract ValueTask<IQsiAnalysisResult> OnExecute(ExecutionContext context);

        #region Utilities
        protected bool Match(QsiIdentifier a, QsiIdentifier b)
        {
            return Engine.LanguageService.MatchIdentifier(a, b);
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

        // TODO: For .NET 5 record feature
        protected readonly struct ExecutionContext
        {
            public readonly QsiScript Script;
            public readonly IQsiTreeNode Tree;
            public readonly QsiAnalyzerOptions Options;
            public readonly CancellationToken CancellationToken;

            public ExecutionContext(QsiScript script, IQsiTreeNode tree, QsiAnalyzerOptions options, CancellationToken cancellationToken)
            {
                Script = script;
                Tree = tree;
                Options = options;
                CancellationToken = cancellationToken;
            }
        }

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
