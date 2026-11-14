using FluentAvalonia.UI.Windowing;
using Volo.Abp.DependencyInjection;

namespace Nexus.Views;

public class MainWindow : FAAppWindow, ISingletonDependency
{
    public MainWindow()
    {
        Title = NexusConsts.Name;
        TitleBar.ExtendsContentIntoTitleBar = true;

        Content = ViewFactory.Create<MainView>();
    }
}
