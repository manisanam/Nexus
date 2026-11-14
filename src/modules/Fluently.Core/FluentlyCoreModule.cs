using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace Fluently;

public class FluentlyCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddSingleton(new ToastOptions());
        context.Services.AddSingleton(static provider => new ViewModelScopes(provider));
        context.Services.AddSingleton(static provider => new ViewProvider(
            provider.GetRequiredService<ViewModelScopes>()
        ));
        context.Services.AddSingleton<IViewProvider>(static provider =>
            provider.GetRequiredService<ViewProvider>()
        );
        context.Services.AddSingleton<INavigationService, NavigationService>();
        context.Services.AddSingleton<INavigationServiceFactory, NavigationServiceFactory>();
        context.Services.AddSingleton<IDialogService, DialogService>();
        context.Services.AddSingleton(TimeProvider.System);
        context.Services.AddSingleton<IToastService, ToastService>();
    }
}
