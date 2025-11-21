using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnnetPlayerController : MonoBehaviour
{
    private UnnetPlayerData playerData;

    public bool FaceRight;


    public Transform GroundCheck;
    public Transform GroundCheck2;




    private Rigidbody2D rb;
    public float MoveSpeed
    {
        get { return playerData.Jixing; }
    }
    public float JumpSpeed
    {
        get { return Mathf.Sqrt(Mathf.Max(playerData.Tengkong * 20, 0)); }
    }
    [HideInInspector] public int JumpCount = 1;



    public void Init(UnnetPlayerData data)
    {
        playerData = data;
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
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
            FaceRight = false;
        }
        else if (x==1)
        {
            FaceRight = true;
        }
    }
    private void Ground()
    {
        if (Physics2D.OverlapPoint(GroundCheck.position, Tool.Settings.Ground) || Physics2D.OverlapPoint(GroundCheck2.position, Tool.Settings.Ground))// && rb.velocity.y <= 0)
        {
            if (rb.velocity.y <= 0.01f) JumpCount = 0;
        }
    }
}
