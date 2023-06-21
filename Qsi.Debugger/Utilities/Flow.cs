using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Qsi.Debugger.Collections;

namespace Qsi.Debugger.Utilities;

public static class Flow
{
    public delegate bool Predicate();

    public delegate Task<bool> AsyncPredicate();

    private static readonly ConcurrentHashSet<object> ThrottleKeys;
    private static readonly ConcurrentDictionary<object, CancellationTokenSource> DebounceTokens;

    static Flow()
    {
        ThrottleKeys = new ConcurrentHashSet<object>();
        DebounceTokens = new ConcurrentDictionary<object, CancellationTokenSource>();
    }

    private static bool ThottleInternal(object key, int milliseconds = 1000)
    {
        if (ThrottleKeys.Contains(key))
            return false;

        if (milliseconds > 0)
        {
            ThrottleKeys.Add(key);

            Task.Delay(milliseconds).ContinueWith(_ =>
            {
                ThrottleKeys.Remove(key);
            });
        }

        return true;
    }

    public static void Throttle(object key, Action action, int milliseconds = 1000)
    {
        if (ThottleInternal(key, milliseconds))
        {
            action();
        }
    }

    public static async Task ThrottleAsync(object key, Func<Task> asyncAction, int milliseconds = 1000)
    {
        if (ThottleInternal(key, milliseconds))
        {
            await asyncAction();
        }
    }

    public static void Debounce(object key, Action action, int milliseconds = 1000)
    {
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        if (DebounceTokens.TryRemove(key, out var tokenSource))
        {
            tokenSource.Cancel();
        }

        if (milliseconds == 0)
        {
            action.Invoke();
            return;
        }

        tokenSource = new CancellationTokenSource();
        DebounceTokens.TryAdd(key, tokenSource);

        var taskScheduler = SynchronizationContext.Current != null
            ? TaskScheduler.FromCurrentSynchronizationContext()
            : TaskScheduler.Current;

        Task.Delay(milliseconds, tokenSource.Token).ContinueWith(_ =>
            {
                action.Invoke();
                DebounceTokens.TryRemove(key, out tokenSource);
            },
            tokenSource.Token,
            TaskContinuationOptions.ExecuteSynchronously,
            taskScheduler);
    }

    public static bool Retry(Predicate predicate, int tryCount, int tryDelay = 1000, bool throwException = false)
    {
        do
        {
            try
            {
                if (predicate())
                    return true;
            }
            catch (Exception)
            {
                if (throwException)
                    throw;
            }

            Thread.Sleep(tryDelay);
        } while (tryCount-- > 0);

        return false;
    }

    public static async Task<bool> RetryAsync(AsyncPredicate predicate, int tryCount, int tryDelay = 1000, bool throwException = false)
    {
        do
        {
            try
            {
                if (await predicate())
                    return true;
            }
            catch (Exception)
            {
                if (throwException)
                    throw;
            }

            await Task.Delay(tryDelay);
        } while (tryCount-- > 0);

        return false;
    }

    public static void TryInvoke(Action action, Action<Exception> exceptionCallback = null)
    {
        try
        {
            action();
        }
        catch (Exception e)
        {
            exceptionCallback?.Invoke(e);
        }
    }

    public static T TryInvoke<T>(Func<T> func, Func<Exception, T> exceptionCallback = null)
    {
        try
        {
            return func();
        }
        catch (Exception e)
        {
            if (exceptionCallback != null)
                return exceptionCallback(e);
        }

        return default;
    }

    public static async Task TryInvokeAsync(Task task, Action<Exception> exceptionCallback = null)
    {
        try
        {
            await task;
        }
        catch (Exception e)
        {
            exceptionCallback?.Invoke(e);
        }
    }

    public static async Task<T> TryInvokeAsync<T>(Task<T> task, Func<Exception, T> exceptionCallback = null)
    {
        try
        {
            await task;
        }
        catch (Exception e)
        {
            if (exceptionCallback != null)
                return exceptionCallback(e);
        }

        return default;
    }
}