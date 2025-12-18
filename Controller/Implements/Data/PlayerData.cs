using AttributeSystem.Attributes;
using AttributeSystem.Effect;
using SF.UI.Bar;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : Target
{
    public bool isLocalPlayer => FightController.localPlayerId == Owner;
    public override bool UpdateLocally => isLocalPlayer;

    public BarController bar;

    public override void Init(TargetInfo info, Dictionary<string, string> param)
    {
        base.Init(info, param);
        if (isLocalPlayer)
        {
            var att = Tool.AttributesManager.GetDynamicAttribute(this);
            BaseAttributes = att.GetDynamicAttributes(Level).Clone();
            FloatingAttributes = BaseAttributes.Clone();
            float reg=0.01f;
            if (param.ContainsKey("PReg")) reg = float.Parse(param["PReg"]);
            ApplyEffect(new _HealthRegeneration(RegenerationAdderId, this, (int)(reg * BaseAttributes.Shengming.Value), 100000));
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
            bar = PlayModeController.Instance.CreateBar();
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
    }
    protected override void RegistOnDestroy()
    {
        base.RegistOnDestroy();
        if (bar != null)
        {
            PlayModeController.Instance.DestroyBar(bar);
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
        PlayModeController.Instance.ShowKilledSignal();
        base.OnKilled(killer);
    }
}
