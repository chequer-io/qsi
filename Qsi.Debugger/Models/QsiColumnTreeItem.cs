using System.Linq;
using Qsi.Data;

namespace Qsi.Debugger.Models
{
    public class QsiColumnTreeItem
    {
        public int Depth { get; }

        public QsiDataColumn Column { get; }

        public QsiColumnTreeItem[] Items { get; }

        public QsiColumnTreeItem(QsiDataColumn column, int depth = 0)
        {
            Depth = depth;
            Column = column;

            Items = column.References
                .Select(c => new QsiColumnTreeItem(c, depth + 1))
                .ToArray();
        }
    }
}
