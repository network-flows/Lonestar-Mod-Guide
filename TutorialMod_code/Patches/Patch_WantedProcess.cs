using UnityEngine;
using HarmonyLib;
using Tool.Database;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;

namespace TutorialMod.Patches
{
    // If patching a private method, use [HarmonyPatch(typeof(WantedProcess), "PrivateMethodName")]
    // If patching a private class or a functions with multiple overrides, remove the annotation and use manual patch in UserMod.OnLoad() instead.
    // To read more, visit Harmony Manual at https://harmony.pardeike.net/articles/intro.html
    [HarmonyPatch(typeof(WantedProcess), nameof(WantedProcess.Init))]
    public static class Patch_WantedProcess
    {
        [HarmonyPrefix]
        public static void Prefix(WantedProcess __instance, ProcessAdvance processAdvance, WantedPhases wantedPhases, string _seed, CusMod cusMod)
        {
            // Patch methods with Harmony annotations will be automatically applied when mod is loaded.
            // Here are some examples on how to use harmony features: 

            // Access public members
            // Debug.Log("Star Coin: " + __instance.starCoin);

            // Access private fields
            // Traverse.Create(__instance).Field("_order_count").SetValue(-1);
            // int result = Traverse.Create(__instance).Field("_order_count").GetValue<int>();

            // Call private methods
            // Traverse.Create(__instance).Method("InitShipUnitPool").GetValue();

            // Call private methods with multiple overrides
            /*
            List<MethodInfo> methodList = typeof(WantedProcess).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).ToList();
            Debug.Log(methodList.Count);
            MethodInfo exactMethod = methodList.Where(method =>
                method.Name == "RemoveTreasurePool" &&
                method.GetParameters().Length == 1 &&
                method.GetParameters()[0].ParameterType == typeof(ItemTreasure)
            ).FirstOrDefault();
            DataTreasure dataTreasure = DataTreasureManager.Instance().GetDataByID(102);
            ItemTreasure itemtreasure = new ItemTreasure(dataTreasure);
            if (exactMethod != null) exactMethod.Invoke(__instance, new System.Object[] { itemtreasure });
            */
        }
    }
}
