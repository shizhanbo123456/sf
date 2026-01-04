using AttributeSystem.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lantern : Target
{
    public SpriteRenderer Render;
    public Sprite On;
    public Sprite Off;

    public bool Alive => TimeOfDie < 0.001f;
    private float TimeOfDie;//生命为0的时间
    private float RegenerationTime;

    public override void Init(TargetInfo info, Dictionary<TargetParams, string> param)
    {
        base.Init(info, param);

        if (UpdateLocally)
        {
            float healthRate = param.ContainsKey(TargetParams.HealthScale) ? float.Parse(param[TargetParams.HealthScale]) : 1;
            BaseAttributes = TargetAttributes.GetGameTimeAttributes(info.level,healthRate);
            FloatingAttributes = BaseAttributes.Clone();
            if (param.ContainsKey(TargetParams.LanternRegenerationTime)) RegenerationTime = float.Parse(param[TargetParams.LanternRegenerationTime]);
            else RegenerationTime = 30;

            RegistSyncAttributes();
        }
        InitNameAndBar();
    }
    protected override void Update()
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
}
