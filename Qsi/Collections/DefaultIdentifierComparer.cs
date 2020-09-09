using System.Collections.Generic;
using Qsi.Data;
using Qsi.Utilities;

namespace Qsi.Collections
{
    public class DefaultIdentifierComparer : IEqualityComparer<QsiIdentifier>
    {
        public static DefaultIdentifierComparer Default => _default ??= new DefaultIdentifierComparer();

        private static DefaultIdentifierComparer _default;

        public bool Equals(QsiIdentifier x, QsiIdentifier y)
        {
            if (x == null && y == null)
                return true;

            if (x == null || y == null)
                return false;

            string nX = x.IsEscaped ? IdentifierUtility.Unescape(x.Value) : x.Value;
            string nY = y.IsEscaped ? IdentifierUtility.Unescape(y.Value) : y.Value;

            return nX == nY;
        }

        public int GetHashCode(QsiIdentifier obj)
        {
            return obj.GetHashCode();
        }
    }
}
