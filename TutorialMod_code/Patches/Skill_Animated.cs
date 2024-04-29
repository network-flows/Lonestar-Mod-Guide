using SpriterDotNetUnity;
using System;
using UnityEngine;

namespace TutorialMod.Patches
{
    public class Skill_Animated : ShipUnitSkill, IPowerLoaded, IPlayerTurnStart
    {
        private SpriterDotNetBehaviour script;
        public override void OnInitController(ShipUnitController shipUnitController)
        {
            base.OnInitController(shipUnitController);
            // Gets the reference of the animator
            script = shipUnitController.GetComponentInChildren<SpriterDotNetBehaviour>();
        }

        public void OnPowerLoaded(PowerData powerData)
        {
            if (powerData.shipUnitController != shipUnitController) return;
            if (script == null || script.Animator == null) return;
            if (shipUnitController.unitData.lv == 1)
            {
                // Plays "anim1" loop
                script.Animator.Play("anim1");
            }
            else if (shipUnitController.unitData.lv == 2)
            {
                // Plays "anim2" once
                script.Animator.Play("anim2");
                Action<string> callback = null;
                callback = (s) =>
                {
                    // On finish, reset to default
                    script.Animator.Play("default");
                    script.Animator.AnimationFinished -= callback;
                };
                script.Animator.AnimationFinished += callback;
            }
        }

        public void OnPowerLoadedPreview(PowerData powerData)
        {

        }

        public void OnPlayerTurnStart()
        {
            if (script == null || script.Animator == null) return;
            script.Animator.Play("default");
        }
    }
}
