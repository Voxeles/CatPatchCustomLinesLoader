using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace CatPatchCustomLinesLoader;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("meow.catpatch")]
public class Plugin : BaseUnityPlugin
{
	public const string ModGuid = MyPluginInfo.PLUGIN_GUID;
	public const string ModName = MyPluginInfo.PLUGIN_NAME;
	public const string ModVersion = MyPluginInfo.PLUGIN_VERSION;
	
	public static Plugin Instance { get; private set; } = null!;

	internal static new ManualLogSource Logger;

	private readonly Harmony _harmony = new(ModGuid);
	
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
