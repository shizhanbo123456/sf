using AttributeSystem.DataOrientedEffects;
using LevelCreator.TargetTemplate;
using System;
using System.Collections.Generic;

namespace AttributeSystem.Effect
{
    public struct EffectCollection
    {
        private int adder;
        private SingleEffect[] effects;
        /// <summary>
        /// effectType,Value,Time
        /// </summary>
        public EffectCollection(int adder,SingleEffect[] effects)
        {
            this.adder = adder;
            this.effects = effects;
        }
        public void ApplyEffects(Target receiver)
        {
            if (effects == null) return;
            if(effects.Length == 0) return;
            foreach(var i in effects)
            {
                receiver.effectController.EffectStart(adder, i.effectType);
                AddEffect(i.effectType,i.value,i.time,receiver);
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
                case EffectType.HealthRegeneration:HealthRegenerationSystem.Instance.AddEffect(adder,receiverid,value,time);break;
                case EffectType.Burning: BurningSystem.Instance.AddEffect(adder, receiverid, value, time); break;
                case EffectType.Speed: SpeedSystem.Instance.AddEffect(adder,receiverid,value,time); break;
                case EffectType.Slowness: SlownessSystem.Instance.AddEffect(adder,receiverid,value,time); break;
                case EffectType.JumpBoost: JumpBoostSystem.Instance.AddEffect(adder,receiverid,value,time); break;
                case EffectType.AgileBoost: AgileBoostSystem.Instance.AddEffect(adder,receiverid,value,time); break;
                case EffectType.AccuracyBoost: AccuracyBoostSystem.Instance.AddEffect(adder,receiverid,value,time); break;
                case EffectType.AttackBoost: AttackBoostSystem.Instance.AddEffect(adder,receiverid,value,time); break;
                case EffectType.DefenseBoost: DefenseBoostSystem.Instance.AddEffect(adder,receiverid,value,time); break;
                case EffectType.AgileDecrease: AgileDecreaseSystem.Instance.AddEffect(adder,receiverid,value,time); break;
                case EffectType.AccuracyDecrease: AccuracyDecreaseSystem.Instance.AddEffect(adder,receiverid,value,time); break;
                case EffectType.AttackDecrease: AttackDecreaseSystem.Instance.AddEffect(adder,receiverid,value,time); break;
                case EffectType.DefenseDecrease: DefenseDecreaseSystem.Instance.AddEffect(adder,receiverid,value,time); break;
                case EffectType.ArmorFortity: ArmorFortitySystem.Instance.AddEffect(adder,receiverid,value,time); break;
                case EffectType.ArmorShatter: ArmorShatterSystem.Instance.AddEffect(adder, receiverid,value, time); break;
                case EffectType.DamageBoost: DamageBoostSystem.Instance.AddEffect(adder, receiverid,value, time); break;
                case EffectType.DamageDecrease: DamageDecreaseSystem.Instance.AddEffect(adder, receiverid, value, time); break;
                case EffectType.LifeSteal: LifeStealSystem.Instance.AddEffect(adder, receiverid, value, time); break;
                case EffectType.Luck: LuckSystem.Instance.AddEffect(adder, receiverid, value, time); break;
                case EffectType.BadLuck: BadLuckSystem.Instance.AddEffect(adder, receiverid, value, time); break;
                case EffectType.Freeze: FreezeSystem.Instance.AddEffect(adder, receiverid,0, time); break;
                case EffectType.Stun: StunSystem.Instance.AddEffect(adder, receiverid,0, time); break;
                case EffectType.Sticky: StickySystem.Instance.AddEffect(adder, receiverid,0, time); break;
                case EffectType.Silence: SilenceSystem.Instance.AddEffect(adder, receiverid,0, time); break;
                case EffectType.Paralysis: ParalysisSystem.Instance.AddEffect(adder, receiverid,value, time); break;
                default:UnityEngine.Debug.LogError("有效果类型未注册");break;
            }
        }
    }
}
public struct SingleEffect
{
    public EffectType effectType;
    public float value;
    public float time;
    public SingleEffect(EffectType effectType,float value,float time)
    {
        this.effectType = effectType;
        this.value = value;
        this.time = time;
    }
}