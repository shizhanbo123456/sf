using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [Header("Scaler")]
    [SerializeField]private float size=10;
    [SerializeField]private Vector2 offset = Vector2.zero;
    [SerializeField] private Transform MinimapCamera;
    private static readonly float widthfactor = 2f;
    private static readonly float heightfactor = 1f;
    [Header("Common")]
    public List<Transform> SpawnPlace = new List<Transform>();
    public Transform Canvas;
    public List<Transform> Anchors = new List<Transform>();
    [Space]
    [Header("Background")]
    public Color StartColor = new Color(183f / 255, 243f / 255, 237f / 255, 1);
    public bool ColorTint;
    public Color EndColor;
    [Space]
    [Header("SpawnHandles")]
    public List<OreSpawnHandle>OreSpawnHandles = new List<OreSpawnHandle>();
    public List<LanternSpawnHandle> LanternSpawnHandles = new List<LanternSpawnHandle>();
    public List<MonsterSpawnHandle>MonsterSpawnHandles = new List<MonsterSpawnHandle>();

    protected int mode
    {
        get
        {
            return Tool.FightController.ModeList[0]-'0';
        }
    }
    protected int submode
    {
        get
        {
            return Tool.FightController.ModeList[1]-'0';
        }
    }
    protected List<int> KillCount
    {
        get
        {
            return Tool.FightController.KillCount;
        }
    }
    protected List<int> KilledCount
    {
        get
        {
            return Tool.FightController.KilledCount;
        }
    }
    protected float TimeUsed
    {
        get
        {
            return Tool.FightController.FightTimeCount;
        }
    }

    private void OnDrawGizmos()
    {
        Vector2 center = (Vector2)transform.position + offset;
        Vector2 s = new Vector2(size*widthfactor, size * heightfactor);
        Gizmos.DrawLine(new Vector3(center.x + s.x, center.y + s.y), new Vector3(center.x + s.x, center.y - s.y));
        Gizmos.DrawLine(new Vector3(center.x + s.x, center.y + s.y), new Vector3(center.x - s.x, center.y + s.y));
        Gizmos.DrawLine(new Vector3(center.x - s.x, center.y - s.y), new Vector3(center.x + s.x, center.y - s.y));
        Gizmos.DrawLine(new Vector3(center.x - s.x, center.y - s.y), new Vector3(center.x - s.x, center.y + s.y));
    }
    private void OnValidate()
    {
        if(MinimapCamera != null)
        {
            MinimapCamera.transform.position = offset;
            MinimapCamera.GetChild(0).GetComponent<Camera>().orthographicSize = size;
            MinimapCamera.GetChild(1).GetComponent<Camera>().orthographicSize = size;
        }
    }


    void Awake()
    {
        StartCoroutine(nameof(ColorByTime));
    }
    private IEnumerator ColorByTime()
    {
        while(!Tool.BackgroundController)
        {
            yield return null;
        }
        Tool.BackgroundController.UpdateColor(StartColor);

        if (!ColorTint) yield break;
        int d = 10;
        int count=0;
        while (count < 300)
        {
            count+=d;
            yield return new WaitForSeconds(d);

            Tool.BackgroundController.UpdateColor(StartColor*(1-(float)count/300)+EndColor*((float)count/300));
        }
    }
    void OnDestroy()
    {
        StopAllCoroutines();
        ClearEvents();
    }

    //在每个客户端调用
    public virtual void OnFightStart()
    {
        Tool.ScoreboardController.InitScoreboard();
        if (FightController.localPlayerId != 0) return;
        StartCoroutine(SpawnHandles());
    }
    public virtual void OnEndFight()
    {
        Tool.ScoreboardController.CloseScoreBoard();
        StopAllCoroutines();
    }
    private IEnumerator SpawnHandles()
    {
        yield return new WaitForSeconds(1);
        foreach (var i in OreSpawnHandles) i.Spawn();
        yield return new WaitForSeconds(0.5f);
        foreach (var i in LanternSpawnHandles) i.Spawn();
        yield return new WaitForSeconds(0.5f);
        foreach (var i in MonsterSpawnHandles) i.Spawn();
    }
    



    public Vector3 GetSpawnPlace()
    {
        return SpawnPlace[UnityEngine.Random.Range(0, SpawnPlace.Count)].position;
    }
    public virtual Vector3 GetSpawnPlace(Target data)
    {
        return SpawnPlace[UnityEngine.Random.Range(0,SpawnPlace.Count)].position;
    }

    public virtual bool ProcessCampData(Dictionary<int,int> data)
    {
        return true;
    }
    public virtual Func<bool> GetEndCondition()
    {
        return ()=> { return false; };
    }
    public virtual void CalculateScore(out int killscore,out int alivescore,out int score)
    {
        killscore = 0;
        alivescore = 0;
        score = 0;
    }



    private static Dictionary<string, Dictionary<int, Action<Target>>> Events = new Dictionary<string, Dictionary<int, Action<Target>>>();
    public static void RegistEvent(string key,Target t,Action<Target> action)
    {
        if (!Events.ContainsKey(key))Events.Add(key,new());
        Events[key].Add(t.ObjectId,action);
    }
    public static void TrigEvent(string key,Target t)
    {
        if (!Events.ContainsKey(key)) return;
        foreach(var i in Events.Values)
        {
            foreach (var j in i.Values) j.Invoke(t);
        }
    }
    public static void ClearEvent(Target t)
    {
        foreach(var i in Events.Values)if(i.ContainsKey(t.ObjectId))i.Remove(t.ObjectId);
    }
    public static void ClearEvents()
    {
        foreach (var i in Events.Values) i.Clear();
        Events.Clear();
    }
}
