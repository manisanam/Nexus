namespace Nexus.Views.Pages;

public class HomePageView : View<HomePageViewModel>
{
    protected override object Build(HomePageViewModel vm) =>
        new Button().Content("Set Busy").Command(vm, x => x.SetGlobalBusyCommand);
}
