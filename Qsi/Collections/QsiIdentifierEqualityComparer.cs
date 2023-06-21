using System;
using System.Collections.Generic;
using Qsi.Data;
using Qsi.Utilities;

namespace Qsi.Collections;

public sealed class QsiIdentifierEqualityComparer : IEqualityComparer<QsiIdentifier>
{
    public static QsiIdentifierEqualityComparer Default =>
        _default ??= new QsiIdentifierEqualityComparer(StringComparison.Ordinal);

    private static QsiIdentifierEqualityComparer _default;

    private readonly StringComparison _comparison;

    public QsiIdentifierEqualityComparer(StringComparison comparison)
    {
        _comparison = comparison;
    }

    public bool Equals(QsiIdentifier x, QsiIdentifier y)
    {
        if (x == null && y == null)
            return true;

        if (x == null || y == null)
            return false;

        string nX = x.IsEscaped ? IdentifierUtility.Unescape(x.Value) : x.Value;
        string nY = y.IsEscaped ? IdentifierUtility.Unescape(y.Value) : y.Value;

        return string.Equals(nX, nY, _comparison);
    }

    public int GetHashCode(QsiIdentifier obj)
    {
        return obj.GetHashCode();
    }
}