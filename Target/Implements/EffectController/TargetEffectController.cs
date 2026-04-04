using AttributeSystem.Attributes;
using AttributeSystem.Effect;
using System.Collections.Generic;
using UnityEngine;

namespace LevelCreator.TargetTemplate
{
    public class TargetEffectController : MonoBehaviour
    {
        public struct EffectIdentity
        {
            private static readonly byte[] buffer = new byte[4];
            public static int Encode(EffectType type,int adderId)
            {
                buffer[0] = (byte)type;
                int idstart = 1;
                ShortSerializer.Serialize((short)adderId, buffer,ref idstart);
                idstart = 0;
                return IntSerializer.Deserialize(buffer,ref idstart,4);
            }
            public static void Decode(int code,out EffectType type,out int adderId)
            {
                int idstart = 0;
                IntSerializer.Serialize(code, buffer, ref idstart);
                type = (EffectType)buffer[0];
                idstart = 1;
                adderId = ShortSerializer.Deserialize(buffer, ref idstart, 3);
            }
        }
        private Target target;
        private HashSet<int> Effects = new();

        public void Init(Target t, Dictionary<TargetParams, string> param)
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
        public void EffectStart(int adder, EffectType type)
        {
            int id = EffectIdentity.Encode(type, adder);
            if (Effects.Contains(id)) return;
            Effects.Add(id);
        }
        public void EffectEnd(int adder, EffectType type)
        {
            int id = EffectIdentity.Encode(type, adder);
            if (!Effects.Contains(id)) return;
            Effects.Remove(id);
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
}