using System.Collections.Generic;
using System.Linq;

namespace Qsi.Data
{
    public sealed class QsiQualifiedIdentifier
    {
        public QsiIdentifier[] Identifiers { get; }

        public QsiQualifiedIdentifier(IEnumerable<QsiIdentifier> identifiers) : this(identifiers.ToArray())
        {
        }

        public QsiQualifiedIdentifier(params QsiIdentifier[] identifiers)
        {
            Identifiers = identifiers;
        }
    }
}
