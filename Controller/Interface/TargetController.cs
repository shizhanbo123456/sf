using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Variety.Base;

public abstract class TargetController : MonoBehaviour
{
    protected Target target;
    protected Rigidbody2D rb;
    private GroundDetector groundDetector;

    

    [HideInInspector] public bool isGrounded;
    public float MoveSpeed
    {
        get { return Mathf.Max(target.FloatingAttributes.Jixing.Value, 0); }
    }
    public float JumpSpeed
    {
        get { return Mathf.Sqrt(Mathf.Max(target.FloatingAttributes.Tengkong.Value *20, 0)); }
    }
    [HideInInspector] public int JumpCount = 1;


    private MotionBase motion;
    public MotionBase Motion
    {
        get { return motion; }
        set 
        {
            motion = value;
            MotionEntered = false;
            rb.gravityScale = value == null ? 1 : 0;
            if (motion != null)
            {
                OperationLock_Motion.Locked = motion.MoveLock;
                SkillLock_Motion.Locked = motion.SkillLock;
            }
            else
            {
                OperationLock_Motion.Locked = false;
                SkillLock_Motion.Locked = false;
            }
        }
    }
    private bool MotionEntered = true;

    protected virtual float MinResisiance => -1f;
    [HideInInspector]public float Resistance=0.01f;
    public bool InHitDuration
    {
        get
        {
            return Resistance < 0;
        }
    }

    [HideInInspector]public Vector2 MotionVector;

    private bool Initialized = false;

    public LockChain OperationLock_Hit;
    public LockChain SkillLock_Hit;
    public LockChain OperationLock_Motion;
    public LockChain SkillLock_Motion;

    public virtual void Init(Target t)
    {
        if (!t.UpdateLocally)
        {
            Debug.LogError("非本地角色");
            return;
        }

        target = t;
        rb = GetComponent<Rigidbody2D>();
        groundDetector= GetComponent<GroundDetector>();

        var operationlock = t.OperationLock;
        var skilllock= t.SkillLock;
        OperationLock_Hit=operationlock.GetChain();
        SkillLock_Hit=skilllock.GetChain();
        OperationLock_Motion = operationlock.GetChain();
        SkillLock_Motion=skilllock.GetChain();

        Initialized = true;
    }
    protected virtual void Update()
    {
        PlayerUpdate();
        if (target.targetInfoSync.OnPlayerPostUpdate())
        {
            target.targetInfoSync.SyncController(transform.position, rb.velocity, Resistance,
                MotionVector.y<-0.5f, OperationLock_Motion.LockedInHierechy, isGrounded, motion == null);
        }
    }
    private void PlayerUpdate()
    {
        if (!Initialized) return;
        Ground();
        if (InHitDuration)
        {
            OperationLock_Hit.Locked = true;
            SkillLock_Hit.Locked = true;

            if (isGrounded && Motion == null) Resistance += Time.deltaTime;
            else Resistance = MinResisiance;
        }
        else
        {
            OperationLock_Hit.Locked = false;
            SkillLock_Hit.Locked = false;
        }
        if (Motion != null) UpdateMotion();

        MotionVector = new Vector2();
        if (InHitDuration)
        {
            if (isGrounded && Mathf.Abs(rb.velocity.y) < 0.01f)
            {
                rb.velocity = new Vector2();
            }
            return;
        }
        if (!OperationLock_Motion.LockedInHierechy)
        {
            MotionVector = GetInputVector();
            InputWalk(MotionVector.x, MotionVector.y > 0.5f);
        }
    }
    private void Ground()
    {
        if (!groundDetector||groundDetector.IsGround())
        {
            isGrounded = true;
            if (rb.velocity.y <= 0.01f) JumpCount = 0;
        }
        else
        {
            isGrounded = false;
        }
    }
    private float jumpcd=-1;
    private void InputWalk(float x, bool jump)
    {
        if (Motion == null) rb.velocity = new Vector2(x * MoveSpeed, rb.velocity.y);
        else
        {
            rb.velocity = new Vector2(x * MoveSpeed + rb.velocity.x, rb.velocity.y);
        }
        if (jump)
        {
            if (Time.time - jumpcd < 0.2f) return;
            jumpcd = Time.time;
            if (JumpCount < target.FloatingAttributes.Liantiao.Value)
            {
                rb.velocity = new Vector2(rb.velocity.x, JumpSpeed);
                JumpCount += 1;
            }
        }
    }
    private void UpdateMotion()
    {
        if (!MotionEntered)
        {
            rb.velocity = Motion.Entry(rb.velocity);
            MotionEntered = true;
        }
        else if (Motion.SpawnTime > Motion.WorkTime)
        {
            rb.velocity = Motion.Exit(rb.velocity);
            Motion = null;
        }
        else
        {
            rb.velocity = Motion.GetVelocity(rb.velocity);
        }
    }
    public abstract Vector2 GetInputVector();



    public virtual bool OnHitBack(Bullet b)
    {
        int hitbackResist = target.FloatingAttributes.Kangjitui.Value;
        if (InHitDuration) hitbackResist = 0;
        else if (Motion != null&&Motion.ActiveAdded) hitbackResist = Mathf.Max(hitbackResist, Motion.StoicLevel);
        if (b.liftStoicLevel > hitbackResist)
        {
            if (Motion != null)
            {
                Motion.Exit(rb.velocity);
                Motion = null;
            }
            target.TimeLineWork.Interrupted();

            rb.velocity = b.hitbackForce.Invoke(b.transform.position, transform.position);

            Resistance = MinResisiance;
            return true;
        }
        return false;
    }
    public void ApplyMotion(MotionBase m)
    {
        if (m == null)
        {
            Debug.LogError("添加了空的运动行为");
            return;
        }
        if (Motion == null)
        {
            int hitbackResist = target.FloatingAttributes.Kangjitui.Value;
            if (InHitDuration) hitbackResist = 0;
            if(m.StoicLevel>=hitbackResist)Motion = m;
        }
        else if (m.StoicLevel >= Motion.StoicLevel||m.ActiveAdded)
        {
            Motion = m;
        }
    }
}
