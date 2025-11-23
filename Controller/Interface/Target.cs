using AttributeSystem.Attributes;
using AttributeSystem.Effect;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Variety.Base;
using static WorldTextController;


public abstract class Target : EnsBehaviour
{
    [Space]
    /// <summary>
    /// 除了玩家都是-1
    /// </summary>
    public int Camp = -1;


    protected DynamicAttributes BaseAttributes;
    protected DynamicAttributes FloatingAttributes;
    [HideInInspector] public DynamicAttributes DedicatedAttributes;

    [HideInInspector]public TargetController controller;
    [HideInInspector]public TargetEffectController effectController;
    [HideInInspector]public TargetSkillController skillController;
    [Tooltip("anim可以设置为空")]
    public TargetAnim anim;
    public TargetVisible visible;

    public TimeLineWork TimeLineWork;
    private RepeatWork repeatWork;
    public RepeatWork RepeatWork
    {
        get
        {
            if(repeatWork==null)repeatWork=gameObject.AddComponent<RepeatWork>();
            return repeatWork;
        }
    }

    public bool FaceRight
    {
        get
        {
            return anim.faceright;
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
    public virtual int Hudun
    {
        get { return FloatingAttributes.Hudun.Value; }
        set
        {
            FloatingAttributes.Hudun.Value = Mathf.Max(value,0);
        }
    }
    public virtual bool UpdateLocally
    {
        get
        {
            return FightController.localPlayerId == 0;
        }
    }

    public LockChain OperationLock=new LockChain();
    public LockChain SkillLock=new LockChain();

    protected float HealthDirtyClearCD;
    protected bool HealthDirty;
    protected float ShieldDirtyClearCD;
    protected bool ShieldDirty;

    public float colliderRadius = -1;
    public Vector2 offset;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + (Vector3)offset, colliderRadius);
    }

    /// <summary>
    ///需要初始化Base/FloatingAttributes
    /// </summary>
    protected void GetAndInitComponents()
    {
        TimeLineWork=gameObject.AddComponent<TimeLineWork>();

        controller=GetComponent<TargetController>();
        effectController=GetComponent<TargetEffectController>();
        skillController=GetComponent<TargetSkillController>();

        if(controller!=null)controller.Init(this);
        if(effectController!=null)effectController.Init(this,BaseAttributes,FloatingAttributes);
        if(skillController!=null)skillController.Init(this);
        if(anim!=null)anim.Init(gameObject,Camp);

        if(TryGetComponent<SyncPosition>(out var sync))sync.Init(this);
    }
    /// <summary>
    /// 需要初始化Base/FloatingAttributes
    /// </summary>
    protected virtual void RegistSyncAttributesEvent()
    {
        //生命每过一段时间同步(脏标记)
        //攻击类属性远程同步
        //防御类属性本地同步
        FloatingAttributes.Shengming.OnValueChanged += _ => { HealthDirty = true; };
        FloatingAttributes.Gongji.OnValueChanged += v => { CallFuncRpc(nameof(Sgj), SendTo.ExcludeSender,v.ToString());DedicatedAttributes.Gongji.Value = v; };
        FloatingAttributes.Mingzhong.OnValueChanged += v => { CallFuncRpc(nameof(Smz), SendTo.ExcludeSender,v.ToString());DedicatedAttributes.Mingzhong.Value = v; };
        FloatingAttributes.Baoji.OnValueChanged += v => { CallFuncRpc(nameof(Sbj), SendTo.ExcludeSender,v.ToString());DedicatedAttributes.Baoji.Value = v; };
        FloatingAttributes.Jiashang.OnValueChanged += v => { CallFuncRpc(nameof(Sjs), SendTo.ExcludeSender, v.ToString());DedicatedAttributes.Jiashang.Value = v; };
        if (UpdateLocally)
        {
            FloatingAttributes.Fangyu.OnValueChanged += v => DedicatedAttributes.Gongji.Value = v;
            FloatingAttributes.Shanbi.OnValueChanged += v => DedicatedAttributes.Shanbi.Value = v;
            FloatingAttributes.Renxing.OnValueChanged += v => DedicatedAttributes.Renxing.Value = v;
            FloatingAttributes.Jianshang.OnValueChanged += v => DedicatedAttributes.Jianshang.Value = v;

            FloatingAttributes.Kangjitui.OnValueChanged += v => DedicatedAttributes.Kangjitui.Value = v;
            FloatingAttributes.Jixing.OnValueChanged += v => DedicatedAttributes.Jixing.Value = v;
            FloatingAttributes.Tengkong.OnValueChanged += v => DedicatedAttributes.Tengkong.Value = v;
            FloatingAttributes.Liantiao.OnValueChanged += v=>DedicatedAttributes.Liantiao.Value = v;
        }
        FloatingAttributes.SetAllDirty();

        DedicatedAttributes.Shengming.OnValueChanged += v =>
        {
            if (visible != null) visible.SetNum(v * 1f / BaseAttributes.Shengming.Value);
        };
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
    }

    protected virtual HashSet<Bullet> DetectBullet() 
    {
        Vector3 playerPos = transform.position + (Vector3)offset;
        if (colliderRadius < 0)
        {
            Debug.Log(gameObject.name + "未初始化");
            return new HashSet<Bullet>();
        }
        HashSet<Bullet> result = new HashSet<Bullet>();
        foreach(var i in Bullet.Bullets)
        {
            if (!HostilityWith(i.Key)) continue;
            foreach(var j in i.Value.Values)
            {
                float s = j.transform.localScale.x;
                if (CalHit(j.transform.position, j.LastFramePos, playerPos, colliderRadius + j.radius * s)) result.Add(j);
            }
        }
        return result;
    }
    private bool CalHit(Vector3 lineStart, Vector3 lineEnd, Vector3 point, float distanceThreshold)
    {
        // 计算线段向量
        Vector3 lineVector = lineEnd - lineStart;
        // 计算点到线段起点的向量
        Vector3 pointVector = point - lineStart;

        // 计算线段长度的平方（避免开方，提高性能）
        float lineLengthSquared = lineVector.sqrMagnitude;

        // 处理线段长度为0的特殊情况（起点和终点重合）
        if (lineLengthSquared < 0.001f)
        {
            // 此时线段退化为点，直接计算点到起点的距离
            return (point - lineStart).sqrMagnitude < distanceThreshold * distanceThreshold;
        }

        // 计算投影参数t（点在直线上的投影位置相对于线段的比例）
        float t = Vector3.Dot(pointVector, lineVector) / lineLengthSquared;
        // 限制t在[0,1]范围内，得到线段上最近点的投影参数
        t = Mathf.Clamp01(t);

        // 计算线段上的最近点
        Vector3 closestPoint = lineStart + t * lineVector;
        // 计算点到最近点的距离平方
        float distanceSquared = (point - closestPoint).sqrMagnitude;

        // 比较距离平方与阈值平方（避免开方运算，提高效率）
        return distanceSquared < distanceThreshold * distanceThreshold;
    }
    public virtual bool HostilityWith(int camp)
    {
        return camp != Camp;
    }

    /// <summary>
    /// 默认根据DetectBullet被子弹击中，同步属性
    /// </summary>
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
            CallFuncRpc(nameof(Ssm), SendTo.ExcludeSender,FloatingAttributes.Shengming.Value.ToString());
            DedicatedAttributes.Shengming.Value=FloatingAttributes.Shengming.Value;
            HealthDirtyClearCD = 0.15f;
        }
        if (ShieldDirtyClearCD > 0) ShieldDirtyClearCD -= Time.deltaTime;
        else if (ShieldDirty)
        {
            ShieldDirty = false;
            CallFuncRpc(nameof(Shd), SendTo.ExcludeSender, FloatingAttributes.Shengming.Value.ToString());
            DedicatedAttributes.Hudun.Value = FloatingAttributes.Hudun.Value;
            ShieldDirtyClearCD = 0.15f;
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
            Damaged(d);
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
        Tool.WorldTextController.ShowTextLocal(s[0], visible.transform.position+Vector3.up*1.5f, (TextColor)int.Parse(s[1]));
    }
    protected void Damaged(int d)
    {
        if (d == 0) { }
        else if (Hudun > d)
        {
            Hudun -= d;
        }
        else
        {
            if (Hudun > 0)
            {
                d -= Hudun;
                Hudun = 0;
            }
            Shengming -= d;
            if (Shengming <= 0)
            {
                Shengming = 0;
            }
        }
    }

    protected virtual void OnHitBack(Bullet b)
    {
        if(controller!=null)controller.OnHitBack(b);
    }
    protected virtual void ApplyEffects(Bullet b)
    {
        if (effectController == null) return;
        var ec = b.GetEffects();
        if (ec != null)
        {
            foreach (var i in ec.GetEffectBases(this))
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
    protected virtual int GetId()
    {
        return ObjectId;
    }
    public virtual bool FacingRight()
    {
        return controller.FaceRight;
    }



    public virtual Target GetNearestEnemy(float range=999999, bool requireInFront=false)
    {
        float DMin = 999999;
        Target r = null;
        foreach (var i in Tool.SceneController.Targets)
        {
            if (i.Key == Camp) continue;
            foreach(var j in i.Value.Values)
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
    public virtual List<Target> GetEnemyInRange(float range=999999, bool requireInFront=false)
    {
        List<Target> r = new List<Target>();
        foreach (var i in Tool.SceneController.Targets)
        {
            if (i.Key == Camp) continue;
            foreach (var j in i.Value.Values)
            {
                if (requireInFront && !InFront(j)) continue;
                if (Tool.GetDistance(transform.position, j.transform.position) <= range) r.Add(j);
            }
        }
        return r;
    }
    public virtual Target GetNearestPartner(float range, bool requireInFront)
    {
        float DMin = 999999;
        Target r = null;
        foreach (var i in Tool.SceneController.Targets)
        {
            if (i.Key != Camp) continue;
            foreach (var j in i.Value.Values)
            {
                if (j.GetId() == GetId()) continue;
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
    public virtual List<Target> GetPartnerInRange(float range, bool requireInFront)
    {
        List<Target> r = new List<Target>();
        foreach (var i in Tool.SceneController.Targets)
        {
            if (i.Key != Camp) continue;
            foreach (var j in i.Value.Values)
            {
                if (j.GetId() == GetId()) continue;
                if (requireInFront && !InFront(j)) continue;
                if (Tool.GetDistance(transform.position, j.transform.position) <= range) r.Add(j);
            }
        }
        return r;
    }

    protected bool InFront(Target data)
    {
        return (data.transform.position.x > transform.position.x) == FacingRight();
    }


    public void RegistEvent(string key,Action<Target> action)
    {
        Level.RegistEvent(key, this,action);
    }
    public void TrigEvent(string key)
    {
        Level.TrigEvent(key,this);
    }

    private void Ssm(string data)
    {
        DedicatedAttributes.Shengming.Value = int.Parse(data);
    }
    private void Shd(string data)
    {
        DedicatedAttributes.Hudun.Value = int.Parse(data);
    }
    private void Sgj(string data)
    {
        DedicatedAttributes.Gongji.Value = int.Parse(data);
    }
    private void Smz(string data)
    {
        DedicatedAttributes.Mingzhong.Value = int.Parse(data);
    }
    private void Sbj(string data)
    {
        DedicatedAttributes.Baoji.Value = int.Parse(data);
    }
    private void Sjs(string data)
    {
        DedicatedAttributes.Jiashang.Value = int.Parse(data);
    }
}