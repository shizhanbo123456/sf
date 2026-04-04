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
        public Action OnPostSync { get; set; }

        [HideInInspector] public GameTimeAttributes DedicatedAttributes;
        public TargetTransformInfo Info { get; set; } = TargetTransformInfo.Create();

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
            OnPostSync += SetLayer;
            lastSyncPosition = transform.position;
        }
        public void SetLayer()
        {
            if (!colliderGameObject) colliderGameObject = GetComponentInChildren<Collider2D>().gameObject;
            if (!Info.motionIsNull || Info.ignoreLevitatingPlatform || !Info.isGrounded)
                colliderGameObject.layer = Tool.Settings.FallingTargetLayer;
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
        
        public void SyncController(Vector3 pos, Vector2 velocity, bool hitdown, bool ignoreLevitatingPlatform, bool isGrounded, bool motionIsNull)
        {
            var info = Info;
            info.isGrounded = isGrounded;
            info.hitDown= hitdown;
            info.ignoreLevitatingPlatform = ignoreLevitatingPlatform;
            info.motionIsNull= motionIsNull;
            if (!hitdown)
            {
                if (rb.velocity.x > 0.01f) info.faceRight = true;
                else if (rb.velocity.x < -0.01f) info.faceRight = false;
            }
            Info = info;

            var sb = Tool.stringBuilder;

            //posX:0-256*8
            //posY:0-128*8
            //vx:-64-64
            //vy=-64-64
            sb.Append((int)(pos.x * 10)).Append('_').
                Append((int)(pos.y * 10)).Append('_').
                Append((int)(velocity.x * 10)).Append('_').
                Append((int)(velocity.y * 10)).Append('_');
            CallFuncRpc(SyncControllerRpc, SendTo.ExcludeSender, Delivery.Unreliable, sb.ToString(),(int)Info.ToFlags());

            OnPostSync?.Invoke();
        }
        [Rpc]
        private void SyncControllerRpc(string data,int flags)
        {
            string[] s = data.Split('_');
            transform.position = new Vector3(int.Parse(s[0])*0.1f, int.Parse(s[1])*0.1f, 0);
            rb.velocity = new Vector2(int.Parse(s[2])*0.1f, int.Parse(s[3])*0.1f);

            Info = new TargetTransformInfo(flags);

            OnPostSync?.Invoke();
        }
    }
}