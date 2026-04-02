using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletAutoRotate : MonoBehaviour
{
    private Vector3 lastPos;
    private Vector3 scale;
    private Vector3 iscale;
    private void Awake()
    {
        lastPos=transform.position;
        scale = transform.localScale;
        iscale=transform.localScale;
        iscale.x *= -1;
    }
    private void Update()
    {
        var p=transform.position-lastPos;
        if (p.x > 0.1f || p.x < 0.1f || p.y > 0.1f|| p.y < 0.1f)
        {
            lastPos = p;
            float angle = Mathf.Atan(p.y / p.x)*Mathf.Rad2Deg;
            if (p.x > 0)
            {
                transform.localScale = scale;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
            else
            {
                transform.localScale = scale;
                transform.rotation = Quaternion.Euler(0, 0, -angle);
            }
        }
    }
}
