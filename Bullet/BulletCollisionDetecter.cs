using UnityEngine;

public class BulletCollisionDetecter : MonoBehaviour
{
    public float radius=2;
    [HideInInspector]public Vector3 LastFramePos;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius*transform.lossyScale.x);
    }
}