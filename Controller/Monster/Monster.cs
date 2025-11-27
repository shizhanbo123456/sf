using AttributeSystem.Attributes;
using SF.UI.Bar;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Variety.Base;

public class Monster : Target
{
    public bool MonsterCanMove = true;
    public bool MonsterCanFly = false;

    public static SortedDictionary<int, Monster> Monsters = new SortedDictionary<int, Monster>();
    public static int MonsterIndexNext=0;
    private int MonsterIndex;

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


    public void Init(string data)
    {
        string[] s = data.Split('_', StringSplitOptions.RemoveEmptyEntries);
        transform.position = new Vector3(float.Parse(s[0]), float.Parse(s[1]), 0);
        MonsterIndex = int.Parse(s[2]);

        var att = Tool.AttributesManager.GetDynamicAttribute(this) as MonsterAttributes;
        StateInterval = att.StateInterval.GetValue(Tool.AttributesManager.GetLevel());
        BaseAttributes = att.GetDynamicAttributes(Tool.AttributesManager.GetLevel());
        BloodPerM = BaseAttributes.Shengming.Value / 100;
        BaseAttributes.Shengming.Value = BaseAttributes.Shengming.Value / BloodPerM * BloodPerM;
        FloatingAttributes = BaseAttributes.Clone();
        GetAndInitComponents();
        RegistSyncAttributesEvent();
        InitEssential();

        Bar = Tool.Instance.CreateBossBar();
        Bar.Enter(Type.ToString(), 100,Shengming);

        OnCreated();
    }
    protected override void RegistSyncAttributesEvent()
    {
        base.RegistSyncAttributesEvent();
        DedicatedAttributes.Shengming.OnValueChanged += v => 
        {
            Bar.SetBlood(v);
        };
    }
    protected override void OnCreated()
    {
        base.OnCreated();
        Monsters.Add(MonsterIndex, this);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (Bar != null)
        {
            Bar.SetBlood(0);
        }
        Monsters.Remove(MonsterIndex);
    }
    protected override TargetController AddController()=>gameObject.AddComponent<MonsterController>();
    protected override bool DamageByBullet(Bullet b)
    {
        if (!base.DamageByBullet(b)) return false;
        if(Shengming<=1)DestroyRpc();
        return true;
    }
}
