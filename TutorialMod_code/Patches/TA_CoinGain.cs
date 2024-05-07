using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutorialMod
{
    public class TA_CoinGain: TalentSkill, IBattleEnd
    {
        public void OnBattleEnd()
        {
            WantedManager.Instance().wantedProcess.AddCoin(args[0]);
            Blink();
        }
    }
}
