using AttributeSystem.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Variety.Base;

public class Lantern : Target
{
    public static SortedDictionary<int, Lantern> Lanterns = new SortedDictionary<int, Lantern>();

    public SpriteRenderer Render;
    public Sprite On;
    public Sprite Off;

    public bool Alive => TimeOfDie < 0.001f;
    private float TimeOfDie;//生命为0的时间
    private float RegenerationTime;

    public override void Init(TargetInfo info)
    {
        base.Init(info);

        if (UpdateLocally)
        {
            var att = Tool.AttributesManager.GetDynamicAttribute(this) as LanternAttributes;
            BaseAttributes = att.GetDynamicAttributes(info.level);
            FloatingAttributes = BaseAttributes.Clone();
            RegenerationTime = att.RegenerationTime.GetValue(info.level);

            RegistSyncAttributes();
        }
        InitNameAndBar();
    }
    protected override void RegistOnCreated()
    {
        base.RegistOnCreated();
        Lanterns.Add(ObjectId, this);
    }
    protected override void RegistOnDestroy()
    {
        base.RegistOnDestroy();
        Lanterns.Remove(ObjectId);
    }
    protected override void FixedUpdate()
    {
        if (DedicatedAttributes.Shengming.Value.Item2 == 0)
        {
            Render.sprite = Off;
            TimeOfDie += Time.deltaTime;
        }
        else
        {
            Render.sprite = On;
            TimeOfDie = 0;
        }

        if (UpdateLocally && TimeOfDie > RegenerationTime)
        {
            Shengming = 999999999;
        }
    }




    private static HashSet<Bullet> Empty= new HashSet<Bullet>();
    protected override HashSet<Bullet> DetectBullet()
    {
        if(TimeOfDie<0.0001f)return base.DetectBullet();
        return Empty;
    }
    public static Lantern GetNearestLantern(Vector3 pos, float range = 99999f)
    {
        if (Lanterns.Count == 0) return null;
        float DMin = range * range;
        Lantern r = null;
        foreach (var i in Lanterns.Values)
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
    public static float GetAverageHealth()
    {
        int healthSum = 0;
        int each = 0;
        foreach(var i in Lanterns.Values)
        {
            if (each == 0) each = i.BaseAttributes.Shengming.Value;
            healthSum += i.FloatingAttributes.Shengming.Value;
        }
        return healthSum/(Lanterns.Count*(float)each);
    }
}
