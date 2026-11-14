using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using FluentAvalonia.UI.Navigation;
using Fluently;
using Lucide.Avalonia;
using Nexus.Controls;
using Nexus.Extensions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DynamicProxy;

namespace Nexus.Views;

public class MainView : View<MainViewModel>, ISingletonDependency
{
    private FANavigationView _navView = null!;
    private FAFrame _frameView = null!;

    public MainView(MainViewModel viewModel)
        : base()
    {
        DataContext = viewModel;
    }

    protected override void OnInitialized()
    {
        var pageViewModels = LazyServiceProvider.GetServices<PageViewModel>();
        PopulateNavigation(pageViewModels);

        var navigationPageFactory =
            LazyServiceProvider.GetRequiredService<IFANavigationPageFactory>();
        _frameView.NavigationPageFactory = navigationPageFactory;
        ViewModel.NavigationService.Navigated += NavigationServiceOnNavigated;
        _navView.SelectionChanged += NavViewOnSelectionChanged;
        _navView.BackRequested += NavViewOnBackRequested;

        ViewModel.NavigationService.NavigateToAsync<HomePageViewModel>();
    }

    private void NavViewOnBackRequested(object? sender, FANavigationViewBackRequestedEventArgs e)
    {
        if (_frameView.CanGoBack)
        {
            _frameView.GoBack();
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _navView.SelectionChanged -= NavViewOnSelectionChanged;
        _navView.BackRequested -= NavViewOnBackRequested;
        ViewModel.NavigationService.Navigated -= NavigationServiceOnNavigated;
    }

    private void NavViewOnSelectionChanged(
        object? sender,
        FANavigationViewSelectionChangedEventArgs e
    )
    {
        if (e.SelectedItem is not FANavigationViewItem nvi)
            return;

        var target = nvi.Name == "SettingsItem" ? typeof(SettingsPageViewModel) : nvi.Tag as Type;

        if (target is null)
            return;

        ViewModel.NavigationService.NavigateToAsync(target).WaitOnDispatcher();
    }

    private void NavigationServiceOnNavigated(object? sender, NavigatedEventArgs e)
    {
        _frameView.NavigateFromObject(
            e.Page,
            new FAFrameNavigationOptions { IsNavigationStackEnabled = true }
        );
    }

    private void PopulateNavigation(IEnumerable<PageViewModel> pageViewModels)
    {
        var itemsByFooter = pageViewModels.OrderBy(x => x.Order).ToLookup(page => page.IsFooter);

        _navView.MenuItemsSource = itemsByFooter[false].Select(CreateNavigationViewItem);
        _navView.FooterMenuItemsSource = itemsByFooter[true].Select(CreateNavigationViewItem);
    }

    private static FANavigationViewItem CreateNavigationViewItem(PageViewModel pageViewModel) =>
        new()
        {
            Content = pageViewModel.Name,
            IconSource = new FAPathIconSource
            {
                Data = StreamGeometry.Parse(pageViewModel.IconKind.GetGeometryData()),
            },
            IsVisible = pageViewModel.IsVisible,
            Tag = ProxyHelper.GetUnProxiedType(pageViewModel),
        };

    protected override object Build(MainViewModel vm) =>
        new Grid()
            .Rows("Auto, *")
            .Children(
                new Grid()
                    .Name("TitleBarHost", Scope)
                    .Cols("Auto,Auto,*,Auto")
                    .Background(Brushes.Transparent)
                    .WindowDecorationProperties_ElementRole(WindowDecorationsElementRole.TitleBar)
                    .Children(
                        new Image()
                            .Margin(12, 4)
                            .IsHitTestVisible(false)
                            .Source(
                                Bitmap.DecodeToWidth(
                                    AssetLoader.Open(
                                        new Uri(
                                            "avares://Nexus.Avalonia/Assets/Images/PNB-Logo-Short-011117-FC-HiRes.png"
                                        )
                                    ),
                                    48
                                )
                            )
                            .Width(48)
                            .Height(48)
                            .DockPanel_Dock(Dock.Left)
                            .Name("WindowIcon", Scope)
                            .RenderOptions_BitmapInterpolationMode(
                                BitmapInterpolationMode.HighQuality
                            )
                            .IsVisible(OperatingSystem.IsWindows()),
                        new TextBlock()
                            .Text(
                                new Binding(nameof(Window.Title))
                                {
                                    RelativeSource = new RelativeSource
                                    {
                                        Mode = RelativeSourceMode.FindAncestor,
                                        AncestorType = typeof(Window),
                                    },
                                }
                            )
                            .FontSize(12)
                            .IsHitTestVisible(false)
                            .VerticalAlignment(VerticalAlignment.Center)
                            .Grid_Column(1)
                            .IsVisible(OperatingSystem.IsWindows())
                    ),
                new BusyArea()
                    .WindowDecorationProperties_ElementRole(WindowDecorationsElementRole.User)
                    .Grid_Row(1)
                    .IsBusy(vm, x => x.IsBusy, BindingMode.OneWay)
                    .BusyText(vm, x => x.BusyText, BindingMode.OneWay)
                    .Content(
                        new ToastHost()
                            .Toasts(vm, x => x.ToastService, BindingMode.OneWay)
                            .Content(
                                new DialogHost()
                                    .Dialogs(vm, x => x.DialogService, BindingMode.OneWay)
                                    .DismissOnBackdropClick(true)
                                    .DynamicResource(
                                        DialogHost.BackdropProperty,
                                        "SmokeFillColorDefaultBrush"
                                    )
                                    .Content(
                                        new FANavigationView()
                                            .AlwaysShowHeader(true)
                                            .WindowDecorationProperties_ElementRole(
                                                WindowDecorationsElementRole.User
                                            )
                                            .PaneDisplayMode(FANavigationViewPaneDisplayMode.Left)
                                            .Name("NavView", Scope)
                                            .Ref(out _navView)
                                            .IsSettingsVisible(true)
                                            .IsBackButtonVisible(true)
                                            .IsBackEnabled(false)
                                            .Content(
                                                new Border()
                                                    .DynamicResource(
                                                        Border.BorderBrushProperty,
                                                        "CardStrokeColorDefaultBrush"
                                                    )
                                                    .BorderThickness(1)
                                                    .CornerRadius(8)
                                                    .Child(
                                                        new FAFrame()
                                                            .Name("FrameView", Scope)
                                                            .Ref(out _frameView)
                                                            .OnNavigated(
                                                                (_, args) =>
                                                                {
                                                                    _navView.IsBackEnabled =
                                                                        _frameView.CanGoBack;
                                                                }
                                                            )
                                                    )
                                            )
                                    )
                            )
                    )
            );
}
