using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvalidArea : MonoBehaviour
{
    private Vector2 zx;
    private Vector2 ys;
    private BoxCollider2D c;
    private void Awake()
    {
        if (c == null) c = GetComponent<BoxCollider2D>();
        zx = new Vector2(transform.position.x, transform.position.y) + c.offset - c.size / 2;
        ys = zx + c.size;
    }
    private void FixedUpdate()
    {
        foreach (var c in Physics2D.OverlapAreaAll(zx, ys, Tool.Settings.Player))
        {
            if(c.TryGetComponent<PlayerData>(out var data))
            {
                if(data.UpdateLocally)data.Die(null);
            }
        }
    }
}
