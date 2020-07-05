using System;
using System.Collections.Generic;
using System.Linq;
using Qsi.Data;

namespace Qsi.Runtime.Internal
{
    internal sealed class CompileContext : IDisposable
    {
        public CompileContext Parent { get; }

        public int Depth { get; }

        public IEnumerable<QsiDataTable> Directives { get; }

        public QsiDataTable SourceTable { get; set; }

        public List<QsiDataTable> SourceTables { get; }

        private readonly List<QsiDataTable> _directives;

        public CompileContext() : this(null)
        {
        }

        public CompileContext(CompileContext context)
        {
            Parent = context;

            _directives = new List<QsiDataTable>();
            Directives = _directives;

            SourceTables = new List<QsiDataTable>();

            if (context != null)
            {
                Depth = context.Depth + 1;

                // Priority: this > parent
                Directives = Directives.Concat(context.Directives);
            }
        }

        public void AddDirective(QsiDataTable directiveTable)
        {
            _directives.Add(directiveTable);
        }

        public void AddDirectives(params QsiDataTable[] directiveTables)
        {
            _directives.AddRange(directiveTables);
        }

        void IDisposable.Dispose()
        {
            _directives.Clear();
            SourceTables.Clear();
        }
    }
}
