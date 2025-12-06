using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetector : MonoBehaviour
{
    public Vector2 Center = new Vector2(0f, -0.5f);
    public Vector2 HalfSize = new Vector2(0.3f, 0.2f);

    public virtual bool IsGround()
    {
        // 获取物体的缩放（处理可能的负缩放，取绝对值）
        Vector2 objectScale = new Vector2(
            Mathf.Abs(transform.localScale.x),
             Mathf.Abs(transform.localScale.y)
        );

        // 计算世界空间下的实际中心点（本地中心点 * 缩放 + 物体位置）
        Vector2 worldCenter = (Vector2)transform.position +
                              new Vector2(Center.x * objectScale.x, Center.y * objectScale.y);

        // 计算世界空间下的实际半尺寸（本地半尺寸 * 缩放）
        Vector2 worldHalfSize = new Vector2(
            HalfSize.x * objectScale.x,
            HalfSize.y * (objectScale.y)
        );

        // 计算检测区域的两个对角点
        Vector2 minPoint = worldCenter - worldHalfSize;
        Vector2 maxPoint = worldCenter + worldHalfSize;

        // 检测地面
        return Physics2D.OverlapArea(minPoint, maxPoint, Tool.Settings.Ground) != null;
    }

    private void OnDrawGizmos()
    {
        // Gizmos也同步适配缩放，保持可视化和实际检测区域一致
        Vector2 objectScale = new Vector2(
            Mathf.Abs(transform.localScale.x),
            Mathf.Abs(transform.localScale.y)
        );

        Vector3 gizmoCenter = transform.position +
                              new Vector3(Center.x * objectScale.x, Center.y * objectScale.y, 0f);

        Vector3 gizmoSize = new Vector3(
            2 * HalfSize.x * objectScale.x,
            2 * HalfSize.y * (objectScale.y),
            1f
        );

        Gizmos.color = new Color(0.7f, 0.7f, 1f, 0.6f);
        Gizmos.DrawCube(gizmoCenter, gizmoSize);
    }
}
