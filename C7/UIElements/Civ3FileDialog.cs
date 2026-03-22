using C7Engine;
using C7GameData;
using Godot;
using Serilog;
using System.IO;

[GlobalClass]
public partial class Civ3FileDialog : FileDialog {
	// An object for passing information (like save file paths) between scenes.
	GlobalSingleton Global;
	private ILogger log;

	// If true, go to scenario setup after loading, for scenarios.
	public bool GoToScenarioSetupAfterLoading = false;

	public override void _Ready() {
		base._Ready();
		log = LogManager.ForContext<Civ3FileDialog>();

		FileMode = FileDialog.FileModeEnum.OpenFile;
		Access = AccessEnum.Filesystem;

		Global = GetNode<GlobalSingleton>("/root/GlobalSingleton");
		FileSelected += OnFileSelected;
	}

	public void SetDirectoryForLoading(string RelPath) {
		CurrentDir = Util.Civ3Root + "/" + RelPath;
		FileMode = FileDialog.FileModeEnum.OpenFile;
	}

	public void ConfigureForProjectSaveLoading() {
		Title = "Load OpenCiv3 Save";
		ClearFilters();
		AddFilter("*.json,*.zip ; OpenCiv3 saves");
	}

	public void ConfigureForLegacyScenarioLoading() {
		Title = "Import Civilization III Scenario";
		ClearFilters();
		AddFilter("*.biq ; Civilization III scenarios");
	}

	public void SetDirectoryForLoadingAt(string absolutePath) {
		Directory.CreateDirectory(absolutePath);
		CurrentDir = absolutePath;
		FileMode = FileDialog.FileModeEnum.OpenFile;
	}

	public void SetDirectoryForSaving(string RelPath) {
		CurrentDir = Util.Civ3Root + "/" + RelPath;
		FileMode = FileDialog.FileModeEnum.SaveFile;
	}

	public void SetDirectoryForSavingAt(string absolutePath) {
		Directory.CreateDirectory(absolutePath);
		CurrentDir = absolutePath;
		FileMode = FileDialog.FileModeEnum.SaveFile;
	}

	private void OnFileSelected(string path) {
		if (FileMode == FileDialog.FileModeEnum.OpenFile) {
			log.Information($"loading {path}");
			Global.LoadGamePath = path;
			if (GoToScenarioSetupAfterLoading) {
				GetTree().ChangeSceneToFile("res://UIElements/NewGame/scenario_setup.tscn");
			} else {
				GetTree().ChangeSceneToFile("res://C7Game.tscn");
			}
		} else {
			if (!path.EndsWith(".json")) {
				path = path + ".json";
			}

			log.Information($"Saving game to {path}");
			EngineStorage.ReadGameData((GameData gameData) => {
				C7GameData.Save.SaveGame.FromGameData(gameData).Save(path);
			});
		}
	}
}
