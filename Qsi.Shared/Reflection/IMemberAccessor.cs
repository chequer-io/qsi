namespace Qsi.Shared.Reflection
{
    internal interface IMemberAccessor<in TType, TValue>
    {
        TValue GetValue(TType obj);

        void SetValue(TType obj, TValue value);
    }
}
