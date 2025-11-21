using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncPosition : EnsBehaviour
{
    [SerializeField] private float MinimumDistance = 0.1f;
    public Action PreUpdate;

    private TargetController controller;
    private Rigidbody2D rb;
    private Vector3 LastFramePosition;
    private float sqrDist;
    private float interval=0f;//时间间隔

    public bool FaceRight;

    private bool Initialized = false;

    public void Init(NonSkillPlayerData p)
    {
        nomEnabled = p.isLocalPlayer;
        Init();
    }
    public void Init(Target t)
    {
        nomEnabled = t.UpdateLocally;
        controller = t.controller;
        Init();
    }
    private void Init()
    {
        rb=GetComponent<Rigidbody2D>();
        LastFramePosition = transform.position;
        sqrDist = MinimumDistance * MinimumDistance;

        Initialized = true;
    }
    [EnsPriority(2)]
    public override void ManagedUpdate()
    {
        if (!Initialized) return;
        PreUpdate?.Invoke();
        if ((transform.position - LastFramePosition).sqrMagnitude > sqrDist || rb.velocity.sqrMagnitude > sqrDist)
        {
            Sync();
            interval = 0;
        }
        else if (controller!=null&&controller.InHitDuration)
        {
            Sync();
        }
        else if (interval < 0.07f)
        {
            Sync();
        }
        else if (interval > 0.5f)
        {
            Sync();
            interval = 0.1f;
        }
        interval += Time.deltaTime;
    }
    private float LastSyncTime=-1;
    private void Sync()
    {
        if (Time.time < LastSyncTime + 0.02f) return;
        else LastSyncTime = Time.time;
        string param = $"{transform.position.x:F3}_{transform.position.y:F3}_{rb.velocity.x:F3}_{rb.velocity.y:F3}";
        if (controller != null) param +=$"_{controller.Resistance:F2}_{controller.MotionVector.x:F2}_{controller.MotionVector.y:F2}_{((controller.InHitDuration||controller.Motion!=null)?0:1)}";
        CallFuncRpc(nameof(SetPosition), SendTo.ExcludeSender,param);//忽略自身
        LastFramePosition = transform.position;
    }
    public void SetPosition(string pos)
    {
        try
        {
            string[] s = pos.Split('_');
            transform.position = new Vector3(float.Parse(s[0]), float.Parse(s[1]), 0);
            rb.velocity = new Vector2(float.Parse(s[2]), float.Parse(s[3]));
            if (s.Length > 4)
            {
                controller.Resistance = float.Parse(s[4]);
                controller.MotionVector = new Vector2(float.Parse(s[5]), float.Parse(s[6]));
                if (int.Parse(s[7])==1)
                {
                    if (rb.velocity.x > 0.01f) FaceRight = true;
                    else if (rb.velocity.x < -0.01f) FaceRight = false;
                }
            }
            else
            {
                if (rb.velocity.x > 0.01f) FaceRight = true;
                else if (rb.velocity.x < -0.01f) FaceRight = false;
            }
            LastFramePosition = transform.position;
        }
        catch(Exception e)
        {
            Debug.LogError("同步位置出错:"+pos+" "+e.ToString());
        }
    }
}
