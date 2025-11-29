using UnityEngine;
using Variety.Template;

namespace AttributeSystem.Effect
{
    public enum Effects
    {
        HealthRegeneration, MagicRegeneration, Burning,Speed,Slowness,JumpBoost,AgileBoost,AccuracyBoost,AttackBoost,DefenseBoost,
        AgileDecrease, AccuracyDecrease, AttackDecrease, DefenseDecrease,ArmorFortity,ArmorShatter,DamageBoost,DamageDecrease,
        LifeSteal,Luck,BadLuck,Freeze,Stun,Sticky,Silence,Paralysis,Stoic
    }
    public class HealthRegeneration : EffectBase
    {
        private int value;
        public HealthRegeneration(Target adder,Target receiver, int value, float time) : base(receiver, 10000 + adder.ObjectId, time, 1f)
        {
            this.value = value;
        }
        public override void Repeat()
        {
            receiver.Shengming += value;
        }
        public override Effects GetEffectType()
        {
            return Effects.HealthRegeneration;
        }
        public override bool Positive()
        {
            return true;
        }
    }
    public class MagicRegeneration : EffectBase
    {
        private int value;
        public MagicRegeneration(Target adder, Target receiver, int value, float time) : base(receiver, 20000 + adder.ObjectId, time, 1f)
        {
            this.value = value;
        }
        public override void Repeat()
        {
            if (receiver is PlayerData p) p.Mofa += value;
        }
        public override Effects GetEffectType()
        {
            return Effects.MagicRegeneration;
        }
        public override bool Positive()
        {
            return true;
        }
    }
    public class Burning : EffectBase
    {
        private int damageValue;
        public Burning(Target adder, Target receiver, int value, float time) : base(receiver,30000+adder.ObjectId, time, 1f)
        {
            damageValue = value;
        }
        public override void Repeat()
        {
            var a = receiver.effectController.GetFloatingAttributes();
            if (a.Shengming.Value <= 1) return;
            if (a.Shengming.Value > damageValue) a.Shengming.Value -= damageValue;
            else a.Shengming.Value = 1;
        }
        public override Effects GetEffectType()
        {
            return Effects.Burning;
        }
        public override bool Positive()
        {
            return false;
        }
    }
    public class Speed : EffectBase
    {
        private float value;
        public Speed(Target adder, Target receiver, float value, float time) : base(receiver, 40000 + adder.ObjectId, time)
        {
            this.value = value;
        }
        public override void OnEntry()
        {
            receiver.effectController.GetFloatingAttributes().Jixing.Value += value;
        }
        public override void OnExit()
        {
            receiver.effectController.GetFloatingAttributes().Jixing.Value -= value;
        }
        public override Effects GetEffectType()
        {
            return Effects.Speed;
        }
        public override bool Positive()
        {
            return true;
        }
    }
    public class Slowness : EffectBase
    {
        private float value;
        public Slowness(Target adder, Target receiver, float value, float time) : base(receiver, 50000 + adder.ObjectId, time)
        {
            this.value = value;
        }
        public override void OnEntry()
        {
            receiver.effectController.GetFloatingAttributes().Jixing.Value -= value;
        }
        public override void OnExit()
        {
            receiver.effectController.GetFloatingAttributes().Jixing.Value += value;
        }
        public override Effects GetEffectType()
        {
            return Effects.Slowness;
        }
        public override bool Positive()
        {
            return false;
        }
    }
    public class JumpBoost : EffectBase
    {
        private float value;
        public JumpBoost(Target adder, Target receiver, float value, float time) : base(receiver, 60000 + adder.ObjectId, time)
        {
            this.value = value;
        }
        public override void OnEntry()
        {
            receiver.effectController.GetFloatingAttributes().Tengkong.Value += value;
        }
        public override void OnExit()
        {
            receiver.effectController.GetFloatingAttributes().Tengkong.Value -= value;
        }
        public override Effects GetEffectType()
        {
            return Effects.JumpBoost;
        }
        public override bool Positive()
        {
            return true;
        }
    }
    public class AgileBoost : EffectBase
    {
        private int value;
        public AgileBoost(Target adder, Target receiver, int rate, float time) : base(receiver, 70000 + adder.ObjectId, time)
        {
            value = rate;
        }
        public override void OnEntry()
        {
            receiver.effectController.GetFloatingAttributes().Shanbi.Value += value;
        }
        public override void OnExit()
        {
            receiver.effectController.GetFloatingAttributes().Shanbi.Value -= value;
        }
        public override Effects GetEffectType()
        {
            return Effects.AgileBoost;
        }
        public override bool Positive()
        {
            return true;
        }
    }
    public class AccuracyBoost : EffectBase
    {
        private int value;
        public AccuracyBoost(Target adder, Target receiver, int rate, float time) : base(receiver, 80000 + adder.ObjectId, time)
        {
            value = rate;
        }
        public override void OnEntry()
        {
            receiver.effectController.GetFloatingAttributes().Mingzhong.Value += value;
        }
        public override void OnExit()
        {
            receiver.effectController.GetFloatingAttributes().Mingzhong.Value -= value;
        }
        public override Effects GetEffectType()
        {
            return Effects.AccuracyBoost;
        }
        public override bool Positive()
        {
            return true;
        }
    }
    public class AttackBoost : EffectBase
    {
        private int value;
        public AttackBoost(Target adder, Target receiver, float rate, float time) : base(receiver, 90000 + adder.ObjectId, time)
        {
            value = (int)(receiver.effectController.GetBaseAttributes().Gongji.Value * rate);
        }
        public override void OnEntry()
        {
            receiver.effectController.GetFloatingAttributes().Gongji.Value += value;
        }
        public override void OnExit()
        {
            receiver.effectController.GetFloatingAttributes().Gongji.Value -= value;
        }
        public override Effects GetEffectType()
        {
            return Effects.AttackBoost;
        }
        public override bool Positive()
        {
            return true;
        }
    }
    public class DefenseBoost : EffectBase
    {
        private int value;
        public DefenseBoost(Target adder, Target receiver, float rate, float time) : base(receiver, 100000 + adder.ObjectId, time)
        {
            value = (int)(receiver.effectController.GetBaseAttributes().Fangyu.Value * rate);
        }
        public override void OnEntry()
        {
            receiver.effectController.GetFloatingAttributes().Fangyu.Value += value;
        }
        public override void OnExit()
        {
            receiver.effectController.GetFloatingAttributes().Fangyu.Value -= value;
        }
        public override Effects GetEffectType()
        {
            return Effects.DefenseBoost;
        }
        public override bool Positive()
        {
            return true;
        }
    }
    public class AgileDecrease : EffectBase
    {
        private int value;
        public AgileDecrease(Target adder, Target receiver, int rate, float time) : base(receiver, 110000 + adder.ObjectId, time)
        {
            value = rate;
        }
        public override void OnEntry()
        {
            receiver.effectController.GetFloatingAttributes().Shanbi.Value -= value;
        }
        public override void OnExit()
        {
            receiver.effectController.GetFloatingAttributes().Shanbi.Value += value;
        }
        public override Effects GetEffectType()
        {
            return Effects.AgileDecrease;
        }
        public override bool Positive()
        {
            return false;
        }
    }
    public class AccuracyDecrease : EffectBase
    {
        private int value;
        public AccuracyDecrease(Target adder, Target receiver, int rate, float time) : base(receiver, 120000 + adder.ObjectId, time)
        {
            value = rate;
        }
        public override void OnEntry()
        {
            receiver.effectController.GetFloatingAttributes().Mingzhong.Value -= value;
        }
        public override void OnExit()
        {
            receiver.effectController.GetFloatingAttributes().Mingzhong.Value += value;
        }
        public override Effects GetEffectType()
        {
            return Effects.AccuracyDecrease;
        }
        public override bool Positive()
        {
            return false;
        }
    }
    public class AttackDecrease : EffectBase
    {
        private int value;
        public AttackDecrease(Target adder, Target receiver, float rate, float time) : base(receiver, 130000 + adder.ObjectId, time)
        {
            value = (int)(receiver.effectController.GetBaseAttributes().Gongji.Value * rate);
        }
        public override void OnEntry()
        {
            receiver.effectController.GetFloatingAttributes().Gongji.Value -= value;
        }
        public override void OnExit()
        {
            receiver.effectController.GetFloatingAttributes().Gongji.Value += value;
        }
        public override Effects GetEffectType()
        {
            return Effects.AttackDecrease;
        }
        public override bool Positive()
        {
            return false;
        }
    }
    public class DefenseDecrease : EffectBase
    {
        private int value;
        public DefenseDecrease(Target adder, Target receiver, float rate, float time) : base(receiver, 1400000 + adder.ObjectId, time)
        {
            value = (int)(receiver.effectController.GetBaseAttributes().Fangyu.Value * rate);
        }
        public override void OnEntry()
        {
            receiver.effectController.GetFloatingAttributes().Fangyu.Value -= value;
        }
        public override void OnExit()
        {
            receiver.effectController.GetFloatingAttributes().Fangyu.Value += value;
        }
        public override Effects GetEffectType()
        {
            return Effects.DefenseDecrease;
        }
        public override bool Positive()
        {
            return false;
        }
    }
    public class ArmorFortity : EffectBase
    {
        private float value;
        public ArmorFortity(Target adder, Target receiver, float rate, float time) : base(receiver, 1500000 + adder.ObjectId, time)
        {
            value = rate;
        }
        public override void OnEntry()
        {
            receiver.effectController.GetFloatingAttributes().Jianshang.Value += value;
        }
        public override void OnExit()
        {
            receiver.effectController.GetFloatingAttributes().Jianshang.Value -= value;
        }
        public override Effects GetEffectType()
        {
            return Effects.ArmorFortity;
        }
        public override bool Positive()
        {
            return true;
        }
    }
    public class ArmorShatter : EffectBase
    {
        private float value;
        public ArmorShatter(Target adder, Target receiver, float rate, float time) : base(receiver, 1600000 + adder.ObjectId, time)
        {
            value = rate;
        }
        public override void OnEntry()
        {
            receiver.effectController.GetFloatingAttributes().Jianshang.Value -= value;
        }
        public override void OnExit()
        {
            receiver.effectController.GetFloatingAttributes().Jianshang.Value += value;
        }
        public override Effects GetEffectType()
        {
            return Effects.ArmorShatter;
        }
        public override bool Positive()
        {
            return false;
        }
    }
    public class DamageBoost : EffectBase
    {
        private float value;
        public DamageBoost(Target adder, Target receiver, float rate, float time) : base(receiver, 1700000 + adder.ObjectId, time)
        {
            value = rate;
        }
        public override void OnEntry()
        {
            receiver.effectController.GetFloatingAttributes().Jiashang.Value += value;
        }
        public override void OnExit()
        {
            receiver.effectController.GetFloatingAttributes().Jiashang.Value -= value;
        }
        public override Effects GetEffectType()
        {
            return Effects.DamageBoost;
        }
        public override bool Positive()
        {
            return true;
        }
    }
    public class DamageDecrease : EffectBase
    {
        private float value;
        public DamageDecrease(Target adder, Target receiver, float rate, float time) : base(receiver, 1800000 + adder.ObjectId, time)
        {
            value = rate;
        }
        public override void OnEntry()
        {
            receiver.effectController.GetFloatingAttributes().Jiashang.Value -= value;
        }
        public override void OnExit()
        {
            receiver.effectController.GetFloatingAttributes().Jiashang.Value += value;
        }
        public override Effects GetEffectType()
        {
            return Effects.DamageDecrease;
        }
        public override bool Positive()
        {
            return false;
        }
    }
    public class LifeSteal : EffectBase
    {
        private int value;
        public LifeSteal(Target adder, Target receiver, float rate, float time) : base(receiver, 1900000 + adder.ObjectId, time)
        {
            value = (int)(receiver.effectController.GetBaseAttributes().Shengming.Value*rate);
            var f = receiver.effectController.GetFloatingAttributes().Shengming.Value;
            if (value>= f)
            {
                value = f - 1;
            }
        }
        public override void OnEntry()
        {
            receiver.Shengming -= value;
        }
        public override void OnExit()
        {
            receiver.Shengming+= value;
        }
        public override Effects GetEffectType()
        {
            return Effects.LifeSteal;
        }
        public override bool Positive()
        {
            return false;
        }
    }
    public class Luck : EffectBase
    {
        private int value;
        public Luck(Target adder, Target receiver, int rate, float time) : base(receiver, 200000 + adder.ObjectId, time)
        {
            value = rate;
        }
        public override void OnEntry()
        {
            var e = receiver.effectController.GetFloatingAttributes();
            e.Mingzhong.Value += value;
            e.Shanbi.Value += value;
            e.Baoji.Value += value;
            e.Renxing.Value += value;
        }
        public override void OnExit()
        {
            var e = receiver.effectController.GetFloatingAttributes();
            e.Mingzhong.Value -= value;
            e.Shanbi.Value -= value;
            e.Baoji.Value -= value;
            e.Renxing.Value -= value;
        }
        public override Effects GetEffectType()
        {
            return Effects.Luck;
        }
        public override bool Positive()
        {
            return true;
        }
    }
    public class BadLuck : EffectBase
    {
        private int value;
        public BadLuck(Target adder, Target receiver, int rate, float time) : base(receiver, 210000 + adder.ObjectId, time)
        {
            value = rate;
        }
        public override void OnEntry()
        {
            var e = receiver.effectController.GetFloatingAttributes();
            e.Mingzhong.Value -= value;
            e.Shanbi.Value -= value;
            e.Baoji.Value -= value;
            e.Renxing.Value -= value;
        }
        public override void OnExit()
        {
            var e = receiver.effectController.GetFloatingAttributes();
            e.Mingzhong.Value += value;
            e.Shanbi.Value += value;
            e.Baoji.Value += value;
            e.Renxing.Value += value;
        }
        public override Effects GetEffectType()
        {
            return Effects.BadLuck;
        }
        public override bool Positive()
        {
            return false;
        }
    }
    public class Freeze : EffectBase
    {
        private LockChain operationLock;
        private LockChain skillLock;
        public Freeze(Target adder, Target receiver, float time) : base(receiver, 220000 + adder.ObjectId, time)
        {
        }
        public override void OnEntry()
        {
            operationLock = receiver.OperationLock.GetChain();
            skillLock=receiver.SkillLock.GetChain();
            operationLock.Locked = true;
            skillLock.Locked = true;
            receiver.effectController.GetFloatingAttributes().Kangjitui.Value += 2;
        }
        public override void OnExit()
        {
            operationLock.Discard();
            skillLock.Discard();
            operationLock = null;
            skillLock= null;
            receiver.effectController.GetFloatingAttributes().Kangjitui.Value -= 2;
        }
        public override Effects GetEffectType()
        {
            return Effects.Freeze;
        }
        public override bool Positive()
        {
            return false;
        }
    }
    public class Stun : EffectBase
    {
        private LockChain operationLock;
        private LockChain skillLock;
        public Stun(Target adder, Target receiver, float time) : base(receiver, 230000 + adder.ObjectId, time)
        {
        }
        public override void OnEntry()
        {
            operationLock = receiver.OperationLock.GetChain();
            skillLock = receiver.SkillLock.GetChain();
            operationLock.Locked = true;
            skillLock.Locked = true;
        }
        public override void OnExit()
        {
            operationLock.Discard();
            skillLock.Discard();
            operationLock = null;
            skillLock = null;
        }
        public override Effects GetEffectType()
        {
            return Effects.Stun;
        }
        public override bool Positive()
        {
            return false;
        }
    }
    public class Sticky : EffectBase
    {
        private LockChain operationLock;
        public Sticky(Target adder, Target receiver, float time) : base(receiver, 240000 + adder.ObjectId, time)
        {
        }
        public override void OnEntry()
        {
            operationLock = receiver.OperationLock.GetChain();
            operationLock.Locked = true;
        }
        public override void OnExit()
        {
            operationLock.Discard();
            operationLock = null;
        }
        public override Effects GetEffectType()
        {
            return Effects.Sticky;
        }
        public override bool Positive()
        {
            return false;
        }
    }
    public class Silence : EffectBase
    {
        private LockChain skillLock;
        public Silence(Target adder, Target receiver, float time) : base(receiver, 250000 + adder.ObjectId, time)
        {
        }
        public override void OnEntry()
        {
            skillLock = receiver.SkillLock.GetChain();
            skillLock.Locked = true;
        }
        public override void OnExit()
        {
            skillLock.Discard();
            skillLock = null;
        }
        public override Effects GetEffectType()
        {
            return Effects.Silence;
        }
        public override bool Positive()
        {
            return false;
        }
    }
    public class Paralysis : EffectBase
    {
        public Paralysis(Target adder, Target receiver, float time) : base(receiver, 260000 + adder.ObjectId, time, 1f)
        {
        }
        public override void Repeat()
        {
            receiver.InterruptRpc();
            receiver.controller.ApplyMotion(new MotionStatic(0.01f, false, 2, true, true));
        }
        public override Effects GetEffectType()
        {
            return Effects.Paralysis;
        }
        public override bool Positive()
        {
            return false;
        }
    }
    public class Stoic : EffectBase
    {
        private int value;
        public Stoic(Target adder, Target receiver, int rate, float time) : base(receiver, 2700000 + adder.ObjectId, time)
        {
            value = rate;
        }
        public override void OnEntry()
        {
            receiver.effectController.GetFloatingAttributes().Kangjitui.Value += value;
        }
        public override void OnExit()
        {
            receiver.effectController.GetFloatingAttributes().Kangjitui.Value -= value;
        }
        public override Effects GetEffectType()
        {
            return Effects.Stoic;
        }
        public override bool Positive()
        {
            return true;
        }
    }
}