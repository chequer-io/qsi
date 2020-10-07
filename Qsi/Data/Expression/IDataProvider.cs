namespace Qsi.Data
{
    public interface IDataProvider
    {
        QsiDataType Type { get; }

        object Value { get; }
    }

    public interface IDataProvider<out T> : IDataProvider
    {
        new T Value { get; }

        object IDataProvider.Value => Value;
    }
}
