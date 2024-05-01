using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutorialMod
{
    public class TS_Vulcan : TreasureSkill, IPlayerTurnStart, IBattleEnd, IBattleStart
    {
        public void OnBattleEnd()
        {
            // Hides the counter out of battle
            this.count.Value = 0;
        }

        public void OnBattleStart()
        {
            this.count.Value = args[1];
        }

        public void OnPlayerTurnStart()
        {
            for (int i = 0; i < this.count.Value; i++)
                new DamageMaker(base.OppositeController(), this.shipController, args[0], null).Damage(true);
            this.count.Value += args[1];
            base.Blink();
        }
    }
}
