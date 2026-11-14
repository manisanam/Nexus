using AsyncImageLoader;
using Avalonia.Controls.Chrome;
using Avalonia.Threading;
using AvaloniaRichEditor.Controls;
using FluentAvalonia.Styling;
using FluentAvalonia.UI.Windowing;
using Fluently;
using Lucide.Avalonia;
using Nexus.Extensions;

[assembly: GenerateMarkupExtensionsForAvalonia(true)]
[assembly: GenerateMarkupExtensionsForAssembly(typeof(LucideIcon))]
[assembly: GenerateMarkupExtensionsForAssembly(typeof(AdvancedImage))]
[assembly: GenerateMarkupExtensionsForAssembly(typeof(FAAppWindow))]
[assembly: GenerateMarkupExtensionsForAssembly(typeof(RichEditor))]
[assembly: GenerateMarkupExtensionsForAssembly(typeof(WindowDecorationProperties))]
[assembly: GenerateMarkupExtensionsForAssembly(typeof(FluentlyAvaloniaModule))]

namespace Nexus;

public class App : Application
{
    public App(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    protected IServiceProvider ServiceProvider { get; }

    public override void Initialize()
    {
        Styles.Add(
            new FluentAvaloniaTheme { PreferSystemTheme = true, PreferUserAccentColor = true }
        );
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
        {
            Dispatcher.UIThread.UnhandledException += OnUnhandledException;

            desktopLifetime.MainWindow = ServiceProvider.GetRequiredService<MainWindow>();

            ServiceProvider
                .GetRequiredService<INavigationService>()
                .NavigateToAsync<HomePageViewModel>()
                .WaitOnDispatcher();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void OnUnhandledException(object? sender, DispatcherUnhandledExceptionEventArgs e)
    {
        ServiceProvider
            .GetRequiredService<ILogger<App>>()
            .LogError(e.Exception, "Unhandled exception on the UI thread");
        //ServiceProvider
        //    .GetRequiredService<IToastService>()
        //    .ShowError("Something went wrong", e.Exception.Message);
        e.Handled = true;
    }
}
