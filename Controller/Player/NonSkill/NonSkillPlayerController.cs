using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonSkillPlayerController : MonoBehaviour
{
    private NonSkillPlayerData playerData;
    private NonSkillPlayerControllerSync targetInfoSync;
    private GroundDetector groundDetector => playerData.Anim.groundDetector;

    private bool isGrounded;

    
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


    private bool Initialized = false;

    public void Init(int id, NonSkillPlayerData data)
    {
        playerData = data; 
        if (!TryGetComponent(out targetInfoSync))
        {
            Debug.LogError("未找到信息同步");
        }
        Initialized = true;
    }
    void Update()
    {
        if (!Initialized) return;

        Ground();
        Motion_Update();

        if (targetInfoSync.OnPlayerPostUpdate())
        {
            targetInfoSync.SyncMotion(transform.position, rb.velocity, isGrounded,Tool.SubInput.FallSignal());
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
            isGrounded = false;
        }
    }
}
