using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetGraphic : MonoBehaviour
{
    private static readonly Vector3 R = new Vector3(1, 1, 1);
    private static readonly Vector3 L = new Vector3(-1, 1, 1);

    private Animator anim;
    private TargetInfoSync targetInfoSync;
    private Rigidbody2D rb;

    public SpriteRenderer MinimapIcon;
    [SerializeField] private bool SetState = true;


    private bool Initialized = false;

    public void Init(GameObject obj)
    {
        targetInfoSync= obj.GetComponent<TargetInfoSync>();
        rb = obj.GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if(obj.TryGetComponent<Target>(out var t))
        {
            MinimapIcon.color = Tool.SpriteManager.TargetToColor(t);
        }
        if(obj.TryGetComponent(out targetInfoSync))
        {
            targetInfoSync.OnPostSyncRpc += OnSync;
            enabled = false;
        }
        else
        {
            Debug.LogError(gameObject.name + "未挂载同步组件");
        }
        Initialized = true;
    }
    private void OnSync()
    {
        if (!Initialized) return;

        if (targetInfoSync.FaceRight) transform.localScale = R;
        else transform.localScale = L;

        if (!SetState) return;
        if (targetInfoSync.isGrounded)
        {
            if (rb.velocity.x < 0.001f && rb.velocity.x > -0.001f)
            {
                anim.SetInteger("state", 0);
            }
            else
            {
                anim.SetInteger("state", 1);
            }
        }
        else
        {
            if (rb.velocity.y > 0.01f)
            {
                anim.SetInteger("state", 2);
            }
            else
            {
                anim.SetInteger("state", 3);
            }
        }
    }
}
