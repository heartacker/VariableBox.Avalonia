using System.Globalization;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using VariableBox.Themes.Locale;

namespace VariableBox.Avalonia.Themes;

/// <summary>
/// Notice: Don't set Locale if your app is in InvariantGlobalization mode.
/// </summary>
public class SimpleTheme: Styles
{
    private static readonly Lazy<Dictionary<CultureInfo, ResourceDictionary>> _localeToResource 
        = new Lazy<Dictionary<CultureInfo, ResourceDictionary>>(
        () => new Dictionary<CultureInfo, ResourceDictionary>
        {
            { new CultureInfo("zh-CN"), new zh_cn() },
            { new CultureInfo("en-US"), new en_us() },
        });

    private static readonly ResourceDictionary _defaultResource = new zh_cn();

    private readonly IServiceProvider? sp;
    public SimpleTheme(IServiceProvider? provider = null)
    {
        sp = provider;
        AvaloniaXamlLoader.Load(provider, this);
    }

    private CultureInfo? _locale;
    public CultureInfo? Locale
    {
        get => _locale;
        set
        {
            try
            {
                _locale = value;
                var resource = TryGetLocaleResource(value);
                if (resource is null) return;
                foreach (var kv in resource)
                {
                    this.Resources[kv.Key] = kv.Value;
                }
            }
            catch
            {
                _locale = CultureInfo.InvariantCulture;
            }

        }
    }

    private static ResourceDictionary? TryGetLocaleResource(CultureInfo? locale)
    {
        if (Equals(locale, CultureInfo.InvariantCulture))
        {
            return _defaultResource;
        }
        if (locale is null)
        {
            return _localeToResource.Value[new CultureInfo("zh-CN")];
        }
        if (_localeToResource.Value.TryGetValue(locale, out var resource))
        {
            return resource;
        }
        return _localeToResource.Value[new CultureInfo("zh-CN")];
    }
}