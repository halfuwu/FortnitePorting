using Avalonia.Threading;
using FortnitePorting.Framework.Application;

namespace FortnitePorting.Framework.Services;

public static class TaskService
{
    private static List<int> RunningHashes = new();

    public static void Run(Func<Task> function, bool oneInstance = false)
    {
        Task.Run(async () =>
        {
            try
            {
                await function().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                AppBase.HandleException(e);
            }
        });
    }

    public static async Task RunAsync(Func<Task> function)
    {
        await Task.Run(async () =>
        {
            try
            {
                await function().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                AppBase.HandleException(e);
            }
        });
    }

    public static void Run(Action function)
    {
        Task.Run(() =>
        {
            try
            {
                function();
            }
            catch (Exception e)
            {
                AppBase.HandleException(e);
            }
        });
    }

    public static async Task RunAsync(Action function)
    {
        await Task.Run(() =>
        {
            try
            {
                function();
            }
            catch (Exception e)
            {
                AppBase.HandleException(e);
            }
        });
    }

    public static void RunDispatcher(Action function, DispatcherPriority priority = default)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            try
            {
                function();
            }
            catch (Exception e)
            {
                AppBase.HandleException(e);
            }
        }, priority);
    }

    public static async Task RunDispatcherAsync(Action function, DispatcherPriority priority = default)
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            try
            {
                function();
            }
            catch (Exception e)
            {
                AppBase.HandleException(e);
            }
        }, priority);
    }
}