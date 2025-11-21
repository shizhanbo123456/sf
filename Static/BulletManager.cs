using System.Collections.Generic;
using UnityEngine;

public class BulletManager:MonoBehaviour
{
    private void Awake()
    {
        Tool.BulletManager = this;
    }
    private List<GameObjectPool>Pools=new List<GameObjectPool>();
    private void Start()
    {
        for(int i = 0;i< Tool.PrefabManager.BulletList.Count; i++)
        {
            Pools.Add(GameObjectPool.Create(Tool.PrefabManager.BulletList[i]));
            Pools[i].PreWarm(10);
        }
    }
    public Bullet GetBullet(int index)
    {
        return Pools[index].Get().GetComponent<Bullet>();
    }
    public void ReturnBullet(Bullet bullet)
    {
        Pools[bullet.bulletType].Return(bullet.gameObject);
    }
}