using System.Reflection;
using System.Reflection.Emit;

namespace Qsi.Shared.Reflection;

internal sealed class FieldAccessor<TType, TValue> : MemberAccessor<TType, TValue>
{
    public FieldAccessor(FieldInfo fieldInfo) : base(fieldInfo)
    {
    }

    protected override Getter CreateGetter(MemberInfo memberInfo)
    {
        var fieldInfo = (FieldInfo)memberInfo;

        var method = new DynamicMethod(
            $"{typeof(TType).Name}<{fieldInfo.Name}>_Getter",
            typeof(TValue),
            new[] { typeof(TType) }
        );

        var il = method.GetILGenerator();

        il.DeclareLocal(typeof(TValue));
        il.Emit(OpCodes.Nop);
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldfld, fieldInfo);
        il.Emit(OpCodes.Stloc_0);
        il.Emit(OpCodes.Ldloc_0);
        il.Emit(OpCodes.Ret);

        return method.CreateDelegate<Getter>();
    }

    protected override Setter CreateSetter(MemberInfo memberInfo)
    {
        var fieldInfo = (FieldInfo)memberInfo;

        var method = new DynamicMethod(
            $"{typeof(TType).Name}<{fieldInfo.Name}>_Setter",
            typeof(void),
            new[] { typeof(TType), typeof(TValue) }
        );

        var il = method.GetILGenerator();

        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Stfld, fieldInfo);
        il.Emit(OpCodes.Ret);

        return method.CreateDelegate<Setter>();
    }
}