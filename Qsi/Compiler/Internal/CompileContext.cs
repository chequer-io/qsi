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

        public IEnumerable<QsiDataTable> Tables { get; }

        private readonly List<QsiDataTable> _directives;
        private readonly List<QsiDataTable> _tables;

        public CompileContext() : this(null)
        {
        }

        public CompileContext(CompileContext context)
        {
            Parent = context;

            _directives = new List<QsiDataTable>();
            _tables = new List<QsiDataTable>();

            Directives = _directives;

            // Stack
            Tables = _tables.AsEnumerable().Reverse();

            if (context != null)
            {
                Depth = context.Depth + 1;

                // Priority: this > parent
                Directives = Directives.Concat(context.Directives);
                Tables = Tables.Concat(context.Tables);
            }
        }

        public void PushTable(QsiDataTable table)
        {
            _tables.Add(table);
        }

        public QsiDataTable PeekTable()
        {
            return _tables.FirstOrDefault();
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
