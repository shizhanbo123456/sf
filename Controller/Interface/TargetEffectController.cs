using AttributeSystem.Attributes;
using AttributeSystem.Effect;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetEffectController : MonoBehaviour
{
    private Target target;
    private Dictionary<int,EffectBase> Effect=new Dictionary<int, EffectBase>();
    private List<int>ToRemoveKeys= new List<int>();

    protected DynamicAttributes BaseAttributes;
    protected DynamicAttributes FloatingAttributes;

    private bool Dirty = false;


    public void Init(Target t,DynamicAttributes b,DynamicAttributes f)
    {
        target = t;
        BaseAttributes = b;
        FloatingAttributes = f;
    }
    private void Update()
    {
        foreach (var i in Effect.Keys)
        {
            Effect[i].Update();
            if (Effect[i].End.End) ToRemoveKeys.Add(i);
        }
        if (ToRemoveKeys.Count > 0)
        {
            foreach (var i in ToRemoveKeys)
            {
                Effect[i].OnExit();
                Effect.Remove(i);
            }
            ToRemoveKeys.Clear();
            Dirty = true;
        }
        if (Dirty)
        {
            SyncEffects();
            Dirty = false;
        }
    }
    public void AddEffect(EffectBase effect)
    {
        int hash = effect.GetCustomHash();
        if (Effect.ContainsKey(hash))
        {
            Effect[hash].OnExit();
            Effect[hash]=effect;
            Effect[hash].OnEntry();
        }
        else
        {
            Effect.Add(hash, effect);
            Effect[hash].OnEntry();
        }
        Dirty = true;
    }
    private void SyncEffects()
    {
        if (Effect.Count == 0)
        {
            target.SyncEffectIconRpc(null);
        }
        else
        {
            List<int> values = new List<int>();
            foreach (var i in Effect.Values)
            {
                values.Add((int)i.GetEffectType());
            }
            target.SyncEffectIconRpc(values);
        }
    }
    public DynamicAttributes GetBaseAttributes()
    {
        return BaseAttributes;
    }
    public DynamicAttributes GetFloatingAttributes()
    {
        return FloatingAttributes;
    }
}