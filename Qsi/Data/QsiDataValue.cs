namespace Qsi.Data
{
    public sealed class QsiDataValue
    {
        #region Static
        public static QsiDataValue Unknown => _unknown ??= new QsiDataValue(null, QsiDataType.Unknown);

        public static QsiDataValue Unset => _unset ??= new QsiDataValue(null, QsiDataType.Unset);

        public static QsiDataValue Default => _default ??= new QsiDataValue(null, QsiDataType.Default);

        private static QsiDataValue _unknown;
        private static QsiDataValue _unset;
        private static QsiDataValue _default;
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
