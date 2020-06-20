using System.Collections.Generic;

namespace Qsi.Data
{
    public sealed class QsiDataTable
    {
        public QsiDataTableType Type { get; set; }

        public QsiQualifiedIdentifier Identifier { get; set; }

        public List<QsiDataColumn> Columns { get; } = new List<QsiDataColumn>();
    }
}
