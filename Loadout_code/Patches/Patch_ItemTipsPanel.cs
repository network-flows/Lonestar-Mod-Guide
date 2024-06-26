using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using Utils;
using Lean.Localization;
using TMPro;

namespace Loadout
{
    // To allow the tooltip display of ItemAny, a custom itemtip for displaying events, talents, etc.
    [HarmonyPatch(typeof(ItemTipsPanel), nameof(ItemTipsPanel.CreateTips))]
    public class Patch_ItemTipsPanel
    {
        [HarmonyPrefix]
        public static bool Prefix(ItemTipsPanel __instance, List<Item> items, ShowDir showDir, Transform target)
        {
            KeyWord.dataKeyWords.Clear();
            __instance.showDir = showDir;
            __instance.target = target;
            foreach (Item item in items)
            {
                if (item is ItemTalent)
                {
                    var datatalent = (item as ItemTalent).dataTalent;
                    var color = datatalent.Type == 1 ? UtilsClass.GetColor255(229f, 244f, 255f, 255f) : UtilsClass.GetColor255(255f, 240f, 229f, 255f);
                    Traverse.Create(__instance).Method(datatalent.Type == 1 ? "CreateTalentRandomTips" : "CreateTalentTips").GetValue<ItemTips>().RefreshTalent(datatalent, color, showDir, __instance.keyWord, null);
                }
                else if (item is ItemAny)
                {
                    ItemAny itemPseudo = item as ItemAny;
                    if (itemPseudo.SkipTip) continue;
                    ItemTips tip = Traverse.Create(__instance).Method("CreateTip").GetValue<ItemTips>();
                    tip.Refresh(itemPseudo.dataTreasure, showDir, __instance.keyWord);
                    tip.tagText.text = itemPseudo.Type_text;
                    tip.rareText.text = itemPseudo.Rare_text;
                    Traverse.Create(tip).Field("skillDes").GetValue<TextMeshProUGUI>().text = itemPseudo.Des_text;
                }
                else
                {
                    Traverse.Create(__instance).Method("CreateTip").GetValue<ItemTips>().Refresh(item, showDir, __instance.keyWord);
                }
            }
            return false;
        }
    }
}
