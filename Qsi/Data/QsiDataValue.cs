namespace Qsi.Data
{
    public sealed class QsiDataValue
    {
        #region Static
        public static QsiDataValue Unknown => _unknown ??= new QsiDataValue(null, QsiDataType.Unknown);

        public static QsiDataValue Unset => _unset ??= new QsiDataValue(null, QsiDataType.Unset);

        public static QsiDataValue Default => _default ??= new QsiDataValue(null, QsiDataType.Default);

        public static QsiDataValue Null => _null ??= new QsiDataValue(null, QsiDataType.Null);

        private static QsiDataValue _unknown;
        private static QsiDataValue _unset;
        private static QsiDataValue _default;
        private static QsiDataValue _null;
        #endregion

        public object Value { get; }

        public QsiDataType Type { get; }

        public QsiDataValue(object value, QsiDataType type)
        {
            Value = value;
            Type = type;
        }
    }
}
