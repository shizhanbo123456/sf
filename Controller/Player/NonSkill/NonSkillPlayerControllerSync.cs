using System;
using UnityEngine;

public class NonSkillPlayerControllerSync : EnsBehaviour,ITargetcontrollerInfo
{
    public Action OnPostSyncRpc { get; set; }

    public bool FaceRight { get; set; } = true;
    public bool isGrounded { get; set; } = true;
    public float Resistance { get; set; } = 1f;
    public bool IgnoreLevitaningPlatrm { get; set; } = false;
    public bool MotionIsNull { get; set; } = true;

    private const float sqrDist = 0.01f;
    private const float sqrVelocity = 1f;
    private const float minSyncInterval = 0.02f;
    private Rigidbody2D rb;
    private Vector3 lastSyncPosition;
    private float lastSyncTime = 0f;
    private GameObject colliderGameObject;

    private void Awake()
    {
        nomEnabled = false;
        rb = GetComponent<Rigidbody2D>();
        OnPostSyncRpc += OnPostSyncCommon;
        lastSyncPosition = transform.position;
    }
    public void OnPostSyncCommon()
    {
        if (!colliderGameObject) GetCollider();
        if (!MotionIsNull || IgnoreLevitaningPlatrm || !isGrounded) colliderGameObject.layer = Tool.Settings.FallingTargetLayer;
        else colliderGameObject.layer = Tool.Settings.TargetLayer;
    }
    private void GetCollider()
    {
        colliderGameObject = GetComponentInChildren<Collider2D>().gameObject;
    }


    public bool OnPlayerPostUpdate()
    {
        if (Time.time - lastSyncTime < minSyncInterval) return false;
        if ((transform.position - lastSyncPosition).sqrMagnitude > sqrDist || rb.velocity.sqrMagnitude > sqrVelocity)
        {
            lastSyncTime = 0;
            lastSyncPosition = transform.position;
            return true;
        }
        else if (lastSyncTime < 0.05f + Time.time)
        {
            lastSyncPosition = transform.position;
            return true;
        }
        else if (lastSyncTime > 0.5f + Time.time)
        {
            lastSyncTime = 0.1f;
            lastSyncPosition = transform.position;
            return true;
        }
        return false;
    }
    public void SyncMotion(Vector3 pos, Vector2 velocity, bool isGrounded, bool ignoreLevitatingPlatform)
    {
        var sb = Tool.stringBuilder;
        sb.Append(pos.x.ToString("F3")).Append('_').
            Append(pos.y.ToString("F3")).Append('_').
            Append(velocity.x.ToString()).Append('_').
            Append(velocity.y.ToString()).Append('_').
            Append(isGrounded ? '1' : '0');
        CallFuncRpc(nameof(SyncMotionRpc), SendTo.ExcludeSender, sb.ToString());

        IgnoreLevitaningPlatrm = ignoreLevitatingPlatform;
        this.isGrounded = isGrounded;
        if (rb.velocity.x > 0.01f) FaceRight = true;
        else if (rb.velocity.x < -0.01f) FaceRight = false;

        OnPostSyncRpc?.Invoke();
    }
    private void SyncMotionRpc(string data)
    {
        string[] s = data.Split('_');
        transform.position = new Vector3(float.Parse(s[0]), float.Parse(s[1]), 0);
        rb.velocity = new Vector2(float.Parse(s[2]), float.Parse(s[3]));
        if (rb.velocity.x > 0.01f) FaceRight = true;
        else if (rb.velocity.x < -0.01f) FaceRight = false;
        isGrounded = int.Parse(s[4]) == 1;

        OnPostSyncRpc?.Invoke();
    }
}