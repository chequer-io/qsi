using System.Collections.Generic;
using System.Linq;
using Qsi.Utilities;

namespace Qsi.Data
{
    public sealed class QsiQualifiedIdentifier
    {
        public QsiIdentifier[] Identifiers { get; }

        public int Level => Identifiers?.Length ?? 0;

        public QsiQualifiedIdentifier(IEnumerable<QsiIdentifier> identifiers) : this(identifiers.ToArray())
        {
        }

        public QsiQualifiedIdentifier(params QsiIdentifier[] identifiers)
        {
            Identifiers = identifiers;
        }

        public override int GetHashCode()
        {
            return HashCodeUtility.Combine(Identifiers.Select(i => i.GetHashCode()));
        }

        public override string ToString()
        {
            return string.Join(".", Identifiers.Select(x => x.Value));
        }
    }
}
