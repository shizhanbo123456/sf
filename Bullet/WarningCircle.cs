using System;
using UnityEngine;

public class WarningCircle:BulletWarning
{
    private const float OriginalRadius = 2.56f;

    private Transform targetTransform;

    private float maxTime;
    private float spawnTime;

    private bool initializd = false;

    public static void Warn(Vector3 center, float radius, float time)
    {
        var w=Instantiate(Tool.BulletManager.BulletWarningCircle, center, Quaternion.identity).GetComponent<WarningCircle>();
        w.Init(center, radius, time);
    }
    public static void Warn(Transform t, float radius, float time)
    {
        var w = Instantiate(Tool.BulletManager.BulletWarningCircle,t.position , Quaternion.identity).GetComponent<WarningCircle>();
        w.Init(t, radius, time);
    }
    public void Init(Vector3 center,float radius, float time)
    {
        maxTime = time;

        transform.position = center;
        transform.localScale = (radius / OriginalRadius)*Vector3.one;

        Inner.transform.localScale = Vector3.zero;

        initializd = true;
    }
    public void Init(Transform t, float radius, float time)
    {
        maxTime = time;

        transform.position = t.position;
        targetTransform= t;
        transform.localScale = (radius / OriginalRadius) * Vector3.one;

        Inner.transform.localScale = Vector3.zero;

        initializd = true;
    }
    private void Update()
    {
        if (!initializd) return;
        if (targetTransform != null) transform.position = targetTransform.position;
        spawnTime += Time.deltaTime;
        Inner.localScale = spawnTime / maxTime * Vector3.one;
        if (spawnTime >= maxTime)
        {
            Destroy(gameObject);
        }
    }
}