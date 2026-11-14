using Avalonia.Controls.Chrome;
using Avalonia.Input;
using Avalonia.Media.Imaging;

namespace Nexus.Extensions;

public static class AttachPropertyExtensions
{
    public static T WindowDecorationProperties_ElementRole<T>(
        this T control,
        WindowDecorationsElementRole role
    )
        where T : Control
    {
        WindowDecorationProperties.SetElementRole(control, role);
        return control;
    }

    public static T RenderOptions_BitmapInterpolationMode<T>(
        this T control,
        BitmapInterpolationMode mode
    )
        where T : Visual
    {
        RenderOptions.SetBitmapInterpolationMode(control, mode);
        return control;
    }
}
