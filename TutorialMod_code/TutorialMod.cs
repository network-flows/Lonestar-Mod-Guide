using HarmonyLib;
using Mods;

namespace TutorialMod
{
    public class TutorialMod : UserMod
    {
        public override void OnLoad(bool autopatch = true)
        {
            base.OnLoad(autopatch);
            UnityEngine.Debug.Log("Successfully Loaded [" + this.modID + "] at " + this.path);
        }
    }
}
