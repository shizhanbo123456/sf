using System;
using UnityEngine;

public class WarningRect : BulletWarning
{
    private const float OriginalWidth = 5.12f;
    private const float OriginalHeight = 5.12f;

    private float maxTime;
    private float spawnTime;

    private bool initializd = false;
    public static void Warn(Vector3 start, Vector3 end, float width, float time)
    {
        var w=Instantiate(Tool.PrefabManager.BulletWarningRect, (start + end) / 2, Quaternion.identity).GetComponent<WarningRect>();
        w.Init(start, end, width, time);
    }
    public void Init(Vector3 start, Vector3 end, float width, float time)
    {
        maxTime= time;

        // 计算矩形中心位置（start和end的中点）
        Vector3 centerPos = (start + end) / 2f;
        transform.position = centerPos;

        // 计算start到end的方向向量并获取长度（作为矩形高度）
        Vector3 direction = end - start;
        float height = direction.magnitude;

        // 计算旋转角度（绕Z轴旋转，使矩形上下边对齐start和end）
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // 计算缩放比例（基于原始尺寸）
        float scaleX = width / OriginalWidth;    // 宽度缩放（对应矩形宽度）
        float scaleY = height / OriginalHeight;  // 高度缩放（对应矩形长度）
        transform.localScale = new Vector3(scaleX, scaleY, 1);

        Inner.localScale = new Vector3(0,1,1);

        initializd = true;
    }
    private void Update()
    {
        if (!initializd) return;
        spawnTime += Time.deltaTime;
        Inner.localScale=new Vector3(spawnTime/maxTime,1,1);
        if (spawnTime >= maxTime)
        {
            Destroy(gameObject);
        }
    }
}