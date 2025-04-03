using HarmonyLib;
using Mods;
using System.Linq;
using Tool.Database;

namespace TutorialMod
{
    public class TutorialMod : UserMod
    {
        public class TutorialModConfig
        {
            [StringField("String field", "String field tooltip")]
            public string stringField = "default value";

            [IntField(0, 10, "Int field 1", "Int field tooltip 1")]
            public int intField1;
            
            [IntField(0, 20, "Int field 2", "Int field tooltip 2")]
            public int intField2;

            [Label("@/Settings/Label", "@/Settings/LabelTooltips")]
            public string labelField = "This is no use, only the annotation works.";
            
            [BoolField("Bool field", "Bool tooltip")]
            public bool boolField;
            
            [FloatField(0.0f, 1.0f, "Float field", "Float tooltip")]
            public float floatField;
            
            [ChoiceField(new string[] {"A", "B", "C", "D"}, "Choice field", "Choice Tooltip")]
            public string choice;
        }
        public static TutorialModConfig config = new TutorialModConfig();
        public override void OnLoad()
        {
            base.OnLoad();
            UnityEngine.Debug.Log("Successfully Loaded [" + this.modID + "] at " + this.path);
            MigrateItems();
        }

        // Migrate vanilla events, treasures, pilots, etc to the ship "Tiny", give it shielder unit pool
        // You can write your filters
        // If you have an own set of events or treasures, it's alright not to migrate
        public void MigrateItems()
        {
            // Get the integer ID of the ship
            int shipID = AutoAssign("Tiny");
            foreach (var ev in DataEncounterEventsManager.Instance().GetDataList())
            {
                if (!ev.Tags.Contains((int)EncounterEventTag.Shield) &&
                    !ev.Tags.Contains((int)EncounterEventTag.Exchange) &&
                    !ev.Tags.Contains((int)EncounterEventTag.ColorSlot))
                {
                    ev.Pros = ev.Pros.Append(shipID).ToArray();
                }
            }

            foreach (var ts in DataTreasureManager.Instance().GetDataList())
            {
                // Filter shield(9), swap(12) and paint(13) events, also ignore energy resource mix-up event(2600)
                if (!ts.Genera.Contains(9) && !ts.Genera.Contains(12) && !ts.Genera.Contains(13) && ts.ID != 2600)
                {
                    ts.Pros = ts.Pros.Append(shipID).ToArray();
                }
            }

            foreach (var p in DataPilotManager.Instance().GetDataList())
            {
                if (p.Pros.Contains(7) && p.Pros.Contains(77) && p.Pros.Contains(88)) // Universal pilots
                {
                    p.Pros = p.Pros.Append(shipID).ToArray();
                }
            }

            foreach (var ta in DataTalentManager.Instance().GetDataList())
            {
                if (ta.Pros.Contains(7) && ta.Pros.Contains(77) && ta.Pros.Contains(88)) // Universal talents
                {
                    ta.Pros = ta.Pros.Append(shipID).ToArray();
                }
            }

            foreach (var u in DataShipUnitManager.Instance().GetDataList())
            {
                if (u.Pros.Contains(7)) // shielder unit pool
                {
                    u.Pros = u.Pros.Append(shipID).ToArray();
                }
            }
        }
    }
}
