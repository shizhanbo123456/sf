using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [Header("Scaler")]
    [SerializeField]private float size=10;
    [SerializeField]private Vector2 offset = Vector2.zero;
    [SerializeField] private Transform MinimapCamera;
    private static readonly float widthfactor = 2f;
    private static readonly float heightfactor = 1f;
    [Header("Common")]
    public Transform Canvas;
    [Space]
    [Header("Background")]
    [SerializeField] private SpriteRenderer BgSprite;

    private void OnDrawGizmos()
    {
        Vector2 center = (Vector2)transform.position + offset;
        Vector2 s = new Vector2(size*widthfactor, size * heightfactor);
        Gizmos.DrawLine(new Vector3(center.x + s.x, center.y + s.y), new Vector3(center.x + s.x, center.y - s.y));
        Gizmos.DrawLine(new Vector3(center.x + s.x, center.y + s.y), new Vector3(center.x - s.x, center.y + s.y));
        Gizmos.DrawLine(new Vector3(center.x - s.x, center.y - s.y), new Vector3(center.x + s.x, center.y - s.y));
        Gizmos.DrawLine(new Vector3(center.x - s.x, center.y - s.y), new Vector3(center.x - s.x, center.y + s.y));
    }
    private void OnValidate()
    {
        if(MinimapCamera != null)
        {
            MinimapCamera.transform.position = offset;
            MinimapCamera.GetChild(0).GetComponent<Camera>().orthographicSize = size;
            MinimapCamera.GetChild(1).GetComponent<Camera>().orthographicSize = size;
        }
        if (BgSprite != null)
        {
            BgSprite.transform.position = offset;
            float s = size / 9.5f * 1.4f;
            BgSprite.transform.localScale=new Vector3(s, s, 1);
        }
    }

    public Vector3 GetPos(float x,float y)
    {
        Vector2 center = (Vector2)transform.position + offset;
        Vector2 s = new Vector2(size * widthfactor, size * heightfactor);
        Vector3 lb = center - s;
        s *= 2;
        x = Mathf.Clamp01(x);
        y= Mathf.Clamp01(y);
        return new Vector3(x*s.x,y*s.y) + lb;
    }
    

    void OnDestroy()
    {
        StopAllCoroutines();
    }
}
