using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class GroundDetector : MonoBehaviour
{
    public const float LevitatingPlatformThickness = 0.2f;
    // 检测高度的偏移量（从底边向上0.1f）
    public const float groundCheckHeight = 0.1f;

    private static readonly Color gizmoColor = new Color(0.7f, 0.7f, 1f, 0.6f);

    private BoxCollider2D _collider;
    private BoxCollider2D boxCollider
    {
        get
        {
            if(_collider == null)_collider= GetComponent<BoxCollider2D>();
            return _collider;
        }
    }
    private Rigidbody2D _rb;
    private Vector2 leftBottom;
    private Vector2 rightTop;
    private void Start()
    {
        _rb=GetComponentInParent<Rigidbody2D>();
        if (_rb == null) Debug.LogError(transform.parent.name+"未找到刚体");

        if (boxCollider == null)
        {
            Debug.LogError("未找到碰撞器");
            return;
        }
        if (Mathf.Abs(boxCollider.offset.x) > 0.01f) Debug.LogError(gameObject.name + "的碰撞器发送了x向偏移");
        FigurePos();
    }
    private void FigurePos()
    {
        float x = boxCollider.size.x / 2f ;
        float y = boxCollider.offset.y - boxCollider.size.y / 2f;
        leftBottom = new Vector2(-x * transform.localScale.x -boxCollider.edgeRadius+0.01f,
            y * transform.localScale.y - boxCollider.edgeRadius - LevitatingPlatformThickness);
        rightTop = new Vector2(-leftBottom.x, leftBottom.y + groundCheckHeight);
    }
    public virtual bool IsGround()
    {
        if (boxCollider == null) return false;
        if(_rb.velocity.y>0.1f)return false;
        Vector2 pos = transform.position;
        Collider2D hitCollider = Physics2D.OverlapArea(
            leftBottom+pos, rightTop+pos,
            Tool.Settings.Ground
        );

        return hitCollider != null;
    }

    private void OnDrawGizmos()
    {
        if (boxCollider == null) return;
        FigurePos();

        // 在Scene视图中绘制检测区域的Gizmos
        Gizmos.color = gizmoColor;
        Vector2 pos = transform.position;
        Gizmos.DrawCube((leftBottom+rightTop)/2f+pos,rightTop-leftBottom);
    }
}