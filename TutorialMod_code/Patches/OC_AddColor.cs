using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TutorialMod
{
    public class OC_AddColor : OptionContent
    {
        const string color_key = "TutorialMod/SH_Tiny/Args/1";
        public override bool Check()
        {
            if (!Define.defineValue.eventValuesGlobal.ContainsKey(color_key)) return false;
            if (Define.defineValue.eventValuesGlobal[color_key] == 3) return false;
            return true;
        }
        public override void Do()
        {
            Define.defineValue.eventValuesGlobal[color_key] += 1;
        }
    }
}
