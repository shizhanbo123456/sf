using AttributeSystem.Attributes;
using AttributeSystem.Effect;
using EC;
using SF.UI.Bar;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : Target
{
    public static Dictionary<int, PlayerData> Players = new Dictionary<int, PlayerData>();
    public bool isLocalPlayer => FightController.localPlayerId == Owner;
    public override bool UpdateLocally => isLocalPlayer;

    [HideInInspector]public BarBase bar;

    public override void Init(TargetInfo info)
    {
        base.Init(info);
        if (isLocalPlayer)
        {
            var att = Tool.AttributesManager.GetDynamicAttribute(this) as PlayerAttributes;
            BaseAttributes = att.GetDynamicAttributes(Level).Clone();
            FloatingAttributes = BaseAttributes.Clone();
            ApplyEffect(new HealthRegeneration(RegenerationAdderId, this, (int)(att.Huixie * BaseAttributes.Shengming.Value), 100000));
            RegistSyncAttributes();

            CameraInstance.instance.Init(transform);
            Tool.SceneController.Player = gameObject;
        }
        InitNameAndBar();
    }
    protected override void InitNameAndBar()
    {
        base.InitNameAndBar();

        if (isLocalPlayer)
        {
            bar = Tool.PageManager.PlayModePage.CreateBar();
            bar.SetScale(1f);
            bar.SetColor(new Color(1f, 0.4f, 0.4f, 1f));

            BaseAttributes.Shengming.OnValueChanged += _ => UpdateBar();
            FloatingAttributes.Shengming.OnValueChanged += _ => UpdateBar();

            BaseAttributes.Shengming.OnValueChanged.Invoke(BaseAttributes.Shengming.Value);
            FloatingAttributes.Shengming.OnValueChanged.Invoke(FloatingAttributes.Shengming.Value);
        }
    }
    private void UpdateBar()
    {
        bar.SetValue(FloatingAttributes.Shengming.Value, BaseAttributes.Shengming.Value);
    }
    protected override void RegistOnCreated()
    {
        base.RegistOnCreated();
        Players.Add(ObjectId, this);
    }
    protected override void RegistOnDestroy()
    {
        base.RegistOnDestroy();
        Players.Remove(ObjectId);
        if (bar != null)
        {
            Tool.PageManager.PlayModePage.DestroyBar(bar);
        }
    }
    protected override bool DamageByBullet(Bullet b)
    {
        if (!base.DamageByBullet(b)) return false;

        CameraInstance.instance.ShakeCamera();
        return true;
    }
    public override void OnKilled(Target killer)
    {
        Tool.UIEventCenter.TrigEvent(new ShowKilledSignalEvent());
        base.OnKilled(killer);
    }
}
