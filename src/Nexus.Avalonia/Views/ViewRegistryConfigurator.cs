using System.Runtime.CompilerServices;
using Fluently;
using Nexus.ViewModels;
using ServiceScan.SourceGenerator;

namespace Nexus.Views;

public static partial class ViewRegistryConfigurator
{
    [ModuleInitializer]
    [ScanForTypes(AssignableTo = typeof(IViewFor<>), Handler = nameof(RegisterHandler))]
    public static partial void Register();

    private static void RegisterHandler<TView, TViewModel>()
        where TView : Control
        where TViewModel : ViewModel
    {
        ViewRegistry.Add<TViewModel>(
            (sp, viewModel) =>
            {
                var view = ActivatorUtilities.GetServiceOrCreateInstance<TView>(sp);
                view.DataContext = viewModel;
                return (IViewFor<TViewModel>)view;
            }
        );
    }
}
