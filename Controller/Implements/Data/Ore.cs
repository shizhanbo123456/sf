using System.Collections;
using System.Collections.Generic;
using Variety.Base;

public class Ore : Target
{
    public override void Init(TargetInfo info, Dictionary<string, string> param)
    {
        base.Init(info, param);

        if (UpdateLocally)
        {
            var att = Tool.AttributesManager.GetDynamicAttribute(this);
            BaseAttributes = att.GetDynamicAttributes(info.level);
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
