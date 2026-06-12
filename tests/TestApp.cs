using Avalonia;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;

// Registers the headless Avalonia application that every [AvaloniaFact] runs
// against. Without an initialized runtime, loading the embedded avares://
// palette dictionaries throws — the tests exist precisely to load them.
[assembly: AvaloniaTestApplication(typeof(SciFiThemePlugin.Tests.TestApp))]

namespace SciFiThemePlugin.Tests;

public sealed class TestApp : Application
{
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<TestApp>().UseHeadless(new AvaloniaHeadlessPlatformOptions());
}
