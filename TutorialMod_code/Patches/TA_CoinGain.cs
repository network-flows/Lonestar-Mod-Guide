using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tool.Database;

namespace TutorialMod
{
    public class TA_CoinGain: TalentSkill, IBattleEnd
    {
        public override void Init(ShipData shipData, Talent talent, DataTalent dataTalent)
        {
            base.Init(shipData, talent, dataTalent);
            SignProcessText();
        }

        public void OnBattleEnd()
        {
            WantedManager.Instance().wantedProcess.AddCoin(args[0]);
            this.count.Value += args[0];
            Blink();
        }

        protected override void ProcessText()
        {
            if (slotResult != null)
            {
                slotResult.processText.text = this.count.Value.ToString();
            }
        }
    }
}
