using Avalonia.Controls;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Media;
using Avalonia.Headless.XUnit;
using FluentAssertions;
using SoundBoard.PluginApi;
using Xunit;

namespace SciFiThemePlugin.Tests;

/// <summary>
/// Verifies the shipped palette catalog, that each palette's embedded
/// resource dictionary loads, that every host semantic key is present as a
/// <see cref="SolidColorBrush"/>, and that no palette regressed back to the
/// old Dark/Light <c>ThemeDictionaries</c> variant model.
/// </summary>
public class PaletteTests
{
    /// <summary>
    /// The 25 semantic brush keys the host merges over its own when a palette
    /// is selected. Every palette must define all of them, or a user picking
    /// the theme would meet an unstyled control.
    /// </summary>
    public static readonly string[] SemanticKeys =
    {
        "SidebarBackground", "ContentBackground",
        "PanelBackground1", "PanelBackground2", "PanelBackground3",
        "SubtleBorder",
        "PrimaryAccent", "PrimaryAccentHover", "OnPrimaryAccent", "SecondaryAccent",
        "TextPrimary", "TextSecondary",
        "SuccessBackground", "SuccessForeground",
        "DangerBackground", "DangerForeground",
        "InfoBackground", "InfoForeground",
        "WarningBackground", "WarningForeground",
        "DropZoneHighlight", "WaveformBrush",
        "LoopInheritForeground", "LoopForceOnForeground", "LoopForceOffForeground",
    };

    public static IEnumerable<object[]> Palettes =>
        new SciFiThemePlugin().GetPalettes()
            .Select(p => new object[] { p.Id, p.ResourceUris.Single() });

    public static IEnumerable<object[]> PaletteKeys =>
        from p in new SciFiThemePlugin().GetPalettes()
        from key in SemanticKeys
        select new object[] { p.Id, p.ResourceUris.Single(), key };

    // ── Palette catalog ────────────────────────────────────────────────

    [Fact]
    public void GetPalettes_returns_the_two_shipped_palettes()
    {
        var palettes = new SciFiThemePlugin().GetPalettes().ToList();

        palettes.Should().HaveCount(2);
        palettes.Select(p => p.Id).Should()
            .Equal("starship-command", "neon-cyberpunk");
        palettes.Select(p => p.Name).Should()
            .Equal("Starship Command", "Neon Cyberpunk");
        palettes.Should().OnlyContain(
            p => p.ResourceUris.Single()
                == $"avares://SciFiThemePlugin/Themes/{Axaml(p.Id)}");
    }

    private static string Axaml(string id) => id switch
    {
        "starship-command" => "StarshipCommand.axaml",
        "neon-cyberpunk"   => "NeonCyberpunk.axaml",
        _                  => throw new ArgumentOutOfRangeException(nameof(id), id, null),
    };

    // ── Resources resolve ──────────────────────────────────────────────

    [AvaloniaTheory]
    [MemberData(nameof(Palettes))]
    public void Palette_dictionary_loads_and_is_not_empty(string id, string uri)
    {
        Load(uri).Count.Should().BeGreaterThan(0,
            "palette '{0}' must contribute at least one brush", id);
    }

    // ── Semantic-key completeness (the important test) ──────────────────

    [AvaloniaTheory]
    [MemberData(nameof(PaletteKeys))]
    public void Palette_defines_every_host_key_as_a_brush(string id, string uri, string key)
    {
        var dict = Load(uri);

        dict.TryGetResource(key, null, out var value).Should().BeTrue(
            "palette '{0}' must define host key '{1}'", id, key);
        value.Should().BeOfType<SolidColorBrush>(
            "host key '{0}' in palette '{1}' must resolve to a SolidColorBrush", key, id);
    }

    // ── Flatness guard ─────────────────────────────────────────────────

    [AvaloniaTheory]
    [MemberData(nameof(Palettes))]
    public void Palette_is_a_flat_dictionary_with_no_variant_blocks(string id, string uri)
    {
        var dict = Load(uri);

        dict.ThemeDictionaries.Should().BeEmpty(
            "palette '{0}' is flat — no Dark/Light ThemeDictionaries variant blocks", id);
        dict.MergedDictionaries.Should().BeEmpty(
            "palette '{0}' declares its brushes inline — no merged sub-dictionaries", id);
    }

    private static ResourceDictionary Load(string uri)
    {
        var source = new Uri(uri);
        var include = new ResourceInclude(source) { Source = source };
        return (ResourceDictionary)include.Loaded;
    }
}
