using BepInEx.Configuration;
using HarmonyLib;

namespace HelloWorldPlugin.Patches
{
	[HarmonyPatch(typeof(Weapon))]
	internal static class WeaponPatch
	{
		private static ConfigFile Config;
		public static ConfigEntry<int> bulletCount;
		public static ConfigEntry<bool> unlimitedAmmoToggle;
		public static ConfigEntry<int> additionalAmmo;

		public static void Setup(ConfigFile config)
		{
			Config = config;
			unlimitedAmmoToggle = Config.Bind("Ammo", "Unlimited ammo", false, new ConfigDescription("Toggles if guns should have unlimited ammo"));
			additionalAmmo = Config.Bind("Ammo", "Additional ammo", 0, new ConfigDescription("Amount of additional ammo each gun should have"));
			bulletCount = Config.Bind("Ammo", "Number of bullets", 50, new ConfigDescription("Controls the number of bullets weapons will maintain"));
		}
		[HarmonyPostfix]
		[HarmonyPatch(nameof(Weapon.WeaponUpdate))]
		static void InfiniteAmmoPatch(ref bool ___needsAmmo, ref int ___currentAmmo)
		{
			if (!unlimitedAmmoToggle.Value)
			{
				___currentAmmo += additionalAmmo.Value;
			}
			___needsAmmo = !unlimitedAmmoToggle.Value;
		}
	}
}
