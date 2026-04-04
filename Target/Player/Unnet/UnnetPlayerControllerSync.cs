using System;
using UnityEngine;

public class UnnetPlayerControllerSync : MonoBehaviour,ITargetcontrollerInfo
{
    public Action OnPostSync { get; set; }
    public TargetTransformInfo Info { get; set; } = TargetTransformInfo.Create();

    private const float sqrDist = 0.01f;
    private const float sqrVelocity = 1f;
    private const float minSyncInterval = 0.02f;
    private Rigidbody2D rb;
    private Vector3 lastSyncPosition;
    private float lastSyncTime = 0f;
    private GameObject colliderGameObject;

    private void Awake()
    {
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
        var info = Info;
        info.isGrounded = isGrounded;
        info.ignoreLevitatingPlatform = ignoreLevitatingPlatform;
        if (rb.velocity.x > 0.01f) info.faceRight = true;
        else if (rb.velocity.x < -0.01f) info.faceRight = false;
        Info = info;

        OnPostSync?.Invoke();
    }
}