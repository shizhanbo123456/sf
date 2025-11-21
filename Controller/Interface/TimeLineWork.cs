using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variety.Base;

public class TimeLineWork : MonoBehaviour
{
    private SortedDictionary<uint, (TimeLineCancel,TimeLineData, Action<TimeLineData>)> Events = 
        new SortedDictionary<uint, (TimeLineCancel,TimeLineData, Action<TimeLineData>)>();
    private void Update()
    {
        if (Events.Count == 0)
        {
            enabled = false;
            return;
        }
        var first=Events.First();
        if (Time.time*1000 > first.Key)
        {
            first.Value.Item3.Invoke(first.Value.Item2);
            Events.Remove(first.Key);
        }
    }
    public void AddEvent(float delay,TimeLineData data,Action<TimeLineData> action,TimeLineCancel c)
    {
        uint t = (uint)((delay + Time.time) * 1000);
        Events.Add(t,(c,data,action));
        enabled=true;
    }
    public void CancelTrigged()
    {
        if (Events.Count == 0) return;
        List<uint>toremove= new List<uint>();
        foreach(var i in Events)
        {
            if(i.Value.Item1.Cancelled)toremove.Add(i.Key);
        }
        foreach(var i in toremove)
        {
            Events.Remove(i);
        }
    }
}
