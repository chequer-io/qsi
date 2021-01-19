using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Qsi.Utilities
{
    internal static class HashCodeUtility
    {
        public static int Combine<T>(IEnumerable<T> enumerable)
        {
            if (enumerable is not IList list)
            {
                list = enumerable.Take(9).ToArray();
            }

            switch (list.Count)
            {
                case 0:
                    return 0;

                case 1:
                    return HashCode.Combine(list[0]);

                case 2:
                    return HashCode.Combine(list[0], list[1]);

                case 3:
                    return HashCode.Combine(list[0], list[1], list[2]);

                case 4:
                    return HashCode.Combine(list[0], list[1], list[2], list[3]);

                case 5:
                    return HashCode.Combine(
                        list[0], list[1], list[2], list[3],
                        list[4]);

                case 6:
                    return HashCode.Combine(
                        list[0], list[1], list[2], list[3],
                        list[4], list[5]);

                case 7:
                    return HashCode.Combine(
                        list[0], list[1], list[2], list[3],
                        list[4], list[5], list[6]);

                case 8:
                    return HashCode.Combine(
                        list[0], list[1], list[2], list[3],
                        list[4], list[5], list[6], list[7]);

                default:
                    throw new NotSupportedException("The elements of combine are supported from 0 to 8.");
            }
        }
    }
}
