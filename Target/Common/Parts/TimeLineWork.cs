using LevelCreator.TargetTemplate;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variety.Base;

public class TimeLineWork : MonoBehaviour
{
    private Target target;
    private SortedList<uint, (TimeLineData, ITimeLineActor)> Events =new();
    private CircularQueue<uint> OrderMap = new();
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
        OrderMap.Top(out var time);
        var first = Events[time];
        if (Time.time*1000 > time)
        {
            first.Item2.Act(first.Item1); 
            Events.Remove(time);
            OrderMap.Read(out _);
        }
    }
    public void AddEvent(float delay,TimeLineData data,ITimeLineActor actor)
    {
        if (OrderMap.Full())
        {
            Debug.LogWarning("同时添加了太多事件");
            return;
        }
        uint t = (uint)((delay + Time.time) * 1000);
        while (Events.ContainsKey(t+indexOffset))
        {
            indexOffset++;
        }
        OrderMap.Write(t+indexOffset);
        Events.Add(t+indexOffset,(data,actor));
        enabled=true;
    }
    public void Interrupted()
    {
        Events.Clear();
    }
}
