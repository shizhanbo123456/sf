using System;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager:MonoBehaviour
{
    public List<GameObject> BulletList;
    public List<GameObject> HitImpactList;
    private void Awake()
    {
        Tool.BulletManager = this;
    }
    private readonly List<GameObjectPool>HitImpactPools=new List<GameObjectPool>();
    private readonly List<GameObjectPool>BulletPools=new List<GameObjectPool>();

    public static Action SystemUpdateLoop;
    private void Start()
    {
        for(int i = 0;i< BulletList.Count; i++)
        {
            BulletPools.Add(GameObjectPool.Create(BulletList[i]));
        }
        for(int i = 0; i< HitImpactList.Count; i++)
        {
            HitImpactPools.Add(GameObjectPool.Create(HitImpactList[i]));
        }
    }
    private void Update()
    {
        SystemUpdateLoop?.Invoke();
    }
    private static Bullet b;
    public Bullet GetBullet(int index)
    {
        b= BulletPools[index].Get().GetComponent<Bullet>();
        b.bulletType = index;
        b.transform.localScale=Vector3.one;
        b.transform.parent = null;
        //Debug.Log("获取：" + index);
        return b;
    }
    public void ReturnBullet(Bullet bullet)
    {
        BulletPools[bullet.bulletType].Return(bullet.gameObject);
        bullet.transform.parent = BulletPools[bullet.bulletType].transform;
        //Debug.Log("归还：" + bullet.bulletType);
    }
    private static GameObject o;
    public GameObject GetHitImpact(int index)
    {
        o = BulletPools[index].Get();
        o.transform.localScale = Vector3.one;
        o.name = index.ToString();
        o.transform.parent = null;
        //Debug.Log("获取：" + index);
        return o;
    }
    public void ReturnHitImpact(GameObject obj)
    {
        int index = obj.name switch
        {
            "0" => 0,
            "1" => 1,
            "2" => 2,
            _ => 0,
        };
        BulletPools[index].Return(obj);
        obj.transform.parent = BulletPools[index].transform;
        //Debug.Log("归还：" + bullet.bulletType);
    }
}