using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public enum LevelType
    {
        Home, Prepare, Luandou, Gongfang,
        PVE1, PVE2, PVE3, PVE4, PVE5, PVE6, PVE7
    }
    [Header("Scaler")]
    [SerializeField]private float size=10;
    [SerializeField]private Vector2 offset = Vector2.zero;
    [SerializeField] private Transform MinimapCamera;
    private static readonly float widthfactor = 2f;
    private static readonly float heightfactor = 1f;
    [Header("Common")]
    public Transform Canvas;
    [SerializeField]private Transform AnchorLB;
    [SerializeField]private Transform AnchorRT;
    [Space]
    [Header("Background")]
    public Color BgColor = new Color(183f / 255, 243f / 255, 237f / 255, 1);

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
    }


    void Awake()
    {
        StartCoroutine(nameof(ColorByTime));
    }
    private IEnumerator ColorByTime()
    {
        while(!Tool.BackgroundController)
        {
            yield return null;
        }
        Tool.BackgroundController.UpdateColor(BgColor);
    }

    public Vector3 GetPos(Vector2 pos)
    {
        float x = Mathf.Clamp01(pos.x);
        float y= Mathf.Clamp01(pos.y);
        return new Vector3(
            Mathf.Lerp(AnchorLB.position.x, AnchorRT.position.x, x), 
            Mathf.Lerp(AnchorLB.position.y, AnchorRT.position.y, y));
    }


    void OnDestroy()
    {
        StopAllCoroutines();
    }
}
