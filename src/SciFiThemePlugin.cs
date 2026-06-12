using System.Collections.Generic;
using SoundBoard.PluginApi;

namespace SciFiThemePlugin;

/// <summary>
/// Sci-Fi theme pack — two selectable palettes for science-fiction RPG
/// sessions. Each appears as its own entry in the host's theme dropdown,
/// prefixed with the pack name ("Sci-Fi: Starship Command", "Sci-Fi: Neon
/// Cyberpunk").
///
/// <list type="bullet">
///   <item><b>Starship Command</b> (telemetry &amp; clean-room) — a
///   sterile, brightly lit federation bridge or corporate laboratory.
///   Crisp high-contrast whites and cool telemetry grays, navy-blue
///   primary, warning-hazard orange secondary.</item>
///   <item><b>Neon Cyberpunk</b> (the grid) — a rainy, neon-lit alleyway
///   at midnight. Pure obsidian and slate-950 surfaces, blinding neon
///   magenta primary, electric cyan secondary, laser green success.</item>
/// </list>
///
/// <para>Each palette is a flat set of colours; the host has no Dark/Light toggle.</para>
/// </summary>
public sealed class SciFiThemePlugin : IThemePlugin
{
    public string Id => "theme.sci-fi";
    public string Name => "Sci-Fi";
    public string Version => PluginVersion.OfAssembly(typeof(SciFiThemePlugin));
    public string Author => "Devin Sanders";
    public string Description => "Two Sci-Fi palettes: Starship Command (telemetry & clean-room) and Neon Cyberpunk (the grid).";

    public void Initialize(IPluginContext context) { }
    public void Shutdown() { }

    public IEnumerable<ThemePalette> GetPalettes() => new[]
    {
        new ThemePalette("starship-command", "Starship Command",
            new[] { "avares://SciFiThemePlugin/Themes/StarshipCommand.axaml" }),
        new ThemePalette("neon-cyberpunk",   "Neon Cyberpunk",
            new[] { "avares://SciFiThemePlugin/Themes/NeonCyberpunk.axaml" }),
    };
}
