using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Tool.Database;

namespace loadout.Patches
{
	// Creates the DataShipUnit for Skill_Hybrid.
    [HarmonyPatch(typeof(UnitData), nameof(UnitData.GetData))]
    public class Patch_UnitData
    {
        [HarmonyPrefix]
		public static bool Prefix(UnitData __instance)
		{
			if (__instance.args != null && __instance.args.Count > 1 && __instance.args[0] == -1)
			{
				DataShipUnit result = (DataShipUnit)Traverse.Create(__instance).Field("dataShipUnit").GetValue();
				if (result != null) return true;

				DataShipUnit merged_Unit = null;
				int lv_cap = -1;

				for (int i = 1; i < __instance.args.Count; i++)
				{
					merged_Unit = UnitData.GetDataShipUnit(__instance.args[i], __instance.lv);
					for (int offset = 0; __instance.lv - offset >= 1 && merged_Unit == null; offset++)
						merged_Unit = UnitData.GetDataShipUnit(__instance.args[i], __instance.lv - offset);
					if (merged_Unit == null) continue;

					if (result == null)
					{
						result = Clone_DataShipUnit(merged_Unit);
						result.InGame = false;
						result.SkillPath = Skill2_Hybrid.SkillPath;
						result.Description = $"缝合怪。\n缝了的部件：<UNIT>{result.ID}|{result.Lv}</UNIT>";
						result.Args = new int[__instance.args.Count - 1];
					}

					int lv_cap2 = DataShipUnitManager.Instance().GetDataListByID(merged_Unit.ID).Count;
					if (lv_cap2 > lv_cap)
					{
						lv_cap = lv_cap2;
						result.ID = merged_Unit.ID;
						result.Name = merged_Unit.Name;
						__instance.id = merged_Unit.ID;
					}
					result.Args[i - 1] = __instance.args[i];

					if (i > 1)
					{
						foreach (KeyValuePair<string, int> kv in merged_Unit.Propertys)
						{
							if (kv.Key == "PA" && result.Propertys.ContainsKey(kv.Key))
								result.Propertys[kv.Key] += kv.Value;
							else
								result.Propertys[kv.Key] = kv.Value;
						}
						result.Description += $"，<UNIT>{merged_Unit.ID}|{merged_Unit.Lv}</UNIT>";
						if (merged_Unit.Type == 1) result.Type = 1;
					}
				}
				__instance.SetData(result);
				return true;
			}
			return true;
		}

		public static DataShipUnit Clone_DataShipUnit(DataShipUnit original)
		{
			return new DataShipUnit
			{
				ID = original.ID,
				Name = original.Name,
				Lv = original.Lv,
				SkillName = original.SkillName,
				UnlockLV = original.UnlockLV,
				Rare = original.Rare,
				Genera = (int[])original.Genera.Clone(),
				InGame = original.InGame,
				Pros = (int[])original.Pros.Clone(),
				PowerSlot = (int[])original.PowerSlot.Clone(),
				CountOffset = original.CountOffset,
				WeightOffset = original.WeightOffset,
				EquiptLimit = original.EquiptLimit,
				Description = original.Description,
				Propertys = original.Propertys.ToDictionary(entry => entry.Key, entry => entry.Value),
				Args = (int[])original.Args.Clone(),
				ExtraDes = original.ExtraDes,
				Type = original.Type,
				SkillPath = original.SkillPath,
				SpritePath = original.SpritePath,
				ModPath = original.ModPath,
			};
		}
	}
}
