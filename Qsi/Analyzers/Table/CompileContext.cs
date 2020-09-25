using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Qsi.Data;

namespace Qsi.Analyzers.Table
{
    public partial class QsiTableAnalyzer
    {
        public sealed class CompileContext : IDisposable
        {
            public CompileContext Parent { get; }

            public QsiAnalyzerOptions Options { get; }

            public CancellationToken CancellationToken { get; }

            public int Depth { get; }

            public IEnumerable<QsiTableStructure> Directives { get; }

            public QsiTableStructure SourceTable { get; set; }

            public List<QsiTableStructure> SourceTables { get; }

            private readonly List<QsiTableStructure> _directives;
            private Stack<QsiQualifiedIdentifier> _identifierScope;

            public CompileContext(QsiAnalyzerOptions options, CancellationToken cancellationToken)
            {
                Options = options ?? throw new ArgumentNullException(nameof(options));
                CancellationToken = cancellationToken;

                _directives = new List<QsiTableStructure>();
                Directives = _directives;

                SourceTables = new List<QsiTableStructure>();
            }

            public CompileContext(CompileContext context) : this(context.Options, context.CancellationToken)
            {
                Parent = context;
                Depth = context.Depth + 1;

                // Priority: this > parent
                Directives = Directives.Concat(context.Directives);
            }

            public void ThrowIfCancellationRequested()
            {
                CancellationToken.ThrowIfCancellationRequested();
            }

            public void AddDirective(QsiTableStructure directiveTable)
            {
                _directives.Add(directiveTable);
            }

            public void AddDirectives(params QsiTableStructure[] directiveTables)
            {
                _directives.AddRange(directiveTables);
            }

            public QsiQualifiedIdentifier PeekIdentifierScope()
            {
                if (_identifierScope == null)
                    return Parent?.PeekIdentifierScope();

                if (_identifierScope.TryPeek(out var scope))
                    return scope;

                return null;
            }

            public void PushIdentifierScope(QsiQualifiedIdentifier identifier)
            {
                _identifierScope ??= new Stack<QsiQualifiedIdentifier>();
                _identifierScope.Push(identifier);
            }

            void IDisposable.Dispose()
            {
                _directives.Clear();
                _identifierScope?.Clear();
                SourceTables.Clear();
            }
        }
    }
}
