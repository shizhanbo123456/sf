using System;
using System.Collections.Generic;

namespace AttributeSystem.Effect
{
    public struct EffectCollection
    {
        private int adder;
        private (EffectType, float, float)[] effects;
        private static List<EffectBase>Effects = new List<EffectBase>();
        /// <summary>
        /// effectType,Value,Time
        /// </summary>
        public EffectCollection(Target adder,params (EffectType, float,float)[] effects)
        {
            this.adder = adder.ObjectId;
            this.effects = effects;
        }
        public List<EffectBase> GetEffectBases(Target receiver)
        {
            if(effects.Length == 0) return null;
            Effects.Clear();
            foreach (var effect in effects)
            {
                Effects.Add(GetEffectBase(effect.Item1,effect.Item2,effect.Item3,receiver));
            }
            return Effects;
        }
        private EffectBase GetEffectBase(EffectType effect,float value,float time,Target receiver)
        {
            switch (effect)
            {
                case EffectType.HealthRegeneration:return new HealthRegeneration(adder,receiver,(int)value,time);
                case EffectType.MagicRegeneration: return new MagicRegeneration(adder, receiver, (int)value, time);
                case EffectType.Burning: return new Burning(adder, receiver, (int)value, time);
                case EffectType.Speed:return new Speed(adder,receiver,value,time);
                case EffectType.Slowness:return new Slowness(adder,receiver,value,time);
                case EffectType.JumpBoost:return new JumpBoost(adder,receiver,value,time);
                case EffectType.AgileBoost:return new AgileBoost(adder,receiver,(int)value,time);
                case EffectType.AccuracyBoost:return new AccuracyBoost(adder,receiver,(int)value,time);
                case EffectType.AttackBoost:return new AttackBoost(adder,receiver,value,time);
                case EffectType.DefenseBoost:return new DefenseBoost(adder,receiver,value,time);
                case EffectType.AgileDecrease:return new AgileDecrease(adder,receiver,(int)value,time);
                case EffectType.AccuracyDecrease:return new AccuracyDecrease(adder,receiver,(int)value,time);
                case EffectType.AttackDecrease:return new AttackDecrease(adder,receiver,value,time);
                case EffectType.DefenseDecrease:return new DefenseDecrease(adder,receiver,value,time);
                case EffectType.ArmorFortity:return new ArmorFortity(adder,receiver,(int)value,time);
                case EffectType.ArmorShatter: return new ArmorShatter(adder, receiver, (int)value, time);
                case EffectType.DamageBoost: return new DamageBoost(adder, receiver, (int)value, time);
                case EffectType.DamageDecrease: return new DamageDecrease(adder, receiver, (int)value, time);
                case EffectType.LifeSteal: return new LifeSteal(adder, receiver, value, time);
                case EffectType.Luck: return new Luck(adder, receiver, (int)value, time);
                case EffectType.BadLuck: return new BadLuck(adder, receiver, (int)value, time);
                case EffectType.Freeze: return new Freeze(adder, receiver, time);
                case EffectType.Stun: return new Stun(adder, receiver, time);
                case EffectType.Sticky: return new Sticky(adder, receiver, time);
                case EffectType.Silence: return new Silence(adder, receiver, time);
                case EffectType.Paralysis: return new Paralysis(adder, receiver, time);
                case EffectType.Stoic: return new Stoic(adder, receiver,(int)value, time);
            }
            UnityEngine.Debug.LogError("有效果类型未注册");
            return null;
        }
    }
}