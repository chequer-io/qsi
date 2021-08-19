using System.Collections.Generic;
using Qsi.Data;
using Qsi.Utilities;

namespace Qsi.Oracle.Collections
{
    public class OracleQsiIdentifierEqualityComparer : IEqualityComparer<QsiIdentifier>
    {
        public static OracleQsiIdentifierEqualityComparer Default =>
            _default ??= new OracleQsiIdentifierEqualityComparer();

        private static OracleQsiIdentifierEqualityComparer _default;

        public bool Equals(QsiIdentifier x, QsiIdentifier y)
        {
            if (x == null && y == null)
                return true;

            if (x == null || y == null)
                return false;

            string nX = x.IsEscaped ? IdentifierUtility.Unescape(x.Value) : x.Value.ToUpper();
            string nY = y.IsEscaped ? IdentifierUtility.Unescape(y.Value) : y.Value.ToUpper();

            return string.Equals(nX, nY);
        }

        public int GetHashCode(QsiIdentifier obj)
        {
            return obj.GetHashCode();
        }
    }
}
