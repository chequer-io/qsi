namespace Qsi.Data
{
    public sealed class QsiDataRow
    {
        public int Length { get; }

        public object[] Values { get; }

        public QsiDataType[] Types { get; }

        public QsiDataRow(int length)
        {
            Length = length;
            Values = new object[length];
            Types = new QsiDataType[length];
        }
    }
}
