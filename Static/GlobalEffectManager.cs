using AttributeSystem.DataOrientedEffects;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GlobalEffectManager:MonoBehaviour
{
    public static Action EffectUpdate;
    public static HashSet<EffectId> ToRemove = new HashSet<EffectId>();
    public static Action<int> OnTargetDestroyed;

    public static bool TargetCheck(int receiver)
    {
        var t = Tool.SceneController.GetTarget(receiver);
        if (t == null || t.effectController == null) return false;
        return true;
    }
    private void Update()
    {
        EffectUpdate?.Invoke();
    }
}