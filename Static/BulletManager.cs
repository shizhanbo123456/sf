using System;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager:MonoBehaviour
{
    private void Awake()
    {
        Tool.BulletManager = this;
    }
    private readonly List<GameObjectPool>Pools=new List<GameObjectPool>();
    private readonly List<float>Size=new List<float>();
    public static Action SystemUpdateLoop;
    private void Start()
    {
        for(int i = 0;i< Tool.PrefabManager.BulletList.Count; i++)
        {
            Pools.Add(GameObjectPool.Create(Tool.PrefabManager.BulletList[i]));
            Size.Add(Tool.PrefabManager.BulletList[i].transform.localScale.x);
            //Pools[i].PreWarm(4);
        }
    }
    private void Update()
    {
        SystemUpdateLoop?.Invoke();
    }
    private static Bullet b;
    public Bullet GetBullet(int index)
    {
        b= Pools[index].Get().GetComponent<Bullet>();
        b.bulletType = index;
        b.transform.localScale=Vector3.one* Size[index];
        b.transform.parent = null;
        //Debug.Log("ªÒ»°£∫" + index);
        return b;
    }
    public void ReturnBullet(Bullet bullet)
    {
        Pools[bullet.bulletType].Return(bullet.gameObject);
        bullet.transform.parent = Pools[bullet.bulletType].transform;
        //Debug.Log("πÈªπ£∫" + bullet.bulletType);
    }
}