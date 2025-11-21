using AttributeSystem.Attributes;
using AttributeSystem.Effect;
using UnityEngine;
using Variety.Damageable;

namespace Variety.Base
{
    public class BulletDataBase
    {
        public Target shooter;
        public IDamageable DamageType;
        public float damageRate;
        public int liftStoicLevel;
        public float hitbackForce;
        public EffectCollection Effects;
        public System.Func<Target, Target, Bullet, MotionBase> ApplyMotion;//¹¥»÷Õß£¬ÊÜ»÷Õß

        public BulletDataBase(Target shooter,IDamageable damagetype,float damagerate,int liftstoiclevel,float hitbackforce,EffectCollection effect=null,System.Func<Target, Target, Bullet, MotionBase> getmotion =null)
        {
            this.shooter = shooter;
            DamageType = damagetype;
            damageRate=damagerate;
            liftStoicLevel = liftstoiclevel;
            hitbackForce=hitbackforce;
            Effects = effect;
            ApplyMotion = getmotion;
        }
        public virtual int FigureDamage(DynamicAttributes attacker, DynamicAttributes defenser,out bool hit, out bool strike)
        {
            strike = false;
            hit = true;
            if (Random.Range(0f, 1f) < Lerp01((defenser.Shanbi.Value - attacker.Mingzhong.Value) / 100f))
            {
                hit = false;
                return 0;
            }
            float damage = damageRate * attacker.Gongji.Value  / Mathf.Max(attacker.Gongji.Value + defenser.Fangyu.Value, 1) * attacker.Gongji.Value;
            
            if (Random.Range(0f, 1f) > Lerp01((defenser.Renxing.Value + 100 - attacker.Baoji.Value) / 100f))
            {
                damage *= 2;
                strike = true;
            }

            damage *= (attacker.Jiashang.Value - defenser.Jianshang.Value) / 100f + 1;

            if (damage <= 1) return 1;
            return (int)damage;
        }
        protected float Lerp01(float x)
        {
            if (x < 0) return 0.1f / (1 - x);
            if (x < 1) return 0.8f * x + 0.1f;
            return 1 - 0.1f / x;
        }
    }
}