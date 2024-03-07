using System;
using System.IO;
using Avalonia;
using Avalonia.Platform;

namespace Qsi.Debugger.Utilities;

internal static class AssetManager
{
    public static Stream FindResource(string resource)
    {
        var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
        var resourceUri = new Uri($"avares://Qsi.Debugger/Assets/{resource}");

        return assets.Open(resourceUri);
    }
}