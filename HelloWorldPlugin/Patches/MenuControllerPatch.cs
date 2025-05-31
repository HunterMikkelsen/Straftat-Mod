using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using UnityEngine.UIElements;
using Toggle = UnityEngine.UI.Toggle;
using Image = UnityEngine.UI.Image;
using Text = UnityEngine.UI.Text;
using BepInEx.Logging;
using BepInEx.Configuration;

namespace HelloWorldPlugin.Patches
{
	[HarmonyPatch(typeof(MenuController))]
	internal static class MenuControllerPatch
	{
		[HarmonyPostfix]
		[HarmonyPatch(nameof(MenuController.ActivateMenu))]
		static void AddWeaponList(GameObject menu, ref GameObject ___playMenu)
		{
			//if (Input.GetKeyDown(KeyCode.F2)) // Toggle menu with F2
			//{
			//	Plugin.Logger.LogInfo("Test UI KEY");
			//}

			//if (menu == ___playMenu)
			//{
			//	var canvas = ___playMenu.transform.Find("RandomWeapons");
			//	if (canvas != null)
			//	{
			//		Plugin.Logger.LogInfo("canvas found");
			//	}
			//}
			//Plugin.Logger.LogInfo("Going to: " + menu.ToString());
		}
	}
}
