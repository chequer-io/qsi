using System.Collections.Generic;

namespace Qsi.Data
{
    public interface IQsiActionSet<out T> : IQsiAction, IReadOnlyList<T> where T : IQsiAction
    {
    }
}
