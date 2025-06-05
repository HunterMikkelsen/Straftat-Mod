using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using ComputerysModdingUtilities;
using HarmonyLib;
using Unrandomizer.Patches;

[assembly: StraftatMod(isVanillaCompatible: false)]

namespace Unrandomizer;

[HarmonyPatch(typeof(Weapon))]
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
	public static ConfigFile ConfigFileRef;
	internal new static ManualLogSource Logger;
	private static Plugin Instance;
	private readonly Harmony harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		Logger = base.Logger;
		Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

		ConfigFileRef = Config;

		harmony.PatchAll(typeof(Plugin));
		//harmony.PatchAll(typeof(WeaponPatch));
		harmony.PatchAll(typeof(ItemSpawnerPatch));

		ItemSpawnerPatch.Setup(ConfigFileRef);
		//WeaponPatch.Setup(ConfigFileRef);
	}
}