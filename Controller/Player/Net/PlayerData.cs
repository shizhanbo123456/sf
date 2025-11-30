using AttributeSystem.Attributes;
using AttributeSystem.Effect;
using EC;
using SF.UI.Bar;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : Target
{
    [HideInInspector]public int id;
    private bool Initialized;
    [HideInInspector]public bool isLocalPlayer;

    [HideInInspector] public string Name;
    [HideInInspector] public int Vocation;

    [HideInInspector]public BarBase bar;

    public override bool UpdateLocally
    {
        get
        {
            return isLocalPlayer;
        }
    }

    [SerializeField] private TargetName targetName;
    

    public void Init(string d)//由Spawner在生成时传入信息
    {
        ServerDataContainer.PlayerDataContainer playerData= new ServerDataContainer.PlayerDataContainer(d);
        id = playerData.id;
        Name = playerData.name;
        Vocation=playerData.vocation;
        Camp = playerData.camp;
        isLocalPlayer=FightController.localPlayerId == id;

        var att = Tool.AttributesManager.GetDynamicAttribute(this) as PlayerAttributes;
        int level = Tool.AttributesManager.GetLevel();
        BaseAttributes = att.GetDynamicAttributes(level).Clone();
        FloatingAttributes = BaseAttributes.Clone();

        GetAndInitComponents();
        if (isLocalPlayer) CameraInstance.instance.Init(transform);
        if (isLocalPlayer) Init_Bars();
        RegistSyncAttributesEvent();
        InitEssential();
        Init_Name();

        effectController.AddEffect(new HealthRegeneration(RegenerationAdderId,this, (int)(att.Huixie*BaseAttributes.Shengming.Value), 100000));

        transform.position = Tool.SceneController.Level.GetSpawnPlace(this);
        if(isLocalPlayer)Tool.SceneController.Player = gameObject;

        Initialized =true;

        OnCreated();
    }
    private void Init_Name()
    { 
        targetName.text = $"P{id}-{Name}";
        targetName.color = Tool.SpriteManager.TargetToColor(this);
    }
    private void Init_Bars()
    {
        bar = Tool.Instance.CreateBar();
        bar.SetScale(1f);
        bar.SetColor(new Color(1f, 0.4f, 0.4f, 1f));

        BaseAttributes.Shengming.OnValueChanged += _ => UpdateBar();
        FloatingAttributes.Shengming.OnValueChanged += _ => UpdateBar();

        BaseAttributes.Shengming.OnValueChanged.Invoke(BaseAttributes.Shengming.Value);
        FloatingAttributes.Shengming.OnValueChanged.Invoke(FloatingAttributes.Shengming.Value);
    }
    private void UpdateBar()
    {
        bar.SetValue(FloatingAttributes.Shengming.Value, BaseAttributes.Shengming.Value);
    }

    public override void ManagedUpdate()
    {
        if (!Initialized) return;

        base.ManagedUpdate();
    }
    protected override bool DamageByBullet(Bullet b)
    {
        if (!base.DamageByBullet(b)) return false;

        CameraInstance.instance.ShakeCamera();
        if (Shengming == 0)
        {
            Die(b.Shooter);
        }
        return true;
    }
    public void Die(Target t)//死亡时调用
    {
        Tool.FightController.OnDeathRpc(Camp, ((t!=null&&t is PlayerData)?t.Camp:9));
        Shengming = BaseAttributes.Shengming.Value;
        transform.position = Tool.SceneController.Level.GetSpawnPlace(this);
        Tool.UIEventCenter.TrigEvent(new ShowKilledSignalEvent());
    }

    protected override TargetController AddController() => gameObject.AddComponent<PlayerController>();
    protected override TargetSkillController AddSkillController() => gameObject.AddComponent<PlayerSkillController>();
}
