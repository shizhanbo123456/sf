using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Variety.Base;

public abstract class TargetController : MonoBehaviour
{
    protected Target target;
    protected Rigidbody2D rb;
    private SyncPosition syncPosition;

    public Transform GroundCheck;
    public Transform GroundCheck2;
    public float GroundCheckDistance = 1f;

    public bool isGrounded;
    public bool FaceRight
    {
        get { return syncPosition.FaceRight; }
    }
    public float MoveSpeed
    {
        get { return Mathf.Max(target.DedicatedAttributes.Jixing.Value, 0); }
    }
    public float JumpSpeed
    {
        get { return Mathf.Sqrt(Mathf.Max(target.DedicatedAttributes.Tengkong.Value *20, 0)); }
    }
    public int JumpCount = 1;


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
    public float Resistance=0.01f;
    public bool InHitDuration
    {
        get
        {
            return Resistance < 0;
        }
    }

    public Vector2 MotionVector;

    private bool updateLocally = false;
    private bool Initialized = false;

    public LockChain OperationLock_Hit;
    public LockChain SkillLock_Hit;
    public LockChain OperationLock_Motion;
    public LockChain SkillLock_Motion;

    public virtual void Init(Target t)
    {
        updateLocally = t.UpdateLocally;

        target = t;
        rb = GetComponent<Rigidbody2D>();
        syncPosition = GetComponent<SyncPosition>();

        if (updateLocally) syncPosition.PreUpdate += _Update;

        var operationlock = t.OperationLock;
        var skilllock= t.SkillLock;
        OperationLock_Hit=operationlock.GetChain();
        SkillLock_Hit=skilllock.GetChain();
        OperationLock_Motion = operationlock.GetChain();
        SkillLock_Motion=skilllock.GetChain();

        Initialized = true;
    }
    public abstract Vector2 GetInputVector();
    protected virtual void Update()
    {
        Ground();
        LayerUpdate();
    }
    private void _Update()
    {
        if (!Initialized) return;
        if (InHitDuration)
        {
            OperationLock_Hit.Locked = true;
            SkillLock_Hit.Locked = true;

            if (isGrounded && Motion == null) Resistance += Time.deltaTime;
            else Resistance = MinResisiance;
        }
        else
        {
            OperationLock_Hit.Locked=false;
            SkillLock_Hit.Locked = false;
        }

        if(Motion!=null)UpdateMotion();

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
        if (MotionVector.x < -0.01f) syncPosition.FaceRight = false;
        else if (MotionVector.x > 0.01f) syncPosition.FaceRight = true;
    }
    protected virtual void LayerUpdate()
    {
        if (Motion!=null|| MotionVector.y < -0.5f||!isGrounded) gameObject.layer = Tool.Settings.FallingTargetLayer;
        else gameObject.layer=Tool.Settings.TargetLayer;
    }
    private float jumpcd=-1;
    private void InputWalk(float x, bool jump)
    {
        if(Motion==null)rb.velocity = new Vector2(x * MoveSpeed, rb.velocity.y);
        else rb.velocity = new Vector2(x * MoveSpeed+rb.velocity.x, rb.velocity.y);
        if (jump)
        {
            if (Time.time - jumpcd < 0.2f) return;
            jumpcd = Time.time;
            if (JumpCount < target.DedicatedAttributes.Liantiao.Value)
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

        if (Motion.SpawnTime > Motion.WorkTime)
        {
            rb.velocity = Motion.Exit(rb.velocity);
            Motion = null;
        }
        else
        {
            rb.velocity = Motion.GetVelocity(rb.velocity);
        }
    }
    protected virtual void Ground()
    {
        if(Physics2D.Raycast(GroundCheck.position,Vector2.down, GroundCheckDistance,Tool.Settings.Ground)||Physics2D.Raycast(GroundCheck2.position, Vector2.down, GroundCheckDistance,Tool.Settings.Ground))
        //if (Physics2D.OverlapPoint(GroundCheck.position, Tool.Settings.Ground) || Physics2D.OverlapPoint(GroundCheck2.position, Tool.Settings.Ground))// && rb.velocity.y <= 0)
        {
            isGrounded = true;
            if (rb.velocity.y <= 0.01f) JumpCount = 0;
        }
        else
        {
            isGrounded = false;
        }
    }


    public virtual bool OnHitBack(Bullet b)
    {
        int hitbackResist = target.DedicatedAttributes.Kangjitui.Value;
        if (InHitDuration) hitbackResist = 0;
        else if (Motion != null&&Motion.ActiveAdded) hitbackResist = Mathf.Max(hitbackResist, Motion.StoicLevel);
        if (b.liftStoicLevel > hitbackResist)
        {
            if (Motion != null)
            {
                Motion.Exit(rb.velocity);
                Motion = null;
            }
            target.skillController.Interrupt();

            var v =  new Vector2((transform.position.x > b.transform.position.x) ?0.5f:-0.5f, 1);
            rb.velocity += b.hitbackForce.Invoke(b.transform.position, transform.position);

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
            int hitbackResist = target.DedicatedAttributes.Kangjitui.Value;
            if (InHitDuration) hitbackResist = 0;
            if(m.StoicLevel>=hitbackResist)Motion = m;
        }
        else if (m.StoicLevel >= Motion.StoicLevel)
        {
            Motion = m;
        }
    }
    public Vector2 GroundUnderward(float distance)
    {
        var hit = Physics2D.Raycast(GroundCheck.position, Vector2.down, distance, Tool.Settings.Ground);
        var hit2 = Physics2D.Raycast(GroundCheck2.position, Vector2.down, distance, Tool.Settings.Ground);
        float y = transform.position.y - distance;
        if (hit) y = Mathf.Max(hit.point.y, y);
        if (hit2) y = Mathf.Max(hit2.point.y, y);
        return new Vector2(transform.position.x, y);
    }
    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(GroundCheck.position,GroundCheck.position+Vector3.down*GroundCheckDistance);
        Gizmos.DrawLine(GroundCheck2.position,GroundCheck2.position+Vector3.down*GroundCheckDistance);
    }
}
