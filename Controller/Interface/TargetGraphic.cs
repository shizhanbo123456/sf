using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BulletDetector))]
[RequireComponent(typeof(GroundDetector))]
public class TargetGraphic : MonoBehaviour
{
    public const string NullName = "null";

    public float SpawnOffset = 1;
    [SerializeField] private bool UseAnim = true;
    [SerializeField] private bool SetState = true;
    private static readonly Vector3 R = new Vector3(1, 1, 1);
    private static readonly Vector3 L = new Vector3(-1, 1, 1);

    private Animator anim;
    private ITargetcontrollerInfo Icontroller;
    private Rigidbody2D rb;

    public SpriteRenderer MinimapIcon;

    [HideInInspector]public BulletDetector bulletDetector;
    [HideInInspector]public GroundDetector groundDetector;
    [Space]
    public TargetBar targetBar;
    public TargetName targetName;

    private bool Initialized = false;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position - Vector3.up * SpawnOffset*transform.localScale.y, 0.1f);
    }
    public void Init(GameObject obj)
    {
        if (!UseAnim) return;
        rb = obj.GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        bulletDetector = GetComponent<BulletDetector>();
        groundDetector= GetComponent<GroundDetector>();

        if(obj.TryGetComponent<Target>(out var t))
        {
            MinimapIcon.color = Tool.SpriteManager.TargetToColor(t);
        }
        if(obj.TryGetComponent(out Icontroller))
        {
            Icontroller.OnPostSyncRpc += OnSync;
            enabled = false;
        }
        else
        {
            Debug.LogError(gameObject.name + "未挂载同步组件");
        }
        Initialized = true;
    }
    public void SetName(string text,Color color=default)
    {
        if (text == NullName)
        {
            targetName.gameObject.SetActive(false);
            return;
        }
        else
        {
            targetName.gameObject.SetActive(true);
        }
        targetName.text = text;
        targetName.color = color;
    }
    public void SetBarActive(bool active)
    {
        targetBar.gameObject.SetActive(active);
    }
    private void OnSync()
    {
        if (!Initialized) return;

        if (Icontroller.FaceRight) transform.localScale = R;
        else transform.localScale = L;

        if (!SetState) return;
        if (Icontroller.isGrounded)
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
