namespace C7Engine {
	using System;
	using System.IO;
	using C7GameData;
	using C7GameData.Save;

	enum SaveFileFormat {
		Sav,
		Biq,
		C7,
		Invalid,
	}

	// The engine performs all save file creating, reading, and updating
	// via the SaveManager
	public static class SaveManager {
		public static bool RequiresLegacyImport(string path) {
			return getFileFormat(path) switch {
				SaveFileFormat.Sav => true,
				SaveFileFormat.Biq => true,
				_ => false,
			};
		}

		private static SaveFileFormat getFileFormat(string path) {
			return Path.GetExtension(path).ToUpper() switch {
				".SAV" => SaveFileFormat.Sav,
				".BIQ" => SaveFileFormat.Biq,
				".JSON" => SaveFileFormat.C7,
				".ZIP" => SaveFileFormat.C7,
				_ => SaveFileFormat.Invalid,
			};
		}

		// Load and initialize a save
		public static SaveGame LoadSave(string path, string bicPath, Func<string, string> getPediaIconsPath) {
			SaveGame save = getFileFormat(path) switch {
				SaveFileFormat.Sav => ImportCiv3.ImportSav(path, RequireBiqPath(bicPath), getPediaIconsPath),
				SaveFileFormat.Biq => ImportCiv3.ImportBiq(path, RequireBiqPath(bicPath), getPediaIconsPath),
				SaveFileFormat.C7 => SaveGame.Load(path, getPediaIconsPath),
				_ => throw new FileLoadException("invalid save format"),
			};
			return save;
		}

		private static string RequireBiqPath(string bicPath) {
			if (string.IsNullOrEmpty(bicPath)) {
				throw new InvalidOperationException("A Civilization III BIQ path is required when importing legacy .sav or .biq files");
			}
			return bicPath;
		}

		public static void Save(string path) {
			GameData gameData = EngineStorage.gameData;
			SaveGame save = SaveGame.FromGameData(gameData);
			save.Save(path);
		}

	}
}
