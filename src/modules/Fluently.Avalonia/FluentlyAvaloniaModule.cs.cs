using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace Fluently;

[DependsOn(typeof(FluentlyCoreModule))]
public class FluentlyAvaloniaModule : AbpModule
{
    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddSingleton<IThemeService>(static _ => new ThemeService(
            Application.Current
                ?? throw new InvalidOperationException(
                    "The IThemeService was resolved before Avalonia created the application."
                )
        ));
    }
}
