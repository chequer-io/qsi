using System.Reflection;

namespace Qsi.Shared.Reflection;

internal abstract class MemberAccessor<TType, TValue> : IMemberAccessor<TType, TValue>
{
    protected delegate TValue Getter(TType obj);

    protected delegate void Setter(TType obj, TValue value);

    private readonly Getter _getter;
    private readonly Setter _setter;

    protected MemberAccessor(MemberInfo memberInfo)
    {
        _getter = CreateGetter(memberInfo);
        _setter = CreateSetter(memberInfo);
    }

    protected abstract Getter CreateGetter(MemberInfo memberInfo);

    protected abstract Setter CreateSetter(MemberInfo memberInfo);

    public TValue GetValue(TType obj)
    {
        return _getter(obj);
    }

    public void SetValue(TType obj, TValue value)
    {
        _setter(obj, value);
    }
}