using AttributeSystem.Attributes;
using System;
using UnityEngine;

/// <summary>
/// 手动调用OnPlayerPostUpdate，第一时间同步<br></br>
/// 必须调用SyncMotion/SyncController来同步
/// </summary>
public class TargetInfoSync:EnsBehaviour
{
    public Action OnPostSyncRpc;

    [HideInInspector] public DynamicAttributes DedicatedAttributes;
    public bool FaceRight { get; private set; } = true;
    public bool isGrounded { get; private set; } = true;
    public float Resistance { get; private set; } = 1f;
    public bool IgnoreLevitaningPlatrm { get; private set; } = false;
    public bool MotionIsNull { get; private set; } = true;

    private const float sqrDist = 0.01f;
    private const float sqrVelocity = 1f;
    private const float minSyncInterval = 0.02f;
    private Rigidbody2D rb;
    private Vector3 lastSyncPosition;
    private float lastSyncTime = 0f;

    private void Awake()
    {
        nomEnabled = false;
        rb = GetComponent<Rigidbody2D>();
        OnPostSyncRpc += OnPostSyncCommon;
        lastSyncPosition = transform.position;

        ObjectId = 9655617;
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
        else if (lastSyncTime < 0.05f+Time.time)
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
    public void SyncMotion(Vector3 pos,Vector2 velocity,bool isGrounded, bool ignoreLevitatingPlatform)
    {
        if (EnsInstance.Corr.networkMode != EnsCorrespondent.NetworkMode.None)
        {
            var sb = Tool.stringBuilder;
            sb.Clear();
            sb.Append(pos.x.ToString("F3")).Append('_').
                Append(pos.y.ToString("F3")).Append('_').
                Append(velocity.x.ToString()).Append('_').
                Append(velocity.y.ToString()).Append('_').
                Append(isGrounded ? '1' : '0');
            CallFuncRpc(nameof(SyncMotionRpc), SendTo.ExcludeSender, sb.ToString());
        }

        IgnoreLevitaningPlatrm = ignoreLevitatingPlatform;
        this.isGrounded= isGrounded;
        if (rb.velocity.x > 0.01f) FaceRight = true;
        else if (rb.velocity.x < -0.01f) FaceRight = false;

        OnPostSyncRpc?.Invoke();
    }
    public void SyncController(Vector3 pos, Vector2 velocity,float resistance,bool ignoreLevitatingPlatform,bool moveLock, bool isGrounded,bool motionIsNull)
    {
        if (EnsInstance.Corr.networkMode != EnsCorrespondent.NetworkMode.None)
        {
            var sb = Tool.stringBuilder;
            sb.Clear();
            sb.Append(pos.x.ToString("F3")).Append('_').
                Append(pos.y.ToString("F3")).Append('_').
                Append(velocity.x.ToString()).Append('_').
                Append(velocity.y.ToString()).Append('_').
                Append(resistance.ToString("F1")).Append('_').
                Append(ignoreLevitatingPlatform?1:0).Append('_').
                Append(moveLock ? 0 : 1).Append('_').
                Append(isGrounded ? '1' : '0').Append('_').
                Append(motionIsNull ? '1' : '0');
            CallFuncRpc(nameof(SyncControllerRpc), SendTo.ExcludeSender, sb.ToString());
        }

        Resistance = resistance;
        IgnoreLevitaningPlatrm = ignoreLevitatingPlatform;
        if (!moveLock)
        {
            if (rb.velocity.x > 0.01f) FaceRight = true;
            else if (rb.velocity.x < -0.01f) FaceRight = false;
        }
        this.isGrounded= isGrounded;

        OnPostSyncRpc?.Invoke();
    }
    private void SyncMotionRpc(string data)
    {
        string[] s = data.Split('_');
        transform.position = new Vector3(float.Parse(s[0]), float.Parse(s[1]), 0);
        rb.velocity = new Vector2(float.Parse(s[2]), float.Parse(s[3]));
        if (rb.velocity.x > 0.01f) FaceRight = true;
        else if (rb.velocity.x < -0.01f) FaceRight = false;
        isGrounded=int.Parse(s[4])==1;

        OnPostSyncRpc?.Invoke();
    }
    private void SyncControllerRpc(string data)
    {
        string[] s = data.Split('_');
        transform.position = new Vector3(float.Parse(s[0]), float.Parse(s[1]), 0);
        rb.velocity = new Vector2(float.Parse(s[2]), float.Parse(s[3]));
        Resistance = float.Parse(s[4]);
        IgnoreLevitaningPlatrm = int.Parse(s[5]) == 1;
        if (int.Parse(s[6]) == 1)
        {
            if (rb.velocity.x > 0.01f) FaceRight = true;
            else if (rb.velocity.x < -0.01f) FaceRight = false;
        }
        isGrounded = int.Parse(s[7]) == 1;
        MotionIsNull = int.Parse(s[8]) == 1;

        OnPostSyncRpc?.Invoke();
    }
}