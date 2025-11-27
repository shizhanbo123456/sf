using AttributeSystem.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Variety.Base;

public class Lantern : Target
{
    public static SortedDictionary<int, Lantern> Lanterns = new SortedDictionary<int, Lantern>();
    public static int LanternIndexNext = 0;//服务器带参创建时访问
    public int LanternIndex;

    public SpriteRenderer Render;
    public Sprite On;
    public Sprite Off;

    public float TimeOfDie;//生命为0的时间
    public float RegenerationTime;

    public void Init(string data)
    {
        string[] s = data.Split('_',System.StringSplitOptions.RemoveEmptyEntries);
        transform.position = new Vector3(float.Parse(s[0]), float.Parse(s[1]));
        LanternIndex = int.Parse(s[2]);

        var att = Tool.AttributesManager.GetDynamicAttribute(this) as LanternAttributes;
        BaseAttributes = att.GetDynamicAttributes(Tool.AttributesManager.GetLevel());
        FloatingAttributes = BaseAttributes.Clone();
        RegenerationTime = att.RegenerationTime.GetValue(Tool.AttributesManager.GetLevel());
        GetAndInitComponents();
        RegistSyncAttributesEvent();
        InitEssential();

        OnCreated();
    }
    protected override void OnCreated()
    {
        base.OnCreated();
        Lanterns.Add(LanternIndex, this);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        Lanterns.Remove(LanternIndex);
    }
    private void Update()
    {
        if (DedicatedAttributes.Shengming.Value == 0)
        {
            Render.sprite = Off;
            TimeOfDie += Time.deltaTime;
        }
        else
        {
            Render.sprite = On;
            TimeOfDie = 0;
        }

        if (UpdateLocally && TimeOfDie > RegenerationTime)
        {
            Shengming = 999999999;
        }
    }
    protected override HashSet<Bullet> DetectBullet()
    {
        if(TimeOfDie<0.01f)return base.DetectBullet();
        return new HashSet<Bullet>();
    }
}
