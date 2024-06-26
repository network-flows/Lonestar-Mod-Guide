using System;
using System.Collections.Generic;
using System.Text;
using Lean.Localization;
using Tool.Database;

namespace Loadout
{
    public class ItemAny: ItemTreasure
    {
        public int ID;
        public int Type;
        public string Rare_text;
        public string Type_text;
        public string Des_text;
        public bool SkipTip;
        public ItemAny() { }
        public ItemAny(int id, int type, int rare, string name, string des, string type_text = null, string rare_text = null, bool skiptip = false)
        {
            if (rare > 2 || rare < 0) rare = 0;
            base.dataTreasure = new DataTreasure()
            {
                ID = id,
                Rare = rare,
                Name = name,
                Description = "Loadout/Text/Empty",
                Args = new int[] { }
            };
            Type = type;
            Des_text = des;
            Rare_text = rare_text;
            if(Rare_text == null)
            {
                if (rare == 1) Rare_text = LeanLocalization.GetTranslationText("Tips/Blue");
                else if (rare == 2) Rare_text = LeanLocalization.GetTranslationText("Tips/Purple");
                else Rare_text = LeanLocalization.GetTranslationText("Tips/White");
            }
            Type_text = type_text;
            if(Type_text == null) Type_text = LeanLocalization.GetTranslationText("Tips/Treasure");
            SkipTip = skiptip;
        }
    }
}
