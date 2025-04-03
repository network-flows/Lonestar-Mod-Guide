using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TutorialMod
{
    public class OC_AddPoint : OptionContent
    {
        const string point_key = "TutorialMod/SH_Tiny/Args/2";
        public override bool Check()
        {
            if (!Define.defineValue.eventValuesGlobal.ContainsKey(point_key)) return false;
            if (Define.defineValue.eventValuesGlobal[point_key] == 9) return false;
            return true;
        }
        public override void Do()
        {
            Define.defineValue.eventValuesGlobal[point_key] += 2;
        }
    }
}