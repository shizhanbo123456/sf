using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variety.Base;

public class Ore : Target
{
    public static SortedDictionary<int,Ore>Ores=new SortedDictionary<int,Ore>();
    public override void Init(TargetInfo info)
    {
        base.Init(info);

        if (UpdateLocally)
        {
            var att = Tool.AttributesManager.GetDynamicAttribute(this);
            BaseAttributes = att.GetDynamicAttributes(info.level);
            FloatingAttributes = BaseAttributes.Clone();
            RegistSyncAttributes();
        }
    }
    protected override void RegistOnCreated()
    {
        base.RegistOnCreated();
        Ores.Add(ObjectId, this);
    }
    protected override void RegistOnDestroy()
    {
        base.RegistOnDestroy();
        Ores.Remove(ObjectId);
    }
    protected override bool DamageByBullet(Bullet b)
    {
        if (!base.DamageByBullet(b)) return false;

        targetDataSync.DestroyRpc();
        return true;
    }


    public static Ore GetNearestOre(Vector3 pos, float range = 99999f)
    {
        if(Ores.Count== 0) return null;
        float DMin = range * range;
        Ore r = null;
        foreach (var i in Ores.Values)
        {
            var mSqr = (pos - i.transform.position).sqrMagnitude;
            if (mSqr < DMin)
            {
                r = i;
                DMin = mSqr;
            }
        }
        return r;
    }
}
