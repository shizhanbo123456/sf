using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnnetPlayerController : MonoBehaviour
{
    private UnnetPlayerData playerData;
    private UnnetPlayerControllerSync targetInfoSync;
    private Rigidbody2D rb;
    private GroundDetector groundDetector => playerData.Anim.groundDetector;

    private bool isGrounded;


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

        if (!TryGetComponent(out targetInfoSync))
        {
            targetInfoSync = gameObject.AddComponent<UnnetPlayerControllerSync>();
        }
    }
    void Update()
    {
        Ground();
        Motion_Update();

        if (targetInfoSync.OnPlayerPostUpdate())
        {
            targetInfoSync.SyncMotion(isGrounded,Tool.SubInput.FallSignal());
        }
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
    }
    private void Ground()
    {
        if (!groundDetector||groundDetector.IsGround())// && rb.velocity.y <= 0)
        {
            isGrounded = true;
            if (rb.velocity.y <= 0.01f) JumpCount = 0;
        }
        else
        {
            isGrounded=false;
        }
    }
}
