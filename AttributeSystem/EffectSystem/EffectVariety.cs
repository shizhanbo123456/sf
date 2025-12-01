using UnityEngine;
using Variety.Template;

namespace AttributeSystem.Effect
{
    public enum EffectType
    {
        HealthRegeneration, MagicRegeneration, Burning,Speed,Slowness,JumpBoost,AgileBoost,AccuracyBoost,AttackBoost,DefenseBoost,
        AgileDecrease, AccuracyDecrease, AttackDecrease, DefenseDecrease,ArmorFortity,ArmorShatter,DamageBoost,DamageDecrease,
        LifeSteal,Luck,BadLuck,Freeze,Stun,Sticky,Silence,Paralysis,Stoic
    }
    public class HealthRegeneration : EffectBase
    {
        private int value;
        public HealthRegeneration(int adder,Target receiver, int value, float time) : base(receiver, 10000 + adder, time, 1f)
        {
            this.value = value;
        }
        public override void Repeat()
        {
            receiver.Shengming += value;
        }
        public override EffectType GetEffectType()
        {
            return EffectType.HealthRegeneration;
        }
        public override bool Positive()
        {
            return true;
        }
    }
    public class MagicRegeneration : EffectBase
    {
        private int value;
        public MagicRegeneration(int adder, Target receiver, int value, float time) : base(receiver, 20000 + adder, time, 1f)
        {
            this.value = value;
        }
        public override void Repeat()
        {
            //if (receiver is PlayerData p) p.Mofa += value;
        }
        public override EffectType GetEffectType()
        {
            return EffectType.MagicRegeneration;
        }
        public override bool Positive()
        {
            return true;
        }
    }
    public class Burning : EffectBase
    {
        private int damageValue;
        public Burning(int adder, Target receiver, int value, float time) : base(receiver,30000+adder, time, 1f)
        {
            damageValue = value;
        }
        public override void Repeat()
        {
            var a = GetBaseAttributes();
            if (a.Shengming.Value <= 1) return;
            if (a.Shengming.Value > damageValue) a.Shengming.Value -= damageValue;
            else a.Shengming.Value = 1;
        }
        public override EffectType GetEffectType()
        {
            return EffectType.Burning;
        }
        public override bool Positive()
        {
            return false;
        }
    }
    public class Speed : EffectBase
    {
        private float value;
        public Speed(int adder, Target receiver, float value, float time) : base(receiver, 40000 + adder, time)
        {
            this.value = value;
        }
        public override void OnEntry()
        {
            GetFloatingAttributes().Jixing.Value += value;
        }
        public override void OnExit()
        {
            GetFloatingAttributes().Jixing.Value -= value;
        }
        public override EffectType GetEffectType()
        {
            return EffectType.Speed;
        }
        public override bool Positive()
        {
            return true;
        }
    }
    public class Slowness : EffectBase
    {
        private float value;
        public Slowness(int adder, Target receiver, float value, float time) : base(receiver, 50000 + adder, time)
        {
            this.value = value;
        }
        public override void OnEntry()
        {
            GetFloatingAttributes().Jixing.Value -= value;
        }
        public override void OnExit()
        {
            GetFloatingAttributes().Jixing.Value += value;
        }
        public override EffectType GetEffectType()
        {
            return EffectType.Slowness;
        }
        public override bool Positive()
        {
            return false;
        }
    }
    public class JumpBoost : EffectBase
    {
        private float value;
        public JumpBoost(int adder, Target receiver, float value, float time) : base(receiver, 60000 + adder, time)
        {
            this.value = value;
        }
        public override void OnEntry()
        {
            GetFloatingAttributes().Tengkong.Value += value;
        }
        public override void OnExit()
        {
            GetFloatingAttributes().Tengkong.Value -= value;
        }
        public override EffectType GetEffectType()
        {
            return EffectType.JumpBoost;
        }
        public override bool Positive()
        {
            return true;
        }
    }
    public class AgileBoost : EffectBase
    {
        private int value;
        public AgileBoost(int adder, Target receiver, float rate, float time) : base(receiver, 70000 + adder, time)
        {
            value = (int)(GetBaseAttributes().Shanbi.Value*rate);
        }
        public override void OnEntry()
        {
            GetBaseAttributes().Shanbi.Value += value;
        }
        public override void OnExit()
        {
            GetBaseAttributes().Shanbi.Value -= value;
        }
        public override EffectType GetEffectType()
        {
            return EffectType.AgileBoost;
        }
        public override bool Positive()
        {
            return true;
        }
    }
    public class AccuracyBoost : EffectBase
    {
        private int value;
        public AccuracyBoost(int adder, Target receiver, float rate, float time) : base(receiver, 80000 + adder, time)
        {
            value =(int)(GetBaseAttributes().Mingzhong.Value* rate);
        }
        public override void OnEntry()
        {
            GetBaseAttributes().Mingzhong.Value += value;
        }
        public override void OnExit()
        {
            GetBaseAttributes().Mingzhong.Value -= value;
        }
        public override EffectType GetEffectType()
        {
            return EffectType.AccuracyBoost;
        }
        public override bool Positive()
        {
            return true;
        }
    }
    public class AttackBoost : EffectBase
    {
        private int value;
        public AttackBoost(int adder, Target receiver, float rate, float time) : base(receiver, 90000 + adder, time)
        {
            value = (int)(receiver.effectController.GetBaseAttributes().Gongji.Value * rate);
        }
        public override void OnEntry()
        {
            GetBaseAttributes().Gongji.Value += value;
        }
        public override void OnExit()
        {
            GetBaseAttributes().Gongji.Value -= value;
        }
        public override EffectType GetEffectType()
        {
            return EffectType.AttackBoost;
        }
        public override bool Positive()
        {
            return true;
        }
    }
    public class DefenseBoost : EffectBase
    {
        private int value;
        public DefenseBoost(int adder, Target receiver, float rate, float time) : base(receiver, 100000 + adder, time)
        {
            value = (int)(receiver.effectController.GetBaseAttributes().Fangyu.Value * rate);
        }
        public override void OnEntry()
        {
            GetBaseAttributes().Fangyu.Value += value;
        }
        public override void OnExit()
        {
            GetBaseAttributes().Fangyu.Value -= value;
        }
        public override EffectType GetEffectType()
        {
            return EffectType.DefenseBoost;
        }
        public override bool Positive()
        {
            return true;
        }
    }
    public class AgileDecrease : EffectBase
    {
        private int value;
        public AgileDecrease(int adder, Target receiver, float rate, float time) : base(receiver, 110000 + adder, time)
        {
            value =(int)(GetBaseAttributes().Shanbi.Value* rate);
        }
        public override void OnEntry()
        {
            GetBaseAttributes().Shanbi.Value -= value;
        }
        public override void OnExit()
        {
            GetBaseAttributes().Shanbi.Value += value;
        }
        public override EffectType GetEffectType()
        {
            return EffectType.AgileDecrease;
        }
        public override bool Positive()
        {
            return false;
        }
    }
    public class AccuracyDecrease : EffectBase
    {
        private int value;
        public AccuracyDecrease(int adder, Target receiver, float rate, float time) : base(receiver, 120000 + adder, time)
        {
            value =(int)(GetBaseAttributes().Mingzhong.Value* rate);
        }
        public override void OnEntry()
        {
            GetBaseAttributes().Mingzhong.Value -= value;
        }
        public override void OnExit()
        {
            GetBaseAttributes().Mingzhong.Value += value;
        }
        public override EffectType GetEffectType()
        {
            return EffectType.AccuracyDecrease;
        }
        public override bool Positive()
        {
            return false;
        }
    }
    public class AttackDecrease : EffectBase
    {
        private int value;
        public AttackDecrease(int adder, Target receiver, float rate, float time) : base(receiver, 130000 + adder, time)
        {
            value = (int)(receiver.effectController.GetBaseAttributes().Gongji.Value * rate);
        }
        public override void OnEntry()
        {
            GetBaseAttributes().Gongji.Value -= value;
        }
        public override void OnExit()
        {
            GetBaseAttributes().Gongji.Value += value;
        }
        public override EffectType GetEffectType()
        {
            return EffectType.AttackDecrease;
        }
        public override bool Positive()
        {
            return false;
        }
    }
    public class DefenseDecrease : EffectBase
    {
        private int value;
        public DefenseDecrease(int adder, Target receiver, float rate, float time) : base(receiver, 1400000 + adder, time)
        {
            value = (int)(receiver.effectController.GetBaseAttributes().Fangyu.Value * rate);
        }
        public override void OnEntry()
        {
            GetBaseAttributes().Fangyu.Value -= value;
        }
        public override void OnExit()
        {
            GetBaseAttributes().Fangyu.Value += value;
        }
        public override EffectType GetEffectType()
        {
            return EffectType.DefenseDecrease;
        }
        public override bool Positive()
        {
            return false;
        }
    }
    public class ArmorFortity : EffectBase
    {
        private int value;
        public ArmorFortity(int adder, Target receiver, int rate, float time) : base(receiver, 1500000 + adder, time)
        {
            value = rate;
        }
        public override void OnEntry()
        {
            GetBaseAttributes().Jianshang.Value += value;
        }
        public override void OnExit()
        {
            GetBaseAttributes().Jianshang.Value -= value;
        }
        public override EffectType GetEffectType()
        {
            return EffectType.ArmorFortity;
        }
        public override bool Positive()
        {
            return true;
        }
    }
    public class ArmorShatter : EffectBase
    {
        private int value;
        public ArmorShatter(int adder, Target receiver, int rate, float time) : base(receiver, 1600000 + adder, time)
        {
            value = rate;
        }
        public override void OnEntry()
        {
            GetBaseAttributes().Jianshang.Value -= value;
        }
        public override void OnExit()
        {
            GetBaseAttributes().Jianshang.Value += value;
        }
        public override EffectType GetEffectType()
        {
            return EffectType.ArmorShatter;
        }
        public override bool Positive()
        {
            return false;
        }
    }
    public class DamageBoost : EffectBase
    {
        private int value;
        public DamageBoost(int adder, Target receiver, int rate, float time) : base(receiver, 1700000 + adder, time)
        {
            value = rate;
        }
        public override void OnEntry()
        {
            GetBaseAttributes().Jiashang.Value += value;
        }
        public override void OnExit()
        {
            GetBaseAttributes().Jiashang.Value -= value;
        }
        public override EffectType GetEffectType()
        {
            return EffectType.DamageBoost;
        }
        public override bool Positive()
        {
            return true;
        }
    }
    public class DamageDecrease : EffectBase
    {
        private int value;
        public DamageDecrease(int adder, Target receiver, int rate, float time) : base(receiver, 1800000 + adder, time)
        {
            value = rate;
        }
        public override void OnEntry()
        {
            GetBaseAttributes().Jiashang.Value -= value;
        }
        public override void OnExit()
        {
            GetBaseAttributes().Jiashang.Value += value;
        }
        public override EffectType GetEffectType()
        {
            return EffectType.DamageDecrease;
        }
        public override bool Positive()
        {
            return false;
        }
    }
    public class LifeSteal : EffectBase
    {
        private int value;
        public LifeSteal(int adder, Target receiver, float rate, float time) : base(receiver, 1900000 + adder, time)
        {
            value = (int)(GetFloatingAttributes().Shengming.Value*rate);
            var f = GetBaseAttributes().Shengming.Value;
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
        public override EffectType GetEffectType()
        {
            return EffectType.LifeSteal;
        }
        public override bool Positive()
        {
            return false;
        }
    }
    public class Luck : EffectBase
    {
        private int a;
        private int b;
        private int c;
        private int d;
        public Luck(int adder, Target receiver, float rate, float time) : base(receiver, 200000 + adder, time)
        {
            a = (int)(GetBaseAttributes().Mingzhong.Value * rate);
            b = (int)(GetBaseAttributes().Shanbi.Value * rate);
            c = (int)(GetBaseAttributes().Baoji.Value * rate);
            d = (int)(GetBaseAttributes().Renxing.Value * rate);
        }
        public override void OnEntry()
        {
            var e = GetBaseAttributes();
            e.Mingzhong.Value += a;
            e.Shanbi.Value += b;
            e.Baoji.Value += c;
            e.Renxing.Value += d;
        }
        public override void OnExit()
        {
            var e = GetBaseAttributes();
            e.Mingzhong.Value -= a;
            e.Shanbi.Value -= b;
            e.Baoji.Value -= c;
            e.Renxing.Value -= d;
        }
        public override EffectType GetEffectType()
        {
            return EffectType.Luck;
        }
        public override bool Positive()
        {
            return true;
        }
    }
    public class BadLuck : EffectBase
    {
        private int a;
        private int b;
        private int c;
        private int d;
        public BadLuck(int adder, Target receiver, float rate, float time) : base(receiver, 210000 + adder, time)
        {
            a = (int)(GetBaseAttributes().Mingzhong.Value * rate);
            b = (int)(GetBaseAttributes().Shanbi.Value * rate);
            c = (int)(GetBaseAttributes().Baoji.Value * rate);
            d = (int)(GetBaseAttributes().Renxing.Value * rate);
        }
        public override void OnEntry()
        {
            var e = GetBaseAttributes();
            e.Mingzhong.Value -= a;
            e.Shanbi.Value -= b;
            e.Baoji.Value -= c;
            e.Renxing.Value -= d;
        }
        public override void OnExit()
        {
            var e = GetBaseAttributes();
            e.Mingzhong.Value += a;
            e.Shanbi.Value += b;
            e.Baoji.Value += c;
            e.Renxing.Value += d;
        }
        public override EffectType GetEffectType()
        {
            return EffectType.BadLuck;
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
        public Freeze(int adder, Target receiver, float time) : base(receiver, 220000 + adder, time)
        {
        }
        public override void OnEntry()
        {
            operationLock = receiver.OperationLock.GetChain();
            skillLock=receiver.SkillLock.GetChain();
            operationLock.Locked = true;
            skillLock.Locked = true;
            GetBaseAttributes().Kangjitui.Value += 2;
        }
        public override void OnExit()
        {
            if (operationLock.InUse) operationLock.Discard();
            if (skillLock.InUse) skillLock.Discard();
            operationLock = null;
            skillLock= null;
            GetBaseAttributes().Kangjitui.Value -= 2;
        }
        public override EffectType GetEffectType()
        {
            return EffectType.Freeze;
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
        public Stun(int adder, Target receiver, float time) : base(receiver, 230000 + adder, time)
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
            if(operationLock.InUse)operationLock.Discard();
            if (skillLock.InUse) skillLock.Discard();
            operationLock = null;
            skillLock = null;
        }
        public override EffectType GetEffectType()
        {
            return EffectType.Stun;
        }
        public override bool Positive()
        {
            return false;
        }
    }
    public class Sticky : EffectBase
    {
        private LockChain operationLock;
        public Sticky(int adder, Target receiver, float time) : base(receiver, 240000 + adder, time)
        {
        }
        public override void OnEntry()
        {
            operationLock = receiver.OperationLock.GetChain();
            operationLock.Locked = true;
        }
        public override void OnExit()
        {
            if (operationLock.InUse) operationLock.Discard();
            operationLock = null;
        }
        public override EffectType GetEffectType()
        {
            return EffectType.Sticky;
        }
        public override bool Positive()
        {
            return false;
        }
    }
    public class Silence : EffectBase
    {
        private LockChain skillLock;
        public Silence(int adder, Target receiver, float time) : base(receiver, 250000 + adder, time)
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
        public override EffectType GetEffectType()
        {
            return EffectType.Silence;
        }
        public override bool Positive()
        {
            return false;
        }
    }
    public class Paralysis : EffectBase
    {
        public Paralysis(int adder, Target receiver, float time) : base(receiver, 260000 + adder, time, 1f)
        {
        }
        public override void Repeat()
        {
            receiver.InterruptRpc();
            receiver.ApplyMotion(new MotionStatic(0.01f, false, 2, true, true));
        }
        public override EffectType GetEffectType()
        {
            return EffectType.Paralysis;
        }
        public override bool Positive()
        {
            return false;
        }
    }
    public class Stoic : EffectBase
    {
        private int value;
        public Stoic(int adder, Target receiver, int rate, float time) : base(receiver, 2700000 + adder, time)
        {
            value = rate;
        }
        public override void OnEntry()
        {
            GetBaseAttributes().Kangjitui.Value += value;
        }
        public override void OnExit()
        {
            GetBaseAttributes().Kangjitui.Value -= value;
        }
        public override EffectType GetEffectType()
        {
            return EffectType.Stoic;
        }
        public override bool Positive()
        {
            return true;
        }
    }
}