using System;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool:MonoBehaviour
{
    public static GameObjectPool Create(GameObject source,Action<GameObject>onReturn=null,Action<GameObject>onGet = null)
    {
        var p=new GameObject("Pool").AddComponent<GameObjectPool>();
        p.source=source;
        p.OnGet=onGet;
        p.OnReturn=onReturn;
        return p;
    }
    private GameObject source;
    private Queue<GameObject> GameObjects = new Queue<GameObject>();

    private Action<GameObject> OnReturn;
    private Action<GameObject> OnGet;
    private void Awake()
    {
        gameObject.hideFlags= HideFlags.HideInHierarchy;
    }
    public void PreWarm(int count)
    {
        while (GameObjects.Count < count)
        {
            var s=Instantiate(source);
            OnReturn?.Invoke(s);
            GameObjects.Enqueue(s);
        }
    }
    public GameObject Get()
    {
        if (GameObjects.Count == 0) PreWarm(1);
        var obj = GameObjects.Dequeue();
        OnGet?.Invoke(obj);
        return obj;
    }
    public void Return(GameObject obj)
    {
        OnReturn?.Invoke(obj);
        GameObjects.Enqueue(obj);
    }
}