using Godot;
using System.IO;
using C7Engine.Lua;

public static class GamePaths {
	// Base directory for finding data files (Lua/, Text/, Assets/).
	// In the editor this is "res://". In exports, it's the directory
	// containing the executable — on macOS, navigated out of the .app bundle.
	private static string _baseDir;
	public static string BaseDir {
		get {
			if (_baseDir == null) {
				if (OS.HasFeature("editor")) {
					_baseDir = "";
				} else {
					string exeDir = OS.GetExecutablePath().GetBaseDir();
					// On macOS the exe is inside *.app/Contents/MacOS/;
					// data files live alongside the .app bundle.
					if (exeDir.Contains(".app/Contents/MacOS")) {
						_baseDir = Path.GetFullPath(Path.Combine(exeDir, "..", "..", "..")) + "/";
					} else {
						_baseDir = exeDir + "/";
					}
				}
			}
			return _baseDir;
		}
	}

	// New games now always start from the OpenCiv3-owned ruleset. Classic Civ3
	// assets may still be imported for compatibility, but they no longer define
	// the default game data source.
	public static GameModeConfig DefaultNewGameMode => standalone;

	public static GameModeConfig basic = new(
		id: "classic-base",
		displayName: "Classic Base Ruleset",
		baseModePath: "base-ruleset.json"
	);
	public static GameModeConfig standalone = new(
		id: "openciv3-standalone",
		displayName: "OpenCiv3 Standalone Ruleset",
		baseModePath: "base-ruleset.json",
		addonPaths: ["standalone.lua"]
	);

	public static string LuaRulesDir => Path.Combine(BaseDir, "Lua/rules/");
	public static string TextureConfigsDir => Path.Combine(BaseDir, "Lua/texture_configs/");
	public static string GameModesDir => Path.Combine(BaseDir, "Lua/game_modes/");
	public static string SaveGamesDir => ProjectSettings.GlobalizePath("user://Saves");

	public const string ModernGraphicsConfig = "c7.lua";
	public const string ClassicGraphicsConfig = "civ3.lua";

	// Legacy Civ3 import still uses the stock BIQ as a compatibility baseline.
	public static string DefaultBicPath => Path.Combine(Util.GetCiv3Path(), "Conquests", "conquests.biq");
}
