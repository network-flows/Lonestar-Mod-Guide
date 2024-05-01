using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TutorialMod
{
    public class BE_TopRain : BattleEventSkill, IBattleStartPre, IRangeEffectUnit
    {
        public void OnBattleStartPre()
        {
            foreach (ShipCell cell in base.GetEmptyShipCells())
            {
                (this.shipController as PlayerShipController).ReplaceUnit(args[0], args[1], cell.coor, true);
                base.Blink();
            }
        }

        // When hovering, highlights the units of the same ID
        public void OnRangeEffectUnit(Action<ShipUnitController> action)
        {
            foreach (ShipUnitController shipUnitController in base.GetShipUnits())
            {
                if (shipUnitController.unitData.id == args[0])
                {
                    action(shipUnitController);
                }
            }
        }
    }
}
