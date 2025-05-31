using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using HelloWorldPlugin.Patches;

namespace HelloWorldPlugin;

[HarmonyPatch(typeof(Weapon))]
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
	internal static new ManualLogSource Logger;
	private static Plugin Instance;
	private readonly Harmony harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
	public static ConfigFile ConfigFileRef;

	private void Awake()
	{
		if(Instance == null)
		{
			Instance = this;
		}
		Logger = base.Logger;
		Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

		ConfigFileRef = Config;

		harmony.PatchAll(typeof(Plugin));
		harmony.PatchAll(typeof(WeaponPatch));
		harmony.PatchAll(typeof(ItemSpawnerPatch));
		harmony.PatchAll(typeof(MenuControllerPatch));

		ItemSpawnerPatch.Setup(ConfigFileRef);
		WeaponPatch.Setup(ConfigFileRef);
	}
}
