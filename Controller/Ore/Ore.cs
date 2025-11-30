using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variety.Base;

public class Ore : Target
{
    public static SortedDictionary<int,Ore>Ores=new SortedDictionary<int,Ore>();
    public static int OreIndexNext=0;//服务器带参创建时访问
    private int OreIndex;
    public void Init(string data)
    {
        string[] s = data.Split('_', System.StringSplitOptions.RemoveEmptyEntries);
        transform.position = new Vector3(float.Parse(s[0]), float.Parse(s[1]));
        OreIndex = int.Parse(s[2]);

        var att = Tool.AttributesManager.GetDynamicAttribute(this);
        BaseAttributes = att.GetDynamicAttributes(Tool.AttributesManager.GetLevel());
        FloatingAttributes = BaseAttributes.Clone();
        GetAndInitComponents();
        RegistSyncAttributesEvent();
        InitEssential();

        OnCreated();
    }
    protected override void OnCreated()
    {
        base.OnCreated();
        Ores.Add(OreIndex, this);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        Ores.Remove(OreIndex);
    }
    private float framecount = 0;
    private void Update()
    {
        framecount += Time.deltaTime;
        if (framecount > 0.5f)
        {
            framecount = 0;
            if (FloatingAttributes.Shengming.Value == 0)
            {
                DestroyRpc();
            }
        }
    }

    public static List<Ore> OreHealthRate(out int maxEach)
    {
        maxEach = Tool.AttributesManager.GetDynamicAttribute(Ores.First().Value).
            GetDynamicAttributes(Tool.AttributesManager.GetLevel()).Shengming.Value;
        if (Ores.Count == 0) return null;
        return Ores.Values.ToList();
    }
}
