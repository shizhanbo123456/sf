using LevelCreator.TargetTemplate;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityInArea : MonoBehaviour
{
    private Vector2 Velocity;
    private Vector2 GridCount = new Vector2(1, 1);

    private Vector2 zx;
    private Vector2 ys;
    private SpriteRenderer Spr;
    private BoxCollider2D c;
    private float x;
    private float y;

    public void SetInfo(Vector2 size,Vector2 velocity)
    {
        if (Spr == null)
        {
            Spr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        }
        if (c == null) c = GetComponent<BoxCollider2D>();

        GridCount=size;
        Velocity = velocity;

        c.size = GridCount;
        Spr.size = new Vector2(GridCount.x * 2, GridCount.y * 2);
        zx = new Vector2(transform.position.x, transform.position.y) + c.offset - c.size / 2;
        ys = zx + c.size;
    }
    private void FixedUpdate()
    {
        foreach (var c in Physics2D.OverlapAreaAll(zx, ys,Tool.Settings.TargetLayer))
        {
            if(c.TryGetComponent<Rigidbody2D>(out var rb))
            {
                OnDetected(rb);
            }
        }
        foreach (var c in Physics2D.OverlapAreaAll(zx, ys, Tool.Settings.FallingTargetLayer))
        {
            if (c.TryGetComponent<Rigidbody2D>(out var rb))
            {
                OnDetected(rb);
            }
        }
    }
    private void OnDetected(Rigidbody2D rb)
    {
        rb.velocity = Velocity;

        if (rb.TryGetComponent<TargetController>(out var con))
        {
            if (Mathf.Abs(Velocity.x) > 1f)
            {
                con.SetResistance(-1f, true);
            }
            else
            {
                con.SetResistance(0.01f, true);
            }
        }
    }
}
