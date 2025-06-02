using BepInEx.Configuration;
using HarmonyLib;

namespace Unrandomizer.Patches
{
	[HarmonyPatch(typeof(Weapon))]
	internal static class WeaponPatch
	{
		private static ConfigFile Config;
		private static ConfigEntry<bool> unlimitedAmmoToggle;
		private static ConfigEntry<int> additionalAmmo;

		public static void Setup(ConfigFile config)
		{
			Config = config;
			unlimitedAmmoToggle = Config.Bind("Ammo", "Unlimited ammo (Host only)", false, new ConfigDescription("Toggles if guns should have unlimited ammo"));
			additionalAmmo = Config.Bind("Ammo", "Additional ammo", 0, new ConfigDescription("Amount of additional ammo each gun spawns with.", new AcceptableValueRange<int>(0, 1000)));
		}

		/// <summary>
		/// Adds the ability to set weapons to unlimited ammo. (Host only)
		/// </summary>
		/// <param name="___needsAmmo"></param>
		[HarmonyPostfix]
		[HarmonyPatch(nameof(Weapon.WeaponUpdate))]
		private static void InfiniteAmmoPatch(ref bool ___needsAmmo)
		{
			___needsAmmo = !unlimitedAmmoToggle.Value;
		}

		/// <summary>
		/// Sets the amount of additional ammo that should spawn with each weapon.<br />
		/// This takes place each time the weapon spawns so if the value is changed it wont take effect until the weapon spawns on the pedestal again.
		/// </summary>
		/// <param name="___currentAmmo"></param>
		[HarmonyPostfix]
		[HarmonyPatch(nameof(Weapon.Awake))]
		private static void AdditionalAmmoPatch(ref int ___currentAmmo)
		{
			___currentAmmo += additionalAmmo.Value;
		}
	}
}