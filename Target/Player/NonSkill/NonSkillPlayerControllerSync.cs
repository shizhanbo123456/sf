using System;
using UnityEngine;

public partial class NonSkillPlayerControllerSync : EnsBehaviour,ITargetcontrollerInfo
{
    public Action OnPostSync { get; set; }
    public TargetTransformInfo Info { get; set; } = TargetTransformInfo.Create();

    private const float sqrDist = 0.01f;
    private const float sqrVelocity = 1f;
    private const float minSyncInterval = 0.04f;
    private Rigidbody2D rb;
    private Vector3 lastSyncPosition;
    private float lastSyncTime = 0f;
    private GameObject colliderGameObject;

    private void Awake()
    {
        nomEnabled = false;
        rb = GetComponent<Rigidbody2D>();
        OnPostSync += OnPostSyncCommon;
        lastSyncPosition = transform.position;
    }
    public void OnPostSyncCommon()
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
        else if (lastSyncTime + 0.05f > Time.time)
        {
            lastSyncPosition = transform.position;
            return true;
        }
        else if (lastSyncTime + 0.5f < Time.time)
        {
            lastSyncTime = 0.1f+Time.time;
            lastSyncPosition = transform.position;
            return true;
        }
        return false;
    }
    public void SyncMotion(Vector3 pos, Vector2 velocity, bool isGrounded, bool ignoreLevitatingPlatform)
    {
        var info = Info;
        info.isGrounded = isGrounded;
        info.ignoreLevitatingPlatform = ignoreLevitatingPlatform;
        if (rb.velocity.x > 0.01f) info.faceRight = true;
        else if (rb.velocity.x < -0.01f) info.faceRight = false;
        Info =info;

        var sb = Tool.stringBuilder;
        sb.Append(pos.x.ToString("F1")).Append('_').
            Append(pos.y.ToString("F1")).Append('_').
            Append(velocity.x.ToString()).Append('_').
            Append(velocity.y.ToString()).Append('_');
        CallFuncRpc(SyncMotionRpc, SendTo.ExcludeSender, Delivery.Unreliable,sb.ToString(),(int)Info.ToFlags());

        OnPostSync?.Invoke();
    }
    [Rpc]
    private void SyncMotionRpc(string data,int infoFlag)
    {
        string[] s = data.Split('_');
        transform.position = new Vector3(float.Parse(s[0]), float.Parse(s[1]), 0);
        rb.velocity = new Vector2(float.Parse(s[2]), float.Parse(s[3]));
        Info = new TargetTransformInfo(infoFlag);

        OnPostSync?.Invoke();
    }
}