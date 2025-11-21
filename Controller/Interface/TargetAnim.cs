using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetAnim : MonoBehaviour
{
    private Animator anim;
    private static readonly Vector3 R = new Vector3(1, 1, 1);
    private static readonly Vector3 L = new Vector3(-1, 1, 1);

    private SyncPosition syncPosition;
    private Rigidbody2D rb;

    public SpriteRenderer MinimapIcon;
    [SerializeField] private bool SetState = true;



    [HideInInspector]public bool faceright;

    private bool Initialized = false;

    public void Init(GameObject obj,int camp)
    {
        syncPosition = obj.GetComponent<SyncPosition>();
        rb = obj.GetComponent<Rigidbody2D>();

        anim = GetComponent<Animator>();
        MinimapIcon.color = Tool.SpriteManager.TargetToColor(obj.GetComponent<Target>());
        Initialized = true;
    }
    public void Init(GameObject obj)
    {
        syncPosition = obj.GetComponent<SyncPosition>();
        rb = obj.GetComponent<Rigidbody2D>();

        anim = GetComponent<Animator>();
        Initialized = true;
    }
    private void FixedUpdate()//idle run rise fall
    {
        if (!Initialized) return;

        if (syncPosition != null)
        {
            if (syncPosition.FaceRight) { transform.localScale = R; faceright = true; }
            else { transform.localScale = L; faceright = false; }
        }
        else
        {
            if(rb.velocity.x>0.01f) { transform.localScale = R; faceright = true; }
            else if(rb.velocity.x<-0.01f) { transform.localScale = L; faceright = false; }
        }

        if (!SetState) return;
        if (Mathf.Abs(rb.velocity.y) < 0.1f)
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
