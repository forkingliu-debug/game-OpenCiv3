using System;
using System.IO;
using System.Collections.Generic;
using Serilog;
using C7GameData.Save;
using MoonSharp.Interpreter;

namespace C7Engine.Lua;

public class GameModeConfig {
	public string id;
	public string displayName;

	// The base scenario definition file. Should be a JSON or a Lua
	// script returning a table
	public string baseModePath;

	// Optional addon Lua scripts. Each script should return a
	// function that modifies the scenario data and returns it.
	public List<string> addonPaths = [];

	// The Lua rules script used to interpret the data once a SaveGame becomes
	// runtime GameData.
	public string rulesScript;

	public GameModeConfig(string id, string displayName, string baseModePath, string rulesScript = "civ3.lua", List<string> addonPaths = null) {
		this.id = id;
		this.displayName = displayName;
		this.baseModePath = baseModePath;
		this.rulesScript = rulesScript;
		this.addonPaths = addonPaths ?? [];
	}
}

// Loads OpenCiv3 scenario definitions
public class GameModeLoader {
	private static ILogger log = Log.ForContext<GameModeLoader>();

	public static SaveGame Load(string gameModesDir, GameModeConfig config) {
		Script lua = new();
		JsonConverter converter = new (lua);

		string baseModePath = config.baseModePath;

		string fullBaseModePath = Path.Combine(gameModesDir, baseModePath);
		log.Information("Loading base game mode file: {baseModePath}", fullBaseModePath);

		// Load base scenario from JSON or Lua file
		DynValue current = Path.GetExtension(fullBaseModePath).ToUpper() switch {
			".JSON" => converter.Decode(File.ReadAllText(fullBaseModePath)),
			".LUA" => lua.SafeDoFile(fullBaseModePath),
			_ => throw new InvalidOperationException("Invalid base mode file format")
		};

		// Process through addon scripts
		foreach (string addonPath in config.addonPaths) {
			log.Information("Loading addon: {addonPath}", addonPath);

			DynValue addon = lua.SafeDoFile(Path.Combine(gameModesDir, addonPath));

			if (addon.Type != DataType.Function)
				throw new InvalidOperationException(
					$"Addon '{addonPath}' must return a function"
				);

			// Each addon function modifies and returns the scenario data
			current = lua.SafeCall(addon.Function, current);
		}

		// Convert final Lua table to JSON for deserialization
		string json = converter.Encode(current);

		SaveGame save = SaveGame.LoadFromJSON(json);
		save.DataSourceId = config.id;
		save.DataSourceDisplayName = config.displayName;
		save.DataSourceBasePath = config.baseModePath;
		save.DataSourceAddons = [.. config.addonPaths];
		save.RulesScript = config.rulesScript;

		return save;
	}
}
