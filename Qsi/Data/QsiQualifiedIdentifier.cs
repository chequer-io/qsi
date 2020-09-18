using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Qsi.Utilities;

namespace Qsi.Data
{
    public sealed class QsiQualifiedIdentifier : IEnumerable<QsiIdentifier>
    {
        public QsiIdentifier this[int index] => _identifiers[index];

        public QsiIdentifier this[Index index] => _identifiers[index];

        public QsiIdentifier[] this[Range range] => _identifiers[range];

        public int Level { get; }

        internal readonly QsiIdentifier[] _identifiers;

        public QsiQualifiedIdentifier(IEnumerable<QsiIdentifier> identifiers) : this(identifiers.ToArray())
        {
        }

        public QsiQualifiedIdentifier(params QsiIdentifier[] identifiers)
        {
            _identifiers = identifiers;
            Level = _identifiers?.Length ?? 0;
        }

        public QsiQualifiedIdentifier SubIdentifier(Range range)
        {
            return new QsiQualifiedIdentifier(_identifiers[range]);
        }

        public QsiQualifiedIdentifier SubIdentifier(Index index)
        {
            return new QsiQualifiedIdentifier(_identifiers[index]);
        }

        public IEnumerator<QsiIdentifier> GetEnumerator()
        {
            return _identifiers.OfType<QsiIdentifier>().GetEnumerator();
        }

        public override int GetHashCode()
        {
            return HashCodeUtility.Combine(_identifiers.Select(i => i.GetHashCode()));
        }

        public override string ToString()
        {
            return string.Join(".", _identifiers.Select(x => x.Value));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
