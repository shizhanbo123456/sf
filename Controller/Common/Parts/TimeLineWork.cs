using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variety.Base;

public class TimeLineWork : MonoBehaviour
{
    private Target target;
    private SortedList<uint, (TimeLineData, Action<TimeLineData>)> Events =
        new SortedList<uint, (TimeLineData, Action<TimeLineData>)>();
    private uint indexOffset = 0;
    private void Awake()
    {
        target = GetComponent<Target>();
    }
    private void Update()
    {
        if (Events.Count == 0)
        {
            enabled = false;
            return;
        }
        indexOffset = 0;
        BulletSystemCommon.CurrentShooter = target;
        var first=Events.First();
        if (Time.time*1000 > first.Key)
        {
            first.Value.Item2.Invoke(first.Value.Item1); 
            Events.Remove(first.Key);
        }
    }
    public void AddEvent(float delay,TimeLineData data,Action<TimeLineData> action)
    {
        uint t = (uint)((delay + Time.time) * 1000);
        while (Events.ContainsKey(t+indexOffset))
        {
            indexOffset++;
        }
        Events.Add(t+indexOffset,(data,action));
        enabled=true;
    }
    public void Interrupted()
    {
        Events.Clear();
    }
}
