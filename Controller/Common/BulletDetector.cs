using System.Collections.Generic;
using UnityEngine;

public class BulletDetector : MonoBehaviour
{
    [SerializeField] private float offsetY=0.5f;
    [SerializeField] private float detectRadius=1f;
    private Target target;
    private static HashSet<Bullet> Bullets = new HashSet<Bullet>();
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.5f, 1f, 0.5f, 0.7f);
        Gizmos.DrawSphere(transform.position + Vector3.up * offsetY, detectRadius);
    }
    public HashSet<Bullet> DetectBullet()
    {
        if(target == null)
        {
            if(!TryGetComponent(out target))
            {
                Debug.LogError(gameObject.name + "未挂载目标组件");
                return null;
            }
        }
        Bullets.Clear();
        
        Vector3 playerPos = transform.position + Vector3.up*offsetY;
        foreach (var i in Bullet.Bullets)
        {
            if (target.Camp!=i.Key) continue;
            foreach (var j in i.Value.Values)
            {
                float s = j.transform.localScale.x;
                if (CalHit(j.transform.position, j.LastFramePos, playerPos, detectRadius + j.radius * s)) Bullets.Add(j);
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