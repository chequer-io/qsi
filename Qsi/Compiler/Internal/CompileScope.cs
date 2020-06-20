using System;
using System.Collections.Generic;
using System.Linq;
using Qsi.Data;

namespace Qsi.Runtime.Internal
{
    internal sealed class CompileScope : IDisposable
    {
        public CompileScope Parent { get; }

        public int Depth { get; }

        public IEnumerable<QsiDataTable> Directives { get; }

        public IEnumerable<QsiDataTable> Tables { get; }

        private readonly List<QsiDataTable> _directives;
        private readonly List<QsiDataTable> _tables;

        public CompileScope() : this(null)
        {
        }

        public CompileScope(CompileScope scope)
        {
            Parent = scope;

            _directives = new List<QsiDataTable>();
            _tables = new List<QsiDataTable>();

            Directives = _directives;

            // Stack
            Tables = _tables.AsEnumerable().Reverse();

            if (scope != null)
            {
                Depth = scope.Depth + 1;

                // Priority: this > parent
                Directives = Directives.Concat(scope.Directives);
                Tables = Tables.Concat(scope.Tables);
            }
        }

        public void PushTable(QsiDataTable table)
        {
            _tables.Add(table);
        }

        public void AddDirective(QsiDataTable directiveTable)
        {
            _directives.Add(directiveTable);
        }

        public void AddDirectives(IEnumerable<QsiDataTable> directiveTables)
        {
            _directives.AddRange(directiveTables);
        }

        void IDisposable.Dispose()
        {
            _directives.Clear();
            _tables.Clear();
        }
    }
}
