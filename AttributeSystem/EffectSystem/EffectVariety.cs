using System;
using System.Collections.Generic;
using UnityEngine;
using Variety.Template;

public enum EffectType
{
    HealthRegeneration, Burning, Speed, Slowness, JumpBoost, AgileBoost, AccuracyBoost, AttackBoost, DefenseBoost,
    AgileDecrease, AccuracyDecrease, AttackDecrease, DefenseDecrease, ArmorFortity, ArmorShatter, DamageBoost, DamageDecrease,
    LifeSteal, Luck, BadLuck, Freeze, Stun, Sticky, Silence, Paralysis
}

namespace AttributeSystem.DataOrientedEffects
{
    public struct EffectId
    {
        public int adder;
        public int receiver;
        public EffectId(int adder, int receiver)
        {
            this.adder = adder;
            this.receiver = receiver;
        }
        public readonly Target GetAdder() => Tool.SceneController.GetTarget(adder);
        public readonly Target GetReceiver() => Tool.SceneController.GetTarget(receiver);
        public override int GetHashCode()
        {
            return adder * 10000 + receiver;
        }
    }
    public interface IEffect
    {
        float GetEndTime();
    }
    public abstract class EffectSystem<T>where T: IEffect
    {
        protected static Dictionary<EffectId, T> Effects = new();
        public abstract void AddEffect(int adder, int receiver, float value, float time);
        protected virtual void Update()
        {
            foreach (var key in Effects.Keys)
            {
                if (Time.time > Effects[key].GetEndTime()) GlobalEffectManager.ToRemove.Add(key);
            }
            if (GlobalEffectManager.ToRemove.Count > 0)
            {
                foreach (var i in GlobalEffectManager.ToRemove)
                {
                    var t = i.GetReceiver();
                    if (t && t.effectController) t.effectController.EffectEnd(i.adder, GetEffectType());
                    SetEffectActive(false, i, Effects[i]);
                }
                GlobalEffectManager.ToRemove.Clear();
            }
        }
        protected virtual void OnTargetDestroyed(int id)
        {
            foreach (var key in Effects.Keys)
            {
                if (key.adder == id || key.receiver == id)
                {
                    GlobalEffectManager.ToRemove.Add(key);
                }
            }
            if (GlobalEffectManager.ToRemove.Count > 0)
            {
                foreach (var i in GlobalEffectManager.ToRemove)
                {
                    SetEffectActive(false, i, Effects[i]);
                }
                GlobalEffectManager.ToRemove.Clear();
            }
        }
        protected abstract EffectType GetEffectType();
        protected virtual void SetEffectActive(bool active, EffectId id, T effect)
        {
            if (active)
            {
                Effects.Add(id, effect);
                EnableEvents();
            }
            else
            {
                Effects.Remove(id);
                DisableEvents();
            }
        }

        private bool eventsActive = false;
        protected void EnableEvents()
        {
            if (eventsActive) return;
            if (Effects.Count == 0) return;
            GlobalEffectManager.EffectUpdate += Update;
            GlobalEffectManager.OnTargetDestroyed += OnTargetDestroyed;
            eventsActive = true;
        }
        protected void DisableEvents()
        {
            if (!eventsActive) return;
            if (Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate -= Update;
            GlobalEffectManager.OnTargetDestroyed -= OnTargetDestroyed;
            eventsActive = false;
        }
    }
    public struct HealthRegeneration: IEffect
    {
        public int value;
        public float endTime;
        public float nextUpdateTime;
        public float GetEndTime() => endTime;
    }
    public class HealthRegenerationSystem : EffectSystem<HealthRegeneration>
    {
        private Dictionary<EffectId, HealthRegeneration> Temp = new Dictionary<EffectId, HealthRegeneration>();
        public override void AddEffect(int adder, int receiver, float value, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            SetEffectActive(true,new EffectId(adder, receiver), new HealthRegeneration()
            { value = (int)value, endTime = Time.time + time, nextUpdateTime = Time.time + 1f });
        }
        protected override void Update()
        {
            base.Update();
            if (Effects.Count > 0)
            {
                foreach (var key in Effects.Keys)
                {
                    if (Time.time > Effects[key].nextUpdateTime)
                    {
                        var p = Effects[key];
                        var t = Tool.SceneController.GetTarget(key.receiver);
                        if (t != null) t.Shengming += p.value;
                        Temp.Add(key, new HealthRegeneration() { value = p.value, endTime = p.endTime, nextUpdateTime = Time.time + 1 });
                    }
                }
                if (Temp.Count > 0)
                {
                    foreach(var key in Temp.Keys)
                    {
                        Effects[key]= Temp[key];
                    }
                    Temp.Clear();
                }
            }
        }
        protected override EffectType GetEffectType() => EffectType.HealthRegeneration;
    }
    // 燃烧效果实现
    public struct Burning:IEffect
    {
        public int damageValue;
        public float endTime;
        public float nextUpdateTime;
        public float GetEndTime() => endTime;
    }
    public class BurningSystem : EffectSystem<Burning>
    {
        private Dictionary<EffectId, Burning> Temp=new Dictionary<EffectId, Burning>();
        public override void AddEffect(int adder, int receiver, float value, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            SetEffectActive(true,new EffectId(adder, receiver), new Burning
            {
                damageValue = (int)value,
                endTime = Time.time + time,
                nextUpdateTime = Time.time + 1f
            });
        }

        protected override void Update()
        {
            base.Update();
            if (Effects.Count > 0)
            {
                foreach (var key in Effects.Keys)
                {
                    var effect = Effects[key];
                    if (Time.time > effect.nextUpdateTime)
                    {
                        var receiver = key.GetReceiver();
                        if (receiver == null) continue;
                        var attributes = receiver.FloatingAttributes;
                        if (attributes.Shengming.Value > 1)
                        {
                            attributes.Shengming.Value = Mathf.Max(1, attributes.Shengming.Value - effect.damageValue);
                        }

                        Temp.Add(key, new Burning
                        {
                            damageValue = effect.damageValue,
                            endTime = effect.endTime,
                            nextUpdateTime = Time.time + 1f
                        });
                    }
                }
                if(Temp.Count>0)
                {
                    foreach(var key in Temp.Keys)
                    {
                        Effects[key] = Temp[key];
                    }
                    Temp.Clear();
                }
            }
        }
        protected override EffectType GetEffectType() => EffectType.Burning;
    }

    // 速度加成效果
    public struct Speed:IEffect
    {
        public float value;
        public float endTime;
        public float GetEndTime() => endTime;
    }
    public class SpeedSystem : EffectSystem<Speed>
    {
        public override void AddEffect(int adder, int receiver, float value, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            SetEffectActive(true, new EffectId(adder, receiver), new Speed
            {
                value = value,
                endTime = Time.time + time,
            });
        }
        protected override EffectType GetEffectType() => EffectType.Speed;
        protected override void SetEffectActive(bool active, EffectId id, Speed effect)
        {
            base.SetEffectActive(active, id, effect);
            var t = id.GetReceiver();
            if (t)
            {
                t.FloatingAttributes.Jixing.Value += effect.value*(active?1:-1);
            }
        }
    }

    // 减速效果 (与速度效果类似，仅方向相反)
    public struct Slowness:IEffect
    {
        public float value;
        public float endTime;
        public float GetEndTime() => endTime;
    }
    public class SlownessSystem:EffectSystem<Slowness>
    {
        public override void AddEffect(int adder, int receiver, float value, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            SetEffectActive(true,new EffectId(adder, receiver), new Slowness
            {
                value = value,
                endTime = Time.time + time
            });
        }
        protected override EffectType GetEffectType() => EffectType.Slowness;
        protected override void SetEffectActive(bool active, EffectId id, Slowness effect)
        {
            base.SetEffectActive(active, id, effect);
            var t = id.GetReceiver();
            if (t)
            {
                t.FloatingAttributes.Jixing.Value -= effect.value * (active ? 1 : -1);
            }
        }
    }

    // 跳跃提升效果
    public struct JumpBoost:IEffect
    {
        public float value;
        public float endTime;
        public float GetEndTime() => endTime;
    }
    public class JumpBoosttSystem : EffectSystem<JumpBoost>
    {
        public override void AddEffect(int adder, int receiver, float value, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            SetEffectActive(true,new EffectId(adder, receiver), new JumpBoost
            {
                value = value,
                endTime = Time.time + time,
            });
        }
        protected override EffectType GetEffectType() => EffectType.JumpBoost;
        protected override void SetEffectActive(bool active, EffectId id, JumpBoost effect)
        {
            base.SetEffectActive(active, id, effect);
            var t = id.GetReceiver();
            if (t)
            {
                t.FloatingAttributes.Tengkong.Value += effect.value * (active ? 1 : -1);
            }
        }
    }
    // 敏捷提升效果
    public struct AgileBoost:IEffect
    {
        public int value;
        public float endTime;
        public float GetEndTime() => endTime;
    }
    public class AgileBoostSystem : EffectSystem<AgileBoost>
    {
        public override void AddEffect(int adder, int receiver, float rate, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            SetEffectActive(true,new EffectId(adder, receiver), new AgileBoost
            {
                value = (int)(rate*Tool.SceneController.GetTarget(receiver).BaseAttributes.Shanbi.Value),
                endTime = Time.time + time,
            });
        }
        protected override EffectType GetEffectType() => EffectType.AgileBoost;
        protected override void SetEffectActive(bool active, EffectId id, AgileBoost effect)
        {
            base.SetEffectActive(active, id, effect);
            var t = id.GetReceiver();
            if (t)
            {
                t.FloatingAttributes.Shanbi.Value += effect.value * (active ? 1 : -1);
            }
        }
    }
    // 精准提升效果
    public struct AccuracyBoost:IEffect
    {
        public int value;
        public float endTime;
        public float GetEndTime() => endTime;
    }
    public class AccuracyBoostSystem : EffectSystem<AccuracyBoost>
    {
        public override void AddEffect(int adder, int receiver, float rate, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            SetEffectActive(true,new EffectId(adder, receiver), new AccuracyBoost
            {
                value = (int)(rate * Tool.SceneController.GetTarget(receiver).BaseAttributes.Mingzhong.Value),
                endTime = Time.time + time
            });
        }
        protected override EffectType GetEffectType() => EffectType.AccuracyBoost;
        protected override void SetEffectActive(bool active, EffectId id, AccuracyBoost effect)
        {
            base.SetEffectActive(active, id, effect);
            var t = id.GetReceiver();
            if (t)
            {
                t.FloatingAttributes.Mingzhong.Value += effect.value * (active ? 1 : -1);
            }
        }
    }
    // 攻击提升效果
    public struct AttackBoost:IEffect
    {
        public int value;
        public float endTime;
        public float GetEndTime() => endTime;
    }
    public class AttackBoostSystem : EffectSystem<AttackBoost>
    {
        public override void AddEffect(int adder, int receiver, float rate, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            SetEffectActive(true,new EffectId(adder, receiver), new AttackBoost
            {
                value = (int)(rate*Tool.SceneController.GetTarget(receiver).BaseAttributes.Gongji.Value),
                endTime = Time.time + time
            });
        }
        protected override EffectType GetEffectType() => EffectType.AttackBoost;
        protected override void SetEffectActive(bool active, EffectId id, AttackBoost effect)
        {
            base.SetEffectActive(active, id, effect);
            var t = id.GetReceiver();
            if (t)
            {
                t.FloatingAttributes.Gongji.Value += effect.value * (active ? 1 : -1);
            }
        }
    }
    // 防御提升效果
    public struct DefenseBoost:IEffect
    {
        public int value;
        public float endTime;
        public float GetEndTime() => endTime;
    }
    public class DefenseBoostSystem : EffectSystem<DefenseBoost>
    {
        public override void AddEffect(int adder, int receiver, float rate, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            SetEffectActive(true,new EffectId(adder, receiver), new DefenseBoost
            {
                value = (int)(rate * Tool.SceneController.GetTarget(receiver).BaseAttributes.Fangyu.Value),
                endTime = Time.time + time
            });
        }
        protected override EffectType GetEffectType() => EffectType.DefenseBoost;
        protected override void SetEffectActive(bool active, EffectId id, DefenseBoost effect)
        {
            base.SetEffectActive(active, id, effect);
            var t = id.GetReceiver();
            if (t)
            {
                t.FloatingAttributes.Fangyu.Value += effect.value * (active ? 1 : -1);
            }
        }
    }
    // 敏捷降低效果
    public struct AgileDecrease:IEffect
    {
        public int value;
        public float endTime;
        public float GetEndTime() => endTime;
    }
    public class AgileDecreaseSystem : EffectSystem<AgileDecrease>
    {
        public override void AddEffect(int adder, int receiver, float rate, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            SetEffectActive(true,new EffectId(adder, receiver), new AgileDecrease
            {
                value = (int)(rate * Tool.SceneController.GetTarget(receiver).BaseAttributes.Shanbi.Value),
                endTime = Time.time + time,
            });
        }
        protected override EffectType GetEffectType() => EffectType.AgileDecrease;
        protected override void SetEffectActive(bool active, EffectId id, AgileDecrease effect)
        {
            base.SetEffectActive(active, id, effect);
            var t = id.GetReceiver();
            if (t)
            {
                t.FloatingAttributes.Shanbi.Value -= effect.value * (active ? 1 : -1);
            }
        }
    }
    // 精准度降低效果
    public struct AccuracyDecrease:IEffect
    {
        public int value;
        public float endTime;
        public float GetEndTime() => endTime;
    }
    public class AccuracyDecreaseSystem : EffectSystem<AccuracyDecrease>
    {
        public override void AddEffect(int adder, int receiver, float rate, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            SetEffectActive(true,new EffectId(adder, receiver), new AccuracyDecrease
            {
                value = (int)(rate * Tool.SceneController.GetTarget(receiver).BaseAttributes.Mingzhong.Value),
                endTime = Time.time + time
            });
        }
        protected override EffectType GetEffectType() => EffectType.AccuracyDecrease;
        protected override void SetEffectActive(bool active, EffectId id, AccuracyDecrease effect)
        {
            base.SetEffectActive(active, id, effect);
            var t = id.GetReceiver();
            if (t)
            {
                t.FloatingAttributes.Mingzhong.Value -= effect.value * (active ? 1 : -1);
            }
        }
    }
    // 攻击降低效果
    public struct AttackDecrease:IEffect
    {
        public int value;
        public float endTime;
        public float GetEndTime() => endTime;
    }
    public class AttackDecreaseSystem : EffectSystem<AttackDecrease>
    {
        public override void AddEffect(int adder, int receiver, float rate, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            SetEffectActive(true,new EffectId(adder, receiver), new AttackDecrease
            {
                value = (int)(rate * Tool.SceneController.GetTarget(receiver).BaseAttributes.Gongji.Value),
                endTime = Time.time + time
            });
        }
        protected override EffectType GetEffectType() => EffectType.AttackDecrease;
        protected override void SetEffectActive(bool active, EffectId id, AttackDecrease effect)
        {
            base.SetEffectActive(active, id, effect);
            var t = id.GetReceiver();
            if (t)
            {
                t.FloatingAttributes.Gongji.Value -= effect.value * (active ? 1 : -1);
            }
        }
    }
    // 防御降低效果
    public struct DefenseDecrease:IEffect
    {
        public int value;
        public float endTime;
        public float GetEndTime() => endTime;
    }
    public class DefenseDecreaseSystem : EffectSystem<DefenseDecrease>
    {
        public override void AddEffect(int adder, int receiver, float rate, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            SetEffectActive(true,new EffectId(adder, receiver), new DefenseDecrease
            {
                value = (int)(rate * Tool.SceneController.GetTarget(receiver).BaseAttributes.Fangyu.Value),
                endTime = Time.time + time,
            });
        }
        protected override EffectType GetEffectType() => EffectType.DefenseDecrease;
        protected override void SetEffectActive(bool active, EffectId id, DefenseDecrease effect)
        {
            base.SetEffectActive(active, id, effect);
            var t = id.GetReceiver();
            if (t)
            {
                t.FloatingAttributes.Fangyu.Value -= effect.value * (active ? 1 : -1);
            }
        }
    }
    // 护甲强化效果
    public struct ArmorFortity:IEffect
    {
        public int value;
        public float endTime;
        public float GetEndTime() => endTime;
    }
    public class ArmorFortitySystem : EffectSystem<ArmorFortity>
    {
        public override void AddEffect(int adder, int receiver, float value, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            SetEffectActive(true,new EffectId(adder, receiver), new ArmorFortity
            {
                value = (int)value,
                endTime = Time.time + time
            });
        }
        protected override EffectType GetEffectType() => EffectType.ArmorFortity;
        protected override void SetEffectActive(bool active, EffectId id, ArmorFortity effect)
        {
            base.SetEffectActive(active, id, effect);
            var t = id.GetReceiver();
            if (t)
            {
                t.FloatingAttributes.Jianshang.Value += effect.value * (active ? 1 : -1);
            }
        }
    }
    // 护甲破碎效果
    public struct ArmorShatter:IEffect
    {
        public int value;
        public float endTime;
        public float GetEndTime() => endTime;
    }
    public class ArmorShatterSystem : EffectSystem<ArmorShatter>
    {
        public override void AddEffect(int adder, int receiver, float value, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            SetEffectActive(true,new EffectId(adder, receiver), new ArmorShatter
            {
                value = (int)value,
                endTime = Time.time + time
            });
        }
        protected override EffectType GetEffectType() => EffectType.ArmorShatter;
        protected override void SetEffectActive(bool active, EffectId id, ArmorShatter effect)
        {
            base.SetEffectActive(active, id, effect);
            var t = id.GetReceiver();
            if (t)
            {
                t.FloatingAttributes.Jianshang.Value -= effect.value * (active ? 1 : -1);
            }
        }
    }
    // 伤害提升效果
    public struct DamageBoost:IEffect
    {
        public int value;
        public float endTime;
        public float GetEndTime() => endTime;
    }
    public class DamageBoostSystem : EffectSystem<DamageBoost>
    {
        public override void AddEffect(int adder, int receiver, float value, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            SetEffectActive(true,new EffectId(adder, receiver), new DamageBoost
            {
                value = (int)value,
                endTime = Time.time + time
            });
        }
        protected override EffectType GetEffectType() => EffectType.DamageBoost;
        protected override void SetEffectActive(bool active, EffectId id, DamageBoost effect)
        {
            base.SetEffectActive(active, id, effect);
            var t = id.GetReceiver();
            if (t)
            {
                t.FloatingAttributes.Jianshang.Value += effect.value * (active ? 1 : -1);
            }
        }
    }
    // 伤害降低效果
    public struct DamageDecrease:IEffect
    {
        public int value;
        public float endTime;
        public float GetEndTime() => endTime;
    }
    public class DamageDecreaseSystem : EffectSystem<DamageDecrease>
    {
        public override void AddEffect(int adder, int receiver, float value, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            SetEffectActive(true,new EffectId(adder, receiver), new DamageDecrease
            {
                value = (int)value,
                endTime = Time.time + time
            });
        }
        protected override EffectType GetEffectType() => EffectType.DamageDecrease;
        protected override void SetEffectActive(bool active, EffectId id, DamageDecrease effect)
        {
            base.SetEffectActive(active, id, effect);
            var t = id.GetReceiver();
            if (t)
            {
                t.FloatingAttributes.Jiashang.Value -= effect.value * (active ? 1 : -1);
            }
        }
    }
    // 生命偷取效果
    public struct LifeSteal:IEffect
    {
        public int value;
        public float endTime;
        public float GetEndTime() => endTime;
    }
    public class LifeStealSystem : EffectSystem<LifeSteal>
    {
        public override void AddEffect(int adder, int receiver, float rate, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            var t = Tool.SceneController.GetTarget(receiver);
            int v = (int)(t.BaseAttributes.Shengming.Value * rate);
            if (v >= t.Shengming) v = t.Shengming - 1;
            var l = new LifeSteal
            {
                value = v,
                endTime = Time.time + time
            };
            SetEffectActive(true,new EffectId(adder, receiver), l);
        }
        protected override EffectType GetEffectType() => EffectType.LifeSteal;
        protected override void SetEffectActive(bool active, EffectId id, LifeSteal effect)
        {
            base.SetEffectActive(active, id, effect);
            var t = id.GetReceiver();
            if (t)
            {
                t.Shengming += effect.value * (active ? 1 : -1);
            }
        }
    }
    // 幸运提升效果
    public struct Luck:IEffect
    {
        public int value1;
        public int value2;
        public int value3;
        public int value4;
        public float endTime;
        public float GetEndTime() => endTime;
    }
    public class LuckSystem : EffectSystem<Luck>
    {
        public override void AddEffect(int adder, int receiver, float rate, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            EnableEvents();
            var target = Tool.SceneController.GetTarget(receiver);
            var att = target.FloatingAttributes;
            var l = new Luck
            {
                value1 = (int)(att.Mingzhong.Value * rate),
                value2 = (int)(att.Shanbi.Value * rate),
                value3 = (int)(att.Baoji.Value * rate),
                value4 = (int)(att.Renxing.Value * rate),
                endTime = Time.time + time,
            };
            SetEffectActive(true, new EffectId(adder, receiver), l);
        }
        protected override EffectType GetEffectType() => EffectType.Luck;
        protected override void SetEffectActive(bool active, EffectId id, Luck effect)
        {
            base.SetEffectActive(active, id, effect);
            var t = id.GetReceiver();
            if (t)
            {
                t.FloatingAttributes.Mingzhong.Value += effect.value1 * (active ? 1 : -1);
                t.FloatingAttributes.Shanbi.Value += effect.value2 * (active ? 1 : -1);
                t.FloatingAttributes.Baoji.Value += effect.value3 * (active ? 1 : -1);
                t.FloatingAttributes.Renxing.Value += effect.value4 * (active ? 1 : -1);
            }
        }
    }
    // 厄运效果
    public struct BadLuck:IEffect
    {
        public int value1;
        public int value2;
        public int value3;
        public int value4;
        public float endTime;
        public float GetEndTime() => endTime;
    }
    public class BadLuckSystem : EffectSystem<BadLuck>
    {
        public override void AddEffect(int adder, int receiver, float rate, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            EnableEvents();
            var target = Tool.SceneController.GetTarget(receiver);
            var att = target.FloatingAttributes;
            var l = new BadLuck
            {
                value1 = (int)(att.Mingzhong.Value * rate),
                value2 = (int)(att.Shanbi.Value * rate),
                value3 = (int)(att.Baoji.Value * rate),
                value4 = (int)(att.Renxing.Value * rate),
                endTime = Time.time + time,
            };
            SetEffectActive(true, new EffectId(adder, receiver), l);
        }
        protected override EffectType GetEffectType() => EffectType.BadLuck;
        protected override void SetEffectActive(bool active, EffectId id, BadLuck effect)
        {
            base.SetEffectActive(active, id, effect);
            var t = id.GetReceiver();
            if (t)
            {
                t.FloatingAttributes.Mingzhong.Value -= effect.value1 * (active ? 1 : -1);
                t.FloatingAttributes.Shanbi.Value -= effect.value2 * (active ? 1 : -1);
                t.FloatingAttributes.Baoji.Value -= effect.value3 * (active ? 1 : -1);
                t.FloatingAttributes.Renxing.Value -= effect.value4 * (active ? 1 : -1);
            }
        }
    }
    // 冻结效果
    public struct Freeze:IEffect
    {
        public float endTime;
        public LockChain moveLock;
        public LockChain skillLock;
        public float GetEndTime() => endTime;
    }
    public class FreezeSystem : EffectSystem<Freeze>
    {
        public override void AddEffect(int adder, int receiver,float value, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            var target=Tool.SceneController.GetTarget(receiver);
            SetEffectActive(true,new EffectId(adder, receiver), new Freeze
            {
                endTime = Time.time + time,
                moveLock = target.OperationLock.GetChain(),
                skillLock = target.SkillLock.GetChain()
            });
        }
        protected override EffectType GetEffectType() => EffectType.Freeze;
        protected override void SetEffectActive(bool active, EffectId id, Freeze effect)
        {
            base.SetEffectActive(active, id, effect);
            if (effect.moveLock.InUse)
            {
                effect.moveLock.Locked = active;
                if (!active) effect.moveLock.Discard();
            }
            if (effect.skillLock.InUse)
            {
                effect.skillLock.Locked = active;
                if(!active)effect.skillLock.Discard();
            }
        }
    }
    // 眩晕效果
    public struct Stun:IEffect
    {
        public float endTime;
        public LockChain moveLock;
        public LockChain skillLock;
        public float GetEndTime() => endTime;
    }
    public class StunSystem : EffectSystem<Stun>
    {
        public override void AddEffect(int adder, int receiver,float value, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            var target = Tool.SceneController.GetTarget(receiver);
            SetEffectActive(true, new EffectId(adder, receiver), new Stun
            {
                endTime = Time.time + time,
                moveLock = target.OperationLock.GetChain(),
                skillLock = target.SkillLock.GetChain()
            });
        }
        protected override EffectType GetEffectType() => EffectType.Stun;
        protected override void SetEffectActive(bool active, EffectId id, Stun effect)
        {
            base.SetEffectActive(active, id, effect);
            if (effect.moveLock.InUse)
            {
                effect.moveLock.Locked = active;
                if (!active) effect.moveLock.Discard();
            }
            if (effect.skillLock.InUse)
            {
                effect.skillLock.Locked = active;
                if (!active) effect.skillLock.Discard();
            }
        }
    }
    // 粘性效果
    public struct Sticky:IEffect
    {
        public float endTime;
        public LockChain moveLock;
        public float GetEndTime() => endTime;
    }
    public class StickySystem : EffectSystem<Sticky>
    {
        public override void AddEffect(int adder, int receiver,float value, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            var target = Tool.SceneController.GetTarget(receiver);
            SetEffectActive(true, new EffectId(adder, receiver), new Sticky
            {
                endTime = Time.time + time,
                moveLock = target.OperationLock.GetChain()
            });
        }
        protected override EffectType GetEffectType() => EffectType.Sticky;
        protected override void SetEffectActive(bool active, EffectId id, Sticky effect)
        {
            base.SetEffectActive(active, id, effect);
            if (effect.moveLock.InUse)
            {
                effect.moveLock.Locked = active;
                if (!active) effect.moveLock.Discard();
            }
        }
    }
    // 沉默效果
    public struct Silence:IEffect
    {
        public float endTime;
        public LockChain chain;
        public float GetEndTime() => endTime;
    }
    public class SilenceSystem : EffectSystem<Silence>
    {
        public override void AddEffect(int adder, int receiver,float value, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            var target = Tool.SceneController.GetTarget(receiver);
            SetEffectActive(true, new EffectId(adder, receiver), new Silence
            {
                endTime = Time.time + time,
                chain = target.SkillLock.GetChain()
            });
        }
        protected override EffectType GetEffectType() => EffectType.Silence;
        protected override void SetEffectActive(bool active, EffectId id, Silence effect)
        {
            base.SetEffectActive(active, id, effect);
            if (effect.chain.InUse)
            {
                effect.chain.Locked = active;
                if (!active) effect.chain.Discard();
            }
        }
    }
    // 麻痹效果（间歇性无法行动）
    public struct Paralysis:IEffect
    {
        public float endTime;
        public float interval; // 麻痹间隔时间
        public float nextUpdateTime;
        public float GetEndTime() => endTime;
    }
    public class ParalysisSystem : EffectSystem<Paralysis>
    {
        private Dictionary<EffectId,Paralysis>Temp=new Dictionary<EffectId,Paralysis>();
        public override void AddEffect(int adder, int receiver, float interval, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            SetEffectActive(true,new EffectId(adder, receiver), new Paralysis
            {
                endTime = Time.time + time,
                interval = interval,
                nextUpdateTime = Time.time
            });
        }

        protected override void Update()
        {
            base.Update();
            if (Effects.Count > 0)
            {
                foreach (var key in Effects.Keys)
                {
                    if (Time.time > Effects[key].nextUpdateTime)
                    {
                        var t = key.GetReceiver();
                        t.Interrupt();
                        t.ApplyMotion(new MotionStatic(0.3f, false, 0));
                        var p = Effects[key];
                        Temp.Add(key,new Paralysis() { interval = p.interval, endTime = p.endTime, nextUpdateTime = Time.time + 1 });
                    }
                }
                if (Temp.Count > 0) {
                    foreach (var key in Temp.Keys)
                    {
                        Effects[key] = Temp[key];
                    }
                    Temp.Clear();
                }
            }
        }
        protected override EffectType GetEffectType() => EffectType.Paralysis;
    }
}