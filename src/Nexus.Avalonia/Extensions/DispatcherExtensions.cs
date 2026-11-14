using Avalonia.Threading;

namespace Nexus.Extensions;

public static class DispatcherExtensions
{
    extension<T>(Task<T> task)
    {
        public T WaitOnDispatcher() => WaitOnDispatcherFrame<T>(task);
    }

    extension(Task task)
    {
        public void WaitOnDispatcher() => WaitOnDispatcherFrame(task);
    }

    private static T WaitOnDispatcherFrame<T>(Task<T> task)
    {
        if (!task.IsCompleted)
        {
            EnsureLifetime();
            var frame = new DispatcherFrame();
            task.ContinueWith(static (_, s) => ((DispatcherFrame)s!).Continue = false, frame);
            Dispatcher.UIThread.PushFrame(frame);
        }

        return task.GetAwaiter().GetResult();
    }

    private static void WaitOnDispatcherFrame(Task task)
    {
        if (!task.IsCompleted)
        {
            EnsureLifetime();
            var frame = new DispatcherFrame();
            task.ContinueWith(static (_, s) => ((DispatcherFrame)s!).Continue = false, frame);
            Dispatcher.UIThread.PushFrame(frame);
        }

        task.GetAwaiter().GetResult();
    }

    private static void EnsureLifetime()
    {
        if (!CheckLifetime())
        {
            throw new InvalidOperationException("lifetime has not been ready yet!");
        }
    }

    private static bool CheckLifetime()
    {
        var lifetimeReady =
            Application.Current != null && Application.Current.ApplicationLifetime != null;
        if (
            Application.Current!.ApplicationLifetime
            is IClassicDesktopStyleApplicationLifetime desktopStyleApplicationLifetime
        )
        {
            lifetimeReady = lifetimeReady && desktopStyleApplicationLifetime.MainWindow != null;
        }

        return lifetimeReady;
    }
}
