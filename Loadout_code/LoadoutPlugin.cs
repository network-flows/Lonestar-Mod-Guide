using Mods;

namespace Loadout
{
    public class LoadoutPlugin : UserMod
    {
        public class LoadoutConfig
        {
            [BoolField("@/Config/Dev", "@/Config/DevTooltip")]
            public bool is_dev;
        }

        public static LoadoutConfig config;

        public override void OnLoad()
        {
            base.OnLoad();
            GUILoadout.Instance();
            GameMasterManager.Instance().enabled = false;
        }
    }
}
