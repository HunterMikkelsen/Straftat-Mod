using BepInEx.Configuration;
using FishNet;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unrandomizer.Patches
{
	[HarmonyPatch(typeof(ItemSpawner))]
	internal static class ItemSpawnerPatch
	{
		private static Dictionary<string, ConfigEntry<bool>> WeaponToggles = new();
		private static ConfigEntry<bool> EnableAllWeaponsToggle;
		private static ConfigEntry<bool> DisableAllWeaponsToggle;
		private static ConfigFile Config;
		private static List<GameObject> _completeWeaponList;
		private static string weaponsPath = "RandomWeapons";

		private static List<string> randomWeaponList
		{
			get
			{
				return WeaponToggles.Where(kv => kv.Value.Value)
											.Select(kv => kv.Key)
											.ToList();
			}
		}

		/// <summary>
		/// Updates the list of random weapons to spawn before the first weapon spawn.
		/// </summary>
		/// <param name="___randomWeapons"></param>
		[HarmonyPostfix]
		[HarmonyPatch(nameof(ItemSpawner.LoadAllWeapons))]
		private static void GetRandomWeapons(ref GameObject[] ___randomWeapons, ref string ___weaponsPath)
		{
			SetWeaponList(ref ___randomWeapons, ref ___weaponsPath);
		}

		/// <summary>
		/// Updates the list of random weapons to spawn before every weapon spawn.
		/// </summary>
		/// <param name="___randomWeapons"></param>
		[HarmonyPrefix]
		[HarmonyPatch(nameof(ItemSpawner.PickRandomWeapon))]
		private static void GetRandomWeapon(ref GameObject[] ___randomWeapons, ref string ___weaponsPath)
		{
			SetWeaponList(ref ___randomWeapons, ref ___weaponsPath);
		}

		private static void SetWeaponList(ref GameObject[] ___randomWeapons, ref string ___weaponsPath)
		{
			// If the player is not host then don't let them change the weapons.
			if (!InstanceFinder.NetworkManager.IsServer)
			{
				return;
			}
			weaponsPath = ___weaponsPath;
			// Verify that _completeWeaponList isn't null
			_completeWeaponList ??= Resources.LoadAll<GameObject>(weaponsPath).ToList();
			___randomWeapons = _completeWeaponList
				.Where(weapon => randomWeaponList.Contains(weapon.name))
				.ToArray();
		}

		#region Setup methods

		/// <summary>
		/// The default setup method for this class.
		/// </summary>
		/// <param name="config">The global config file for ConfigurationManager</param>
		public static void Setup(ConfigFile config)
		{
			Config = config;

			SetupWeaponList();
			SetupWeaponResetButton();
			SetupDisableAllWeaponsToggleButton();
		}

		private static void SetupWeaponList()
		{
			EnableAllWeaponsToggle = Config.Bind("General", "Enable all weapons", false, new ConfigDescription("Enables all weapons to become spawnable", null, new ConfigurationManagerAttributes { Category = "General", Order = 0, HideDefaultButton = true }));
			DisableAllWeaponsToggle = Config.Bind("General", "Disable weapons", false, new ConfigDescription("Disables all weapons from being spawnable", null, new ConfigurationManagerAttributes { Category = "General", Order = 1, HideDefaultButton = true }));

			_completeWeaponList ??= Resources.LoadAll<GameObject>(weaponsPath).ToList();

			foreach (GameObject weapon in _completeWeaponList)
			{
				var entry = Config.Bind("Weapons", $"Toggle {weapon.name}", true, new ConfigDescription($"Enable or disable the {weapon.name} appearing in-game.", null, new ConfigurationManagerAttributes { Category = "Weapons" }));
				WeaponToggles[weapon.name] = entry;
			}

			Plugin.Logger.LogInfo($"Registered {WeaponToggles.Count} weapon toggles.");
		}

		private static void SetupWeaponResetButton()
		{
			// Create a hook for ResetWeaponsToggle
			EnableAllWeaponsToggle.SettingChanged += (_, __) =>
			{
				if (EnableAllWeaponsToggle.Value && WeaponToggles != null)
				{
					foreach (var weapon in WeaponToggles.Values)
					{
						weapon.Value = true;
					}

					EnableAllWeaponsToggle.Value = false;
					Plugin.Logger.LogInfo("Enabled all weapons.");
				}
			};
		}

		private static void SetupDisableAllWeaponsToggleButton()
		{
			// Create a hook for ResetWeaponsToggle
			DisableAllWeaponsToggle.SettingChanged += (_, __) =>
			{
				if (DisableAllWeaponsToggle.Value && WeaponToggles != null)
				{
					foreach (var weapon in WeaponToggles.Values)
					{
						weapon.Value = false;
					}

					DisableAllWeaponsToggle.Value = false;
					Plugin.Logger.LogInfo("Disabled all weapons.");
				}
			};
		}

		#endregion Setup methods
	}
}