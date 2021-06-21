using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Qsi.Extensions
{
    internal static class ValueTaskExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ValueTask<T> AsValueTask<T>(this T value)
        {
            return new(value);
        }
    }
}
