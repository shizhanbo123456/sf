using AttributeSystem.Attributes;
using SF.UI.Bar;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Variety.Base;

public class Monster : Target
{
    private const int LayerMax = 100;
    public static SortedDictionary<int, Monster> Monsters = new SortedDictionary<int, Monster>();

    public enum MonsterType
    {
        焚世炎羊,
        业火炎灵,刺魂霜灵,
        星核仲裁者,深渊畸变者, 
        圣辉制裁者, 终焉收割者, 
        秘影法皇, 魔导先知, 圣焰先锋,
        绯影武姬,血刃妖镰,烬灭炮手,
        狂鬃,黑角,永夜,金冠,战宗
    }
    public MonsterType Type;//在预制体中设置

    private BossBar Bar;

    private int BloodPerM;
    [HideInInspector]public float StateInterval=5;


    public override void Init(CustomTargetCreater.TargetInfo info)
    {
        base.Init(info);

        if (UpdateLocally)
        {
            var att = Tool.AttributesManager.GetDynamicAttribute(this) as MonsterAttributes;
            StateInterval = att.StateInterval.GetValue(info.level);
            BaseAttributes = att.GetDynamicAttributes(info.level);
            BloodPerM = BaseAttributes.Shengming.Value / LayerMax;
            BaseAttributes.Shengming.Value = LayerMax * BloodPerM;
            FloatingAttributes = BaseAttributes.Clone();

            RegistSyncAttributes();
        }
    }
    protected override void InitNameAndBar()
    {
        graphic.SetName(string.Empty);
        Bar = Tool.Instance.CreateBossBar();
        Bar.Init(Name, LayerMax, Shengming);
    }
    protected override void RegistSyncAttributes()
    {
        base.RegistSyncAttributes();
        DedicatedAttributes.Shengming.OnValueChanged += v => Bar.SetValue(v.Item2, v.Item1, LayerMax);
    }
    protected override void RegistOnCreated()
    {
        base.RegistOnCreated();
        Monsters.Add(ObjectId, this);
    }
    protected override void RegistOnDestroy()
    {
        base.RegistOnDestroy();
        if (Bar != null)
        {
            Bar.SetValue(0,BaseAttributes.Shengming.Value,LayerMax);
        }
        Monsters.Remove(ObjectId);
    }
    protected override bool DamageByBullet(Bullet b)
    {
        if (!base.DamageByBullet(b)) return false;
        if(Shengming<=1)targetDataSync.DestroyRpc();
        return true;
    }
}
