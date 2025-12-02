using AttributeSystem.Attributes;
using AttributeSystem.Effect;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variety.Base;
using static WorldTextController;

/// <summary>
/// 需要添加TargetDataSync,TargetControllerSync
/// </summary>
public abstract class Target : MonoBehaviour
{
    public const int RegenerationAdderId = -10000;
    public const int SceneEffectId = -10001;

    public int ObjectId=>targetDataSync.ObjectId;

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
    public DedicateSyncAttributes DedicatedAttributes=>targetDataSync.DedicatedAttributes;

    [HideInInspector]public TargetDataSync targetDataSync;
    [HideInInspector]public TargetControllerSync targetInfoSync;
    [HideInInspector]public TargetController controller;
    [HideInInspector]public TargetEffectController effectController;
    [HideInInspector]public TargetSkillController skillController;
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

    /// <summary>
    ///需要初始化Base/FloatingAttributes
    /// </summary>
    protected void GetAndInitComponents()
    {
        TimeLineWork=gameObject.AddComponent<TimeLineWork>();

        if(!TryGetComponent(out targetInfoSync)) Debug.LogError("未找到同步");
        if (TryGetComponent(out targetDataSync)) targetDataSync.Init(this);
        else Debug.LogError("未找到信息同步");

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
    protected virtual void RegistSyncDedicateAttributes()
    {
        void SyncShengming()
        {
            targetDataSync.SyncShengming(BaseAttributes.Shengming.Value, FloatingAttributes.Shengming.Value);
        }
        //生命每过一段时间同步(脏标记)
        //攻击类属性远程同步
        //防御类属性本地同步
        BaseAttributes.Shengming.OnValueChanged += _ => SyncShengming();
        FloatingAttributes.Shengming.OnValueChanged += _ => SyncShengming();
        FloatingAttributes.Gongji.OnValueChanged += v => targetDataSync.SyncGongji(v);
        FloatingAttributes.Mingzhong.OnValueChanged += v => targetDataSync.SyncMingzhong(v);
        FloatingAttributes.Baoji.OnValueChanged += v => targetDataSync.SyncBaoji(v);
        FloatingAttributes.Jiashang.OnValueChanged += v => targetDataSync.SyncJiashang(v);

        FloatingAttributes.SetAllDirty();
    }
    protected virtual void RegistDedicateAttributePostSyncEvent()
    {
        DedicatedAttributes.Shengming.OnValueChanged += v => targetBar.SetNum(v.Item2 / (float)v.Item1);
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

    protected virtual HashSet<Bullet> DetectBullet()=>graphic.bulletDetector.DetectBullet();

    public void Update()
    {
        foreach (var b in DetectBullet())
        {
            DamageByBullet(b);
        }
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
    protected void ShowDamageText(int value,bool hit,bool strike)=>targetDataSync.ShowDamageText(value, hit, strike);
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

    

    public void UseSkillRpc(int index)=>targetDataSync.UseSkillRpc(index);
    public void SyncEffectIconRpc(List<int> values)=>targetDataSync.SyncEffectIconRpc(values);
    public void InterruptRpc()=>targetDataSync.InterruptRpc();


    private static readonly List<Target> targets = new List<Target>();

    protected bool InFront(Target data)
    {
        return (data.transform.position.x > transform.position.x) == FaceRight;
    }
    public Target GetNearest<T>(Dictionary<int,T>src,float range = 99999f, bool requireInFront = false)where T:Target
    {
        float DMin = range * range; // 使用距离平方进行比较
        Target r = null;
        foreach (var i in src.Values)
        {
            if (requireInFront && !InFront(i)) continue;
            var mSqr = (transform.position - i.transform.position).sqrMagnitude;
            if (mSqr < DMin)
            {
                r = i;
                DMin = mSqr;
            }
        }
        return r;
    }
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
    public List<Target> GetInRange<T>(Dictionary<int, T> src,float range = 99999f, bool requireInFront = false)where T : Target
    {
        targets.Clear();
        float rangeSqr = range * range;
        foreach (var i in src.Values)
        {
            if (requireInFront && !InFront(i)) continue;
            if ((transform.position - i.transform.position).sqrMagnitude <= rangeSqr)
            {
                targets.Add(i);
            }
        }
        return targets;
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
    public List<Target> GetInRect<T>(Dictionary<int, T> src,float halfx, float halfy, bool requireInFront = false)where T:Target
    {
        targets.Clear();
        foreach (var i in src.Values)
        {
            if (requireInFront && !InFront(i)) continue;
            if (Mathf.Abs(i.transform.position.x - transform.position.x) <= halfx &&
                Mathf.Abs(i.transform.position.y - transform.position.y) <= halfy)
            {
                targets.Add(i);
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
}