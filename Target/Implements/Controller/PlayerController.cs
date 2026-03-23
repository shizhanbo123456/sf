using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Variety.Base;

namespace LevelCreator.TargetTemplate
{
    public class PlayerController : TargetController
    {
        protected override float MinResisiance => -0.5f;
        public override Vector2Int GetInputVector()
        {
            return new Vector2Int(Tool.SubInput.HorizontalInput(), (Tool.SubInput.JumpSignal() ? 1 : 0) + (Tool.SubInput.FallSignal() ? -1 : 0));
        }
        public override bool OnHitBack(Bullet b)
        {
            var bo = base.OnHitBack(b);
            if (bo) PlayModeController.Instance.DoFlick(0.3f, Color.white);
            return bo;
        }
    }
}