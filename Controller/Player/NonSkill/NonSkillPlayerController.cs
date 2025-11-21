using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonSkillPlayerController : MonoBehaviour
{
    private NonSkillPlayerData playerData;

    private SyncPosition syncPosition;
    public bool FaceRight
    {
        get { return syncPosition.FaceRight; }
    }

    public Transform GroundCheck;
    public Transform GroundCheck2;




    private Rigidbody2D rb => GetComponent<Rigidbody2D>();
    public float MoveSpeed
    {
        get { return playerData.Jixing; }
    }
    public float JumpSpeed
    {
        get { return Mathf.Sqrt(Mathf.Max(playerData.Tengkong * 20, 0)); }
    }
    [HideInInspector] public int JumpCount = 1;


    private int id;
    private bool isLocalPlayer = false;
    private bool Initialized = false;

    public void Init(int id, NonSkillPlayerData data)
    {
        this.id = id;
        isLocalPlayer = id == FightController.localPlayerId;

        playerData = data;
        syncPosition = GetComponent<SyncPosition>();
        if (!isLocalPlayer)
        {
            rb.gravityScale = 0;
            enabled = false;
        }
        else GetComponent<SyncPosition>().PreUpdate += _Update;

        Initialized = true;
    }
    void _Update()
    {
        if (!Initialized) return;
        //此脚本如果不是本地会disable

        Ground();
        Motion_Update();
    }
    private void Motion_Update()
    {
        int x = Tool.SubInput.HorizontalInput();
        rb.velocity = new Vector2(x * MoveSpeed, rb.velocity.y);
        if (Tool.SubInput.JumpSignal())
        {
            if (JumpCount < playerData.Liantiao)
            {
                rb.velocity = new Vector2(rb.velocity.x, JumpSpeed);
                JumpCount += 1;
            }
        }
        if (x==-1)
        {
            syncPosition.FaceRight = false;
        }
        else if (x ==1)
        {
            syncPosition.FaceRight = true;
        }
    }
    private void Ground()
    {
        if (Physics2D.OverlapPoint(GroundCheck.position, Tool.Settings.Ground)|| Physics2D.OverlapPoint(GroundCheck2.position, Tool.Settings.Ground))// && rb.velocity.y <= 0)
        {
            if (rb.velocity.y <= 0.01f) JumpCount = 0;
        }
    }
}
