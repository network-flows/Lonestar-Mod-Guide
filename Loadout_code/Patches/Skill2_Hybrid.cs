using System;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using Tool.Database;
using System.Linq;
using UnityEngine.Events;
using System.Reflection;

namespace loadout
{
    public class Skill2_Hybrid : ShipUnitSkill, ITriangle, ICharge, ILoadFull, IOverclock, ICountDown, IUse, IRangeEffectSelfUnit, IRangeEffectUnit, IRangeEffectCell, IRangeEffectErrorCell, IRangeEffectErrorUnit, IRangeEffectSelfCell
    {
        public static string SkillPath = "Skill2_Hybrid";
        public bool is_loadfull = false;
        public static Dictionary<string, bool> available_skills = new Dictionary<string, bool>();
        public static bool CanAdd(string skillPath)
        {
            if(available_skills.ContainsKey(skillPath)) return available_skills[skillPath];
            available_skills[skillPath] = true;
            Type type = AccessTools.TypeByName(FilePath.GetNameWithPath(skillPath));
            ShipUnitSkill skill = (Activator.CreateInstance(type) as ShipUnitSkill);
            if (skill is ITwoStepUse) available_skills[skillPath] = false;
            if (skill is Skill_Imitation) available_skills[skillPath] = false;
            if (skill is Skill_WIFI) available_skills[skillPath] = false;
            if (skill is Skill_DestroyCopyTurnPower) available_skills[skillPath] = false;
            if (skill is Skill_OverclockThreeCannon) available_skills[skillPath] = false;
            return available_skills[skillPath];
        }

        public override void InitNormalSkill()
        {
            base.InitNormalSkill();
            skills.Clear();
            is_loadfull = false;
            foreach (int i in args)
            {
                DataShipUnit data = UnitData.GetDataShipUnit(i, shipUnitController.unitData.lv);
                for (int offset = 0; shipUnitController.unitData.lv - offset >= 1 && data == null; offset++)
                    data = UnitData.GetDataShipUnit(i, shipUnitController.unitData.lv - offset);
                Type type = AccessTools.TypeByName(FilePath.GetNameWithPath(data.SkillPath));
                ShipUnitSkill skill = (Activator.CreateInstance(type) as ShipUnitSkill);
                if (skill is ILoadFull) is_loadfull = true;
                skills.Add(skill);
                skill.Init(shipUnitController, data, shipData);
                skill.InitNormalSkill();
                if (skills.Count > 1) skill.spriteChangeable = false;
                else skill.spriteChangeable = spriteChangeable;
                skill.args = data.Args.ToList();
            }
        }

        public override void OnInitController(ShipUnitController shipUnitController)
        {
            if (!spriteChangeable) InitNormalSkill();
            this.shipUnitController = shipUnitController;
            shipController = (shipUnitController.shipController as ShipController);
            InitBattleSkill();
            foreach (ShipUnitSkill skill in skills)
            {
                skill.OnInitController(shipUnitController);
                if(!spriteChangeable) // Spawned from a Skill_Imitation
                {
                    if (skill is IBattleStartPreBefore)
                    {
                        (skill as IBattleStartPreBefore).OnBattleStartPreBefore();
                    }
                    if (skill is IBattleStartPre)
                    {
                        (skill as IBattleStartPre).OnBattleStartPre();
                    }
                }
            }
            Activate_Buff<Buff_Use>();
            Activate_Buff<Buff_Overclock>();
            Activate_Buff<Buff_Triangle>();
            Activate_Buff<Buff_Charge>();
            if (!is_loadfull)
            {
                EventCenter.Instance().RemoveEventListener<PowerData>(EventName.powerLoadedAfterOver, new UnityAction<PowerData>(TriggerLoadFull));
                EventCenter.Instance().RemoveEventListener<PowerData>(EventName.powerLoadedAfterOverPreview, new UnityAction<PowerData>(TriggerLoadFullPreview));
            }
        }
        public void Activate_Buff<T>() where T : Buff, new()
        {
            Buff buff = shipUnitController.GetBuff<T>();
            if (buff != null) return;
            shipUnitController.AddBuff<T>();
            buff = shipUnitController.GetBuff<T>();
            Traverse.Create(buff).Method("RemoveBattleSkill").GetValue();
            buff.keyWordString = null;
            if(buff is Buff_Use) (buff as Buff_Use).useable = false;
            if (buff is Buff_Overclock) (buff as Buff_Overclock).count.Value = -1;
            buff.inited = false;
            buff.count.callback = null;
            buff.count.setLimitCallback = null;
            buff.countPreview.callback = null;
            buff.countPreview.setLimitCallback = null;
            buff.once.setLimitCallback = null;
            buff.oncePreview.setLimitCallback = null;
            buff.exhibitController.Hide();
        }

        public override void RemoveListener()
        {
            foreach (var skill in skills) skill.RemoveListener();
            base.RemoveListener();
        }
        public static UnitData Merge_Units(UnitData d1, UnitData d2)
        {
            UnitData d3 = UnitData.Clone(d1);
            if (d3.args == null) d3.args = new List<int> { };
            if (d3.args.Count <= 1 || d3.args[0] != -1)
            {
                d3.args.Clear();
                d3.args.Add(-1);
                d3.args.Add(d3.id);
            }
            if (d2.args != null && d2.args.Count > 1 && d2.args[0] == -1)
            {
                for (int i = 1; i < d2.args.Count; i++)
                    d3.args.Add(d2.args[i]);
            }
            else d3.args.Add(d2.id);
            d3.SetData(null);
            return d3;
        }
        public static UnitData Remove_Merge(UnitData d1, int i)
        {
            UnitData d3 = UnitData.Clone(d1);
            if (d3.args.Count <= 1 || d3.args[0] != -1) return d3;
            d3.args.RemoveAt(i + 1);
            d3.SetData(null);
            return d3;
        }
        public List<ShipUnitSkill> skills = new List<ShipUnitSkill>();
        public void OnCountDown()
        {
            foreach (ShipUnitSkill skill in skills)
            {
                if (skill is ICountDown)
                    (skill as ICountDown).OnCountDown();
            }
        }
        public void OnCharge()
        {
            foreach (ShipUnitSkill skill in skills)
            {
                if (skill is ICharge)
                    (skill as ICharge).OnCharge();
            }
        }
        public void OnChargePreview()
        {
            foreach (ShipUnitSkill skill in skills)
            {
                if (skill is ICharge)
                    (skill as ICharge).OnChargePreview();
            }
        }
        public void OnLoadFull()
        {
            //foreach (ShipUnitSkill skill in skills)
            //{
            //    if (skill is ILoadFull)
            //        (skill as ILoadFull).OnLoadFull();
            //}
        }
        public void OnLoadFullPreview()
        {
            //foreach (ShipUnitSkill skill in skills)
            //{
            //    if (skill is ILoadFull)
            //        (skill as ILoadFull).OnLoadFullPreview();
            //}
        }
        public void OnUse()
        {
            foreach (ShipUnitSkill skill in skills)
            {
                if (skill is IUse)
                    (skill as IUse).OnUse();
            }
        }
        public void OnUsePreview()
        {
            foreach (ShipUnitSkill skill in skills)
            {
                if (skill is IUse)
                    (skill as IUse).OnUsePreview();
            }
        }
        public void OnTriangle()
        {
            foreach (ShipUnitSkill skill in skills)
            {
                if (skill is ITriangle)
                    (skill as ITriangle).OnTriangle();
            }
        }
        public void OnTrianglePreview()
        {
            foreach (ShipUnitSkill skill in skills)
            {
                if (skill is ITriangle)
                    (skill as ITriangle).OnTrianglePreview();
            }
        }
        public void OnOverclock(PowerData d)
        {
            foreach (ShipUnitSkill skill in skills)
            {
                if (skill is IOverclock)
                    (skill as IOverclock).OnOverclock(d);
            }
        }
        public void OnOverclockPreview(PowerData d)
        {
            foreach (ShipUnitSkill skill in skills)
            {
                if (skill is IOverclock)
                    (skill as IOverclock).OnOverclockPreview(d);
            }
        }
        public bool OnTwoStepUse()
        {
            bool result = false;
            foreach(ShipUnitSkill skill in skills)
            {
                if(skill is ITwoStepUse)
                    result = result || (skill as ITwoStepUse).OnTwoStepUse();
            }
            return result;
        }
        public void OnUsePreview(ShipUnitController target)
        {
            foreach (ShipUnitSkill skill in skills)
            {
                if (skill is ITwoStepUse)
                    (skill as ITwoStepUse).OnUsePreview();
            }
        }
        public void OnRangeEffectSelfUnit(Action<ShipUnitController> action)
        {
            foreach(ShipUnitSkill skill in skills)
            {
                if (skill is IRangeEffectSelfUnit) 
                    (skill as IRangeEffectSelfUnit).OnRangeEffectSelfUnit(action);
            }
        }
        public void OnRangeEffectSelfCell(Action<ShipCell> action)
        {
            foreach (ShipUnitSkill skill in skills)
            {
                if (skill is IRangeEffectSelfCell)
                    (skill as IRangeEffectSelfCell).OnRangeEffectSelfCell(action);
            }
        }
        public void OnRangeEffectUnit(Action<ShipUnitController> action)
        {
            foreach (ShipUnitSkill skill in skills)
            {
                if (skill is IRangeEffectUnit)
                    (skill as IRangeEffectUnit).OnRangeEffectUnit(action);
            }
        }
        public void OnRangeEffectCell(Action<ShipCell> action)
        {
            foreach (ShipUnitSkill skill in skills)
            {
                if (skill is IRangeEffectCell)
                    (skill as IRangeEffectCell).OnRangeEffectCell(action);
            }
        }
        public void OnRangeEffectErrorCell(Action<ShipCell> action)
        {
            foreach (ShipUnitSkill skill in skills)
            {
                if (skill is IRangeEffectErrorCell)
                    (skill as IRangeEffectErrorCell).OnRangeEffectErrorCell(action);
            }
        }
        public void OnRangeEffectErrorUnit(Action<ShipUnitController> action)
        {
            foreach (ShipUnitSkill skill in skills)
            {
                if (skill is IRangeEffectErrorUnit)
                    (skill as IRangeEffectErrorUnit).OnRangeEffectErrorUnit(action);
            }
        }

        public void OnUpdater()
        {
            Buff_Use buf1 = shipUnitController.GetBuff<Buff_Use>();
            if (buf1 == null)
            {
                shipUnitController.AddBuff<Buff_Use>();
                buf1 = shipUnitController.GetBuff<Buff_Use>();
                buf1.once.Value = false;
                buf1.useable = false;
                buf1.num.Value = 0;
            }
        }
    }
}