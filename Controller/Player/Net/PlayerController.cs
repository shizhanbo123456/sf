using EC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Variety.Base;

public class PlayerController : TargetController
{
    protected override float MinResisiance => -0.5f;
    public override Vector2 GetInputVector()
    {
        return new Vector2(Tool.SubInput.HorizontalInput(), (Tool.SubInput.JumpSignal() ? 1 : 0) + (Tool.SubInput.FallSignal() ? -1 : 0));
    }
    public override bool OnHitBack(Bullet b)
    {
        var bo=base.OnHitBack(b);
        if(bo)Tool.UIEventCenter.TrigEvent(new DoFlickEvent(0.5f));
        return bo;
    }
}
