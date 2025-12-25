using AttributeSystem.DataOrientedEffects;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalEffectManager
{
    private static Action EffectUpdate;
    public static HashSet<EffectId> ToRemove = new HashSet<EffectId>();
    public static Action<int> OnTargetDestroyed;

    public static bool TargetCheck(int receiver)
    {
        var t = Tool.SceneController.GetTarget(receiver);
        if (t == null || t.effectController == null) return false;
        return true;
    }
    private static void Update()
    {
        EffectUpdate?.Invoke();
    }
    private static int loopHandleCount=0;
    public static void Regist(Action a)
    {
        EffectUpdate += a;
        if (loopHandleCount == 0) Tool.MainLoop += Update;
        loopHandleCount++;
    }
    public static void Unregist(Action a)
    {
        EffectUpdate -= a;
        loopHandleCount--;
        if (loopHandleCount == 0) Tool.MainLoop -= Update;
    }
}