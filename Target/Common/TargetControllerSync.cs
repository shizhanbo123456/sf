using AttributeSystem.Attributes;
using System;
using UnityEngine;

namespace LevelCreator.TargetTemplate
{
    /// <summary>
    /// 手动调用OnPlayerPostUpdate，第一时间同步<br></br>
    /// 必须调用SyncMotion/SyncController来同步
    /// </summary>
    public partial class TargetControllerSync : EnsBehaviour, ITargetcontrollerInfo
    {
        public Action OnPostSyncRpc { get; set; }

        [HideInInspector] public GameTimeAttributes DedicatedAttributes;

        public bool FaceRight { get; set; } = true;
        public bool isGrounded { get; set; } = true;
        public bool HitDown { get; set; } = false;
        public bool IgnoreLevitaningPlatrm { get; set; } = false;
        public bool MotionIsNull { get; set; } = true;

        private const float sqrDist = 0.01f;
        private const float sqrVelocity = 1f;
        private const float minSyncInterval = 0.03f;
        private Rigidbody2D rb;
        private Vector3 lastSyncPosition;
        private float lastSyncTime = 0f;
        private GameObject colliderGameObject;

        private void Awake()
        {
            nomEnabled = false;
            rb = GetComponent<Rigidbody2D>();
            OnPostSyncRpc += SetLayer;
            lastSyncPosition = transform.position;
        }
        public void SetLayer()
        {
            if (!colliderGameObject) colliderGameObject = GetComponentInChildren<Collider2D>().gameObject;
            if (!MotionIsNull || IgnoreLevitaningPlatrm || !isGrounded) colliderGameObject.layer = Tool.Settings.FallingTargetLayer;
            else colliderGameObject.layer = Tool.Settings.TargetLayer;
        }


        public bool OnPlayerPostUpdate()
        {
            if (Time.time - lastSyncTime < minSyncInterval) return false;
            if ((transform.position - lastSyncPosition).sqrMagnitude > sqrDist || rb.velocity.sqrMagnitude > sqrVelocity)
            {
                lastSyncTime = Time.time;
                lastSyncPosition = transform.position;
                return true;
            }
            else if (Time.time < 0.07f + lastSyncTime)
            {
                lastSyncPosition = transform.position;
                return true;
            }
            else if (Time.time > 1f + lastSyncTime)
            {
                lastSyncTime = Time.time;
                lastSyncPosition = transform.position;
                return true;
            }
            else if (Tool.SubInput.FallSignal())
            {
                lastSyncTime = Time.time;
                lastSyncPosition = transform.position;
                return true;
            }
            return false;
        }
        private enum StateFlags
        {
            IgnorLevitatingPlatform=1,
            MoveLock=2,
            IsGrounded=4,
            MotionIsNull=8,
            HitDown=16
        }
        public void SyncController(Vector3 pos, Vector2 velocity, bool hitdown, bool ignoreLevitatingPlatform, bool moveLock, bool isGrounded, bool motionIsNull)
        {
            StateFlags flags = 0;
            if(ignoreLevitatingPlatform)flags|=StateFlags.IsGrounded;
            if(moveLock)flags|=StateFlags.MoveLock;
            if (isGrounded) flags |= StateFlags.IsGrounded;
            if(motionIsNull) flags |= StateFlags.MotionIsNull;
            if(hitdown)flags|=StateFlags.HitDown;

            var sb = Tool.stringBuilder;

            //posX:0-256*8
            //posY:0-128*8
            //vx:-64-64
            //vy=-64-64
            sb.Append((int)(pos.x * 10)).Append('_').
                Append((int)(pos.y * 10)).Append('_').
                Append((int)(velocity.x * 10)).Append('_').
                Append((int)(velocity.y * 10)).Append('_').
                Append((int)flags);
            CallFuncRpc(SyncControllerRpc, SendTo.ExcludeSender, Delivery.Unreliable, sb.ToString());

            HitDown = hitdown;
            IgnoreLevitaningPlatrm = ignoreLevitatingPlatform;
            if (!moveLock)
            {
                if (rb.velocity.x > 0.01f) FaceRight = true;
                else if (rb.velocity.x < -0.01f) FaceRight = false;
            }
            this.isGrounded = isGrounded;
            OnPostSyncRpc?.Invoke();
        }
        [Rpc]
        private void SyncControllerRpc(string data)
        {
            string[] s = data.Split('_');
            transform.position = new Vector3(int.Parse(s[0])*0.1f, int.Parse(s[1])*0.1f, 0);
            rb.velocity = new Vector2(int.Parse(s[2])*0.1f, int.Parse(s[3])*0.1f);

            StateFlags flags = (StateFlags)int.Parse(s[4]);

            IgnoreLevitaningPlatrm = flags.HasFlag(StateFlags.IgnorLevitatingPlatform);
            if (flags.HasFlag(StateFlags.MoveLock))
            {
                if (rb.velocity.x > 0.01f) FaceRight = true;
                else if (rb.velocity.x < -0.01f) FaceRight = false;
            }
            isGrounded = flags.HasFlag(StateFlags.IsGrounded);
            MotionIsNull = flags.HasFlag(StateFlags.MotionIsNull);
            HitDown=flags.HasFlag(StateFlags.HitDown);

            OnPostSyncRpc?.Invoke();
        }
    }
}