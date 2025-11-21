using AttributeSystem.Attributes;
using AttributeSystem.Effect;
using EC;
using SF.UI.Bar;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerData : Target
{
    public SyncPosition SyncPosition;

    public int id;
    private bool Initialized;
    [HideInInspector]public bool isLocalPlayer;

    [HideInInspector] public string Name;
    [HideInInspector] public int Vocation;

    [HideInInspector]public Bar_Main bar;
    [HideInInspector]public Bar_Float magic;
    

    private RegistableVariable<int> mofa;
    private RegistableVariable<int> maxMofa;
    public int Mofa
    {
        get { return mofa.Value; }
        set
        {
            mofa.Value = Mathf.Clamp(value, 0, maxMofa.Value);
        }
    }


    public override bool UpdateLocally
    {
        get
        {
            return isLocalPlayer;
        }
    }

    [Space]
    [Header("Name")]
    public Text Recog;
    public Font Font;
    public Image Back;
    

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
        maxMofa=new RegistableVariable<int>((int)att.Mofa.GetValue(level));
        mofa = new RegistableVariable<int>(maxMofa.Value);
        BaseAttributes = att.GetDynamicAttributes(level).Clone();
        if (Tool.AttributesManager.AsBoss(this)) BaseAttributes.Shengming.Value *= 80;
        FloatingAttributes = BaseAttributes.Clone();

        GetAndInitComponents();
        if (isLocalPlayer) CameraInstance.instance.Init(transform);
        if (isLocalPlayer) Init_Bars();
        RegistSyncAttributesEvent();
        InitEssential();
        Init_Name();

        effectController.AddEffect(new HealthRegeneration(this,this, (int)att.Huixie.GetValue(level), 100000));
        effectController.AddEffect(new MagicRegeneration(this,this, (int)att.Huimo.GetValue(level), 100000));

        transform.position = Tool.SceneController.Level.GetSpawnPlace(this);
        if(isLocalPlayer)Tool.SceneController.Player = gameObject;

        Initialized =true;

        OnCreated();
    }
    private void Init_Name()
    {
        Recog.text = "P" + id + " " + Name;
        Recog.color = Tool.SpriteManager.TargetToColor(this);
        GUIStyle style = new GUIStyle()
        {
            font = Font,
            fontSize = Recog.fontSize,
            fontStyle = Recog.fontStyle
        };
        Texture2D tex = new Texture2D(1024, 16);
        Vector2 size = style.CalcSize(new GUIContent(Recog.text));
        Destroy(tex);
        Back.transform.localScale = new Vector3((size.x + 30) / 100, 0.01f, 1);
    }
    private void Init_Bars()
    {
        bar = Tool.Instance.CreateBar(PlayModePage.BarType.Main) as Bar_Main;
        magic = Tool.Instance.CreateBar(PlayModePage.BarType.Float) as Bar_Float;

        BaseAttributes.Shengming.OnValueChanged += v =>
        {
            bar.Max = v;
            bar.SetValue(FloatingAttributes.Shengming.Value);
            bar.SubMax = v;
            bar.SetValue(FloatingAttributes.Hudun.Value);
        };
        FloatingAttributes.Shengming.OnValueChanged += v => { bar.SetValue(v); };
        FloatingAttributes.Hudun.OnValueChanged += v => { bar.SetSubValue(v); };

        maxMofa.OnValueChanged += v =>
        { magic.Max = v; magic.SetValue(mofa.Value); };
        mofa.OnValueChanged += v => { magic.SetValue(v); };

        BaseAttributes.Shengming.OnValueChanged.Invoke(BaseAttributes.Shengming.Value);
        FloatingAttributes.Shengming.OnValueChanged.Invoke(FloatingAttributes.Shengming.Value);
        FloatingAttributes.Hudun.OnValueChanged.Invoke(FloatingAttributes.Hudun.Value);
        maxMofa.OnValueChanged.Invoke(maxMofa.Value);
        mofa.OnValueChanged.Invoke(mofa.Value);
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
        Hudun = 0;
        Mofa = maxMofa.Value;
        transform.position = Tool.SceneController.Level.GetSpawnPlace(this);
        Tool.UIEventCenter.TrigEvent(new ShowKilledSignalEvent());
    }



    

    protected override int GetId()
    {
        return id;
    }

    public override Target GetNearestEnemy(float range, bool requireInFront)
    {
        if (Tool.FightController.ModeList[0] =='3'&&Camp==3)
        {
            float DMin = 999999;
            Target r = null;
            foreach (var i in Tool.SceneController.Targets)
            {
                if (i.Key != 0) continue;
                foreach (var j in i.Value.Values)
                {
                    if (requireInFront && !InFront(j)) continue;
                    var m = Tool.GetDistance(transform.position, j.transform.position);
                    if (m < DMin && m <= range)
                    {
                        r = j;
                        DMin = m;
                    }
                }
            }
            return r;
        }
        return base.GetNearestEnemy(range, requireInFront);
    }
}
