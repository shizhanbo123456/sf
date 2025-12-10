using AttributeSystem.Attributes;
using AttributeSystem.Effect;
using System;
using System.Collections.Generic;
using UnityEngine;
using Variety.Base;

/// <summary>
/// 需要添加TargetDataSync,TargetControllerSync
/// </summary>
public abstract class Target : MonoBehaviour
{
    public const int RegenerationAdderId = -10000;
    public const int SceneEffectId = -10001;

    public int ObjectId=>targetDataSync.ObjectId;

    public TargetInfo Info;
    public int Camp => Info.camp;
    public int Level => Info.level;
    public int Owner => Info.owner;
    public string Name => Info.name;

    public GameTimeAttributes BaseAttributes;
    public GameTimeAttributes FloatingAttributes;
    public DedicateSyncAttributes DedicatedAttributes=>targetDataSync.DedicatedAttributes;

    [HideInInspector]public TargetDataSync targetDataSync;
    [HideInInspector]public TargetControllerSync targetInfoSync;

    [HideInInspector]public TargetGraphic graphic;
    [HideInInspector]public TargetController controller;
    [HideInInspector]public TargetEffectController effectController;
    [HideInInspector]public TargetSkillController skillController;

    [HideInInspector]public TimeLineWork TimeLineWork;

    public bool FaceRight=> targetInfoSync.FaceRight;
    public virtual int Shengming
    {
        get { return FloatingAttributes.Shengming.Value; }
        set
        {
            FloatingAttributes.Shengming.Value = Mathf.Clamp(value, 0, BaseAttributes.Shengming.Value);
        }
    }
    public virtual bool UpdateLocally=> EnsInstance.HasAuthority;

    public LockChain OperationLock=LockChain.CreateLock();
    public LockChain SkillLock=LockChain.CreateLock();

    /// <summary>
    /// 分配信息，设置位置，获取组件，注册OnCreate。注册属性同步需要额外调用<br/>
    /// 需要手动调用InitNameAndBar(所有客户端)和RegistSyncAttributes(拥有者)
    /// </summary>
    /// <param name="info"></param>
    public virtual void Init(TargetInfo info)
    {
        Info=info;

        graphic.transform.localScale = info.size * Vector3.one;

        transform.position = Tool.SceneController.Level.GetPos(info.spawnX, info.spawnY) +
            graphic.SpawnOffset * graphic.transform.localScale.y * Vector3.up;

        TimeLineWork = gameObject.AddComponent<TimeLineWork>();
        if (!TryGetComponent(out targetInfoSync)) Debug.LogError("未找到同步");
        if (TryGetComponent(out targetDataSync)) targetDataSync.Init(this);
        else Debug.LogError("未找到信息同步");

        RegistOnCreated();
    }
    protected virtual void InitNameAndBar()
    {
        graphic.SetName(Name, Tool.SpriteManager.TargetToColor(this));
    }
    protected virtual void RegistSyncAttributes()
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


        DedicatedAttributes.Shengming.OnValueChanged += v => graphic.targetBar.SetNum(v.Item2 / (float)v.Item1);
    }

    protected virtual void RegistOnCreated()
    {
        Tool.SceneController.OnTargetPostcreated(this);
    }
    protected virtual void RegistOnDestroy()
    {
        Tool.SceneController.OnTargetPredestroy(this);

        OperationLock.Discard();
        SkillLock.Discard();
        OperationLock=null;
        SkillLock=null;
    }
    private void OnDestroy()
    {
        RegistOnDestroy();
    }

    protected virtual HashSet<Bullet> DetectBullet()=>graphic.bulletDetector.DetectBullet();

    protected virtual void FixedUpdate()
    {
        foreach (var b in DetectBullet())
        {
            DamageByBullet(b);
            if (Shengming == 0) break;
        }
    }
    protected virtual bool DamageByBullet(Bullet b)
    {
        if (b.CanDamage==null) return false;
        if(!b.CanDamage.Invoke(this,b))return false;

        int d = b.FigureDamage(FloatingAttributes, out bool hit, out bool strike);
        ShowDamageText(d, hit, strike);

        if (hit)
        {
            Shengming -= d;
            if (Shengming <= 0)
            {
                Shengming = 0;
                OnKilled(b.Shooter);
            }
            else
            {
                OnHitBack(b);
                ApplyEffects(b);
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
    /// <summary>
    /// 被非对象击杀则传入null
    /// </summary>
    public virtual void OnKilled(Target killer)
    {
        Tool.SceneController.RegistTargetKilled(killer,this);
        targetDataSync.DestroyRpc();
    }

    public void UseSkillRpc(int index)=>targetDataSync.UseSkillRpc(index);
    public void SyncEffectIconRpc(List<int> values)=>targetDataSync.SyncEffectIconRpc(values);
    public void InterruptRpc()=>targetDataSync.InterruptRpc();


    private static readonly List<Target> targets = new List<Target>();
    public bool InFront(Target data)
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
    public Target GetNearestEnemy(float range = 99999f, bool requireInFront = false)
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
    public Target GetNearestPartner(float range = 99999f, bool requireInFront = false)
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
    public List<Target> GetEnemyInRange(float range = 99999f, bool requireInFront = false)
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
    public List<Target> GetPartnerInRange(float range = 99999f, bool requireInFront = false)
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
    public List<Target> GetEnemyInRect(float halfx, float halfy, bool requireInFront = false)
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
    public List<Target> GetPartnerInRect(float halfx, float halfy, bool requireInFront = false)
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