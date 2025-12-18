using AttributeSystem.DataOrientedEffects;
using System;
using System.Collections.Generic;

namespace AttributeSystem.Effect
{
    public struct EffectCollection
    {
        private int adder;
        private (EffectType, float, float)[] effects;
        /// <summary>
        /// effectType,Value,Time
        /// </summary>
        public EffectCollection(Target adder,params (EffectType, float,float)[] effects)
        {
            this.adder = adder.ObjectId;
            this.effects = effects;
        }
        public void ApplyEffects(Target receiver)
        {
            if (effects == null) return;
            if(effects.Length == 0) return;
            foreach(var i in effects)
            {
                AddEffect(i.Item1,i.Item2,i.Item3,receiver);
            }
        }
        public bool IsEmpty()
        {
            return effects==null || effects.Length == 0;
        }
        private void AddEffect(EffectType effect,float value,float time,Target receiver)
        {
            int receiverid = receiver.ObjectId;
            switch (effect)
            {
                case EffectType.HealthRegeneration:HealthRegeneration.AddEffect(adder,receiverid,(int)value,time);break;
                case EffectType.Burning: Burning.AddEffect(adder, receiverid, (int)value, time); break;
                case EffectType.Speed:Speed.AddEffect(adder,receiverid,value,time); break;
                case EffectType.Slowness:Slowness.AddEffect(adder,receiverid,value,time); break;
                case EffectType.JumpBoost:JumpBoost.AddEffect(adder,receiverid,value,time); break;
                case EffectType.AgileBoost:AgileBoost.AddEffect(adder,receiverid,value,time); break;
                case EffectType.AccuracyBoost:AccuracyBoost.AddEffect(adder,receiverid,value,time); break;
                case EffectType.AttackBoost:AttackBoost.AddEffect(adder,receiverid,value,time); break;
                case EffectType.DefenseBoost:DefenseBoost.AddEffect(adder,receiverid,value,time); break;
                case EffectType.AgileDecrease:AgileDecrease.AddEffect(adder,receiverid,value,time); break;
                case EffectType.AccuracyDecrease:AccuracyDecrease.AddEffect(adder,receiverid,value,time); break;
                case EffectType.AttackDecrease:AttackDecrease.AddEffect(adder,receiverid,value,time); break;
                case EffectType.DefenseDecrease:DefenseDecrease.AddEffect(adder,receiverid,value,time); break;
                case EffectType.ArmorFortity:ArmorFortity.AddEffect(adder,receiverid,(int)value,time); break;
                case EffectType.ArmorShatter:ArmorShatter.AddEffect(adder, receiverid, (int)value, time); break;
                case EffectType.DamageBoost:DamageBoost.AddEffect(adder, receiverid, (int)value, time); break;
                case EffectType.DamageDecrease:DamageDecrease.AddEffect(adder, receiverid, (int)value, time); break;
                case EffectType.LifeSteal:LifeSteal.AddEffect(adder, receiverid, value, time); break;
                case EffectType.Luck:Luck.AddEffect(adder, receiverid, value, time); break;
                case EffectType.BadLuck:BadLuck.AddEffect(adder, receiverid, value, time); break;
                case EffectType.Freeze:Freeze.AddEffect(adder, receiverid, time); break;
                case EffectType.Stun:Stun.AddEffect(adder, receiverid, time); break;
                case EffectType.Sticky:Sticky.AddEffect(adder, receiverid, time); break;
                case EffectType.Silence:Silence.AddEffect(adder, receiverid, time); break;
                case EffectType.Paralysis:Paralysis.AddEffect(adder, receiverid,value, time); break;
                default:UnityEngine.Debug.LogError("有效果类型未注册");break;
            }
        }
    }
}