using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using HarmonyLib;
using Tool.Database;
using Lean.Localization;
using System.Linq;
using System.Text.RegularExpressions;
using Utils;
using UnityEngine.UIElements;

namespace Loadout
{
    public class GUILoadout : SingletonAutoMono<GUILoadout>
    {
        private List<string> titles_outgame = new List<string> { "Loadout/Keys/Association" };
        private List<string> titles_ingame = new List<string> { "Loadout/Keys/Ship", "Loadout/Keys/Enemy", "Loadout/Keys/ShipUnit", "Loadout/Keys/Treasure", "Loadout/Keys/Talent", "Loadout/Keys/Events", "Loadout/Keys/Mod" };
        private List<string> titles_battle = new List<string> { "Loadout/Keys/Ship", "Loadout/Keys/Enemy" };
        private List<string> options_pool = new List<string> { "Loadout/Operation/Normal", "Loadout/Operation/PoolAdd", "Loadout/Operation/PoolRemove" };
        private List<string> filter_ship;
        private List<int> ship_ids;
        private List<string> filters_rare = new List<string>  { "Loadout/Filter/NoLimit", "Loadout/Filter/Rare1", "Loadout/Filter/Rare2", "Loadout/Filter/Rare3" };
        private List<string> filters_enemy = new List<string> { "Loadout/Filter/NoLimit", "Loadout/Filter/Enemy1", "Loadout/Filter/Enemy2", "Loadout/Filter/Enemy3" };
        private List<string> filters_stage = new List<string> { "Loadout/Filter/NoLimit", "Loadout/Filter/Galaxy1", "Loadout/Filter/Galaxy2", "Loadout/Filter/Galaxy3" };
        private List<string> filters_talent = new List<string> { "Loadout/Filter/NoLimit", "Loadout/Filter/Talent1", "Loadout/Filter/Talent2" };
        private List<string> filters_unittype = new List<string> { "Loadout/Filter/NoLimit", "Loadout/Filter/AttackUnit", "Loadout/Filter/SupportUnit" };
        private const string key_search = "Loadout/Text/Search";
        private bool show_pool = false;
        private bool search_des = false;
        private static int H_COUNT = 6;
        private bool upgrade = false;
        private int option_pool = 0;
        private UnitData selected = null;
        private Dictionary<string, string> args = new Dictionary<string, string>();
        private Dictionary<string, int> args2 = new Dictionary<string, int>();
        private string old_tooltip = "";
        private string new_tooltip = "";
        private List<string> ship_keys_common = new List<string> { "Loadout/Field/Reroll", "Loadout/Field/AssociationLv" };
        private List<string> ship_keys_normal = new List<string> { "Loadout/Field/StarCoin", "Loadout/Field/HP", "Loadout/Field/MaxHP", "Loadout/Field/Move", "Loadout/Field/MaxMove", "Loadout/Field/MaxShield", "Loadout/Field/MaxSwap", "Loadout/Field/EquipLimit", "Loadout/Field/Days", "Loadout/Field/EnergyCount", "Loadout/Field/MoveLimit" };
        private List<string> ship_keys_battle = new List<string> { "Loadout/Field/StarCoin", "Loadout/Field/HP", "Loadout/Field/MaxHP", "Loadout/Field/Move", "Loadout/Field/MaxMove", "Loadout/Field/Shield", "Loadout/Field/MaxShield", "Loadout/Field/Swap", "Loadout/Field/MaxSwap" };
        private List<string> enemy_keys_battle = new List<string> { "Loadout/Field/EnemyHP", "Loadout/Field/EnemyMaxHP", "Loadout/Field/EnemyDurability", "Loadout/Field/EnemyMaxDurability" };
        private List<PowerColors> energy_colors = new List<PowerColors>
        {
            new PowerColors(PowerColor.White),
            new PowerColors(PowerColor.Blue),
            new PowerColors(PowerColor.Purple),
            new PowerColors(PowerColor.Blue, PowerColor.White),
            new PowerColors(PowerColor.Purple, PowerColor.White),
            new PowerColors(PowerColor.Purple, PowerColor.Blue),
            new PowerColors(PowerColor.Purple, PowerColor.Blue, PowerColor.White)
        };
        private List<string> energy_colors_des = new List<string> { "Loadout/Button/Color1", "Loadout/Button/Color2", "Loadout/Button/Color3", "Loadout/Button/Color12", "Loadout/Button/Color13", "Loadout/Button/Color23", "Loadout/Button/Color123" };
        private List<Item> items = new List<Item> { null };
        public class DevInfo
        {
            public string itemID;
            public string skillName;
            public string imageName;
            public string animationName;
            public KeyCode keyPressed = KeyCode.None;
            public int keyExpire;
            const int keyExpireMax = 400; 
            public void CheckKey()
            {
                if (Event.current.keyCode == KeyCode.Q) { keyPressed = KeyCode.Q; keyExpire = keyExpireMax; }
                else if (Event.current.keyCode == KeyCode.W) { keyPressed = KeyCode.W; keyExpire = keyExpireMax; }
                else if (Event.current.keyCode == KeyCode.E) { keyPressed = KeyCode.E; keyExpire = keyExpireMax; }
                else if (Event.current.keyCode == KeyCode.R) { keyPressed = KeyCode.R; keyExpire = keyExpireMax; }
                else 
                {
                    keyExpire--;
                    if (keyExpire <= 0) keyPressed = KeyCode.None;
                    return;
                }
                string toClipboard = keyPressed switch
                {
                    KeyCode.Q => itemID,
                    KeyCode.W => FilePath.GetNameWithPath(skillName),
                    KeyCode.E => imageName,
                    KeyCode.R => animationName,
                    _ => null,
                };
                if (toClipboard != null) GUIUtility.systemCopyBuffer = toClipboard;
            }
            public override string ToString()
            {
                string s0 = keyPressed switch {
                    KeyCode.Q => "<color=#9BF37B>" + LeanLocalization.GetTranslationText("Loadout/Copied") + LeanLocalization.GetTranslationText("Loadout/CopyQ") + "</color>\n",
                    KeyCode.W => "<color=#9BF37B>" + LeanLocalization.GetTranslationText("Loadout/Copied") + LeanLocalization.GetTranslationText("Loadout/CopyW") + "</color>\n",
                    KeyCode.E => "<color=#9BF37B>" + LeanLocalization.GetTranslationText("Loadout/Copied") + LeanLocalization.GetTranslationText("Loadout/CopyE") + "</color>\n",
                    KeyCode.R => "<color=#9BF37B>" + LeanLocalization.GetTranslationText("Loadout/Copied") + LeanLocalization.GetTranslationText("Loadout/CopyR") + "</color>\n",
                    _ => LeanLocalization.GetTranslationText("Loadout/Copy"),
                };
                string s1 = itemID != null ? LeanLocalization.GetTranslationText("Loadout/CopyQ") + $"[Q]: {itemID}\n" : "";
                string s2 = skillName != null ? LeanLocalization.GetTranslationText("Loadout/CopyW") + $"[W]: {FilePath.GetNameWithPath(skillName)}\n" : "";
                string s3 = imageName != null ? LeanLocalization.GetTranslationText("Loadout/CopyE") + $"[E]: {imageName}\n" : "";
                string s4 = animationName != null ? LeanLocalization.GetTranslationText("Loadout/CopyR") + $"[R]: {animationName}\n" : "";
                if (itemID == null && skillName == null && imageName == null && animationName == null) s0 = "";
                return s0 + s1 + s2 + s3 + s4;
            }
        }
        public DevInfo devInfo = new DevInfo();

        private bool InWanted
        {
            get { return WantedManager.Instance().wantedProcess != null && ShipDatasManager.Instance().currentPlayerShipData != null; }
        }
        private bool InBattle
        {
            get { return BattleManager.Instance().inBattle; }
        }
        private static string tr(string s)
        {
            return LeanLocalization.GetTranslationText(s);
        }
        public void OnGUI()
        {
            float scalefactor = Math.Min(Screen.width / 1920f, Screen.height / 1080f);
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(scalefactor, scalefactor, 1));

            if (active)
            {
                UIManager.Instance().ShowPanel<BlockPanel>(FilePath.blockPanel, UI_Layer.System, null);
                width = 1920 * 3 / 5;
                height = 1080 * 3 / 5;
                rect = new Rect(1920 * 1 / 5, 1080 * 1 / 10, width, height);
                GUILayout.BeginArea(rect);
                int page = 0;
                if (InBattle) page = Make_Panel("Loadout/Keys/Home", titles_battle, false, 7);
                else if (InWanted) page = Make_Panel("Loadout/Keys/Home", titles_ingame, false, 7);
                else page = Make_Panel("Loadout/Keys/Home", titles_outgame, false, 7);
                switch (page)
                {
                    case 0:
                        Page_Ship();
                        break;
                    case 1:
                        Page_Enemy();
                        break;
                    case 2:
                        Page_ShipUnit();
                        break;
                    case 3:
                        Page_Treasure();
                        break;
                    case 4:
                        Page_Talent();
                        break;
                    case 5:
                        Page_Event();
                        break;
                    case 6:
                        Page_Refit();
                        break;
                    default:
                        break;
                }
                GUILayout.EndArea();

                if (LoadoutPlugin.config.is_dev && devInfo != null)
                {
                    devInfo.CheckKey();
                    GUILayout.Label(devInfo.ToString());
                }

                if (old_tooltip != new_tooltip && Event.current.type == EventType.Repaint)
                {
                    if (new_tooltip != null)
                    {
                        // Debug.Log("show!");
                        UIManager.Instance().HidePanel(FilePath.itemTipsPanelEvent);
                        UIManager.Instance().ShowPanel<ItemTipsPanel>(FilePath.itemTipsPanelEvent, UI_Layer.Overlay, delegate (ItemTipsPanel panel)
                        {
                            Traverse.Create(panel).Field("targetPos").SetValue(new Vector3(20f, -20f, 90f));
                            Traverse.Create(panel).Field("offsetVector").SetValue(new Vector3(0f, 0f, 0f));
                            panel.CreateTips(items, ShowDir.left, null);
                            LayoutRebuilder.ForceRebuildLayoutImmediate((panel as ItemTipsPanel).GetComponent<RectTransform>());
                        }, false);
                    }
                    else
                    {
                        UIManager.Instance().HidePanel(FilePath.itemTipsPanelEvent);
                        devInfo = new DevInfo();
                    }
                    old_tooltip = new_tooltip;
                }
            }
            if ((Event.current.keyCode == KeyCode.BackQuote && Event.current.type == EventType.KeyDown) || (active && Event.current.keyCode == KeyCode.Escape))
            {
                active = !active;
                if (!active)
                {
                    UIManager.Instance().HidePanelNoDestroy(FilePath.itemTipsPanelEvent);
                    UIManager.Instance().HidePanel(FilePath.blockPanel);
                }
                else
                {
                    args.Clear();
                    selected = null;
                    filter_ship = new List<string> { "Loadout/Filter/NoLimit" };
                    ship_ids = new List<int> { -1 };
                    foreach (DataShip d in DataShipManager.Instance().GetDataListByInGame(true))
                    {
                        filter_ship.Add(d.Name);
                        ship_ids.Add(d.ID);
                    }
                    filter_ship.Add("Loadout/Filter/Special");
                    ship_ids.Add(-2);
                }
            }
        }
        private int Make_Panel(string key, List<string> titles, bool show_label = true, int restrict = 0)
        {
            int page = args2.GetValueOrDefault(key, 0);
            if (page >= titles.Count) page = 0;
            GUILayout.BeginHorizontal();
            if (show_label) GUILayout.Label(tr(key), GUILayout.ExpandWidth(false));
            if (restrict <= 0) page = GUILayout.Toolbar(page, titles.Select(tr).ToArray());
            else page = GUILayout.Toolbar(page, titles.Select(tr).ToArray(), GUILayout.Width(width * titles.Count / restrict));
            GUILayout.EndHorizontal();
            args2[key] = page;
            return page;
        }
        private void Page_Enemy()
        {
            if (!InBattle)
            {
                int fShip = Make_Panel("Loadout/Filter/Ship", filter_ship);
                int fStage = Make_Panel("Loadout/Filter/Galaxy", filters_stage);
                int fRare = Make_Panel("Loadout/Filter/Category", filters_enemy);
                string fSearch = Make_Search(true, false, false).ToUpper();
                List<int> enemy_ships = new List<int> { };
                foreach (DataEnemyShip d in DataEnemyShipManager.Instance().GetDataList())
                {
                    if (enemy_ships.Contains(d.ID)) continue;

                    int ship_id = ship_ids[fShip];
                    bool check = (ship_id == -1 || (ship_id == -2 && !d.InGame) || (ship_id >= 0 && d.InGame && (d.OnlyPro == null || d.OnlyPro.Contains(ship_id))));
                    if (!check) continue;

                    check = (fRare == 0) || (fRare >= 1 && d.EnemyType == fRare - 1);
                    if (!check) continue;

                    check = fStage == 0 || (fStage >= 1 && Math.Min(d.EnemyPhaseType % 10, 3) == fStage);
                    if (!check) continue;

                    check = tr(d.Name).ToUpper().Contains(fSearch) || (search_des && tr(d.Des).ToUpper().Contains(fSearch));
                    if (!check) continue;

                    enemy_ships.Add(d.ID);
                }
                for (int i = enemy_ships.Count; i % H_COUNT != 0; i++) enemy_ships.Add(0);
                enemy_ships.Add(-1);
                foreach (DataBattleEvent d in DataBattleEventManager.Instance().GetDataList())
                {
                    if (enemy_ships.Contains(-d.ID)) continue;

                    int ship_id = ship_ids[fShip];
                    bool check = (ship_id == -1 || (ship_id == -2 && !d.InGame) || (ship_id >= 0 && d.InGame && d.Pros.Contains(ship_id)));
                    if (!check) continue;

                    check = tr(d.Name).ToUpper().Contains(fSearch) || (search_des && tr(d.Des).ToUpper().Contains(fSearch));
                    if (!check) continue;

                    enemy_ships.Add(-d.ID);
                }
                Make_Grid(4, enemy_ships);
            }
            else
            {
                Make_TextFields(enemy_keys_battle);
                if (GUILayout.Button(tr("Loadout/Button/Kill")))
                {
                    BattleManager.Instance().ShipDownHandler(BattleManager.Instance().enemyShip);
                    active = false;
                    UIManager.Instance().HidePanel(FilePath.blockPanel);
                }
                if (GUILayout.Button(tr("Loadout/Button/Suicide")))
                {
                    BattleManager.Instance().ShipDownHandler(BattleManager.Instance().playerShip);
                    active = false;
                    UIManager.Instance().HidePanel(FilePath.blockPanel);
                }

            }
        }
        private void Page_ShipUnit()
        {
            int fShip = Make_Panel("Loadout/Filter/Ship", filter_ship);
            int fRare = Make_Panel("Loadout/Filter/Rare", filters_rare);
            int fType = Make_Panel("Loadout/Filter/Category", filters_unittype);
            string fSearch = Make_Search(true, true, true).ToUpper();
            int batch_option = Make_PoolOptions("Loadout/Text/Operation");
            List<int> shipunits = new List<int> { };
            foreach (DataShipUnit d in DataShipUnitManager.Instance().GetDataList())
            {
                if (shipunits.Contains(d.ID)) continue;

                bool check = (d.Lv == 1 && !upgrade) || (d.Lv == 2 && upgrade);
                if (!check) continue;

                bool d_ingame = d.InGame && (d.GainType == 0 || d.GainType == 4);
                int ship_id = ship_ids[fShip];
                check = (ship_id == -1 || (ship_id == -2 && (!d.InGame || d.GainType == 5)) || (ship_id >= 0 && d_ingame && d.Pros.Contains(ship_id)) || (ship_id >= 0 && d_ingame && d.Pros.Length == 0));
                if (!check) continue;

                check = (fType == 0) || (fType >= 1 && fType == d.Type);
                if (!check) continue;

                check = (fRare == 0) || (fRare >= 1 && d.Rare == fRare - 1);
                if (!check) continue;

                check = tr(d.Name).ToUpper().Contains(fSearch) || (search_des && tr(d.Description).ToUpper().Contains(fSearch));
                if (!check) continue;

                shipunits.Add(d.ID);
                if (batch_option == 1) WantedManager.Instance().wantedProcess.AddShipUnitPool(d);
                if (batch_option == -1) WantedManager.Instance().wantedProcess.RemoveShipUnitPool(d);
            }
            Make_Grid(0, shipunits);
        }
        private void Page_Refit()
        {
            List<int> shipunits = new List<int> { };
            if (selected != null)
            {
                UnitData new_unit = null;
                GUILayout.BeginHorizontal();

                string title = tr(selected.GetData().Name);
                string colorform = tr("Loadout/Text/Operation2") + RareColor.GetColorForm(ColorUtility.ToHtmlStringRGB(RareColor.GetColorByRare(selected.GetData().Rare)), title);
                GUILayout.Label(colorform);

                if (GUILayout.Button(tr("Loadout/Button/Upgrade")))
                {
                    new_unit = UnitData.Clone(selected);
                    if (!new_unit.LvUp()) new_unit = null;
                }
                if (GUILayout.Button(tr("Loadout/Button/Degrade")))
                {
                    new_unit = UnitData.Clone(selected);
                    if (!new_unit.LvDown()) new_unit = null;
                }
                if (GUILayout.Button(tr("Loadout/Button/Copy")))
                {
                    new_unit = UtilsClass.Clone<UnitData>(selected);
                    Singleton<ItemManager>.Instance().AddItem(new_unit);
                    new_unit = null;
                }
                if (GUILayout.Button(tr("Loadout/Button/Remove")))
                {
                    UnitData.RemovePlayerUnit(selected);
                    selected = null;
                }
                if (GUILayout.Button(tr("Loadout/Button/BackToPool")))
                {
                    WantedManager.Instance().wantedProcess.AddShipUnitPool(selected.id);
                    UnitData.RemovePlayerUnit(selected);
                    selected = null;
                }

                if (GUILayout.Button(tr("Loadout/Button/Back"))) selected = null;
                GUILayout.EndHorizontal();
                if (selected == null) return;
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();
                GUILayout.Label(tr("Loadout/Text/ModAdd"));
                scrollVector2 = GUILayout.BeginScrollView(scrollVector2, GUILayout.Width(width / 2 - 50));
                foreach (SlotRefitType t in Enum.GetValues(typeof(SlotRefitType)))
                {
                    string s = "(+)" + tr(UnitData.GetRefitString(t));
                    List<int> refits = new List<int> { (int)t };
                    if (GUILayout.Button(s))
                    {
                        if (UnitData.CheckSlotRefitable(selected, refits))
                        {
                            new_unit = UnitData.Clone(selected);
                            new_unit.CloneSlotRefitsAndAdd(refits);
                        }
                    }
                }
                GUILayout.EndScrollView();
                GUILayout.EndVertical();
                GUILayout.BeginVertical();
                GUILayout.Label(tr("Loadout/Text/ModRemove"));
                scrollVector3 = GUILayout.BeginScrollView(scrollVector3, GUILayout.Width(width / 2 - 50));
                for (int j = 0; j < selected.slotRefitTypes.Count; j++)
                {
                    SlotRefitType t = selected.slotRefitTypes[j];
                    string s = "(-)" + tr(UnitData.GetRefitString(t));
                    if (GUILayout.Button(s))
                    {
                        List<SlotRefitType> new_refits = new List<SlotRefitType> { };
                        for (int k = 0; k < selected.slotRefitTypes.Count; k++)
                            if (k != j) new_refits.Add(selected.slotRefitTypes[k]);
                        if (UnitData.CheckSlotRefitChangeable(selected, new_refits))
                        {
                            new_unit = UnitData.Clone(selected);
                            new_unit.slotRefitTypes = new_refits;
                        }
                    }
                }
                GUILayout.EndScrollView();
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                
                if (new_unit != null)
                {
                    UnitData.RefreshPlayerUnit(selected, new_unit, null);
                    EventCenter.Instance().EventTrigger(EventName.refreshInventory);
                    AudioManager.Instance().PlaySound(FilePath.sound + "Slot/Slot_Reform", false, null, true);
                    selected = new_unit;
                }
            }
            else
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(tr("Loadout/Button/RemoveUnequipped"), GUILayout.Width(width / H_COUNT - 10)))
                {
                    ItemManager.Instance().RemoveAllItems();
                }
                if (GUILayout.Button(tr("Loadout/Button/RecoverUnequipped"), GUILayout.Width(width / H_COUNT - 10)))
                {
                    foreach (ItemUnit itm in ItemManager.Instance().itemUnits)
                    {
                        WantedManager.Instance().wantedProcess.AddShipUnitPool(itm.unitData.id);
                    }
                    ItemManager.Instance().RemoveAllItems();
                }
                GUILayout.EndHorizontal();
                for (int i = 0; i < UnitData.GetPlayerUnits().Count; i++) shipunits.Add(i);
                Make_Grid(5, shipunits);
            }
        }
        private void Page_Treasure()
        {
            int fShip = Make_Panel("Loadout/Filter/Ship", filter_ship);
            int fRare = Make_Panel("Loadout/Filter/Rare", filters_rare);
            string fSearch = Make_Search(true, false, true).ToUpper();
            int batch_option = Make_PoolOptions("Loadout/Text/Operation");
            List<int> treasures = new List<int> { };
            foreach (DataTreasure d in DataTreasureManager.Instance().GetDataList())
            {
                if (treasures.Contains(d.ID)) continue;

                int ship_id = ship_ids[fShip];
                bool check = (ship_id == -1 || (ship_id == -2 && (!d.InGame || d.GainType == 5)) || (ship_id >= 0 && d.InGame && d.Pros.Contains(ship_id)));
                if (!check) continue;

                check = (fRare == 0) || (fRare >= 1 && d.Rare == fRare - 1);
                if (!check) continue;

                check = tr(d.Name).ToUpper().Contains(fSearch) || (search_des && tr(d.Description).ToUpper().Contains(fSearch));
                if (!check) continue;

                treasures.Add(d.ID);

                if (batch_option == 1) WantedManager.Instance().wantedProcess.AddTreasurePool(d);
                if (batch_option == -1) WantedManager.Instance().wantedProcess.RemoveTreasurePool(d);
            }
            Make_Grid(1, treasures);
        }
        private void Page_Talent()
        {
            int fShip = Make_Panel("Loadout/Filter/Ship", filter_ship);
            int fRare = Make_Panel("Loadout/Filter/Category", filters_talent);
            string fSearch = Make_Search(true, false, false).ToUpper();
            List<int> talents = new List<int> { };
            foreach (DataTalent d in DataTalentManager.Instance().GetDataList())
            {
                if (talents.Contains(d.ID)) continue;

                int ship_id = ship_ids[fShip];
                bool check = (ship_id == -1 || (ship_id == -2 && !d.InGame) || (ship_id >= 0 && d.InGame && d.Pros.Contains(ship_id)));
                if (!check) continue;

                check = (fRare == 0) || (fRare >= 1 && d.Type == fRare);
                if (!check) continue;

                check = tr(d.Name).ToUpper().Contains(fSearch) || (search_des && tr(d.Des).ToUpper().Contains(fSearch));
                if (!check) continue;

                talents.Add(d.ID);
            }
            Make_Grid(2, talents);
        }
        private void Page_Event()
        {
            int fShip = Make_Panel("Loadout/Filter/Ship", filter_ship);
            int fRare = Make_Panel("Loadout/Filter/Category", filters_rare);
            string fSearch = Make_Search(true, false, false).ToUpper();
            List<int> events = new List<int> { };
            List<int> event_names = new List<int> { };
            foreach (DataEncounterEvents d in DataEncounterEventsManager.Instance().GetDataList())
            {
                if (events.Contains(d.ID) || event_names.Contains(d.EventGroup)) continue;

                int ship_id = ship_ids[fShip];
                bool check = (ship_id == -1 || (ship_id == -2 && !d.InGame) || (ship_id >= 0 && d.InGame && d.Pros.Contains(ship_id)));
                if (!check) continue;

                check = (fRare == 0) || (fRare >= 1 && fRare == d.Days - 1) || ((d.Days < 2 || d.Days > 4) && fRare == 0);
                if (!check) continue;

                check = tr(d.Name).ToUpper().Contains(fSearch) || (search_des && tr(d.Des).ToUpper().Contains(fSearch));
                if (!check) continue;

                events.Add(d.ID);
                event_names.Add(d.EventGroup);
            }
            Make_Grid(3, events);
        }
        private void Page_Ship()
        {
            if (!InWanted)
            {
                Make_TextFields(ship_keys_common
                    .Concat(ship_ids.Where(d => d >= 0).Select(d => $"_Lv-{d}"))
                    .Concat(ship_ids.Where(d => d >= 0).Select(d => $"_Di-{d}")).ToList());
            }
            else if (InBattle) Make_TextFields(ship_keys_battle);
            else
            {
                // Patch_PreviewManager.Toggle = GUILayout.Toggle(Patch_PreviewManager.Toggle, "启动预览");
                Make_TextFields(ship_keys_normal);
                GUILayout.BeginHorizontal();
                GUILayout.Label(tr("Loadout/Text/EnergyAdd"));
                for (int i = 0; i < energy_colors.Count; i++)
                {
                    if (GUILayout.Button(tr(energy_colors_des[i])))
                    {
                        ShipDatasManager.Instance().currentPlayerShipData.AddPowerColorWeight(energy_colors[i]);
                        UIManager.Instance().CreateCoreInfo(string.Format(tr("Core/Info/Color/NormalAdd"), PowerData.GetPowerColorDes(energy_colors[i])));
                    }
                }
                for (int i = 1; i <= 9; i++)
                {
                    if (GUILayout.Button(i.ToString()))
                    {
                        ShipDatasManager.Instance().currentPlayerShipData.AddPowerWeight(i);
                        UIManager.Instance().CreateCoreInfo(string.Format(tr("Core/Info/Point/NormalAdd"), i));
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label(tr("Loadout/Text/EnergyRemove"));
                for (int i = 0; i < energy_colors.Count; i++)
                {
                    if (GUILayout.Button(tr(energy_colors_des[i])) && ShipDatasManager.Instance().currentPlayerShipData.powerColorWeights.Count > 1)
                    {
                        foreach (PowerColorWeight powerColorWeight in ShipDatasManager.Instance().currentPlayerShipData.powerColorWeights)
                        {
                            if (powerColorWeight.powerColors.EqualColors(energy_colors[i]))
                            {
                                ShipDatasManager.Instance().currentPlayerShipData.RemovePowerColorWeight(powerColorWeight);
                                UIManager.Instance().CreateCoreInfo(string.Format(tr("Core/Info/Color/NormalRemove"), PowerData.GetPowerColorDes(energy_colors[i])));
                                break;
                            }
                        }
                    }
                }
                for (int i = 1; i <= 9; i++)
                {
                    if (GUILayout.Button(i.ToString()) && ShipDatasManager.Instance().currentPlayerShipData.powerWeights.Count > 1)
                    {
                        foreach (PowerWeight powerWeight in Singleton<ShipDatasManager>.Instance().currentPlayerShipData.powerWeights)
                        {
                            if (powerWeight.power == i)
                            {
                                ShipDatasManager.Instance().currentPlayerShipData.RemovePowerWeight(powerWeight);
                                UIManager.Instance().CreateCoreInfo(string.Format(tr("Core/Info/Point/NormalRemove"), powerWeight.power));
                                break;
                            }
                        }
                    }
                }
                GUILayout.EndHorizontal();
            }
        }
        private void Make_TextFields(List<string> keys)
        {
            int i_old, i_new;
            string s_old, s_new;
            bool pressed;

            foreach (string key in keys)
            {
                GUILayout.BeginHorizontal();
                if (key.StartsWith("_Di-")) GUILayout.Label(tr(DataShipManager.Instance().GetDataByID(int.Parse(key.Substring(4))).Name) + tr("Loadout/Field/ShipDifficulty"), GUILayout.Width(width / 5));
                else if (key.StartsWith("_Lv-")) GUILayout.Label(tr(DataShipManager.Instance().GetDataByID(int.Parse(key.Substring(4))).Name) + tr("Loadout/Field/ShipLv"), GUILayout.Width(width / 5));
                else GUILayout.Label(tr(key), GUILayout.Width(width / 5));
                i_old = key switch
                {
                    "Loadout/Field/StarCoin" => WantedManager.Instance().wantedProcess.starCoin,
                    "Loadout/Field/HP" => ShipDatasManager.Instance().currentPlayerShipData.currentHP,
                    "Loadout/Field/MaxHP" => ShipDatasManager.Instance().currentPlayerShipData.maxHP,
                    "Loadout/Field/Move" => ShipDatasManager.Instance().currentPlayerShipData.currentMove,
                    "Loadout/Field/MaxMove" => ShipDatasManager.Instance().currentPlayerShipData.maxMove,
                    "Loadout/Field/Shield" => ShipDatasManager.Instance().currentPlayerShipData.currentShield,
                    "Loadout/Field/MaxShield" => ShipDatasManager.Instance().currentPlayerShipData.maxShield,
                    "Loadout/Field/Swap" => ShipDatasManager.Instance().currentPlayerShipData.currentExchange,
                    "Loadout/Field/MaxSwap" => ShipDatasManager.Instance().currentPlayerShipData.maxExchange,
                    "Loadout/Field/EquipLimit" => ShipDatasManager.Instance().currentPlayerShipData.shipLoadLimit,
                    "Loadout/Field/Days" => WantedManager.Instance().wantedProcess.currentDays,
                    "Loadout/Field/Reroll" => SaveManager.Instance().saveDataPack.GetCurrentData().roll,
                    "Loadout/Field/AssociationLv" => SaveManager.Instance().saveDataPack.GetCurrentData().totalLV,
                    "Loadout/Field/EnergyCount" => Define.defineValue.powerNumEveryTurn,
                    "Loadout/Field/MoveLimit" => Define.defineValue.moveCount,
                    "Loadout/Field/EnemyHP" => BattleManager.Instance().enemyShip.shipData.currentHP,
                    "Loadout/Field/EnemyMaxHP" => BattleManager.Instance().enemyShip.shipData.maxHP,
                    "Loadout/Field/EnemyDurability" => BattleManager.Instance().enemyShip.breakSystem.breakCount.Value,
                    "Loadout/Field/EnemyMaxDurability" => BattleManager.Instance().enemyShip.breakSystem.breakLimit,
                    _ => 0,
                };
                if (key.StartsWith("_Di-") || key.StartsWith("_Lv-"))
                {
                    int ship_id = int.Parse(key.Substring(4));
                    var data = SaveManager.Instance().saveDataPack.GetCurrentData().saveDataShips.Where(d => d.shipID == ship_id).FirstOrDefault();
                    i_old = key.StartsWith("_Di-") ? data.difficulty : data.lv;
                }
                
                s_old = args.GetValueOrDefault(key, i_old.ToString());
                s_new = GUILayout.TextArea(s_old, GUILayout.ExpandWidth(true));
                if (s_new.Contains('\n')) { s_new = s_new.Trim().Replace("\n", ""); pressed = true; }
                else pressed = false;
                args[key] = s_new;

                if (pressed)
                {
                    if (!int.TryParse(s_new, out i_new)) i_new = i_old;
                    switch (key)
                    {
                        case "Loadout/Field/StarCoin":
                            if (i_new < 0 && !Define.defineValue.creditCard) i_new = 0;
                            WantedManager.Instance().wantedProcess.SetCoin(i_new);
                            break;
                        case "Loadout/Field/HP":
                            if (i_new < 1) i_new = 1;
                            if (InBattle) ShipDatasManager.Instance().currentPlayerShipData.SetHP_Battle(i_new - i_old);
                            else ShipDatasManager.Instance().currentPlayerShipData.SetHP_Normal(i_new - i_old);
                            break;
                        case "Loadout/Field/MaxHP":
                            if (i_new < 1) i_new = 1;
                            ShipDatasManager.Instance().currentPlayerShipData.SetMaxHP_Normal(i_new - i_old);
                            break;
                        case "Loadout/Field/Move":
                            if (i_new < 0) i_new = 0;
                            if (InBattle) ShipDatasManager.Instance().currentPlayerShipData.SetMove_Battle(i_new - i_old);
                            else ShipDatasManager.Instance().currentPlayerShipData.SetMove_Normal(i_new - i_old);
                            break;
                        case "Loadout/Field/MaxMove":
                            if (i_new < 0) i_new = 0;
                            ShipDatasManager.Instance().currentPlayerShipData.SetMaxMove(i_new - i_old);
                            break;
                        case "Loadout/Field/Shield":
                            if (i_new < 0) i_new = 0;
                            ShipDatasManager.Instance().currentPlayerShipData.SetCurrentShield_Battle(i_new - i_old);
                            break;
                        case "Loadout/Field/MaxShield":
                            if (i_new < 0) i_new = 0;
                            ShipDatasManager.Instance().currentPlayerShipData.SetMaxShield(i_new - i_old);
                            break;
                        case "Loadout/Field/Swap":
                            if (i_new < 0) i_new = 0;
                            ShipDatasManager.Instance().currentPlayerShipData.SetExchange_Battle(i_new - i_old);
                            break;
                        case "Loadout/Field/MaxSwap":
                            if (i_new < 0) i_new = 0;
                            ShipDatasManager.Instance().currentPlayerShipData.SetMaxExchange_Normal(i_new - i_old);
                            break;
                        case "Loadout/Field/EquipLimit":
                            if (i_new < 0) i_new = 0;
                            ShipDatasManager.Instance().currentPlayerShipData.SetLoad(i_new - i_old);
                            ShipCell shipCell = ShipDatasManager.Instance().currentPlayerShipData.GetEquipedShipCells()[0];
                            Traverse.Create(shipCell).Method("CheckLimitOver").GetValue();
                            break;
                        case "Loadout/Field/Days":
                            if (i_new < 0) i_new = 0;
                            Singleton<WantedManager>.Instance().wantedProcess.AddDays(i_new - i_old);
                            VacationEventPanel panel = Singleton<UIManager>.Instance().GetPanel<VacationEventPanel>(FilePath.vacationEventPanel);
                            if (panel != null && VacationEventPanel.onVacation) panel.RefreshDays();
                            break;
                        case "Loadout/Field/Reroll":
                            if (i_new < 0) i_new = 0;
                            SaveManager.Instance().saveDataPack.GetCurrentData().roll = i_new;
                            ShipSelectPanel panel1 = UIManager.Instance().GetPanel<ShipSelectPanel>(FilePath.shipSelectPanel);
                            if (panel1 != null) panel1.RollText();
                            SaveManager.Instance().SavePermanentData();
                            break;
                        case "Loadout/Field/AssociationLv":
                            if (i_new < 0) i_new = 0;
                            if (i_new > Define.lvLimit) i_new = Define.lvLimit;
                            SaveData saveData = SaveManager.Instance().saveDataPack.GetCurrentData();
                            saveData.totalLV = i_new;
                            foreach (var pilot in saveData.saveDataPilots)
                            {
                                if (saveData.totalLV >= DataPilotManager.Instance().GetDataByID(pilot.ID).UnlockLv)
                                    pilot.unlock = true;
                                else pilot.unlock = false;
                            }
                            foreach (var data in saveData.saveDataShips)
                            {
                                data.pilots.Clear();
                                data.pilotsForCusMod.Clear();
                                data.currentPilot = null;
                            }
                            ShipSelectPanel panel2 = UIManager.Instance().GetPanel<ShipSelectPanel>(FilePath.shipSelectPanel);
                            if (panel2 != null && panel2.isActiveAndEnabled) { panel2.Hide(); panel2.Show(); }
                            SaveManager.Instance().SavePermanentData();
                            break;
                        case "Loadout/Field/EnergyCount":
                            if (i_new < 0) i_new = 0;
                            Define.defineValue.powerInitNum += i_new - i_old;
                            Define.defineValue.powerNumEveryTurn = i_new;
                            break;
                        case "Loadout/Field/MoveLimit":
                            if (i_new < 0) i_new = 0;
                            Define.defineValue.moveCount = i_new;
                            break;
                        case "Loadout/Field/EnemyHP":
                            if (i_new < 1) i_new = 1;
                            BattleManager.Instance().enemyShip.shipData.SetHP_Battle(i_new - i_old);
                            break;
                        case "Loadout/Field/EnemyMaxHP":
                            if (i_new < 1) i_new = 1;
                            BattleManager.Instance().enemyShip.shipData.SetMaxHP_Normal(i_new - i_old);
                            break;
                        case "Loadout/Field/EnemyMaxDurability":
                            if (i_new < 1) i_new = 1;
                            BattleManager.Instance().enemyShip.breakSystem.breakLimit = i_new;
                            break;
                        case "Loadout/Field/EnemyDurability":
                            if (i_new < 0 || i_old == 0) i_new = 0;
                            if (i_new - i_old > 0)
                            {
                                BattleManager.Instance().enemyShip.breakSystem.breakCount.Value = i_new;
                                EventCenter.Instance().EventTrigger(EventName.breakRecover);
                            }
                            else BattleManager.Instance().enemyShip.breakSystem.BreakCount(i_old - i_new);
                            break;
                        default:
                            break;
                    };
                    if (key.StartsWith("_Di-") || key.StartsWith("_Lv-"))
                    {
                        int ship_id = int.Parse(key.Substring(4));
                        var data = SaveManager.Instance().saveDataPack.GetCurrentData().saveDataShips.Where(d => d.shipID == ship_id).FirstOrDefault();
                        if (key.StartsWith("_Di-"))
                        {
                            if (i_new < 0) i_new = 0;
                            if (i_new > Define.difficultyLimit) i_new = Define.difficultyLimit;
                            data.difficulty = i_new;
                            data.selectHandlyDifficulty = i_new;
                        }
                        else
                        {
                            if (i_new < 0) i_new = 0;
                            if (i_new > Define.lvLimit) i_new = Define.lvLimit;
                            data.lv = i_new;
                        }
                    }

                    args.Clear();
                    GUI.FocusControl("");
                }
                GUILayout.EndHorizontal();
            }
        }
        public string Make_Search(bool show_search_desc = true, bool show_upgrade = false, bool show_pool_options = false)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(tr(key_search), GUILayout.ExpandWidth(false));
            string content = args.GetValueOrDefault(key_search, "");
            content = GUILayout.TextArea(content, GUILayout.ExpandWidth(true));
            if (content.Contains('\n')) { GUI.FocusControl(""); content = content.Trim().Replace("\n", ""); }
            if (GUILayout.Button("X", GUILayout.Width(30))) content = "";
            if (show_search_desc) search_des = GUILayout.Toggle(search_des, tr("Loadout/Text/SearchDesc"));
            if (show_upgrade) upgrade = GUILayout.Toggle(upgrade, tr("Loadout/Text/SearchUpgrade"));
            if (show_pool_options) show_pool = GUILayout.Toggle(show_pool, tr("Loadout/Text/SearchAdvanced"));
            args[key_search] = content;
            GUILayout.EndHorizontal();
            return content.Trim();
        }
        public int Make_PoolOptions(string prompt)
        {
            if (!show_pool)
            {
                option_pool = 0;
                return 0;
            }
            // Debug.Log("Make option "+string.Join(" ", options_pool.Select(tr)));
            int result = 0;
            GUILayout.BeginHorizontal();
            GUILayout.Label(tr(prompt));
            option_pool = GUILayout.Toolbar(option_pool, options_pool.Select(tr).ToArray(), "toggle");
            GUILayout.EndHorizontal();
            if (option_pool == 1 || option_pool == 2)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(tr("Loadout/Button/BatchAdd"), GUILayout.Width(width / H_COUNT - 10))) result = 1;
                if (GUILayout.Button(tr("Loadout/Button/BatchRemove"), GUILayout.Width(width / H_COUNT - 10))) result = -1;
                GUILayout.EndHorizontal();
            }
            return result;
        }
        public static string CompressString(string s, int limit = 80)
        {
            s = tr(s).Replace("\n", "    ");
            foreach (Match match in Regex.Matches(s, @"<[^>]+>")) s = s.Replace(match.Value, "");
            if (s.Length > limit) s = s[..(limit - 3)] + "...";
            return s;
        }
        public void Make_Grid(int type, List<int> lid, int hCount = -1)
        {
            int i = 0;
            // Texture texture;
            GUIContent content;
            string tooltip;
            string title;
            string colorform;
            new_tooltip = null;
            int count = 0;
            string rare_text;
            string type_text;
            if (hCount <= 0) hCount = H_COUNT;
            scrollVector = GUILayout.BeginScrollView(scrollVector, GUILayout.MaxHeight(height));
            while (i < lid.Count)
            {
                GUILayout.BeginHorizontal();
                for (int j = 0; j < hCount && i < lid.Count; j++)
                {
                    switch (type)
                    {
                        case 0:
                            foreach (DataShipUnit d in DataShipUnitManager.Instance().GetDataListByID(lid[i]))
                            {
                                if (d.Lv == 1 && upgrade || d.Lv != 1 && !upgrade) continue;
                                // texture = ResourcesManager.Instance().Load<Sprite>(FilePath.spriteUnits + d.SpritePath).texture;
                                title = tr(d.Name);
                                if (show_pool)
                                {
                                    count = 0;
                                    foreach (int k in WantedManager.Instance().wantedProcess._shipUnitPool)
                                        if (k == d.ID) count++;
                                    title += $"({count})";
                                }
                                colorform = RareColor.GetColorForm(ColorUtility.ToHtmlStringRGB(RareColor.GetColorByRare(d.Rare)), title);
                                tooltip = $"部件:{d.ID}-{d.Lv}";
                                content = new GUIContent(colorform, tooltip);
                                if (GUILayout.Button(content, GUILayout.Width(width / hCount - 10)))
                                {
                                    if (option_pool == 0) ItemManager.Instance().AddItem(new ItemUnit(d));
                                    else if (option_pool == 1) WantedManager.Instance().wantedProcess.AddShipUnitPool(d);
                                    else if (option_pool == 2) WantedManager.Instance().wantedProcess.RemoveShipUnitPool(d);
                                }
                                if (GUI.tooltip == tooltip)
                                {
                                    new_tooltip = tooltip;
                                    items[0] = new ItemUnit(d);
                                    if (old_tooltip != tooltip) devInfo = new DevInfo
                                    {
                                        itemID = d.modID == null ? d.ID.ToString() : $"{d.modID}.{d.nameInMod}",
                                        skillName = d.SkillPath,
                                        imageName = d.SpritePath,
                                        animationName = d.ModPath,
                                    };
                                }
                                break;
                            }
                            break;
                        case 1:
                            DataTreasure dt = DataTreasureManager.Instance().GetDataByID(lid[i]);
                            title = tr(dt.Name);
                            if (show_pool)
                            {
                                count = 0;
                                foreach (int k in WantedManager.Instance().wantedProcess._treasurePool)
                                    if (k == dt.ID) count++;
                                title += $"({count})";
                            }
                            colorform = RareColor.GetColorForm(ColorUtility.ToHtmlStringRGB(RareColor.GetColorByRare(dt.Rare)), title);
                            tooltip = $"宝物:{dt.ID}";
                            content = new GUIContent(colorform, tooltip);
                            if (GUILayout.Button(content, GUILayout.Width(width / hCount - 10)))
                            {
                                if (option_pool == 0) ItemManager.Instance().AddTreasure(new ItemTreasure(dt));
                                else if (option_pool == 1) WantedManager.Instance().wantedProcess.AddTreasurePool(dt);
                                else if (option_pool == 2) WantedManager.Instance().wantedProcess.RemoveTreasurePool(dt);
                            }
                            if (GUI.tooltip == tooltip)
                            {
                                new_tooltip = tooltip;
                                items[0] = new ItemTreasure(dt);
                                if (old_tooltip != tooltip) devInfo = new DevInfo
                                {
                                    itemID = dt.modID == null ? dt.ID.ToString() : $"{dt.modID}.{dt.nameInMod}",
                                    skillName = dt.SkillPath,
                                    imageName = dt.SpritePath,
                                };
                            }
                            break;
                        case 2:
                            DataTalent dta = DataTalentManager.Instance().GetDataByID(lid[i]);
                            title = tr(dta.Name);
                            colorform = RareColor.GetColorForm(ColorUtility.ToHtmlStringRGB(RareColor.GetColorByRare(dta.Type)), title);
                            tooltip = $"天赋:{dta.ID}";
                            content = new GUIContent(colorform, tooltip);
                            if (GUILayout.Button(content, GUILayout.Width(width / hCount - 10)))
                                ShipDatasManager.Instance().currentPilotData.AddTalent(dta.ID);
                            if (GUI.tooltip == tooltip)
                            {
                                new_tooltip = tooltip;
                                items[0] = new ItemTalent(dta);
                                if (old_tooltip != tooltip) devInfo = new DevInfo
                                {
                                    itemID = dta.modID == null ? dta.ID.ToString() : $"{dta.modID}.{dta.nameInMod}",
                                    skillName = dta.SkillPath,
                                    imageName = dta.Image,
                                };
                            }
                            break;
                        case 3:
                            DataEncounterEvents de = DataEncounterEventsManager.Instance().GetDataByID(lid[i]);
                            title = tr(de.Name);
                            if (de.Days < 2 || de.Days > 4) rare_text = tr("Loadout/Label/Special");
                            else rare_text = null;
                            if (DataBountyEventsManager.Instance().GetDataByID(de.ID) != null) type_text = tr("Loadout/Label/CallForSupport");
                            else type_text = tr("Loadout/Label/Event");
                            colorform = RareColor.GetColorForm(ColorUtility.ToHtmlStringRGB(RareColor.GetColorByRare(de.Days - 2)), title);
                            tooltip = $"事件:{de.ID}";
                            content = new GUIContent(colorform, tooltip);
                            if (GUILayout.Button(content, GUILayout.Width(width / hCount - 10)))
                            {
                                if (DataBountyEventsManager.Instance().GetDataByID(de.ID) != null) WantedManager.Instance().wantedProcess.AddBountyEvent(de.ID);
                                else EncounterEvent.Event(de.ID);
                            }
                            if (GUI.tooltip == tooltip)
                            {
                                new_tooltip = tooltip;
                                items[0] = new ItemAny(de.ID, -4, de.Days - 2, de.Name, CompressString(de.Des), type_text, rare_text);
                                if (old_tooltip != tooltip) devInfo = new DevInfo
                                {
                                    itemID = de.modID == null ? de.ID.ToString() : $"{de.modID}.{de.nameInMod}",
                                };
                            }
                            break;
                        case 4:
                            WantedProcess p = WantedManager.Instance().wantedProcess;
                            if (lid[i] > 0)
                            {
                                DataEnemyShip des = DataEnemyShipManager.Instance().GetDataByID(lid[i]);
                                title = tr(des.Name);
                                if (des.OnlyPro != null && des.OnlyPro.Length > 0)
                                {
                                    string addon = "";
                                    for (int k = 0; k < ship_ids.Count; k++)
                                    {
                                        if (des.OnlyPro.Contains(ship_ids[k]))
                                        {
                                            if (addon != "") addon += ", ";
                                            addon += k;
                                        }
                                    }
                                    if (addon != "") title = $"{title}({addon})";
                                }
                                int cur_id = p.wantedPhases.enemyIDs[p.wantedPhases.currentBigPhase][p.wantedPhases.currentSmallPhase];
                                if (cur_id == des.ID) title += "(√)";
                                type_text = "WantedPanel/Danger/Low";
                                if (des.EnemyType == 1) type_text = "WantedPanel/Danger/Mid";
                                if (des.EnemyType == 2) type_text = "WantedPanel/Danger/High";
                                if (!des.InGame) type_text = "Loadout/Label/NotAvail";
                                type_text = tr(type_text);
                                rare_text = tr(filters_stage[Math.Min(des.EnemyPhaseType % 10, 3)]);
                                colorform = RareColor.GetColorForm(ColorUtility.ToHtmlStringRGB(RareColor.GetColorByRare(des.EnemyType)), title);
                                tooltip = $"敌人:{des.ID}";
                                content = new GUIContent(colorform, tooltip);
                                if (GUILayout.Button(content, GUILayout.Width(width / hCount - 10)))
                                {
                                    p.currentWanted.battleEventID = -1;
                                    bool isactive = UIManager.Instance().GetPanelActiveStatus<WantedPanel>(FilePath.wantedPanel);
                                    WantedManager.Instance().RerollWanted(des.ID);
                                    if (!isactive) UIManager.Instance().HidePanelNoDestroy(FilePath.wantedPanel);
                                }
                                if (GUI.tooltip == tooltip)
                                {
                                    new_tooltip = tooltip;
                                    items[0] = new ItemAny(des.ID, -5, des.EnemyType, des.Name, CompressString(des.Des), type_text, rare_text);
                                    if (old_tooltip != tooltip) devInfo = new DevInfo
                                    {
                                        itemID = des.modID == null ? des.ID.ToString() : $"{des.modID}.{des.nameInMod}",
                                        imageName = des.Image,
                                        animationName = des.ModPath,
                                    };
                                }
                            }
                            else if (lid[i] < 0)
                            {
                                int cur_id = p.currentWanted.battleEventID;
                                if (lid[i] != -1)
                                {
                                    DataBattleEvent dbe = DataBattleEventManager.Instance().GetDataByID(-lid[i]);
                                    title = tr(dbe.Name);
                                    if (cur_id == dbe.ID) title += "(√)";
                                    colorform = RareColor.GetColorForm(ColorUtility.ToHtmlStringRGB(RareColor.GetColorByRare(1)), title);
                                    tooltip = $"环境:{dbe.ID}";
                                    content = new GUIContent(colorform, tooltip);
                                    if (GUILayout.Button(content, GUILayout.Width(width / hCount - 10)))
                                    {
                                        WantedManager.Instance().wantedProcess.currentWanted.battleEventID = dbe.ID;
                                    }
                                    if (GUI.tooltip == tooltip)
                                    {
                                        new_tooltip = tooltip;
                                        items[0] = new ItemAny(dbe.ID, -6, 1, dbe.Name, CompressString(dbe.Des), tr("Loadout/Label/BattleEvent"), dbe.InGame ? tr("Loadout/Label/Special") : tr("Loadout/Label/NotAvail"));
                                        if (old_tooltip != tooltip) devInfo = new DevInfo
                                        {
                                            itemID = dbe.modID == null ? dbe.ID.ToString() : $"{dbe.modID}.{dbe.nameInMod}",
                                            skillName = dbe.SkillPath,
                                            imageName = dbe.Sprite,
                                        };
                                    }
                                }
                                else
                                {
                                    title = tr("Loadout/Label/NoEvent");
                                    if (cur_id == -1) title += "(√)";
                                    if (GUILayout.Button(title, GUILayout.Width(width / hCount - 10)))
                                        WantedManager.Instance().wantedProcess.currentWanted.battleEventID = -1;
                                }
                            }
                            break;
                        case 5:
                            UnitData d2 = UnitData.GetPlayerUnits()[lid[i]];
                            title = tr(d2.GetData().Name);
                            colorform = RareColor.GetColorForm(ColorUtility.ToHtmlStringRGB(RareColor.GetColorByRare(d2.GetData().Rare)), title);
                            tooltip = $"改造:{i}-{d2.GetData().ID}";
                            content = new GUIContent(colorform, tooltip);
                            if (GUILayout.Button(content, GUILayout.Width(width / hCount - 10)))
                            {
                                selected = d2;
                            }
                            if (GUI.tooltip == tooltip)
                            {
                                new_tooltip = tooltip;
                                items[0] = new ItemUnit(d2);
                                DataShipUnit dtmp = d2.GetData();
                                if (old_tooltip != tooltip) devInfo = new DevInfo
                                {
                                    itemID = dtmp.modID == null ? dtmp.ID.ToString() : $"{dtmp.modID}.{dtmp.nameInMod}",
                                    skillName = dtmp.SkillPath,
                                    imageName = dtmp.SpritePath,
                                    animationName = dtmp.ModPath,
                                };
                            }
                            break;
                        default:
                            break;
                    }
                    i++;
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
        }
        private Vector2 scrollVector = new Vector2(0, 0);
        private Vector2 scrollVector2 = new Vector2(0, 0);
        private Vector2 scrollVector3 = new Vector2(0, 0);
        public bool active = false;
        public Rect rect;
        public int width;
        public int height;
    }
}
