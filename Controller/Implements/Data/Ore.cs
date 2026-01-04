using AttributeSystem.Attributes;
using System.Collections;
using System.Collections.Generic;
using Variety.Base;

public class Ore : Target
{
    public override void Init(TargetInfo info, Dictionary<TargetParams, string> param)
    {
        base.Init(info, param);

        if (UpdateLocally)
        {
            float healthRate = param.ContainsKey(TargetParams.HealthScale) ? float.Parse(param[TargetParams.HealthScale]) : 1;
            BaseAttributes = TargetAttributes.GetGameTimeAttributes(info.level, healthRate);
            FloatingAttributes = BaseAttributes.Clone();
            RegistSyncAttributes();
        }
        InitNameAndBar();
    }
    protected override bool DamageByBullet(Bullet b)
    {
        if (!base.DamageByBullet(b)) return false;

        targetDataSync.DestroyRpc();
        return true;
    }
}
