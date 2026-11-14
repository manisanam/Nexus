using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Styling;

namespace Nexus.Extensions;

public static class ControlExtensoins
{
    public static T DynamicResource<T>(this T control, AvaloniaProperty prop, object resourceKey)
        where T : Control
    {
        control[!prop] = new DynamicResourceExtension(resourceKey);
        return control;
    }

    public static Style DynamicResource(this Style style, AvaloniaProperty prop, object resourceKey)
    {
        style.Setters.Add(
            new Setter() { Property = prop, Value = new DynamicResourceExtension(resourceKey) }
        );
        return style;
    }
}
