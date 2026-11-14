using System.Globalization;
using Avalonia.Animation;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Data.Converters;
using Avalonia.Styling;
using FluentAvalonia.UI.Controls;

namespace Nexus.Controls;

public class BusyArea : ContentControl, IDeclarativeViewBase
{
    public static readonly StyledProperty<bool> IsBusyProperty = AvaloniaProperty.Register<
        BusyArea,
        bool
    >(nameof(IsBusy), defaultValue: false);

    public static readonly StyledProperty<string?> BusyTextProperty = AvaloniaProperty.Register<
        BusyArea,
        string?
    >(nameof(BusyText), defaultValue: null);

    public bool IsBusy
    {
        get => GetValue(IsBusyProperty);
        set => SetValue(IsBusyProperty, value);
    }

    public string? BusyText
    {
        get => GetValue(BusyTextProperty);
        set => SetValue(BusyTextProperty, value);
    }

    public BusyArea()
    {
        Template = new FuncControlTemplate<BusyArea>(
            (busyArea, nameScope) =>
            {
                var busyContent = new ContentControl()
                    .Name("BusyContent")
                    .Content(new TemplateBinding(ContentControl.ContentProperty))
                    .ContentTemplate(new TemplateBinding(ContentControl.ContentTemplateProperty))
                    .Opacity(
                        new Binding(nameof(IsBusy))
                        {
                            Source = busyArea,
                            Converter = BusyContentOpacityConverter.Instance,
                        }
                    )
                    .IsHitTestVisible(
                        new Binding(nameof(IsBusy))
                        {
                            Source = busyArea,
                            Converter = InverseBooleanConverter.Instance,
                        }
                    )
                    .Transitions(
                        new Transitions
                        {
                            new DoubleTransition
                            {
                                Property = OpacityProperty,
                                Duration = TimeSpan.FromSeconds(0.3),
                            },
                        }
                    );

                var busyText = new TextBlock()
                    .Name("PART_BusyText")
                    .Margin(3)
                    .FontWeight(FontWeight.DemiBold)
                    .Text(new Binding(nameof(BusyText)) { Source = busyArea })
                    .IsVisible(new Binding(nameof(IsBusy)) { Source = busyArea });

                DockPanel.SetDock(busyText, Dock.Bottom);

                var progressPresenter = new ContentPresenter()
                    .HorizontalAlignment(HorizontalAlignment.Center)
                    .VerticalAlignment(VerticalAlignment.Center)
                    .IsHitTestVisible(false)
                    .Content(
                        new Binding(nameof(IsBusy))
                        {
                            Source = busyArea,
                            Converter = ProgressToContentConverter.Instance,
                        }
                    );

                var loadingBusyArea = new DockPanel
                {
                    Name = "LoadingBusyArea",
                    LastChildFill = true,
                }
                    .HorizontalAlignment(HorizontalAlignment.Center)
                    .VerticalAlignment(VerticalAlignment.Center)
                    .Opacity(
                        new Binding(nameof(IsBusy))
                        {
                            Source = busyArea,
                            Converter = BusyOverlayOpacityConverter.Instance,
                        }
                    )
                    .IsHitTestVisible(new Binding(nameof(IsBusy)) { Source = busyArea })
                    .Transitions(
                        new Transitions
                        {
                            new DoubleTransition
                            {
                                Property = OpacityProperty,
                                Duration = TimeSpan.FromSeconds(0.3),
                            },
                        }
                    )
                    .Children(busyText, progressPresenter);

                return new Panel().Children(busyContent, loadingBusyArea);
            }
        );
    }
}

file sealed class BusyOverlayOpacityConverter : IValueConverter
{
    public static BusyOverlayOpacityConverter Instance { get; } = new();

    private BusyOverlayOpacityConverter() { }

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is true ? 1.0 : 0.0;
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        throw new NotSupportedException();
    }
}

file sealed class BusyContentOpacityConverter : IValueConverter
{
    public static BusyContentOpacityConverter Instance { get; } = new();

    private BusyContentOpacityConverter() { }

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is true ? 0.1 : 1.0;
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        throw new NotSupportedException();
    }
}

file sealed class InverseBooleanConverter : IValueConverter
{
    public static InverseBooleanConverter Instance { get; } = new();

    private InverseBooleanConverter() { }

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is not true;
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        throw new NotSupportedException();
    }
}

file sealed class ProgressToContentConverter : IValueConverter
{
    public static readonly ProgressToContentConverter Instance = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is true)
        {
            return new FAProgressRing
            {
                IsActive = true,
                Width = 50,
                Height = 50,
            };
        }
        return new Panel();
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        throw new NotSupportedException();
    }
}
