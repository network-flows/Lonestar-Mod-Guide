using HarmonyLib;
using Mods;

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
        }
    }
}
