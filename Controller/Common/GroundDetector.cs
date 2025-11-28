using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetector : MonoBehaviour
{
    public Vector2 Center=new Vector2(0f,-0.5f);
    public Vector2 HalfSize=new Vector2(0.3f,0.2f);
    public virtual bool IsGround()
    {
        return Physics2D.OverlapArea((Vector2)transform.position+Center-HalfSize, 
            (Vector2)transform.position+Center+HalfSize, Tool.Settings.Ground)!=null;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.7f,0.7f,1f,0.6f);
        Gizmos.DrawCube(transform.position+(Vector3)Center,2*HalfSize);
    }
}
