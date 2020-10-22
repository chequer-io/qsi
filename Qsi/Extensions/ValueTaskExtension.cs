using System.Threading.Tasks;

namespace Qsi.Extensions
{
    internal static class ValueTaskExtension
    {
        public static ValueTask<T> AsValueTask<T>(this T value)
        {
            return new ValueTask<T>(value);
        }
    }
}
