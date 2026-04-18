using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletAutoRotate : MonoBehaviour
{
    private Vector3 lastPos;
    private void Update()
    {
        var p=transform.position-lastPos;
        if (p.x > 0.1f || p.x < -0.1f || p.y > 0.1f|| p.y < -0.1f)
        {
            lastPos = transform.position;
            float angle = Mathf.Atan(p.y / p.x)*Mathf.Rad2Deg;
            if (p.x > 0)
            {
                var s = transform.localScale;
                s.x = Mathf.Abs(s.x);
                transform.localScale = s;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
            else
            {
                var s = transform.localScale;
                s.x = -Mathf.Abs(s.x);
                transform.localScale = s;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
    }
}
