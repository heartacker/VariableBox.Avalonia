using Avalonia;
using Avalonia.Headless;
using Avalonia.Themes.Fluent;
using Avalonia.Markup.Xaml.Styling;
using System;
using VariableBox.Tests;

[assembly: AvaloniaTestApplication(typeof(TestApp))]

namespace VariableBox.Tests;

public class TestApp : Application
{
    public override void Initialize()
    {
        Styles.Add(new FluentTheme());
        Resources.MergedDictionaries.Add(new ResourceInclude(new Uri("avares://VariableBox/"))
        {
            Source = new Uri("avares://VariableBox/Controls/_index.axaml")
        });
    }

    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<TestApp>()
        .UseHeadless(new AvaloniaHeadlessPlatformOptions());
}
