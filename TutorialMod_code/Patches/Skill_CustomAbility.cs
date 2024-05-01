using Mods;
using System;

namespace TutorialMod
{
    public class Skill_CustomAbility : ShipUnitSkill, IBattleStartPre, IRangeEffectCell
    {
        public void OnBattleStartPre()
        {
            this.shipController.RemoveUnit(this.shipUnitController);
            if (this.shipUnitController.unitData.lv == 1)
            {
                foreach (ShipCell shipCell in base.GetEmptyShipCells())
                {
                    // Equips Unit (ID = 14002, Lv = 1) to each cell.
                    (this.shipController as PlayerShipController).ReplaceUnit(14002, 1, shipCell.coor, true);
                }
            }
            else
            {
                foreach (ShipCell shipCell in base.GetShipCellsByLine(this.shipUnitController.coor.y))
                {
                    if (shipCell.shipCellStatus == ShipCellStatus.Empty || shipCell == this.shipUnitController.shipCell)
                    {
                        // IDs of Modded Units are dynamically assigned during runtime, call this to get the ID of another unit in the same mod.
                        int moddedUnitID = ModUtils.AutoAssign(this.shipUnitController.unitData.modID, "unitB");

                        // Equips a modded Unit (ModID = same modID, nameInMod = "UnitB", Lv = 2) to each cell.
                        (this.shipController as PlayerShipController).ReplaceUnit(moddedUnitID, 2, shipCell.coor, true);
                    }
                }
            }

            // This shows texts when unit ability is used. (Set the field `SkillName` in Content/ShipUnit.csv)
            base.SkillTextFlow();
        }

        // This function highlights corresponding cells when mouse hovers on the unit
        public void OnRangeEffectCell(Action<ShipCell> action)
        {
            if (this.shipUnitController.unitData.lv == 1)
            {
                // Lv1: Highlights adjacent empty grids
                base.GetAroundCellsAction(delegate (ShipCell cell)
                {
                    if (cell.shipUnitController == null)
                    {
                        action(cell);
                    }
                });
            }
            else
            {
                // Lv2: Highlights empty grids in the same lane
                foreach (ShipCell shipCell in base.GetShipCellsByLine(this.shipUnitController.coor.y))
                {
                    if (shipCell.shipCellStatus == ShipCellStatus.Empty || shipCell == this.shipUnitController.shipCell)
                    {
                        action(shipCell);
                    }
                }
            }
        }
    }
}
