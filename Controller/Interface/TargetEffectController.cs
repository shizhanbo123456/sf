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
    public void EffectEnd(int adder, EffectType type)
    {
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
            List<int> values = new List<int>();
            foreach (var i in Effects)
            {
                values.Add((int)i.Item1);
            }
            target.SyncEffectIconRpc(values);
        }
    }
}