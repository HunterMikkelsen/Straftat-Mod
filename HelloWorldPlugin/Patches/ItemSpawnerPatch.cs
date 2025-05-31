using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BepInEx.Configuration;
using System.Collections;

namespace HelloWorldPlugin.Patches
{
	[HarmonyPatch(typeof(ItemSpawner))]
	internal static class ItemSpawnerPatch
	{
		private static string weaponPath = "RandomWeapons";
		private static ConfigFile Config;

		public static List<GameObject> weapons;
		public static Dictionary<string, ConfigEntry<bool>> WeaponToggles = new();

		[HarmonyPostfix]
		[HarmonyPatch(nameof(ItemSpawner.LoadAllWeapons))]
		static void WeaponUnrandomizer(ref GameObject[] ___randomWeapons, ref string ___weaponsPath)
		{
			var weaponsList = WeaponToggles.Where(kv => kv.Value.Value)
											.Select(kv => kv.Key)
											.ToList();

			___randomWeapons = Resources
				.LoadAll<GameObject>(___weaponsPath)
				.Where(weapon => weaponsList.Contains(weapon.name))
				.ToArray();
		}
		public static void Setup(ConfigFile config)
		{
			Config = config;
			SetupWeaponList();
		}
		private static void SetupWeaponList()
		{
			List<GameObject> weapons = Resources.LoadAll<GameObject>(weaponPath).ToList();

			foreach (GameObject weapon in weapons)
			{
				var entry = Config.Bind("Weapons", $"Enable {weapon.name}", true, $"Enable or disable the {weapon.name} appearing in game.");
				WeaponToggles[weapon.name] = entry;
			}

			Plugin.Logger.LogInfo($"Registered {WeaponToggles.Count} weapon toggles.");
		}
	}
}
