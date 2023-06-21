using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Qsi.Shared.Extensions;

internal static class EnumerableExtension
{
    public static int IndexOf<T>(this IEnumerable<T> source, T find)
    {
        return source.IndexOf(value => Equals(value, find));
    }

    public static int IndexOf<T>(this IEnumerable<T> source, Predicate<T> predicate)
    {
        int index = -1;

        foreach (var value in source)
        {
            index++;

            if (predicate(value))
                return index;
        }

        return -1;
    }

    public static IEnumerable<int> AllIndexOf<T>(this IEnumerable<T> source, T find)
    {
        return source.AllIndexOf(value => Equals(value, find));
    }

    public static IEnumerable<int> AllIndexOf<T>(this IEnumerable<T> source, Predicate<T> predicate)
    {
        int index = -1;

        foreach (var value in source)
        {
            index++;

            if (predicate(value))
                yield return index;
        }
    }

    public static bool TryCast<TOut>(this IEnumerable source, out TOut[] cast)
    {
        if (!source.TryCast(out IEnumerable<TOut> result))
        {
            cast = null;
            return false;
        }

        if (result is TOut[] tArray)
            cast = tArray;
        else
            cast = result.ToArray();

        return true;
    }

    public static bool TryCast<TOut>(this IEnumerable source, out IEnumerable<TOut> cast)
    {
        switch (source)
        {
            case null:
                cast = null;
                return false;

            case IEnumerable<TOut> result:
                cast = result;
                return true;
        }

        foreach (var element in source)
        {
            if (element is not TOut)
            {
                cast = null;
                return false;
            }
        }

        cast = source.Cast<TOut>();
        return true;
    }
}