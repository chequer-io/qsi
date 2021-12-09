using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace Qsi.Shared.Reflection
{
    internal static class ReflectionHelper
    {
        private static readonly ConcurrentDictionary<CacheKey, object> _accessors = new();

        public static IMemberAccessor<TType, TValue> GetAccessor<TType, TValue>(string memberName)
        {
            MemberInfo[] memberInfos = typeof(TType).GetMember(
                memberName,
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
            );

            if (memberInfos.Length == 0)
                throw new Exception($"{typeof(TType).Name}.{memberName} not found.");

            if (memberInfos.Length != 1)
                throw new AmbiguousMatchException($"{typeof(TType).Name}.{memberName}");

            var key = new CacheKey(typeof(TType), memberInfos[0]);

            if (!_accessors.TryGetValue(key, out var accessor))
            {
                accessor = memberInfos[0] switch
                {
                    FieldInfo fieldInfo => new FieldAccessor<TType, TValue>(fieldInfo),
                    PropertyInfo propertyInfo => new PropertyAccessor<TType, TValue>(propertyInfo),
                    _ => throw new NotSupportedException()
                };

                _accessors[key] = accessor;
            }

            return (IMemberAccessor<TType, TValue>)accessor;
        }

        private readonly struct CacheKey
        {
            public readonly Type Type;
            public readonly MemberInfo MemberInfo;

            public CacheKey(Type type, MemberInfo memberInfo)
            {
                Type = type;
                MemberInfo = memberInfo;
            }
        }
    }
}
