using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Variety.Base;

public abstract class TargetController : MonoBehaviour
{
    #region//StateMachine
    private abstract class StateBase
    {
        public virtual void Enter(TargetController controller)
        {
            
        }
        public virtual void Update(TargetController controller)
        {

        }
        public virtual void Exit(TargetController controller) 
        {

        }
    }
    private class WalkIdle:StateBase
    {
        private WalkIdle() { }
        public static WalkIdle Instance=new WalkIdle();
        public override void Enter(TargetController controller)
        {
            controller.rb.gravityScale = 1;
        }
        public override void Update(TargetController c)
        {
            if (c.canFly)
            {
                c.SwitchState(FlyIdle.Instance);
                return;
            }
            if (!c.groundDetector || c.groundDetector.IsGround())
            {
                c.isGrounded = true;
                if (c.rb.velocity.y <= 0.01f) c.JumpCount = 0;
            }
            else
            {
                c.isGrounded = false;
            }
            if (!c.target.OperationLock.LockedInHierechy)
            {
                var vector = c.GetInputVector();
                c.InputWalk(vector.x, vector.y == 1);
                c.ignoreLevitatingPlatform = vector.y == -1;
            }
            else
            {
                c.ignoreLevitatingPlatform = false;
            }
        }
    }
    private class FlyIdle: StateBase
    {
        private FlyIdle() { }
        public static FlyIdle Instance=new FlyIdle();
        public override void Enter(TargetController controller)
        {
            controller.rb.gravityScale = 0;
            controller.isGrounded = false;
            controller.ignoreLevitatingPlatform= true;
        }
        public override void Update(TargetController c)
        {
            if (!c.canFly)
            {
                c.SwitchState(WalkIdle.Instance);
                return;
            }
            if (!c.target.OperationLock.LockedInHierechy)
            {
                var vector = c.GetInputVector();
                c.InputFly(vector.x, vector.y);
            }
            
        }
        public override void Exit(TargetController controller)
        {
            controller.rb.gravityScale = 1;
            controller.ignoreLevitatingPlatform = false;
        }
    }
    private class Rigor : StateBase
    {
        private Rigor() { }
        public static Rigor Instance=new Rigor();
        public override void Enter(TargetController controller)
        {
            controller.rb.gravityScale = 1;
            controller.ignoreLevitatingPlatform = false;
            controller.OperationLock.Locked = true;
            controller.SkillLock.Locked = true;
        }
        public override void Update(TargetController c)
        {
            if (!c.groundDetector || c.groundDetector.IsGround())
            {
                c.isGrounded = true;
                if (c.rb.velocity.y <= 0.01f) c.JumpCount = 0;
                c.rb.velocity=new Vector2(0,c.rb.velocity.y);
            }
            else
            {
                c.isGrounded = false;
            }
            if (c.Motion == null)
            {
                c.Resistance += Time.deltaTime;
                if (c.Resistance > 0)
                {
                    c.SwitchState(c.canFly ? FlyIdle.Instance : WalkIdle.Instance);
                }
            }
        }
        public override void Exit(TargetController controller)
        {
            controller.OperationLock.Locked = false;
            controller.SkillLock.Locked= false;
        }
    }
    private class Hit: StateBase
    {
        private Hit() { }
        public static Hit Instance=new Hit();
        public override void Enter(TargetController controller)
        {
            controller.rb.gravityScale = 1;
            controller.ignoreLevitatingPlatform = false;
            controller.OperationLock.Locked = true;
            controller.SkillLock.Locked = true;
        }
        public override void Update(TargetController c)
        {
            if (!c.groundDetector || c.groundDetector.IsGround())
            {
                c.isGrounded = true;
                if (c.rb.velocity.y <= 0.01f) c.JumpCount = 0;
                c.rb.velocity = new Vector2(0, c.rb.velocity.y);
            }
            else
            {
                c.isGrounded = false;
            }
            if (c.Motion == null)
            {
                if (c.isGrounded) c.Resistance += Time.deltaTime;
                if (c.Resistance > 0)
                {
                    c.SwitchState(c.canFly ? FlyIdle.Instance : WalkIdle.Instance);
                }
            }
        }
        public override void Exit(TargetController controller)
        {
            controller.OperationLock.Locked = false;
            controller.SkillLock.Locked= false;
        }
    }
    private class InMotion:StateBase
    {
        private InMotion() { }
        public static InMotion Instance=new InMotion();
        public override void Enter(TargetController controller)
        {
            controller.rb.gravityScale = 0;
            controller.ignoreLevitatingPlatform= true;
            controller.isGrounded = false;
            controller.OperationLock.Locked = true;
            controller.SkillLock.Locked = true;
        }
        public override void Update(TargetController c)
        {
            if (!c.MotionEntered)
            {
                c.rb.velocity = c.Motion.Entry(c.rb.velocity);
                c.MotionEntered = true;
            }
            else if (c.Motion.SpawnTime > c.Motion.WorkTime)
            {
                c.rb.velocity = c.Motion.Exit(c.rb.velocity);
                bool active = c.Motion.ActiveAdded;
                c.Motion = null;
                if (c.Resistance < 0)
                {
                    if (active) c.SwitchState(Rigor.Instance);
                    else c.SwitchState(Hit.Instance);
                }
                else if (c.canFly) c.SwitchState(FlyIdle.Instance);
                else c.SwitchState(WalkIdle.Instance);
            }
            else
            {
                c.rb.velocity = c.Motion.GetVelocity(c.rb.velocity);
            }
        }
        public override void Exit(TargetController controller)
        {
            controller.rb.gravityScale = 1;
            controller.ignoreLevitatingPlatform =false;
            controller.isGrounded = true;
            controller.OperationLock.Locked = false;
            controller.SkillLock.Locked = false;
            controller.rb.velocity = new Vector2();
        }
    }
    private void SwitchState(StateBase nextState)
    {
        if (currentState != null) currentState.Exit(this);
        currentState = nextState;
        currentState.Enter(this);
    }
    private StateBase currentState;
#endregion

    protected Target target;
    protected Rigidbody2D rb;
    private GroundDetector groundDetector=>target.graphic.groundDetector;

    protected float MoveSpeed=> Mathf.Max(target.FloatingAttributes.Jixing.Value, 0);
    protected float JumpSpeed=> Mathf.Sqrt(Mathf.Max(target.FloatingAttributes.Tengkong.Value * 20, 0));
    private int JumpCount = 1;
    protected virtual float MinResisiance => -1f;


    private float Resistance = 0.01f;
    private bool hitDown = false;
    private bool canFly;

    private bool ignoreLevitatingPlatform;
    private bool isGrounded;

    private LockChain OperationLock;
    private LockChain SkillLock;

    public MotionBase Motion;
    private bool MotionEntered = true;


    private bool Initialized = false;

    public virtual void Init(Target t, Dictionary<string, string> param)
    {
        target = t;
        rb = GetComponent<Rigidbody2D>();

        OperationLock = t.OperationLock.GetChain();
        SkillLock = t.SkillLock.GetChain();

        if (param.ContainsKey("canFly")) canFly = param["canFly"] == "1";
        if (canFly) SwitchState(FlyIdle.Instance);
        else SwitchState(WalkIdle.Instance);

        Initialized = true;
    }
    protected virtual void Update()
    {
        if (!Initialized) return;
        currentState.Update(this);
        if (target.targetControllerSync.OnPlayerPostUpdate())
        {
            target.targetControllerSync.SyncController(transform.position, rb.velocity, Resistance,
                ignoreLevitatingPlatform, OperationLock.LockedInHierechy, isGrounded, Motion == null);
        }
    }
    private float jumpcd = -1;
    private void InputWalk(int x, bool jump)
    {
        rb.velocity = new Vector2(x * MoveSpeed, rb.velocity.y);
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
    private void InputFly(int x,int y)
    {
        rb.velocity = new Vector2(x * MoveSpeed,y * MoveSpeed);
    }
    public abstract Vector2Int GetInputVector();


    public virtual bool OnHitBack(Bullet b)
    {
        int hitbackResist = target.FloatingAttributes.Kangjitui.Value;
        if (Resistance < 0) hitbackResist = 0;
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

            SetResistance(MinResisiance, true);
            return true;
        }
        else
        {
            if (Motion == null)
            {
                SetResistance(-0.2f, false);
                rb.velocity = b.hitbackForce.Invoke(b.transform.position, transform.position);
            }
        }
        
        return false;
    }
    public void SetResistance(float value, bool strike)//僵直还是击倒
    {
        Resistance = value;
        if (strike) hitDown = true;
        if (Resistance < 0&&Motion==null)
        {
            if (hitDown) SwitchState(Hit.Instance);
            else SwitchState(Rigor.Instance);
        }
    }
    public void ApplyMotion(MotionBase m)
    {
        if (m == null)
        {
            Debug.LogError("添加了空的运动行为");
            return;
        }
        MotionEntered = false;
        if (Motion == null)
        {
            int hitbackResist = target.FloatingAttributes.Kangjitui.Value;
            if (Resistance < 0) hitbackResist = 0;
            if(m.StoicLevel>=hitbackResist)Motion = m;
        }
        else if (m.StoicLevel >= Motion.StoicLevel||m.ActiveAdded)
        {
            rb.velocity=Motion.Exit(rb.velocity);
            Motion = m;
        }
        SwitchState(InMotion.Instance);
    }

    public void SetFlyAbility(bool canFly)
    {
        this.canFly = canFly;
    }
}
