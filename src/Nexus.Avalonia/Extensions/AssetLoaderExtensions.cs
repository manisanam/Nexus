using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace Nexus.Extensions;

public static class AssetLoaderExtensions
{
    extension(AssetLoader)
    {
        public static Bitmap OpenAsBitmap(Uri uri, Uri? baseUri = null) =>
            new(AssetLoader.Open(uri, baseUri));

        public static Bitmap OpenAsBitmap(string uri, string? baseUri = null) =>
            new Bitmap(
                AssetLoader.Open(new Uri(uri), baseUri.IsNullOrEmpty() ? null : new Uri(baseUri))
            );
    }
}
