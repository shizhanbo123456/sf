using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(BulletDetector))]
[RequireComponent(typeof(GroundDetector))]
public class TargetGraphic : MonoBehaviour
{
    public const string NullName = "null";

    public float SpawnOffset
    {
        get
        {
            if(boxCollider==null)boxCollider = GetComponent<BoxCollider2D>();
            return boxCollider.edgeRadius - (boxCollider.offset.y - boxCollider.size.y / 2f) * transform.localScale.y;
        }
    }
    private static GameObjectPool pool;

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
    private BoxCollider2D boxCollider;

    [HideInInspector]public TargetHeader header;

    private bool Initialized = false;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(Vector3.down * SpawnOffset + transform.position, 0.1f);
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
        }
        else
        {
            Debug.LogError(gameObject.name + "未挂载同步组件");
        }
        InitHeader();
        Initialized = true;
    }
    private float headerOffset;
    private void InitHeader()
    {
        if (header) return;
        if (pool == null) pool = GameObjectPool.Create(Tool.PrefabManager.TargetHeader.gameObject, o => o.SetActive(false), o => o.SetActive(true));
        header = pool.Get().GetComponent<TargetHeader>();
        header.transform.SetParent(Tool.SceneController.Level.Canvas);
        headerOffset = SpawnOffset+1;
    }
    private void Update()
    {
        if(header)header.transform.position=transform.position+Vector3.up*headerOffset;
    }
    public void SetName(string text,Color color=default)
    {
        if (!header) InitHeader();
        if (text == NullName)
        {
            header.SetNameActive(false);
            return;
        }
        else
        {
            header.SetNameActive(true);
            header.SetNameText(text);
            header.SetNameColor(color);
        }
    }
    public void SetBarActive(bool active)
    {
        header.gameObject.SetActive(active);
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
    private void OnDestroy()
    {
        if (header)
        {
            pool.Return(header.gameObject);
            header = null;
        }
    }
}
