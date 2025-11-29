using System;
using System.Collections.Generic;

namespace AttributeSystem.Effect
{
    public class EffectCollection
    {
        private Target adder;
        private (Effects, float, float)[] effects;
        /// <summary>
        /// effectType,Value,Time
        /// </summary>
        public EffectCollection(Target adder,params (Effects, float,float)[] effects)
        {
            this.adder = adder;
            this.effects = effects;
        }
        public List<EffectBase> GetEffectBases(Target receiver)
        {
            List<EffectBase>effs= new List<EffectBase>();
            foreach (var effect in effects)
            {
                effs.Add(GetEffectBase(effect.Item1,effect.Item2,effect.Item3,receiver));
            }
            return effs;
        }
        private EffectBase GetEffectBase(Effects effect,float value,float time,Target receiver)
        {
            switch (effect)
            {
                case Effects.HealthRegeneration:return new HealthRegeneration(adder,receiver,(int)value,time);
                case Effects.MagicRegeneration: return new MagicRegeneration(adder, receiver, (int)value, time);
                case Effects.Burning: return new Burning(adder, receiver, (int)value, time);
                case Effects.Speed:return new Speed(adder,receiver,value,time);
                case Effects.Slowness:return new Slowness(adder,receiver,value,time);
                case Effects.JumpBoost:return new JumpBoost(adder,receiver,value,time);
                case Effects.AgileBoost:return new AgileBoost(adder,receiver,(int)value,time);
                case Effects.AccuracyBoost:return new AccuracyBoost(adder,receiver,(int)value,time);
                case Effects.AttackBoost:return new AttackBoost(adder,receiver,value,time);
                case Effects.DefenseBoost:return new DefenseBoost(adder,receiver,value,time);
                case Effects.AgileDecrease:return new AgileDecrease(adder,receiver,(int)value,time);
                case Effects.AccuracyDecrease:return new AccuracyDecrease(adder,receiver,(int)value,time);
                case Effects.AttackDecrease:return new AttackDecrease(adder,receiver,value,time);
                case Effects.DefenseDecrease:return new DefenseDecrease(adder,receiver,value,time);
                case Effects.ArmorFortity:return new ArmorFortity(adder,receiver,value,time);
                case Effects.ArmorShatter: return new ArmorShatter(adder, receiver, value, time);
                case Effects.DamageBoost: return new DamageBoost(adder, receiver, value, time);
                case Effects.DamageDecrease: return new DamageDecrease(adder, receiver, value, time);
                case Effects.LifeSteal: return new LifeSteal(adder, receiver, value, time);
                case Effects.Luck: return new Luck(adder, receiver, (int)value, time);
                case Effects.BadLuck: return new BadLuck(adder, receiver, (int)value, time);
                case Effects.Freeze: return new Freeze(adder, receiver, time);
                case Effects.Stun: return new Stun(adder, receiver, time);
                case Effects.Sticky: return new Sticky(adder, receiver, time);
                case Effects.Silence: return new Silence(adder, receiver, time);
                case Effects.Paralysis: return new Paralysis(adder, receiver, time);
                case Effects.Stoic: return new Stoic(adder, receiver,(int)value, time);
            }
            UnityEngine.Debug.LogError("有效果类型未注册");
            return null;
        }
    }
}