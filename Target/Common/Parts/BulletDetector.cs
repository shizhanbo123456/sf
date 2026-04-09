using System.Collections.Generic;
using UnityEngine;

namespace LevelCreator.TargetTemplate
{
    /// <summary>
    /// 圆形子弹 vs 无旋转BoxCollider（膨胀后带圆角，精确碰撞）
    /// </summary>
    public class BulletDetector : MonoBehaviour
    {
        private Target target;
        private static HashSet<Bullet> Bullets = new HashSet<Bullet>();
        private BoxCollider2D _collider;

        // 世界空间下的矩形参数（自动适配负缩放）
        private Vector2 BoxCenter => (Vector2)(transform.position+Vector3.up*_collider.offset.y);
        private float HalfWidth => Mathf.Abs(_collider.size.x * transform.localScale.x) * 0.5f;
        private float HalfHeight => Mathf.Abs(_collider.size.y * transform.localScale.y) * 0.5f;

        private void OnDrawGizmos()
        {
            if (_collider == null) _collider = GetComponent<BoxCollider2D>();
            if (_collider == null) return;

            // 绘制带圆角的碰撞范围（可视化调试）
            Gizmos.color = new Color(0.5f, 1f, 0.5f, 0.3f);
            Gizmos.DrawWireCube(BoxCenter, new Vector2(HalfWidth * 2, HalfHeight * 2));
        }

        public HashSet<Bullet> DetectBullet()
        {
            // 目标组件获取
            if (target == null)
            {
                if (!TryGetComponent(out target) && !transform.parent.TryGetComponent(out target))
                {
                    Debug.LogError(gameObject.name + "未挂载目标组件");
                    return null;
                }
            }
            // 碰撞体获取
            if (_collider == null)
            {
                _collider = GetComponent<BoxCollider2D>();
                if (_collider == null) return null;
            }

            Bullets.Clear();

            foreach (var i in Bullet.Bullets)
            {
                // 同阵营跳过
                if (target.Camp == i.Key) continue;

                foreach (var bullet in i.Value.Values)
                {
                    Vector2 start = bullet.LastFramePos;
                    Vector2 end = bullet.transform.position;
                    float radius = Bullet.radius * Mathf.Abs(bullet.transform.localScale.x);

                    if (CollisionCheck(start, end, BoxCenter, HalfWidth, HalfHeight, radius))
                    {
                        Bullets.Add(bullet);
                    }
                }
            }
            return Bullets;
        }

        private bool CollisionCheck(Vector2 lineStart, Vector2 lineEnd,
        Vector2 boxCenter, float halfW, float halfH, float roundRadius)
        {
            // 1. 坐标平移：将矩形中心移到原点，简化计算
            Vector2 p0 = lineStart - boxCenter;
            Vector2 p1 = lineEnd - boxCenter;
            Vector2 boxMin = new Vector2(-halfW, -halfH);
            Vector2 boxMax = new Vector2(halfW, halfH);

            // 2. 快速投影剔除（用无圆角矩形粗略排除，提升性能）
            if (p0.x < boxMin.x - roundRadius && p1.x < boxMin.x - roundRadius) return false;
            if (p0.x > boxMax.x + roundRadius && p1.x > boxMax.x + roundRadius) return false;
            if (p0.y < boxMin.y - roundRadius && p1.y < boxMin.y - roundRadius) return false;
            if (p0.y > boxMax.y + roundRadius && p1.y > boxMax.y + roundRadius) return false;

            // 3. 圆形包围盒剔除（你要求的逻辑）
            float diagonal = Mathf.Sqrt(halfW * halfW + halfH * halfH);
            float cornerDistSq = (diagonal + roundRadius) * (diagonal + roundRadius);
            float b = PointToSegmentDistanceSquared(Vector2.zero, p0, p1);
            if (b > cornerDistSq) return false;

            // ==============================================================
            // 4. 核心：1个胶囊体替代膨胀矩形 → 自动适配 宽>高 或 高>宽
            // ==============================================================
            Vector2 axisStart, axisEnd;
            float capsuleRadius;

            if (halfH >= halfW)
            {
                // 情况1：矩形更高 → 垂直胶囊（中线垂直）
                capsuleRadius = halfW + roundRadius;
                float capsuleHalfLen = halfH - halfW;
                axisStart = new Vector2(0, -capsuleHalfLen);
                axisEnd = new Vector2(0, capsuleHalfLen);
            }
            else
            {
                // 情况2：矩形更宽（宽>高）→ 水平胶囊（中线水平）
                capsuleRadius = halfH + roundRadius;
                float capsuleHalfLen = halfW - halfH;
                axisStart = new Vector2(-capsuleHalfLen, 0);
                axisEnd = new Vector2(capsuleHalfLen, 0);
            }

            float capsuleRadiusSq = capsuleRadius * capsuleRadius;
            float distSq = SegmentToSegmentDistanceSquared(p0, p1, axisStart, axisEnd);

            // 距离 < 胶囊半径 = 碰撞
            return distSq < capsuleRadiusSq;
        }

        // 点到线段 距离平方
        private float PointToSegmentDistanceSquared(Vector2 point, Vector2 segStart, Vector2 segEnd)
        {
            Vector2 dir = segEnd - segStart;
            float lenSq = dir.sqrMagnitude;
            if (lenSq < Mathf.Epsilon) return (point - segStart).sqrMagnitude;

            float t = Vector2.Dot(point - segStart, dir) / lenSq;
            t = Mathf.Clamp01(t);
            Vector2 closest = segStart + t * dir;
            return (point - closest).sqrMagnitude;
        }

        // 线段到线段 距离平方
        private float SegmentToSegmentDistanceSquared(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
        {
            Vector2 u = a2 - a1;
            Vector2 v = b2 - b1;
            Vector2 w = a1 - b1;

            float a = Vector2.Dot(u, u);
            float b = Vector2.Dot(u, v);
            float c = Vector2.Dot(v, v);
            float d = Vector2.Dot(u, w);
            float e = Vector2.Dot(v, w);
            float denom = a * c - b * b;

            float s, t;

            if (denom < Mathf.Epsilon)
            {
                s = 0;
                t = (b * s + e) / c;
                t = Mathf.Clamp01(t);
            }
            else
            {
                s = (b * e - c * d) / denom;
                t = (a * e - b * d) / denom;
                s = Mathf.Clamp01(s);
                t = Mathf.Clamp01(t);
            }

            Vector2 closestA = a1 + s * u;
            Vector2 closestB = b1 + t * v;
            return (closestA - closestB).sqrMagnitude;
        }
    }
}
/*using System.Collections.Generic;
using UnityEngine;

namespace LevelCreator.TargetTemplate
{
    /// <summary>
    /// 可动态调整大小
    /// </summary>
    public class BulletDetector : MonoBehaviour
    {
        private Target target;
        private static HashSet<Bullet> Bullets = new HashSet<Bullet>();

        private static BoxCollider2D _collider;
        private float offsetY => _collider.offset.y;
        private float detectRadius => _collider.edgeRadius + Mathf.Max(_collider.size.x, _collider.size.y) / 2f + 0.1f;

        private void OnDrawGizmos()
        {
            if (_collider == null)
            {
                _collider = GetComponent<BoxCollider2D>();
                if (_collider == null) return;
            }
            Gizmos.color = new Color(0.5f, 1f, 0.5f, 0.7f);
            Gizmos.DrawSphere(transform.position + Vector3.up * offsetY, detectRadius * transform.localScale.x);
        }
        public HashSet<Bullet> DetectBullet()
        {
            if (target == null)
            {
                if (!TryGetComponent(out target) && !transform.parent.TryGetComponent(out target))
                {
                    Debug.LogError(gameObject.name + "未挂载目标组件");
                    return null;
                }
            }
            if (_collider == null)
            {
                _collider = GetComponent<BoxCollider2D>();
                if (_collider == null) return null;
            }
            Bullets.Clear();

            Vector3 playerPos = transform.position + Vector3.up * offsetY;
            foreach (var i in Bullet.Bullets)
            {
                if (target.Camp == i.Key) continue;
                foreach (var j in i.Value.Values)
                {
                    if (CalHit(j.transform.position, j.LastFramePos, playerPos,
                        detectRadius * Mathf.Abs(transform.localScale.x)
                        + Bullet.radius * Mathf.Abs(j.transform.localScale.x)))
                    {
                        Bullets.Add(j);
                    }
                }
            }
            return Bullets;
        }
        private static bool CalHit(Vector3 lineStart, Vector3 lineEnd, Vector3 point, float distanceThreshold)
        {
            float thresholdpos = point.x + distanceThreshold;
            if (lineStart.x > thresholdpos && lineEnd.x > thresholdpos) return false;
            thresholdpos = point.x - distanceThreshold;
            if (lineStart.x < thresholdpos && lineEnd.x < thresholdpos) return false;
            thresholdpos = point.y + distanceThreshold;
            if (lineStart.y > thresholdpos && lineEnd.y > thresholdpos) return false;
            thresholdpos = point.y - distanceThreshold;
            if (lineStart.y < thresholdpos && lineEnd.y < thresholdpos) return false;

            Vector3 lineVector = lineEnd - lineStart;
            Vector3 pointVector = point - lineStart;

            float lineLengthSquared = lineVector.sqrMagnitude;
            if (lineLengthSquared < 0.001f)
            {
                return (point - lineStart).sqrMagnitude < distanceThreshold * distanceThreshold;
            }
            float t = Vector3.Dot(pointVector, lineVector) / lineLengthSquared;
            t = Mathf.Clamp01(t);

            Vector3 closestPoint = lineStart + t * lineVector;
            float distanceSquared = (point - closestPoint).sqrMagnitude;
            return distanceSquared < distanceThreshold * distanceThreshold;
        }
    }
}*/