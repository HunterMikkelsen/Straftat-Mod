using BepInEx.Configuration;
using FishNet;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Unrandomizer.Patches
{
	[HarmonyPatch(typeof(ItemSpawner))]
	internal static class ItemSpawnerPatch
	{
		private static Dictionary<string, ConfigEntry<bool>> WeaponToggles = new();
		private static ConfigEntry<bool> EnableAllWeaponsToggle;
		private static ConfigEntry<bool> DisableAllWeaponsToggle;
		private static ConfigEntry<bool> ToggleRandomWeaponRespawnTime;
		private static ConfigEntry<float> RandomWeaponRespawnTimerMin;
		private static ConfigEntry<float> RandomWeaponRespawnTimerMax;
		private static ConfigEntry<float> WeaponRespawnTimer;
		private static ConfigFile Config;
		private static List<GameObject> _completeWeaponList;
		private static string weaponsPath = "RandomWeapons";
		private static float countdown = 2.0f;

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
		private static void GetRandomWeapons(ref GameObject[] ___randomWeapons)
		{
			SetWeaponList(ref ___randomWeapons);
		}

		/// <summary>
		/// Updates the list of random weapons to spawn before every weapon spawn.
		/// </summary>
		/// <param name="___randomWeapons"></param>
		[HarmonyPrefix]
		[HarmonyPatch(nameof(ItemSpawner.PickRandomWeapon))]
		private static void GetRandomWeapon(ref GameObject[] ___randomWeapons)
		{
			SetWeaponList(ref ___randomWeapons);
		}

		[HarmonyPrefix]
		[HarmonyPatch(nameof(ItemSpawner.Awake))]
		private static void Awake(ref string ___weaponsPath, ref float ___blankStateProbability, ref float ___countdown)
		{
			// If the player is not host then don't let them change settings.
			if (!InstanceFinder.NetworkManager.IsServer)
			{
				return;
			}

			ValidateRange();
			RandomWeaponRespawnTimerMin.SettingChanged += (_, __) => ValidateRange();
			RandomWeaponRespawnTimerMax.SettingChanged += (_, __) => ValidateRange();

			weaponsPath = ___weaponsPath;
			___countdown = WeaponRespawnTimer.Value;
			___blankStateProbability = -1.0f;
		}

		[HarmonyPrefix]
		[HarmonyPatch("Update")]
		private static void Update(ref float ___countdown)
		{
			// If the player is not host then don't let them change settings.
			if (!InstanceFinder.NetworkManager.IsServer)
			{
				return;
			}

			if (ToggleRandomWeaponRespawnTime.Value)
			{
				___countdown = Random.Range(RandomWeaponRespawnTimerMin.Value, RandomWeaponRespawnTimerMax.Value);
			}
			else
			{
				___countdown = WeaponRespawnTimer.Value;
			}
		}

		/// <summary>
		/// Updates the list of weapons ItemSpawner can pull from based on the configuration settings.
		/// </summary>
		/// <param name="___randomWeapons">The list of weapons Straftat uses to choose what weapon to spawn.</param>
		private static void SetWeaponList(ref GameObject[] ___randomWeapons)
		{
			// If the player is not host then don't let them change the weapons.
			if (!InstanceFinder.NetworkManager.IsServer)
			{
				return;
			}

			// Verify that _completeWeaponList isn't null
			_completeWeaponList ??= Resources.LoadAll<GameObject>(weaponsPath).ToList();
			___randomWeapons = _completeWeaponList
				.Where(weapon => randomWeaponList.Contains(weapon.name))
				.ToArray();
		}

		/// <summary>
		/// Clamps RandomWeaponRespawnTimerMin & RandomWeaponRespawnTimerMax
		/// </summary>
		private static void ValidateRange()
		{
			RandomWeaponRespawnTimerMin.Value = (float)Math.Round(RandomWeaponRespawnTimerMin.Value, 2);
			RandomWeaponRespawnTimerMax.Value = (float)Math.Round(RandomWeaponRespawnTimerMax.Value, 2);

			if (RandomWeaponRespawnTimerMin.Value > RandomWeaponRespawnTimerMax.Value)
			{
				RandomWeaponRespawnTimerMin.Value = RandomWeaponRespawnTimerMax.Value;
			}
		}

		#region Setup methods

		/// <summary>
		/// The default setup method for this class.
		/// </summary>
		/// <param name="config">The global config file for ConfigurationManager</param>
		public static void Setup(ConfigFile config)
		{
			Config = config;

			SetupConfigOptions();
			SetupWeaponList();
			SetupWeaponResetButton();
			SetupDisableAllWeaponsToggleButton();
		}

		private static void SetupWeaponList()
		{
			EnableAllWeaponsToggle = Config.Bind("Weapons Options", "Enable all weapons", false, new ConfigDescription("Enables all weapons to become spawnable.", null, new ConfigurationManagerAttributes { Category = "Weapons Options", Order = 2, HideDefaultButton = true }));
			DisableAllWeaponsToggle = Config.Bind("Weapons Options", "Disable all weapons", false, new ConfigDescription("Disables all weapons from being spawnable.", null, new ConfigurationManagerAttributes { Category = "Weapons Options", Order = 1, HideDefaultButton = true }));
			_completeWeaponList ??= Resources.LoadAll<GameObject>(weaponsPath).ToList();

			foreach (GameObject weapon in _completeWeaponList)
			{
				var entry = Config.Bind("Weapons", $"Toggle {weapon.name}", true, new ConfigDescription($"Enable or disable the {weapon.name} appearing in-game.", null, new ConfigurationManagerAttributes { Category = "Weapons" }));
				WeaponToggles[weapon.name] = entry;
			}

			Plugin.Logger.LogInfo($"Registered {WeaponToggles.Count} weapon toggles.");
		}

		private static void SetupConfigOptions()
		{
			WeaponRespawnTimer = Config.Bind("General", "Weapon respawn timer (seconds)", 2.0f, new ConfigDescription("Controls the amount of time it takes a weapon to respawn.", new AcceptableValueRange<float>(0.0f, 180.0f), new ConfigurationManagerAttributes { Category = "General", Order = 0, }));

			ToggleRandomWeaponRespawnTime = Config.Bind("Random Respawn", "Toggle random respawn time", false, new ConfigDescription("Toggles if weapons should have differing respawn times.", null, new ConfigurationManagerAttributes { Category = "Random Respawn", Order = 2 }));
			RandomWeaponRespawnTimerMin = Config.Bind("Random Respawn", "Minimum time in seconds", 2.0f, new ConfigDescription("The minimum amount of time it should take for a weapon to respawn.", new AcceptableValueRange<float>(0.0f, 180.0f), new ConfigurationManagerAttributes { Category = "Random Respawn", Order = 1, }));
			RandomWeaponRespawnTimerMax = Config.Bind("Random Respawn", "Maximum time in seconds", 2.0f, new ConfigDescription("The maximum amount of time it should take for a weapon to respawn.", new AcceptableValueRange<float>(0.0f, 180.0f), new ConfigurationManagerAttributes { Category = "Random Respawn", Order = 0, }));
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