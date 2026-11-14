using Nexus.ViewModels.Pages;

namespace Nexus.Views.Pages;

public class SettingsPageView : View<SettingsPageViewModel>
{
    protected override object Build(SettingsPageViewModel vm) =>
        new TextBlock().Text("Settings Page");
}
