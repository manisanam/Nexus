using Lucide.Avalonia;

namespace Nexus.ViewModels.Pages;

public class SettingsPageViewModel : PageViewModel
{
    public SettingsPageViewModel()
    {
        IsFooter = true;
        IsVisible = false;
    }

    public override LucideIconKind IconKind => LucideIconKind.Settings;
}
