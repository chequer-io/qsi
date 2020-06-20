using System.Collections.Generic;
using System.Linq;

namespace Qsi.Data
{
    public sealed class QsiDataColumn
    {
        public QsiIdentifier Name { get; set;}

        public List<QsiDataColumn> References { get; } = new List<QsiDataColumn>();

        public bool IsAnonymous { get; set; }

        public bool IsExpression
        {
            get => _isExpression || References.Any(r => r.IsExpression);
            set => _isExpression = value;
        }

        private bool _isExpression;
    }
}
