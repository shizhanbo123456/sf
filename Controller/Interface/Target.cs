using AttributeSystem.Attributes;
using AttributeSystem.Effect;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variety.Base;
using static WorldTextController;


public abstract class Target : EnsBehaviour
{
    public const int RegenerationAdderId = -10000;
    public const int SceneEffectId = -10001;

    /// <summary>
    /// 除了玩家都是-1
    /// </summary>
    [HideInInspector]public int Camp = -1;
    [Space]
    public bool CanMove=true;
    public bool Effectable = true;
    public bool CanUseSkill = true;

    public GameTimeAttributes BaseAttributes;
    public GameTimeAttributes FloatingAttributes;
    public DedicateSyncAttributes DedicatedAttributes;

    private BulletDetector bulletDetector;
    [HideInInspector]public TargetInfoSync targetInfoSync;
    [HideInInspector]public TargetController controller;
    [HideInInspector]public TargetEffectController effectController;
    [HideInInspector]public TargetSkillController skillController;
    [Tooltip("anim可以设置为空")]
    [Space]
    public TargetGraphic graphic;
    public TargetBar targetBar;

    [HideInInspector]public TimeLineWork TimeLineWork;
    public RepeatWork RepeatWork
    {
        get
        {
            var repeatWork=GetComponent<RepeatWork>();
            if(repeatWork==null)repeatWork=gameObject.AddComponent<RepeatWork>();
            return repeatWork;
        }
    }

    public bool FaceRight
    {
        get
        {
            return targetInfoSync.FaceRight;
        }
    }
    public virtual int Shengming
    {
        get { return FloatingAttributes.Shengming.Value; }
        set
        {
            FloatingAttributes.Shengming.Value = Mathf.Clamp(value, 0, BaseAttributes.Shengming.Value);
        }
    }
    public virtual bool UpdateLocally
    {
        get
        {
            return FightController.localPlayerId == 0;
        }
    }

    public LockChain OperationLock=LockChain.CreateLock();
    public LockChain SkillLock=LockChain.CreateLock();

    private float HealthDirtyClearCD;
    private bool HealthDirty;

    /// <summary>
    ///需要初始化Base/FloatingAttributes
    /// </summary>
    protected void GetAndInitComponents()
    {
        TimeLineWork=gameObject.AddComponent<TimeLineWork>();

        if(!TryGetComponent(out targetInfoSync))
        {
            Debug.LogError("未找到同步");
        }
        targetInfoSync.nomEnabled = UpdateLocally;

        if (graphic != null) graphic.Init(gameObject);

        if (UpdateLocally)
        {
            if (CanMove) (controller = AddController()).Init(this);
            if (Effectable) (effectController=AddEffectController()).Init(this, BaseAttributes, FloatingAttributes);
            if (CanUseSkill) (skillController=AddSkillController()).Init(this);
        }
    }
    protected virtual TargetController AddController()=> throw new Exception("添加控制器必须重写添加方法");
    protected virtual TargetEffectController AddEffectController() => gameObject.AddComponent<TargetEffectController>();
    protected virtual TargetSkillController AddSkillController()=> gameObject.AddComponent<TargetSkillController>();
    /// <summary>
    /// 需要初始化Base/FloatingAttributes
    /// </summary>
    protected virtual void RegistSyncAttributesEvent()
    {
        DedicatedAttributes = new DedicateSyncAttributes();
        DedicatedAttributes.Shengming.OnValueChanged += v => targetBar.SetNum(v.Item2 / (float)v.Item1);
        //生命每过一段时间同步(脏标记)
        //攻击类属性远程同步
        //防御类属性本地同步
        FloatingAttributes.Shengming.OnValueChanged += _ => { HealthDirty = true; };
        FloatingAttributes.Gongji.OnValueChanged += v => { CallFuncRpc(nameof(Sgj), SendTo.ExcludeSender,v.ToString());DedicatedAttributes.Gongji = v; };
        FloatingAttributes.Mingzhong.OnValueChanged += v => { CallFuncRpc(nameof(Smz), SendTo.ExcludeSender,v.ToString());DedicatedAttributes.Mingzhong = v; };
        FloatingAttributes.Baoji.OnValueChanged += v => { CallFuncRpc(nameof(Sbj), SendTo.ExcludeSender,v.ToString());DedicatedAttributes.Baoji = v; };
        FloatingAttributes.Jiashang.OnValueChanged += v => { CallFuncRpc(nameof(Sjs), SendTo.ExcludeSender, v.ToString());DedicatedAttributes.Jiashang = v; };

        FloatingAttributes.SetAllDirty();
        HealthDirty = true;
    }
    protected void InitEssential()
    {
        nomEnabled = UpdateLocally;
    }

    /// <summary>
    /// 需要手动调用
    /// </summary>
    protected virtual void OnCreated()
    {
        Tool.SceneController.OnTargetPostcreated(this);
    }
    protected virtual void OnDestroy()
    {
        Level.ClearEvent(this);
        Tool.SceneController.OnTargetPredestroy(this);

        OperationLock.Discard();
        SkillLock.Discard();
        OperationLock=null;
        SkillLock=null;
    }

    protected virtual HashSet<Bullet> DetectBullet()
    {
        if (bulletDetector == null)
        {
            if(!TryGetComponent(out bulletDetector))
            {
                Debug.LogError(gameObject.name + "未挂载子弹检测器");
                return new();
            }
        }
        return bulletDetector.DetectBullet();
    }
    public virtual bool HostilityWith(int camp)
    {
        return camp != Camp;
    }

    public override void ManagedUpdate()
    {
        foreach (var b in DetectBullet())
        {
            DamageByBullet(b);
        }
        if (HealthDirtyClearCD > 0) HealthDirtyClearCD -= Time.deltaTime;
        else if (HealthDirty)
        {
            HealthDirty = false;
            OnSyncHealth();
            HealthDirtyClearCD = 0.15f;
        }
    }
    private void OnSyncHealth()
    {
        CallFuncRpc(nameof(OnSyncHealthLocal),SendTo.ExcludeSender,BaseAttributes.Shengming.ToString()+'_'+FloatingAttributes.Shengming.ToString());
        DedicatedAttributes.Shengming.Value = (BaseAttributes.Shengming.Value, FloatingAttributes.Shengming.Value);
    }
    private void OnSyncHealthLocal(string param)
    {
        var s = param.Split('_');
        DedicatedAttributes.Shengming.Value = (int.Parse(s[0]), int.Parse(s[1]));
    }
    protected virtual bool DamageByBullet(Bullet b)
    {
        if (b.CanDamage==null) return false;
        if(!b.CanDamage.Invoke(this,b))return false;

        int d = b.FigureDamage(FloatingAttributes, out bool hit, out bool strike);
        ShowDamageText(d, hit, strike);

        if (d > 0)
        {
            OnHitBack(b);
            ApplyEffects(b);
            Shengming -= d;
            if (Shengming <= 0)
            {
                Shengming = 0;
            }
        }
        return true;
    }
    protected virtual void ShowDamageText(int value,bool hit,bool strike)
    {
        if (!hit) ShowTextServerRpc("miss", TextColor.Blue);
        else if (!strike) ShowTextServerRpc('-' + value.ToString(),  TextColor.Orange);
        else ShowTextServerRpc('-' + value.ToString(), TextColor.Red);
    }
    protected void ShowTextServerRpc(string text,TextColor color)
    {
        string s = $"{text}_{(int)color}";
        CallFuncRpc(nameof(ShowTextLocal), SendTo.Everyone, s);
    }
    private void ShowTextLocal(string data)
    {
        string[] s=data.Split('_');
        Tool.WorldTextController.ShowTextLocal(s[0], targetBar.transform.position+Vector3.up*1.5f, (TextColor)int.Parse(s[1]));
    }
    protected virtual void OnHitBack(Bullet b)
    {
        if(controller!=null)controller.OnHitBack(b);
    }
    protected virtual void ApplyEffects(Bullet b)
    {
        if (effectController == null) return;
        var effs = b.GetEffects().GetEffectBases(this);
        if (effs != null)
        {
            foreach (var i in effs)
            {
                effectController.AddEffect(i);
            }
        }
    }
    public virtual void ApplyMotion(MotionBase m)
    {
        if(controller!=null)controller.ApplyMotion(m);
    }
    public virtual void ApplyEffect(EffectBase e)
    {
        if (effectController == null) return;
        effectController.AddEffect(e);
    }

    public void RegistEvent(string key, Action<Target> action)
    {
        Level.RegistEvent(key, this, action);
    }
    public void TrigEvent(string key)
    {
        Level.TrigEvent(key, this);
    }

    private void Sgj(string data)
    {
        DedicatedAttributes.Gongji = int.Parse(data);
    }
    private void Smz(string data)
    {
        DedicatedAttributes.Mingzhong = int.Parse(data);
    }
    private void Sbj(string data)
    {
        DedicatedAttributes.Baoji = int.Parse(data);
    }
    private void Sjs(string data)
    {
        DedicatedAttributes.Jiashang = int.Parse(data);
    }

    public void UseSkillRpc(int index)
    {
        var sb = Tool.stringBuilder;
        sb.Clear();
        sb.Append(index).Append('_').
            Append(transform.position.x.ToString("F1")).Append('_').
            Append(transform.position.y.ToString("F2")).Append('_').
            Append(FaceRight ? '1' : '0');
        CallFuncRpc(nameof(UseSkillLocal), SendTo.Everyone, sb.ToString());
    }
    private void UseSkillLocal(string param)
    {
        string[] s = param.Split('_');
        int index=int.Parse(s[0]);
        Vector3 pos = new Vector3(float.Parse(s[1]), float.Parse(s[2]));
        bool faceright = s[3][0] == '1';
        VarietyManager.GetSkill(index).UseSkill(this, pos, faceright);
    }
    public void SyncEffectIconRpc(List<int> values)
    {
        if (values==null||values.Count==0)
        {
            CallFuncRpc(nameof(SyncEffectIconLocal), SendTo.Everyone, "null");
            return;
        }
        CallFuncRpc(nameof(SyncEffectIconLocal), SendTo.Everyone, Format.ListToString(values, '+'));
    }
    private void SyncEffectIconLocal(string data)
    {
        if (data == "null")
        {
            targetBar.ShowEffects(new List<EffectType>());
            return;
        }
        var list = Format.StringToList(data, int.Parse, '+');
        targetBar.ShowEffects(list.Select(i => (EffectType)i).ToList());
    }
    public void InterruptRpc()
    {
        CallFuncRpc(nameof(InterruptLocal), SendTo.ExcludeSender);
        InterruptLocal();
    }
    public void InterruptLocal()
    {
        TimeLineWork.Interrupted();
    }


    private static readonly List<Target> targets = new List<Target>();
    public virtual Target GetNearestEnemy(float range = 99999f, bool requireInFront = false)
    {
        float DMin = range * range; // 使用距离平方进行比较
        Target r = null;
        foreach (var i in Tool.SceneController.Targets)
        {
            if (i.Key == Camp) continue;
            foreach (var j in i.Value.Values)
            {
                if (requireInFront && !InFront(j)) continue;
                var mSqr = (transform.position - j.transform.position).sqrMagnitude;
                if (mSqr < DMin)
                {
                    r = j;
                    DMin = mSqr;
                }
            }
        }
        return r;
    }

    public virtual Target GetNearestPartner(float range = 99999f, bool requireInFront = false)
    {
        float DMin = range * range;
        Target r = null;
        foreach (var i in Tool.SceneController.Targets)
        {
            if (i.Key != Camp) continue;
            foreach (var j in i.Value.Values)
            {
                if (j.ObjectId == ObjectId) continue;
                if (requireInFront && !InFront(j)) continue;
                var mSqr = (transform.position - j.transform.position).sqrMagnitude;
                if (mSqr < DMin)
                {
                    r = j;
                    DMin = mSqr;
                }
            }
        }
        return r;
    }

    public virtual List<Target> GetEnemyInRange(float range = 99999f, bool requireInFront = false)
    {
        targets.Clear();
        float rangeSqr = range * range;
        foreach (var i in Tool.SceneController.Targets)
        {
            if (i.Key == Camp) continue;
            foreach (var j in i.Value.Values)
            {
                if (requireInFront && !InFront(j)) continue;
                if ((transform.position - j.transform.position).sqrMagnitude <= rangeSqr)
                {
                    targets.Add(j);
                }
            }
        }
        return targets;
    }

    public virtual List<Target> GetPartnerInRange(float range = 99999f, bool requireInFront = false)
    {
        targets.Clear();
        float rangeSqr = range * range;
        foreach (var i in Tool.SceneController.Targets)
        {
            if (i.Key != Camp) continue;
            foreach (var j in i.Value.Values)
            {
                if (j.ObjectId == ObjectId) continue;
                if (requireInFront && !InFront(j)) continue;
                if ((transform.position - j.transform.position).sqrMagnitude <= rangeSqr)
                {
                    targets.Add(j);
                }
            }
        }
        return targets;
    }

    public virtual List<Target> GetEnemyInRect(float halfx, float halfy, bool requireInFront = false)
    {
        targets.Clear();
        foreach (var i in Tool.SceneController.Targets)
        {
            if (i.Key == Camp) continue;
            foreach (var j in i.Value.Values)
            {
                if (requireInFront && !InFront(j)) continue;
                if (Mathf.Abs(j.transform.position.x-transform.position.x) <= halfx && 
                    Mathf.Abs(j.transform.position.y-transform.position.y) <= halfy)
                {
                    targets.Add(j);
                }
            }
        }
        return targets;
    }

    public virtual List<Target> GetPartnerInRect(float halfx, float halfy, bool requireInFront = false)
    {
        targets.Clear();
        foreach (var i in Tool.SceneController.Targets)
        {
            if (i.Key != Camp) continue;
            foreach (var j in i.Value.Values)
            {
                if (j.ObjectId==ObjectId) continue;
                if (requireInFront && !InFront(j)) continue;
                if (Mathf.Abs(j.transform.position.x - transform.position.x) <= halfx &&
                    Mathf.Abs(j.transform.position.y - transform.position.y) <= halfy)
                {
                    targets.Add(j);
                }
            }
        }
        return targets;
    }

    protected bool InFront(Target data)
    {
        return (data.transform.position.x > transform.position.x) == FaceRight;
    }
}