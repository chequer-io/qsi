using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Qsi.Shared.Reflection
{
    internal sealed class PropertyAccessor<TType, TValue> : MemberAccessor<TType, TValue>
    {
        public PropertyAccessor(MemberInfo memberInfo) : base(memberInfo)
        {
        }

        protected override Getter CreateGetter(MemberInfo memberInfo)
        {
            var propertyInfo = (PropertyInfo)memberInfo;

            if (propertyInfo.GetMethod == null)
                return _ => throw new Exception("Getter not found.");

            var method = new DynamicMethod(
                $"{typeof(TType).Name}<{propertyInfo.Name}>_Getter",
                typeof(TValue),
                new[] { typeof(TType) }
            );

            var il = method.GetILGenerator();

            il.DeclareLocal(typeof(TValue));
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Callvirt, propertyInfo.GetMethod);
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ret);

            return method.CreateDelegate<Getter>();
        }

        protected override Setter CreateSetter(MemberInfo memberInfo)
        {
            var propertyInfo = (PropertyInfo)memberInfo;

            if (propertyInfo.SetMethod == null)
                return (_, _) => throw new Exception("Setter not found.");

            var method = new DynamicMethod(
                $"{typeof(TType).Name}<{propertyInfo.Name}>_Setter",
                typeof(void),
                new[] { typeof(TType), typeof(TValue) }
            );

            var il = method.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, propertyInfo.SetMethod);
            il.Emit(OpCodes.Ret);

            return method.CreateDelegate<Setter>();
        }
    }
}
