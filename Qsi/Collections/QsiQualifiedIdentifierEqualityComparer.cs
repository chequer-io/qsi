using System.Collections.Generic;
using System.Linq;
using Qsi.Data;

namespace Qsi.Collections;

public sealed class QsiQualifiedIdentifierEqualityComparer : IEqualityComparer<QsiQualifiedIdentifier>
{
    public static QsiQualifiedIdentifierEqualityComparer Default =>
        _default ??= new QsiQualifiedIdentifierEqualityComparer(QsiIdentifierEqualityComparer.Default);

    private static QsiQualifiedIdentifierEqualityComparer _default;

    private readonly IEqualityComparer<QsiIdentifier> _identifierComparer;

    public QsiQualifiedIdentifierEqualityComparer(IEqualityComparer<QsiIdentifier> identifierComparer)
    {
        _identifierComparer = identifierComparer;
    }

    public bool Equals(QsiQualifiedIdentifier x, QsiQualifiedIdentifier y)
    {
        if (x == null && y == null)
            return true;

        if (x == null || y == null)
            return false;

        if (x.Level != y.Level)
            return false;

        return x.SequenceEqual(y, _identifierComparer);
    }

    public int GetHashCode(QsiQualifiedIdentifier obj)
    {
        return obj.GetHashCode();
    }
}