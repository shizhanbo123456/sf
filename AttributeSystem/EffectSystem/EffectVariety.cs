using AttributeSystem.Attributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
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
        public Target GetAdder() => Tool.SceneController.GetTarget(adder);
        public Target GetReceiver() => Tool.SceneController.GetTarget(receiver);
        public override int GetHashCode()
        {
            return adder * 10000 + receiver;
        }
    }
    public struct HealthRegeneration
    {
        private int value;
        private float endTime;
        private float nextUpdateTime;

        private static Dictionary<EffectId, HealthRegeneration> Effects = new();
        public static void AddEffect(int adder, int receiver, int value, float time)
        {
            if(!GlobalEffectManager.TargetCheck(receiver)) return;
            EnableEvents();
            Effects.Add(new EffectId(adder, receiver), new HealthRegeneration()
            { value = value, endTime = Time.time + time, nextUpdateTime = Time.time + 1f });
        }
        private static void Update()
        {
            foreach (var key in Effects.Keys)
            {
                if (Time.time > Effects[key].endTime) GlobalEffectManager.ToRemove.Add(key);
            }
            if (GlobalEffectManager.ToRemove.Count > 0)
            {
                foreach (var i in GlobalEffectManager.ToRemove)
                {
                    var t = i.GetReceiver();
                    if (t && t.effectController) t.effectController.EffectEnd(i.adder, EffectType.HealthRegeneration);
                    Effects.Remove(i);
                }
                GlobalEffectManager.ToRemove.Clear();
                DisableEvents();
            }
            if (Effects.Count > 0)
            {
                foreach (var key in Effects.Keys)
                {
                    if (Time.time > Effects[key].nextUpdateTime)
                    {
                        var p = Effects[key];
                        Effects[key] = new HealthRegeneration() { value = p.value, endTime = p.endTime, nextUpdateTime = Time.time + 1 };
                        var t=Tool.SceneController.GetTarget(key.receiver);
                        if (t != null) t.Shengming += p.value;
                    }
                }
            }
        }
        private static void OnTargetDestroyed(int id)
        {
            foreach (var key in Effects.Keys)
            {
                if (key.adder == id || key.receiver == id)
                {
                    GlobalEffectManager.ToRemove.Add(key);
                }
            }
            foreach (var i in GlobalEffectManager.ToRemove) Effects.Remove(i);
            GlobalEffectManager.ToRemove.Clear();
            DisableEvents();
        }
        private static bool eventsActive = false;
        private static void EnableEvents()
        {
            if (eventsActive) return;
            if (Effects.Count == 0) return;
            GlobalEffectManager.EffectUpdate += Update;
            GlobalEffectManager.OnTargetDestroyed += OnTargetDestroyed;
            eventsActive = true;
        }
        private static void DisableEvents()
        {
            if (!eventsActive) return;
            if (Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate -= Update;
            GlobalEffectManager.OnTargetDestroyed -= OnTargetDestroyed;
            eventsActive = false;
        }
    }
    // 燃烧效果实现
    public struct Burning
    {
        private int damageValue;
        private float endTime;
        private float nextUpdateTime;

        private static Dictionary<EffectId, Burning> Effects = new();
        public static void AddEffect(int adder, int receiver, int value, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            EnableEvents();
            Effects.Add(new EffectId(adder, receiver), new Burning
            {
                damageValue = value,
                endTime = Time.time + time,
                nextUpdateTime = Time.time + 1f
            });
        }

        private static void Update()
        {
            // 移除过期效果
            foreach (var key in Effects.Keys)
            {
                if (Time.time > Effects[key].endTime)
                    GlobalEffectManager.ToRemove.Add(key);
            }

            // 处理移除
            if (GlobalEffectManager.ToRemove.Count > 0)
            {
                foreach (var i in GlobalEffectManager.ToRemove)
                {
                    var t = i.GetReceiver();
                    if (t && t.effectController) t.effectController.EffectEnd(i.adder, EffectType.Burning);
                    Effects.Remove(i);
                }
                GlobalEffectManager.ToRemove.Clear();
                DisableEvents();
            }

            // 执行周期性伤害
            if (Effects.Count > 0)
            {
                foreach (var key in Effects.Keys)
                {
                    var effect = Effects[key];
                    if (Time.time > effect.nextUpdateTime)
                    {
                        var receiver = key.GetReceiver();
                        if (receiver != null)
                        {
                            var attributes = receiver.FloatingAttributes;
                            if (attributes.Shengming.Value > 1)
                            {
                                attributes.Shengming.Value = Mathf.Max(1, attributes.Shengming.Value - effect.damageValue);
                            }
                        }

                        // 更新下次触发时间
                        Effects[key] = new Burning
                        {
                            damageValue = effect.damageValue,
                            endTime = effect.endTime,
                            nextUpdateTime = Time.time + 1f
                        };
                    }
                }
            }
        }

        private static void OnTargetDestroyed(int id)
        {
            foreach (var key in Effects.Keys)
            {
                if (key.adder == id || key.receiver == id)
                    GlobalEffectManager.ToRemove.Add(key);
            }

            foreach (var i in GlobalEffectManager.ToRemove)
                Effects.Remove(i);
            GlobalEffectManager.ToRemove.Clear();
            DisableEvents();
        }

        private static bool eventsActive = false;
        private static void EnableEvents()
        {
            if (eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate += Update;
            GlobalEffectManager.OnTargetDestroyed += OnTargetDestroyed;
            eventsActive = true;
        }

        private static void DisableEvents()
        {
            if (!eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate -= Update;
            GlobalEffectManager.OnTargetDestroyed -= OnTargetDestroyed;
            eventsActive = false;
        }
    }

    // 速度加成效果
    public struct Speed
    {
        private float value;
        private float endTime;

        private static Dictionary<EffectId, Speed> Effects = new();
        public static void AddEffect(int adder, int receiver, float value, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            EnableEvents();
            var target = Tool.SceneController.GetTarget(receiver);
            if (target != null)
            {
                target.FloatingAttributes.Jixing.Value += value;

                Effects.Add(new EffectId(adder, receiver), new Speed
                {
                    value = value,
                    endTime = Time.time + time,
                });
            }
        }

        private static void Update()
        {
            // 移除过期效果并恢复属性
            foreach (var key in Effects.Keys)
            {
                if (Time.time > Effects[key].endTime)
                {
                    var receiver = key.GetReceiver();
                    if (receiver && receiver.effectController)
                    {
                        receiver.effectController.EffectEnd(key.adder, EffectType.Speed);
                        receiver.FloatingAttributes.Jixing.Value -= Effects[key].value;
                    }
                    GlobalEffectManager.ToRemove.Add(key);
                }
            }

            // 处理移除
            if (GlobalEffectManager.ToRemove.Count > 0)
            {
                foreach (var i in GlobalEffectManager.ToRemove)
                    Effects.Remove(i);
                GlobalEffectManager.ToRemove.Clear();
                DisableEvents();
            }
        }

        private static void OnTargetDestroyed(int id)
        {
            foreach (var key in Effects.Keys)
            {
                if (key.adder == id || key.receiver == id)
                {
                    GlobalEffectManager.ToRemove.Add(key);
                }
            }

            foreach (var i in GlobalEffectManager.ToRemove)
                Effects.Remove(i);
            GlobalEffectManager.ToRemove.Clear();
            DisableEvents();
        }

        private static bool eventsActive = false;
        private static void EnableEvents()
        {
            if (eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate += Update;
            GlobalEffectManager.OnTargetDestroyed += OnTargetDestroyed;
            eventsActive = true;
        }

        private static void DisableEvents()
        {
            if (!eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate -= Update;
            GlobalEffectManager.OnTargetDestroyed -= OnTargetDestroyed;
            eventsActive = false;
        }
    }

    // 减速效果 (与速度效果类似，仅方向相反)
    public struct Slowness
    {
        private float value;
        private float endTime;

        private static Dictionary<EffectId, Slowness> Effects = new();
        public static void AddEffect(int adder, int receiver, float value, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            EnableEvents();
            var target = Tool.SceneController.GetTarget(receiver);
            if (target != null)
            {
                target.FloatingAttributes.Jixing.Value -= value;

                Effects.Add(new EffectId(adder, receiver), new Slowness
                {
                    value = value,
                    endTime = Time.time + time
                });
            }
        }

        private static void Update()
        {
            foreach (var key in Effects.Keys)
            {
                if (Time.time > Effects[key].endTime)
                {
                    var receiver = key.GetReceiver();
                    if (receiver && receiver.effectController)
                    {
                        receiver.effectController.EffectEnd(key.adder, EffectType.Slowness);
                        receiver.FloatingAttributes.Jixing.Value += Effects[key].value;
                    }
                    GlobalEffectManager.ToRemove.Add(key);
                }
            }

            if (GlobalEffectManager.ToRemove.Count > 0)
            {
                foreach (var i in GlobalEffectManager.ToRemove)
                    Effects.Remove(i);
                GlobalEffectManager.ToRemove.Clear();
                DisableEvents();
            }
        }

        private static void OnTargetDestroyed(int id)
        {
            foreach (var key in Effects.Keys)
            {
                if (key.adder == id || key.receiver == id)
                {
                    GlobalEffectManager.ToRemove.Add(key);
                }
            }

            foreach (var i in GlobalEffectManager.ToRemove)
                Effects.Remove(i);
            GlobalEffectManager.ToRemove.Clear();
            DisableEvents();
        }

        private static bool eventsActive = false;
        private static void EnableEvents()
        {
            if (eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate += Update;
            GlobalEffectManager.OnTargetDestroyed += OnTargetDestroyed;
            eventsActive = true;
        }

        private static void DisableEvents()
        {
            if (!eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate -= Update;
            GlobalEffectManager.OnTargetDestroyed -= OnTargetDestroyed;
            eventsActive = false;
        }
    }

    // 跳跃提升效果
    public struct JumpBoost
    {
        private float value;
        private float endTime;

        private static Dictionary<EffectId, JumpBoost> Effects = new();
        public static void AddEffect(int adder, int receiver, float value, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            EnableEvents();
            var target = Tool.SceneController.GetTarget(receiver);
            if (target != null)
            {
                target.FloatingAttributes.Tengkong.Value += value;

                Effects.Add(new EffectId(adder, receiver), new JumpBoost
                {
                    value = value,
                    endTime = Time.time + time,
                });
            }
        }

        private static void Update()
        {
            foreach (var key in Effects.Keys)
            {
                if (Time.time > Effects[key].endTime)
                {
                    var receiver = key.GetReceiver();
                    if (receiver && receiver.effectController)
                    {
                        receiver.effectController.EffectEnd(key.adder, EffectType.JumpBoost);
                        receiver.FloatingAttributes.Tengkong.Value -= Effects[key].value;
                    }
                    GlobalEffectManager.ToRemove.Add(key);
                }
            }

            if (GlobalEffectManager.ToRemove.Count > 0)
            {
                foreach (var i in GlobalEffectManager.ToRemove)
                    Effects.Remove(i);
                GlobalEffectManager.ToRemove.Clear();
                DisableEvents();
            }
        }

        private static void OnTargetDestroyed(int id)
        {
            foreach (var key in Effects.Keys)
            {
                if (key.adder == id || key.receiver == id)
                {
                    GlobalEffectManager.ToRemove.Add(key);
                }
            }

            foreach (var i in GlobalEffectManager.ToRemove)
                Effects.Remove(i);
            GlobalEffectManager.ToRemove.Clear();
            DisableEvents();
        }

        private static bool eventsActive = false;
        private static void EnableEvents()
        {
            if (eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate += Update;
            GlobalEffectManager.OnTargetDestroyed += OnTargetDestroyed;
            eventsActive = true;
        }

        private static void DisableEvents()
        {
            if (!eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate -= Update;
            GlobalEffectManager.OnTargetDestroyed -= OnTargetDestroyed;
            eventsActive = false;
        }
    }

    // 敏捷提升效果
    public struct AgileBoost
    {
        private int value;
        private float endTime;

        private static Dictionary<EffectId, AgileBoost> Effects = new();
        public static void AddEffect(int adder, int receiver, float rate, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            EnableEvents();
            var target = Tool.SceneController.GetTarget(receiver);
            if (target != null)
            {
                var attr = target.FloatingAttributes;
                var value = (int)(attr.Shanbi.Value * rate);
                attr.Shanbi.Value += value;

                Effects.Add(new EffectId(adder, receiver), new AgileBoost
                {
                    value = value,
                    endTime = Time.time + time,
                });
            }
        }

        private static void Update()
        {
            foreach (var key in Effects.Keys)
            {
                if (Time.time > Effects[key].endTime)
                {
                    var receiver = key.GetReceiver();
                    if (receiver && receiver.effectController)
                    {
                        receiver.effectController.EffectEnd(key.adder, EffectType.AgileBoost);
                        receiver.FloatingAttributes.Shanbi.Value -= Effects[key].value;
                    }
                    GlobalEffectManager.ToRemove.Add(key);
                }
            }

            if (GlobalEffectManager.ToRemove.Count > 0)
            {
                foreach (var i in GlobalEffectManager.ToRemove)
                    Effects.Remove(i);
                GlobalEffectManager.ToRemove.Clear();
                DisableEvents();
            }
        }

        private static void OnTargetDestroyed(int id)
        {
            foreach (var key in Effects.Keys)
            {
                if (key.adder == id || key.receiver == id)
                {
                    GlobalEffectManager.ToRemove.Add(key);
                }
            }

            foreach (var i in GlobalEffectManager.ToRemove)
                Effects.Remove(i);
            GlobalEffectManager.ToRemove.Clear();
            DisableEvents();
        }

        private static bool eventsActive = false;
        private static void EnableEvents()
        {
            if (eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate += Update;
            GlobalEffectManager.OnTargetDestroyed += OnTargetDestroyed;
            eventsActive = true;
        }

        private static void DisableEvents()
        {
            if (!eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate -= Update;
            GlobalEffectManager.OnTargetDestroyed -= OnTargetDestroyed;
            eventsActive = false;
        }
    }

    // 精准提升效果
    public struct AccuracyBoost
    {
        private int value;
        private float endTime;

        private static Dictionary<EffectId, AccuracyBoost> Effects = new();
        public static void AddEffect(int adder, int receiver, float rate, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            EnableEvents();
            var target = Tool.SceneController.GetTarget(receiver);
            if (target != null)
            {
                var baseAttr = target.FloatingAttributes;
                var value = (int)(baseAttr.Mingzhong.Value * rate);
                baseAttr.Mingzhong.Value += value;

                Effects.Add(new EffectId(adder, receiver), new AccuracyBoost
                {
                    value = value,
                    endTime = Time.time + time
                });
            }
        }

        private static void Update()
        {
            foreach (var key in Effects.Keys)
            {
                if (Time.time > Effects[key].endTime)
                {
                    var receiver = key.GetReceiver();
                    if (receiver && receiver.effectController)
                    {
                        receiver.effectController.EffectEnd(key.adder, EffectType.AccuracyBoost);
                        receiver.FloatingAttributes.Mingzhong.Value -= Effects[key].value;
                    }
                    GlobalEffectManager.ToRemove.Add(key);
                }
            }

            if (GlobalEffectManager.ToRemove.Count > 0)
            {
                foreach (var i in GlobalEffectManager.ToRemove)
                    Effects.Remove(i);
                GlobalEffectManager.ToRemove.Clear();
                DisableEvents();
            }
        }

        private static void OnTargetDestroyed(int id)
        {
            foreach (var key in Effects.Keys)
            {
                if (key.adder == id || key.receiver == id)
                {
                    GlobalEffectManager.ToRemove.Add(key);
                }
            }

            foreach (var i in GlobalEffectManager.ToRemove)
                Effects.Remove(i);
            GlobalEffectManager.ToRemove.Clear();
            DisableEvents();
        }

        private static bool eventsActive = false;
        private static void EnableEvents()
        {
            if (eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate += Update;
            GlobalEffectManager.OnTargetDestroyed += OnTargetDestroyed;
            eventsActive = true;
        }

        private static void DisableEvents()
        {
            if (!eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate -= Update;
            GlobalEffectManager.OnTargetDestroyed -= OnTargetDestroyed;
            eventsActive = false;
        }
    }

    // 攻击提升效果
    public struct AttackBoost
    {
        private int value;
        private float endTime;

        private static Dictionary<EffectId, AttackBoost> Effects = new();
        public static void AddEffect(int adder, int receiver, float rate, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            EnableEvents();
            var target = Tool.SceneController.GetTarget(receiver);
            if (target != null)
            {
                var attr = target.FloatingAttributes;
                var value = (int)(attr.Gongji.Value * rate);
                attr.Gongji.Value += value;

                Effects.Add(new EffectId(adder, receiver), new AttackBoost
                {
                    value = value,
                    endTime = Time.time + time
                });
            }
        }

        private static void Update()
        {
            foreach (var key in Effects.Keys)
            {
                if (Time.time > Effects[key].endTime)
                {
                    var receiver = key.GetReceiver();
                    if (receiver && receiver.effectController)
                    {
                        receiver.effectController.EffectEnd(key.adder, EffectType.AttackBoost);
                        receiver.FloatingAttributes.Gongji.Value -= Effects[key].value;
                    }
                    GlobalEffectManager.ToRemove.Add(key);
                }
            }

            if (GlobalEffectManager.ToRemove.Count > 0)
            {
                foreach (var i in GlobalEffectManager.ToRemove)
                    Effects.Remove(i);
                GlobalEffectManager.ToRemove.Clear();
                DisableEvents();
            }
        }

        private static void OnTargetDestroyed(int id)
        {
            foreach (var key in Effects.Keys)
            {
                if (key.adder == id || key.receiver == id)
                {
                    GlobalEffectManager.ToRemove.Add(key);
                }
            }

            foreach (var i in GlobalEffectManager.ToRemove)
                Effects.Remove(i);
            GlobalEffectManager.ToRemove.Clear();
            DisableEvents();
        }

        private static bool eventsActive = false;
        private static void EnableEvents()
        {
            if (eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate += Update;
            GlobalEffectManager.OnTargetDestroyed += OnTargetDestroyed;
            eventsActive = true;
        }

        private static void DisableEvents()
        {
            if (!eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate -= Update;
            GlobalEffectManager.OnTargetDestroyed -= OnTargetDestroyed;
            eventsActive = false;
        }
    }

    // 防御提升效果
    public struct DefenseBoost
    {
        private int value;
        private float endTime;

        private static Dictionary<EffectId, DefenseBoost> Effects = new();
        public static void AddEffect(int adder, int receiver, float rate, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            EnableEvents();
            var target = Tool.SceneController.GetTarget(receiver);
            if (target != null)
            {
                var attr = target.FloatingAttributes;
                var value = (int)(attr.Fangyu.Value * rate);
                attr.Fangyu.Value += value;

                Effects.Add(new EffectId(adder, receiver), new DefenseBoost
                {
                    value = value,
                    endTime = Time.time + time
                });
            }
        }

        private static void Update()
        {
            foreach (var key in Effects.Keys)
            {
                if (Time.time > Effects[key].endTime)
                {
                    var receiver = key.GetReceiver();
                    if (receiver && receiver.effectController)
                    {
                        receiver.effectController.EffectEnd(key.adder, EffectType.DefenseBoost);
                        receiver.FloatingAttributes.Fangyu.Value -= Effects[key].value;
                    }
                    GlobalEffectManager.ToRemove.Add(key);
                }
            }

            if (GlobalEffectManager.ToRemove.Count > 0)
            {
                foreach (var i in GlobalEffectManager.ToRemove)
                    Effects.Remove(i);
                GlobalEffectManager.ToRemove.Clear();
                DisableEvents();
            }
        }

        private static void OnTargetDestroyed(int id)
        {
            foreach (var key in Effects.Keys)
            {
                if (key.adder == id || key.receiver == id)
                {
                    GlobalEffectManager.ToRemove.Add(key);
                }
            }

            foreach (var i in GlobalEffectManager.ToRemove)
                Effects.Remove(i);
            GlobalEffectManager.ToRemove.Clear();
            DisableEvents();
        }

        private static bool eventsActive = false;
        private static void EnableEvents()
        {
            if (eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate += Update;
            GlobalEffectManager.OnTargetDestroyed += OnTargetDestroyed;
            eventsActive = true;
        }

        private static void DisableEvents()
        {
            if (!eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate -= Update;
            GlobalEffectManager.OnTargetDestroyed -= OnTargetDestroyed;
            eventsActive = false;
        }
    }

    // 敏捷降低效果
    public struct AgileDecrease
    {
        private int value;
        private float endTime;

        private static Dictionary<EffectId, AgileDecrease> Effects = new();
        public static void AddEffect(int adder, int receiver, float rate, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            EnableEvents();
            var target = Tool.SceneController.GetTarget(receiver);
            if (target != null)
            {
                var baseAttr = target.FloatingAttributes;
                var value = (int)(baseAttr.Shanbi.Value * rate);
                var initial = baseAttr.Shanbi.Value;
                baseAttr.Shanbi.Value -= value;

                Effects.Add(new EffectId(adder, receiver), new AgileDecrease
                {
                    value = value,
                    endTime = Time.time + time,
                });
            }
        }

        private static void Update()
        {
            foreach (var key in Effects.Keys)
            {
                if (Time.time > Effects[key].endTime)
                {
                    var receiver = key.GetReceiver();
                    if (receiver && receiver.effectController)
                    {
                        receiver.effectController.EffectEnd(key.adder, EffectType.AgileDecrease);
                        receiver.FloatingAttributes.Shanbi.Value += Effects[key].value;
                    }
                    GlobalEffectManager.ToRemove.Add(key);
                }
            }

            if (GlobalEffectManager.ToRemove.Count > 0)
            {
                foreach (var i in GlobalEffectManager.ToRemove)
                    Effects.Remove(i);
                GlobalEffectManager.ToRemove.Clear();
                DisableEvents();
            }
        }

        private static void OnTargetDestroyed(int id)
        {
            foreach (var key in Effects.Keys)
            {
                if (key.adder == id || key.receiver == id)
                {
                    GlobalEffectManager.ToRemove.Add(key);
                }
            }

            foreach (var i in GlobalEffectManager.ToRemove)
                Effects.Remove(i);
            GlobalEffectManager.ToRemove.Clear();
            DisableEvents();
        }

        private static bool eventsActive = false;
        private static void EnableEvents()
        {
            if (eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate += Update;
            GlobalEffectManager.OnTargetDestroyed += OnTargetDestroyed;
            eventsActive = true;
        }

        private static void DisableEvents()
        {
            if (!eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate -= Update;
            GlobalEffectManager.OnTargetDestroyed -= OnTargetDestroyed;
            eventsActive = false;
        }
    }

    // 精准度降低效果
    public struct AccuracyDecrease
    {
        private int value;
        private float endTime;

        private static Dictionary<EffectId, AccuracyDecrease> Effects = new();
        public static void AddEffect(int adder, int receiver, float rate, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            EnableEvents();
            var target = Tool.SceneController.GetTarget(receiver);
            if (target != null)
            {
                var attr = target.FloatingAttributes;
                var value = (int)(attr.Mingzhong.Value * rate);
                attr.Mingzhong.Value -= value;

                Effects.Add(new EffectId(adder, receiver), new AccuracyDecrease
                {
                    value = value,
                    endTime = Time.time + time
                });
            }
        }

        private static void Update()
        {
            foreach (var key in Effects.Keys)
            {
                if (Time.time > Effects[key].endTime)
                {
                    var receiver = key.GetReceiver();
                    if (receiver && receiver.effectController)
                    {
                        receiver.effectController.EffectEnd(key.adder, EffectType.AccuracyDecrease);
                        receiver.FloatingAttributes.Mingzhong.Value += Effects[key].value;
                    }
                    GlobalEffectManager.ToRemove.Add(key);
                }
            }

            if (GlobalEffectManager.ToRemove.Count > 0)
            {
                foreach (var i in GlobalEffectManager.ToRemove)
                    Effects.Remove(i);
                GlobalEffectManager.ToRemove.Clear();
                DisableEvents();
            }
        }

        private static void OnTargetDestroyed(int id)
        {
            foreach (var key in Effects.Keys)
            {
                if (key.adder == id || key.receiver == id)
                {
                    GlobalEffectManager.ToRemove.Add(key);
                }
            }

            foreach (var i in GlobalEffectManager.ToRemove)
                Effects.Remove(i);
            GlobalEffectManager.ToRemove.Clear();
            DisableEvents();
        }

        private static bool eventsActive = false;
        private static void EnableEvents()
        {
            if (eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate += Update;
            GlobalEffectManager.OnTargetDestroyed += OnTargetDestroyed;
            eventsActive = true;
        }

        private static void DisableEvents()
        {
            if (!eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate -= Update;
            GlobalEffectManager.OnTargetDestroyed -= OnTargetDestroyed;
            eventsActive = false;
        }
    }

    // 攻击降低效果
    public struct AttackDecrease
    {
        private int value;
        private float endTime;

        private static Dictionary<EffectId, AttackDecrease> Effects = new();
        public static void AddEffect(int adder, int receiver, float rate, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            EnableEvents();
            var target = Tool.SceneController.GetTarget(receiver);
            if (target != null)
            {
                var attr = target.FloatingAttributes;
                var value = (int)(attr.Gongji.Value * rate);
                attr.Gongji.Value -= value;

                Effects.Add(new EffectId(adder, receiver), new AttackDecrease
                {
                    value = value,
                    endTime = Time.time + time
                });
            }
        }

        private static void Update()
        {
            foreach (var key in Effects.Keys)
            {
                if (Time.time > Effects[key].endTime)
                {
                    var receiver = key.GetReceiver();
                    if (receiver && receiver.effectController)
                    {
                        receiver.effectController.EffectEnd(key.adder, EffectType.AttackDecrease);
                        receiver.FloatingAttributes.Gongji.Value += Effects[key].value;
                    }
                    GlobalEffectManager.ToRemove.Add(key);
                }
            }

            if (GlobalEffectManager.ToRemove.Count > 0)
            {
                foreach (var i in GlobalEffectManager.ToRemove)
                    Effects.Remove(i);
                GlobalEffectManager.ToRemove.Clear();
                DisableEvents();
            }
        }

        private static void OnTargetDestroyed(int id)
        {
            foreach (var key in Effects.Keys)
            {
                if (key.adder == id || key.receiver == id)
                {
                    GlobalEffectManager.ToRemove.Add(key);
                }
            }

            foreach (var i in GlobalEffectManager.ToRemove)
                Effects.Remove(i);
            GlobalEffectManager.ToRemove.Clear();
            DisableEvents();
        }

        private static bool eventsActive = false;
        private static void EnableEvents()
        {
            if (eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate += Update;
            GlobalEffectManager.OnTargetDestroyed += OnTargetDestroyed;
            eventsActive = true;
        }

        private static void DisableEvents()
        {
            if (!eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate -= Update;
            GlobalEffectManager.OnTargetDestroyed -= OnTargetDestroyed;
            eventsActive = false;
        }
    }

    // 防御降低效果
    public struct DefenseDecrease
    {
        private int value;
        private float endTime;

        private static Dictionary<EffectId, DefenseDecrease> Effects = new();
        public static void AddEffect(int adder, int receiver, float rate, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            EnableEvents();
            var target = Tool.SceneController.GetTarget(receiver);
            if (target != null)
            {
                var attr = target.FloatingAttributes;
                var value = (int)(attr.Fangyu.Value * rate);
                attr.Fangyu.Value -= value;

                Effects.Add(new EffectId(adder, receiver), new DefenseDecrease
                {
                    value = value,
                    endTime = Time.time + time,
                });
            }
        }

        private static void Update()
        {
            foreach (var key in Effects.Keys)
            {
                if (Time.time > Effects[key].endTime)
                {
                    var receiver = key.GetReceiver();
                    if (receiver && receiver.effectController)
                    {
                        receiver.effectController.EffectEnd(key.adder, EffectType.DefenseDecrease);
                        receiver.FloatingAttributes.Fangyu.Value += Effects[key].value;
                    }
                    GlobalEffectManager.ToRemove.Add(key);
                }
            }

            if (GlobalEffectManager.ToRemove.Count > 0)
            {
                foreach (var i in GlobalEffectManager.ToRemove)
                    Effects.Remove(i);
                GlobalEffectManager.ToRemove.Clear();
                DisableEvents();
            }
        }

        private static void OnTargetDestroyed(int id)
        {
            foreach (var key in Effects.Keys)
            {
                if (key.adder == id || key.receiver == id)
                {
                    GlobalEffectManager.ToRemove.Add(key);
                }
            }

            foreach (var i in GlobalEffectManager.ToRemove)
                Effects.Remove(i);
            GlobalEffectManager.ToRemove.Clear();
            DisableEvents();
        }

        private static bool eventsActive = false;
        private static void EnableEvents()
        {
            if (eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate += Update;
            GlobalEffectManager.OnTargetDestroyed += OnTargetDestroyed;
            eventsActive = true;
        }

        private static void DisableEvents()
        {
            if (!eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate -= Update;
            GlobalEffectManager.OnTargetDestroyed -= OnTargetDestroyed;
            eventsActive = false;
        }
    }

    // 护甲强化效果
    public struct ArmorFortity
    {
        private int value;
        private float endTime;

        private static Dictionary<EffectId, ArmorFortity> Effects = new();
        public static void AddEffect(int adder, int receiver, int value, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            EnableEvents();
            var target = Tool.SceneController.GetTarget(receiver);
            if (target != null)
            {
                var baseAttr = target.FloatingAttributes;
                baseAttr.Jianshang.Value += value;

                Effects.Add(new EffectId(adder, receiver), new ArmorFortity
                {
                    value = value,
                    endTime = Time.time + time
                });
            }
        }

        private static void Update()
        {
            foreach (var key in Effects.Keys)
            {
                if (Time.time > Effects[key].endTime)
                {
                    var receiver = key.GetReceiver();
                    if (receiver && receiver.effectController)
                    {
                        receiver.effectController.EffectEnd(key.adder, EffectType.ArmorFortity);
                        receiver.FloatingAttributes.Jianshang.Value -= Effects[key].value;
                    }
                    GlobalEffectManager.ToRemove.Add(key);
                }
            }

            if (GlobalEffectManager.ToRemove.Count > 0)
            {
                foreach (var i in GlobalEffectManager.ToRemove)
                    Effects.Remove(i);
                GlobalEffectManager.ToRemove.Clear();
                DisableEvents();
            }
        }

        private static void OnTargetDestroyed(int id)
        {
            foreach (var key in Effects.Keys)
            {
                if (key.adder == id || key.receiver == id)
                {
                    GlobalEffectManager.ToRemove.Add(key);
                }
            }

            foreach (var i in GlobalEffectManager.ToRemove)
                Effects.Remove(i);
            GlobalEffectManager.ToRemove.Clear();
            DisableEvents();
        }

        private static bool eventsActive = false;
        private static void EnableEvents()
        {
            if (eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate += Update;
            GlobalEffectManager.OnTargetDestroyed += OnTargetDestroyed;
            eventsActive = true;
        }

        private static void DisableEvents()
        {
            if (!eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate -= Update;
            GlobalEffectManager.OnTargetDestroyed -= OnTargetDestroyed;
            eventsActive = false;
        }
    }

    // 护甲破碎效果
    public struct ArmorShatter
    {
        private int value;
        private float endTime;

        private static Dictionary<EffectId, ArmorShatter> Effects = new();
        public static void AddEffect(int adder, int receiver, int value, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            EnableEvents();
            var target = Tool.SceneController.GetTarget(receiver);
            if (target != null)
            {
                var baseAttr = target.FloatingAttributes;
                baseAttr.Jianshang.Value -= value;

                Effects.Add(new EffectId(adder, receiver), new ArmorShatter
                {
                    value = value,
                    endTime = Time.time + time
                });
            }
        }

        private static void Update()
        {
            foreach (var key in Effects.Keys)
            {
                if (Time.time > Effects[key].endTime)
                {
                    var receiver = key.GetReceiver();
                    if (receiver && receiver.effectController)
                    {
                        receiver.effectController.EffectEnd(key.adder, EffectType.ArmorShatter);
                        receiver.FloatingAttributes.Jianshang.Value += Effects[key].value;
                    }
                    GlobalEffectManager.ToRemove.Add(key);
                }
            }

            if (GlobalEffectManager.ToRemove.Count > 0)
            {
                foreach (var i in GlobalEffectManager.ToRemove)
                    Effects.Remove(i);
                GlobalEffectManager.ToRemove.Clear();
                DisableEvents();
            }
        }

        private static void OnTargetDestroyed(int id)
        {
            foreach (var key in Effects.Keys)
            {
                if (key.adder == id || key.receiver == id)
                {
                    GlobalEffectManager.ToRemove.Add(key);
                }
            }

            foreach (var i in GlobalEffectManager.ToRemove)
                Effects.Remove(i);
            GlobalEffectManager.ToRemove.Clear();
            DisableEvents();
        }

        private static bool eventsActive = false;
        private static void EnableEvents()
        {
            if (eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate += Update;
            GlobalEffectManager.OnTargetDestroyed += OnTargetDestroyed;
            eventsActive = true;
        }

        private static void DisableEvents()
        {
            if (!eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate -= Update;
            GlobalEffectManager.OnTargetDestroyed -= OnTargetDestroyed;
            eventsActive = false;
        }
    }

    // 伤害提升效果
    public struct DamageBoost
    {
        private int value;
        private float endTime;

        private static Dictionary<EffectId, DamageBoost> Effects = new();
        public static void AddEffect(int adder, int receiver,int value, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            EnableEvents();
            var target = Tool.SceneController.GetTarget(receiver);
            if (target != null)
            {
                var attr = target.FloatingAttributes;
                attr.Jiashang.Value += value;

                Effects.Add(new EffectId(adder, receiver), new DamageBoost
                {
                    value = value,
                    endTime = Time.time + time
                });
            }
        }

        private static void Update()
        {
            foreach (var key in Effects.Keys)
            {
                if (Time.time > Effects[key].endTime)
                {
                    var receiver = key.GetReceiver();
                    if (receiver && receiver.effectController)
                    {
                        receiver.effectController.EffectEnd(key.adder, EffectType.DamageBoost);
                        receiver.FloatingAttributes.Jiashang.Value -= Effects[key].value;
                    }
                    GlobalEffectManager.ToRemove.Add(key);
                }
            }

            if (GlobalEffectManager.ToRemove.Count > 0)
            {
                foreach (var i in GlobalEffectManager.ToRemove)
                    Effects.Remove(i);
                GlobalEffectManager.ToRemove.Clear();
                DisableEvents();
            }
        }

        private static void OnTargetDestroyed(int id)
        {
            foreach (var key in Effects.Keys)
            {
                if (key.adder == id || key.receiver == id)
                {
                    GlobalEffectManager.ToRemove.Add(key);
                }
            }

            foreach (var i in GlobalEffectManager.ToRemove)
                Effects.Remove(i);
            GlobalEffectManager.ToRemove.Clear();
            DisableEvents();
        }

        private static bool eventsActive = false;
        private static void EnableEvents()
        {
            if (eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate += Update;
            GlobalEffectManager.OnTargetDestroyed += OnTargetDestroyed;
            eventsActive = true;
        }

        private static void DisableEvents()
        {
            if (!eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate -= Update;
            GlobalEffectManager.OnTargetDestroyed -= OnTargetDestroyed;
            eventsActive = false;
        }
    }

    // 伤害降低效果
    public struct DamageDecrease
    {
        private int value;
        private float endTime;

        private static Dictionary<EffectId, DamageDecrease> Effects = new();
        public static void AddEffect(int adder, int receiver, int value, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            EnableEvents();
            var target = Tool.SceneController.GetTarget(receiver);
            if (target != null)
            {
                var baseAttr = target.FloatingAttributes;
                baseAttr.Jiashang.Value -= value;

                Effects.Add(new EffectId(adder, receiver), new DamageDecrease
                {
                    value = value,
                    endTime = Time.time + time
                });
            }
        }

        private static void Update()
        {
            foreach (var key in Effects.Keys)
            {
                if (Time.time > Effects[key].endTime)
                {
                    var receiver = key.GetReceiver();
                    if (receiver && receiver.effectController)
                    {
                        receiver.effectController.EffectEnd(key.adder, EffectType.DamageDecrease);
                        receiver.FloatingAttributes.Jiashang.Value += Effects[key].value;
                    }
                    GlobalEffectManager.ToRemove.Add(key);
                }
            }

            if (GlobalEffectManager.ToRemove.Count > 0)
            {
                foreach (var i in GlobalEffectManager.ToRemove)
                    Effects.Remove(i);
                GlobalEffectManager.ToRemove.Clear();
                DisableEvents();
            }
        }

        private static void OnTargetDestroyed(int id)
        {
            foreach (var key in Effects.Keys)
            {
                if (key.adder == id || key.receiver == id)
                {
                    GlobalEffectManager.ToRemove.Add(key);
                }
            }

            foreach (var i in GlobalEffectManager.ToRemove)
                Effects.Remove(i);
            GlobalEffectManager.ToRemove.Clear();
            DisableEvents();
        }

        private static bool eventsActive = false;
        private static void EnableEvents()
        {
            if (eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate += Update;
            GlobalEffectManager.OnTargetDestroyed += OnTargetDestroyed;
            eventsActive = true;
        }

        private static void DisableEvents()
        {
            if (!eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate -= Update;
            GlobalEffectManager.OnTargetDestroyed -= OnTargetDestroyed;
            eventsActive = false;
        }
    }

    // 生命偷取效果
    public struct LifeSteal
    {
        private int value;
        private float endTime;

        private static Dictionary<EffectId, LifeSteal> Effects = new();
        public static void AddEffect(int adder, int receiver, float rate, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            EnableEvents();
            var t = Tool.SceneController.GetTarget(receiver);
            int v = (int)(t.BaseAttributes.Shengming.Value * rate);
            if (v >= t.Shengming) v = t.Shengming - 1;
            var l = new LifeSteal
            {
                value = v,
                endTime = Time.time + time
            };
            t.Shengming -= l.value;
            Effects.Add(new EffectId(adder, receiver),l);
        }

        private static void Update()
        {
            // 移除过期效果
            foreach (var key in Effects.Keys)
            {
                if (Time.time > Effects[key].endTime)
                    GlobalEffectManager.ToRemove.Add(key);
            }

            // 执行实际移除
            if (GlobalEffectManager.ToRemove.Count > 0)
            {
                foreach (var i in GlobalEffectManager.ToRemove)
                {
                    var receiver = i.GetReceiver();
                    if (receiver && receiver.effectController)
                    {
                        receiver.effectController.EffectEnd(i.adder, EffectType.LifeSteal);
                        receiver.Shengming += Effects[i].value;
                        Effects.Remove(i);
                    }
                }
                GlobalEffectManager.ToRemove.Clear();
                DisableEvents();
            }
        }

        private static void OnTargetDestroyed(int id)
        {
            foreach (var key in Effects.Keys)
            {
                if (key.adder == id || key.receiver == id)
                    GlobalEffectManager.ToRemove.Add(key);
            }

            foreach (var i in GlobalEffectManager.ToRemove)
                Effects.Remove(i);
            GlobalEffectManager.ToRemove.Clear();
            DisableEvents();
        }

        private static bool eventsActive = false;
        private static void EnableEvents()
        {
            if (eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate += Update;
            GlobalEffectManager.OnTargetDestroyed += OnTargetDestroyed;
            eventsActive = true;
        }

        private static void DisableEvents()
        {
            if (!eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate -= Update;
            GlobalEffectManager.OnTargetDestroyed -= OnTargetDestroyed;
            eventsActive = false;
        }
    }

    // 幸运提升效果
    public struct Luck
    {
        private int value1;
        private int value2;
        private int value3;
        private int value4;
        private float endTime;

        private static Dictionary<EffectId, Luck> Effects = new();
        public static void AddEffect(int adder, int receiver, float rate, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            EnableEvents();
            var target = Tool.SceneController.GetTarget(receiver);
            if (target != null)
            {
                var att = target.FloatingAttributes;
                var l = new Luck
                {
                    value1 = (int)(att.Mingzhong.Value * rate),
                    value2 = (int)(att.Shanbi.Value * rate),
                    value3 = (int)(att.Baoji.Value * rate),
                    value4 = (int)(att.Renxing.Value * rate),
                    endTime = Time.time + time,
                };
                att.Mingzhong.Value += l.value1;
                att.Shanbi.Value+= l.value2;
                att.Baoji.Value+= l.value3;
                att.Renxing.Value+= l.value4;
                Effects.Add(new EffectId(adder, receiver),l );
            }
        }

        private static void Update()
        {
            foreach (var key in Effects.Keys)
            {
                if (Time.time > Effects[key].endTime)
                {
                    var receiver = key.GetReceiver();
                    if (receiver && receiver.effectController)
                    {
                        receiver.effectController.EffectEnd(key.adder, EffectType.Luck);
                        var att = receiver.FloatingAttributes;
                        att.Mingzhong.Value -= Effects[key].value1;
                        att.Shanbi.Value -= Effects[key].value2;
                        att.Baoji.Value -= Effects[key].value3;
                        att.Renxing.Value -= Effects[key].value4;
                    }
                    GlobalEffectManager.ToRemove.Add(key);
                }
            }

            if (GlobalEffectManager.ToRemove.Count > 0)
            {
                foreach (var i in GlobalEffectManager.ToRemove)
                    Effects.Remove(i);
                GlobalEffectManager.ToRemove.Clear();
                DisableEvents();
            }
        }

        private static void OnTargetDestroyed(int id)
        {
            foreach (var key in Effects.Keys)
            {
                if (key.adder == id || key.receiver == id)
                {
                    GlobalEffectManager.ToRemove.Add(key);
                }
            }

            foreach (var i in GlobalEffectManager.ToRemove)
                Effects.Remove(i);
            GlobalEffectManager.ToRemove.Clear();
            DisableEvents();
        }

        private static bool eventsActive = false;
        private static void EnableEvents()
        {
            if (eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate += Update;
            GlobalEffectManager.OnTargetDestroyed += OnTargetDestroyed;
            eventsActive = true;
        }

        private static void DisableEvents()
        {
            if (!eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate -= Update;
            GlobalEffectManager.OnTargetDestroyed -= OnTargetDestroyed;
            eventsActive = false;
        }
    }

    // 厄运效果
    public struct BadLuck
    {
        private int value1;
        private int value2;
        private int value3;
        private int value4;
        private float endTime;

        private static Dictionary<EffectId, BadLuck> Effects = new();
        public static void AddEffect(int adder, int receiver, float rate, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            EnableEvents();
            var target = Tool.SceneController.GetTarget(receiver);
            if (target != null)
            {
                var att = target.FloatingAttributes;
                var l = new BadLuck
                {
                    value1 = (int)(att.Mingzhong.Value * rate),
                    value2 = (int)(att.Shanbi.Value * rate),
                    value3 = (int)(att.Baoji.Value * rate),
                    value4 = (int)(att.Renxing.Value * rate),
                    endTime = Time.time + time,
                };
                att.Mingzhong.Value -= l.value1;
                att.Shanbi.Value -= l.value2;
                att.Baoji.Value -= l.value3;
                att.Renxing.Value -= l.value4;
                Effects.Add(new EffectId(adder, receiver), l);
            }
        }

        private static void Update()
        {
            foreach (var key in Effects.Keys)
            {
                if (Time.time > Effects[key].endTime)
                {
                    var receiver = key.GetReceiver();
                    if (receiver && receiver.effectController)
                    {
                        receiver.effectController.EffectEnd(key.adder, EffectType.BadLuck);
                        var att = receiver.FloatingAttributes;
                        att.Mingzhong.Value += Effects[key].value1;
                        att.Shanbi.Value += Effects[key].value2;
                        att.Baoji.Value += Effects[key].value3;
                        att.Renxing.Value += Effects[key].value4;
                    }
                    GlobalEffectManager.ToRemove.Add(key);
                }
            }

            if (GlobalEffectManager.ToRemove.Count > 0)
            {
                foreach (var i in GlobalEffectManager.ToRemove)
                    Effects.Remove(i);
                GlobalEffectManager.ToRemove.Clear();
                DisableEvents();
            }
        }

        private static void OnTargetDestroyed(int id)
        {
            foreach (var key in Effects.Keys)
            {
                if (key.adder == id || key.receiver == id)
                {
                    GlobalEffectManager.ToRemove.Add(key);
                }
            }

            foreach (var i in GlobalEffectManager.ToRemove)
                Effects.Remove(i);
            GlobalEffectManager.ToRemove.Clear();
            DisableEvents();
        }

        private static bool eventsActive = false;
        private static void EnableEvents()
        {
            if (eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate += Update;
            GlobalEffectManager.OnTargetDestroyed += OnTargetDestroyed;
            eventsActive = true;
        }

        private static void DisableEvents()
        {
            if (!eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate -= Update;
            GlobalEffectManager.OnTargetDestroyed -= OnTargetDestroyed;
            eventsActive = false;
        }
    }

    // 冻结效果
    public struct Freeze
    {
        private float endTime;
        private LockChain moveLock;
        private LockChain skillLock;

        private static Dictionary<EffectId, Freeze> Effects = new();
        public static void AddEffect(int adder, int receiver, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            EnableEvents();
            var target = Tool.SceneController.GetTarget(receiver);
            if (target != null)
            {
                var moveLock = target.OperationLock.GetChain();
                var skillLock = target.SkillLock.GetChain();
                moveLock.Locked = true;
                skillLock.Locked = true;
                Effects.Add(new EffectId(adder, receiver), new Freeze
                {
                    endTime = Time.time + time,
                    moveLock = moveLock,
                    skillLock = skillLock
                });
            }
        }

        private static void Update()
        {
            foreach (var key in Effects.Keys)
            {
                if (Time.time > Effects[key].endTime)
                {
                    Effects[key].moveLock.Locked = false;
                    Effects[key].skillLock.Locked = false;
                    Effects[key].moveLock.Discard();
                    Effects[key].skillLock.Discard();
                    GlobalEffectManager.ToRemove.Add(key);
                }
            }

            if (GlobalEffectManager.ToRemove.Count > 0)
            {
                foreach (var i in GlobalEffectManager.ToRemove)
                {
                    var receiver = i.GetReceiver();
                    if (receiver && receiver.effectController)
                    {
                        receiver.effectController.EffectEnd(i.adder, EffectType.Freeze);
                    }
                    Effects.Remove(i);
                }
                GlobalEffectManager.ToRemove.Clear();
                DisableEvents();
            }
        }

        private static void OnTargetDestroyed(int id)
        {
            foreach (var key in Effects.Keys)
            {
                if (key.adder == id || key.receiver == id)
                {
                    GlobalEffectManager.ToRemove.Add(key);
                }
            }

            foreach (var i in GlobalEffectManager.ToRemove)
                Effects.Remove(i);
            GlobalEffectManager.ToRemove.Clear();
            DisableEvents();
        }

        private static bool eventsActive = false;
        private static void EnableEvents()
        {
            if (eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate += Update;
            GlobalEffectManager.OnTargetDestroyed += OnTargetDestroyed;
            eventsActive = true;
        }

        private static void DisableEvents()
        {
            if (!eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate -= Update;
            GlobalEffectManager.OnTargetDestroyed -= OnTargetDestroyed;
            eventsActive = false;
        }
    }

    // 眩晕效果
    public struct Stun
    {
        private float endTime;
        private LockChain moveLock;
        private LockChain skillLock;

        private static Dictionary<EffectId, Stun> Effects = new();
        public static void AddEffect(int adder, int receiver, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            EnableEvents();
            var target = Tool.SceneController.GetTarget(receiver);
            if (target != null)
            {
                var moveLock = target.OperationLock.GetChain();
                var skillLock = target.SkillLock.GetChain();
                moveLock.Locked = true;
                skillLock.Locked = true;
                Effects.Add(new EffectId(adder, receiver), new Stun
                {
                    endTime = Time.time + time,
                    moveLock = moveLock,
                    skillLock = skillLock
                });
            }
        }

        private static void Update()
        {
            foreach (var key in Effects.Keys)
            {
                if (Time.time > Effects[key].endTime)
                {
                    Effects[key].moveLock.Locked = false;
                    Effects[key].skillLock.Locked = false;
                    Effects[key].moveLock.Discard();
                    Effects[key].skillLock.Discard();
                    GlobalEffectManager.ToRemove.Add(key);
                }
            }

            if (GlobalEffectManager.ToRemove.Count > 0)
            {
                foreach (var i in GlobalEffectManager.ToRemove)
                {
                    var receiver = i.GetReceiver();
                    if (receiver && receiver.effectController)
                    {
                        receiver.effectController.EffectEnd(i.adder, EffectType.Stun);
                    }
                    Effects.Remove(i);
                }
                GlobalEffectManager.ToRemove.Clear();
                DisableEvents();
            }
        }

        private static void OnTargetDestroyed(int id)
        {
            foreach (var key in Effects.Keys)
            {
                if (key.adder == id || key.receiver == id)
                {
                    GlobalEffectManager.ToRemove.Add(key);
                }
            }

            foreach (var i in GlobalEffectManager.ToRemove)
                Effects.Remove(i);
            GlobalEffectManager.ToRemove.Clear();
            DisableEvents();
        }

        private static bool eventsActive = false;
        private static void EnableEvents()
        {
            if (eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate += Update;
            GlobalEffectManager.OnTargetDestroyed += OnTargetDestroyed;
            eventsActive = true;
        }

        private static void DisableEvents()
        {
            if (!eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate -= Update;
            GlobalEffectManager.OnTargetDestroyed -= OnTargetDestroyed;
            eventsActive = false;
        }
    }

    // 粘性效果
    public struct Sticky
    {
        private float endTime;
        private LockChain moveLock;

        private static Dictionary<EffectId, Sticky> Effects = new();
        public static void AddEffect(int adder, int receiver, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            EnableEvents();
            var target = Tool.SceneController.GetTarget(receiver);
            if (target != null)
            {
                var chain=target.OperationLock.GetChain();
                chain.Locked=true;
                Effects.Add(new EffectId(adder, receiver), new Sticky
                {
                    moveLock = chain,
                    endTime = Time.time + time,
                });
            }
        }

        private static void Update()
        {
            foreach (var key in Effects.Keys)
            {
                if (Time.time > Effects[key].endTime)
                {
                    Effects[key].moveLock.Locked = false;
                    Effects[key].moveLock.Discard();

                    GlobalEffectManager.ToRemove.Add(key);
                }
            }

            if (GlobalEffectManager.ToRemove.Count > 0)
            {
                foreach (var i in GlobalEffectManager.ToRemove)
                {
                    var receiver = i.GetReceiver();
                    if (receiver && receiver.effectController)
                    {
                        receiver.effectController.EffectEnd(i.adder, EffectType.Sticky);
                    }
                    Effects.Remove(i);
                }
                GlobalEffectManager.ToRemove.Clear();
                DisableEvents();
            }
        }

        private static void OnTargetDestroyed(int id)
        {
            foreach (var key in Effects.Keys)
            {
                if (key.adder == id || key.receiver == id)
                {
                    GlobalEffectManager.ToRemove.Add(key);
                }
            }

            foreach (var i in GlobalEffectManager.ToRemove)
                Effects.Remove(i);
            GlobalEffectManager.ToRemove.Clear();
            DisableEvents();
        }

        private static bool eventsActive = false;
        private static void EnableEvents()
        {
            if (eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate += Update;
            GlobalEffectManager.OnTargetDestroyed += OnTargetDestroyed;
            eventsActive = true;
        }

        private static void DisableEvents()
        {
            if (!eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate -= Update;
            GlobalEffectManager.OnTargetDestroyed -= OnTargetDestroyed;
            eventsActive = false;
        }
    }

    // 沉默效果
    public struct Silence
    {
        private float endTime;
        private LockChain chain;

        private static Dictionary<EffectId, Silence> Effects = new();
        public static void AddEffect(int adder, int receiver, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            EnableEvents();
            var target = Tool.SceneController.GetTarget(receiver);
            if (target != null)
            {
                var c = target.SkillLock.GetChain();
                c.Locked= true;
                Effects.Add(new EffectId(adder, receiver), new Silence
                {
                    endTime = Time.time + time,
                    chain = c
                });
            }
        }

        private static void Update()
        {
            foreach (var key in Effects.Keys)
            {
                if (Time.time > Effects[key].endTime)
                {
                    Effects[key].chain.Locked=false;
                    Effects[key].chain.Discard();
                    GlobalEffectManager.ToRemove.Add(key);
                }
            }

            if (GlobalEffectManager.ToRemove.Count > 0)
            {
                foreach (var i in GlobalEffectManager.ToRemove)
                {
                    var receiver = i.GetReceiver();
                    if (receiver && receiver.effectController)
                    {
                        receiver.effectController.EffectEnd(i.adder, EffectType.Silence);
                    }
                    Effects.Remove(i);
                }
                GlobalEffectManager.ToRemove.Clear();
                DisableEvents();
            }
        }

        private static void OnTargetDestroyed(int id)
        {
            foreach (var key in Effects.Keys)
            {
                if (key.adder == id || key.receiver == id)
                {
                    GlobalEffectManager.ToRemove.Add(key);
                }
            }

            foreach (var i in GlobalEffectManager.ToRemove)
                Effects.Remove(i);
            GlobalEffectManager.ToRemove.Clear();
            DisableEvents();
        }

        private static bool eventsActive = false;
        private static void EnableEvents()
        {
            if (eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate += Update;
            GlobalEffectManager.OnTargetDestroyed += OnTargetDestroyed;
            eventsActive = true;
        }

        private static void DisableEvents()
        {
            if (!eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate -= Update;
            GlobalEffectManager.OnTargetDestroyed -= OnTargetDestroyed;
            eventsActive = false;
        }
    }

    // 麻痹效果（间歇性无法行动）
    public struct Paralysis
    {
        private float endTime;
        private float interval; // 麻痹间隔时间
        private float nextUpdateTime;

        private static Dictionary<EffectId, Paralysis> Effects = new();
        public static void AddEffect(int adder, int receiver, float interval, float time)
        {
            if (!GlobalEffectManager.TargetCheck(receiver)) return;
            EnableEvents();
            var target = Tool.SceneController.GetTarget(receiver);
            if (target != null)
            {
                Effects.Add(new EffectId(adder, receiver), new Paralysis
                {
                    endTime = Time.time + time,
                    interval = interval,
                    nextUpdateTime = Time.time
                });
            }
        }

        private static void Update()
        {
            foreach (var key in Effects.Keys)
            {
                var effect = Effects[key];
                var receiver = key.GetReceiver();
                if (receiver == null)
                {
                    GlobalEffectManager.ToRemove.Add(key);
                    continue;
                }
            }

            if (GlobalEffectManager.ToRemove.Count > 0)
            {
                foreach (var i in GlobalEffectManager.ToRemove)
                {
                    var receiver = i.GetReceiver();
                    if (receiver && receiver.effectController)
                    {
                        receiver.effectController.EffectEnd(i.adder, EffectType.Paralysis);
                    }
                    Effects.Remove(i);
                }
                GlobalEffectManager.ToRemove.Clear();
                DisableEvents();
            }
            if (Effects.Count > 0)
            {
                foreach (var key in Effects.Keys)
                {
                    if (Time.time > Effects[key].nextUpdateTime)
                    {
                        var p = Effects[key];
                        Effects[key] = new Paralysis() { interval=p.interval, endTime = p.endTime, nextUpdateTime = Time.time + 1 };
                        var t = key.GetReceiver();
                        t.Interrupt();
                        t.ApplyMotion(new MotionStatic(0.3f, false, 0));
                    }
                }
            }
        }

        private static void OnTargetDestroyed(int id)
        {
            foreach (var key in Effects.Keys)
            {
                if (key.adder == id || key.receiver == id)
                {
                    GlobalEffectManager.ToRemove.Add(key);
                }
            }

            foreach (var i in GlobalEffectManager.ToRemove)
                Effects.Remove(i);
            GlobalEffectManager.ToRemove.Clear();
            DisableEvents();
        }

        private static bool eventsActive = false;
        private static void EnableEvents()
        {
            if (eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate += Update;
            GlobalEffectManager.OnTargetDestroyed += OnTargetDestroyed;
            eventsActive = true;
        }

        private static void DisableEvents()
        {
            if (!eventsActive || Effects.Count > 0) return;
            GlobalEffectManager.EffectUpdate -= Update;
            GlobalEffectManager.OnTargetDestroyed -= OnTargetDestroyed;
            eventsActive = false;
        }
    }
}