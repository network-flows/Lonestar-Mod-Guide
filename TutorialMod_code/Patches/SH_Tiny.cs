using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tool.Database;
using UnityEngine;

namespace TutorialMod
{
    public class SH_Tiny : ShipSkill, IProcessInited, IBattleStart
    {
        const string color_key = "TutorialMod/SH_Tiny/Args/1";   //Used to save the arguments of the skill
        const string point_key = "TutorialMod/SH_Tiny/Args/2";
        public void OnProcessInited()
        {
            Define.defineValue.eventValuesGlobal[color_key] = args[0];
            Define.defineValue.eventValuesGlobal[point_key] = args[1];
        }

        // Updates the ship-skill-related slot at the top panel (eg. shield count of shielder)
        // Warning: This part is buggy and about to change
        public override void InitBannerPanel(BannerPanel panel)
        {
            if (panel.currentShipSlot != null) GameObject.Destroy(panel.currentShipSlot);

            // We just steal the shields bar prefab and replace the script
            // Also you can create your own
            GameObject go = ResourcesManager.Instance().Load<GameObject>(FilePath.bannerPanelShieldSlot, panel.slotBase);
            go.transform.SetParent(panel.left, false);
            go.transform.SetSiblingIndex(3);
            go.transform.localScale = Vector3.one;
            panel.currentShipSlot = go;

            
            go.GetComponent<BannerPanelShieldSlot>().enabled = false;
            BannerPanelEnergySlot slot = go.AddComponent<BannerPanelEnergySlot>();
            slot.Init(ShipDatasManager.Instance().currentPlayerShipData);
        }

        // Do nothing the status under hp bar
        // You can either steal one from "ExchangeCellBase" or "ColorSlotCell", or create your own
        public override void InitHPBar(PlayerShipController shipController, HPBarPanel hpBarPanel)
        {

        }

        // some events have both "shield" and "swap" tags, should filter out "sheild" tag on spacewalker and vise versa.
        public override void FilterEventTags(ref List<int> tags)
        {
            List<int> banList = new List<int> { (int)EncounterEventTag.Exchange, (int)EncounterEventTag.ColorSlot };
            tags = tags.Where(tag => !banList.Contains(tag)).ToList();
        }

        public void OnBattleStart()
        {
            int color = Define.defineValue.eventValuesGlobal[color_key];
            int point = Define.defineValue.eventValuesGlobal[point_key];
            CreatePower(point, (PowerColor)color);
        }
    }
}