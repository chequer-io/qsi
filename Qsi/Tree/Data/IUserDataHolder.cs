namespace Qsi.Tree.Data;

public interface IUserDataHolder
{
    T GetData<T>(Key<T> key);

    void PutData<T>(Key<T> key, T value);
}