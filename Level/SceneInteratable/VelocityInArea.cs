using LevelCreator.TargetTemplate;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityInArea : MonoBehaviour
{
    public Vector2 Velocity;
    public bool SetVx = true;
    public bool SetVy = true;
    public Vector2 GridCount = new Vector2(1, 1);
    [SerializeField] private bool UpdateSpriteSize=false;
    [SerializeField] private float SetReisience = 0.01f;

    private Vector2 zx;
    private Vector2 ys;
    private SpriteRenderer Spr;
    private BoxCollider2D c;
    private float x;
    private float y;
    private void OnValidate()
    {
        if (Spr == null)
        {
            Spr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        }
        if (c == null) c = GetComponent<BoxCollider2D>();
        c.size = GridCount;
        if(UpdateSpriteSize)Spr.size = new Vector2(GridCount.x * 2, GridCount.y * 2);
    }
    private void Start()
    {
        OnValidate();
        Spr.size = new Vector2(GridCount.x * 2, GridCount.y * 2);
        if (c==null)c = GetComponent<BoxCollider2D>();
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
        if (SetVx) x = Velocity.x;
        else x = rb.velocity.x;
        if (SetVy) y = Velocity.y;
        else y = rb.velocity.y;
        rb.velocity = new Vector2(x, y);

        if (rb.TryGetComponent<TargetController>(out var con))
        {
            con.SetResistance(SetReisience, true);
        }
    }
}
