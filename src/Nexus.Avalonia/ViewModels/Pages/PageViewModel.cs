using Fluently;
using Humanizer;
using Lucide.Avalonia;
using Volo.Abp.DynamicProxy;

namespace Nexus.ViewModels.Pages;

public abstract class PageViewModel : ViewModel, INavigableViewModel
{
    private readonly Type _type;

    protected PageViewModel()
    {
        _type = ProxyHelper.GetUnProxiedType(this);
        Name = _type.Name.Replace("PageViewModel", string.Empty).Humanize(LetterCasing.Title);
    }

    /// <summary>
    /// The Order of the page.
    /// </summary>
    public int Order { get; protected set; } = int.MaxValue;

    /// <summary>
    /// The display name of the page.
    /// </summary>
    public string Name { get; protected set; }
    public bool IsFooter { get; protected set; }
    public bool IsVisible { get; protected set; } = true;

    /// <summary>
    /// The icon of the page.
    /// </summary>
    public abstract LucideIconKind IconKind { get; }

    public static implicit operator FANavigationViewItem(PageViewModel pageViewModel) =>
        new()
        {
            Content = pageViewModel.Name,
            IconSource = new FAPathIconSource
            {
                Data = StreamGeometry.Parse(pageViewModel.IconKind.GetGeometryData()),
            },
            IsVisible = pageViewModel.IsVisible,
            Tag = pageViewModel._type,
        };
}
