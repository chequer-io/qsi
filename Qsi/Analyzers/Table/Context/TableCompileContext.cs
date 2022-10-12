using System;
using System.Collections.Generic;
using System.Linq;
using Qsi.Analyzers.Context;
using Qsi.Data;

namespace Qsi.Analyzers.Table.Context
{
    public sealed class TableCompileContext : AnalyzerContextWrapper, IDisposable
    {
        public TableCompileContext Parent { get; }

        public int Depth { get; }

        public IEnumerable<QsiTableStructure> Directives { get; }

        public QsiTableStructure SourceTable { get; set; }

        public List<QsiTableStructure> JoinedSouceTables { get; }

        private readonly List<QsiTableStructure> _directives;
        private Stack<QsiQualifiedIdentifier> _identifierScope;

        public TableCompileContext(IAnalyzerContext context) : this(context, context.AnalyzerOptions)
        {
        }

        public TableCompileContext(IAnalyzerContext context, QsiAnalyzerOptions options) : base(context, options)
        {
            _directives = new List<QsiTableStructure>();
            Directives = _directives;

            JoinedSouceTables = new List<QsiTableStructure>();
        }

        public TableCompileContext(TableCompileContext context) : this(context, context.AnalyzerOptions)
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

        public IEnumerable<TableCompileContext> Ancestors()
        {
            var node = Parent;

            while (node is not null)
            {
                yield return node;

                node = node.Parent;
            }
        }

        public IEnumerable<TableCompileContext> AncestorsAndSelf()
        {
            yield return this;

            foreach (var node in Ancestors())
                yield return node;
        }

        public IEnumerable<QsiTableStructure> GetAllSourceTables()
        {
            if (SourceTable is not null)
                yield return SourceTable;

            foreach (var joinedSouceTable in JoinedSouceTables)
                yield return joinedSouceTable;
        }

        void IDisposable.Dispose()
        {
            _directives.Clear();
            _identifierScope?.Clear();
            JoinedSouceTables.Clear();
        }
    }
}
