using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace CatPatchCustomLinesLoader;

[BepInPlugin(ModGUID, ModName, ModVersion)]
[BepInDependency("meow.catpatch")]
public class Plugin : BaseUnityPlugin
{
	public const string ModGUID = "meowharder.catpatch.customlinesloader";
	public const string ModName = "CatPatchCustomLinesLoader";
	public const string ModVersion = "0.1.0";
	
	public static Plugin Instance { get; private set; } = null!;

	internal static new ManualLogSource Logger;

	private readonly Harmony _harmony = new(ModGUID);
	
	private ConfigEntry<bool> configFilterIncomingLines;
	
	public static bool ShouldFilterIncomingLines
	{
		get => Instance.configFilterIncomingLines.Value;
		set => Instance.configFilterIncomingLines.Value = value;
	}

	void Awake()
	{
		Logger = base.Logger;
		Instance = this;
		
		configFilterIncomingLines = Config.Bind(
			"General",
			"filterIncomingLines",
			false,
			"Do you want to filter other player's dialogue lines?\nIf set to true, any dialogue lines not present in your CatPatchCustomLines.json will be replaced by lines you do have.\nThis can be changed in-game from CatPatch's config menu");
		
		_harmony.PatchAll();
		Logger.LogInfo($"{ModName} is loaded.");
	}

	void OnDestroy()
	{
		_harmony?.UnpatchSelf();
		Instance = null!;
	}
}
