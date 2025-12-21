using AttributeSystem.Attributes;
using AttributeSystem.Effect;
using System.Collections.Generic;
using UnityEngine;

public class TargetEffectController : MonoBehaviour
{
    private Target target;
    private HashSet<(EffectType, int)> Effects = new();

    public void Init(Target t, Dictionary<string, string> param)
    {
        target = t;
        enabled = false;
    }
    private void Update()
    {
        SyncEffects();
        enabled = false;
    }
    public void AddEffect(EffectCollection effect)
    {
        if (target == null) target = GetComponent<Target>();
        effect.ApplyEffects(target);
        enabled = true;
    }
    public void EffectStart(int adder,EffectType type)
    {
        if (Effects.Contains((type, adder))) return;
        Effects.Add((type,adder));
    }
    public void EffectEnd(int adder, EffectType type)
    {
        if(!Effects.Contains((type, adder))) return;
        Effects.Remove((type, adder));
        enabled = true;
    }
    private void SyncEffects()
    {
        if (Effects.Count == 0)
        {
            target.SyncEffectIconRpc(null);
        }
        else
        {
            target.SyncEffectIconRpc(Effects);
        }
    }
}