using System;
using UnityEngine;

public class UnnetPlayerControllerSync : MonoBehaviour,ITargetcontrollerInfo
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
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        OnPostSyncRpc += OnPostSyncCommon;
        lastSyncPosition = transform.position;
    }
    public void OnPostSyncCommon()
    {
        if (!MotionIsNull || IgnoreLevitaningPlatrm || !isGrounded) gameObject.layer = Tool.Settings.FallingTargetLayer;
        else gameObject.layer = Tool.Settings.TargetLayer;
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
    public void SyncMotion(bool isGrounded, bool ignoreLevitatingPlatform)
    {
        IgnoreLevitaningPlatrm = ignoreLevitatingPlatform;
        this.isGrounded = isGrounded;
        if (rb.velocity.x > 0.01f) FaceRight = true;
        else if (rb.velocity.x < -0.01f) FaceRight = false;

        OnPostSyncRpc?.Invoke();
    }
}