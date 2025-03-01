using Avalonia;
using Avalonia.Media;

namespace Ursa.Demo.Browser;

public static class AvaloniaAppBuilderExtensions
{
    private static string DefaultFontFamily => "avares://VariableBox.Demo.Browser/Assets#Source Han Sans CN";
    //private static string DefaultFontFamily => "Courier New";

    public static AppBuilder WithSourceHanSansCNFont(this AppBuilder builder) =>
        builder.With(new FontManagerOptions
        {
            DefaultFamilyName = DefaultFontFamily,
            FontFallbacks = new[] { new FontFallback { FontFamily = new FontFamily(DefaultFontFamily) } }
        });
}