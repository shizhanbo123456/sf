using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Variety.Damageable
{
    public interface IDamageable
    {
        bool OnDamage(int id);
    }
    /// <summary>
    /// 无需额外设置参数
    /// </summary>
    public class Damage_Once : IDamageable
    {
        public List<int> Id = new List<int>();
        public bool OnDamage(int id)
        {
            if (Id.Contains(id)) return false;
            Id.Add(id);
            return true;
        }
    }
    /// <summary>
    /// 需要设置CoolDown
    /// </summary>
    public class Damage_Time : IDamageable
    {
        public float CoolDown = 0.5f;
        private Dictionary<int,float> LastHitTimeTime = new Dictionary<int,float>();
        public Damage_Time(float cd)
        {
            CoolDown= cd;
        }
        public bool OnDamage(int id)
        {
            if (!LastHitTimeTime.ContainsKey(id))
            {
                LastHitTimeTime.Add(id, Time.time);
                return true;
            }
            else
            {
                if (Time.time - LastHitTimeTime[id] < CoolDown) return false;
                else
                {
                    LastHitTimeTime[id] = Time.time;
                    return true;
                }
            }
        }
    }
    /// <summary>
    /// 不会伤害，仅装饰
    /// </summary>
    public class Damage_VFXOnly:IDamageable
    {
        public bool OnDamage(int id)
        {
            return false;
        }
    }
}
