using Configuration.Writable;
using Configuration.Writable.FormatProvider;
using Fluently;
using Nexus.Settings;
using Volo.Abp.Autofac;
using Volo.Abp.Avalonia;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace Nexus;

[DependsOn(
    typeof(NexusHttpApiClientModule),
    typeof(AbpVirtualFileSystemModule),
    typeof(FluentlyAvaloniaModule),
    typeof(AbpAvaloniaModule),
    typeof(AbpAutofacModule)
)]
public partial class NexusAvaloniaModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddWritableOptions(builder =>
        {
            var appPaths = context.Services.GetSingletonInstance<AppPaths>();
            builder.UseCustomDirectory(appPaths.Config).AddFilePath("settings.json");
            builder.Add<AppSettings>(c =>
            {
                c.SectionName = AppSettings.SectionName;
                c.FormatProvider = new JsonAotFormatProvider(
                    AppSettings.ApplicationSettingsJsonSerializerContext.Default
                );
            });
        });
        context.Services.AddConventionalRegistrar(new ViewModelConventionalRegistrar());
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<NexusAvaloniaModule>("Nexus");
        });
    }
}
