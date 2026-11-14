using Fluently;
using Volo.Abp.DependencyInjection;

namespace Nexus.Services;

[ExposeServices(typeof(IFANavigationPageFactory))]
public class NavigationPageFactory : IFANavigationPageFactory, ISingletonDependency
{
    private readonly IViewProvider _viewProvider;

    public NavigationPageFactory(IViewProvider viewProvider)
    {
        _viewProvider = viewProvider;
    }

    public Control GetPage(Type srcType)
    {
        throw new NotImplementedException();
    }

    public Control GetPageFromObject(object target)
    {
        return (Control)_viewProvider.GetView(target);
    }
}
