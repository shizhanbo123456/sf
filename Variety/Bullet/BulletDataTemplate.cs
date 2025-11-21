using AttributeSystem.Attributes;
using AttributeSystem.Effect;
using System;
using UnityEngine;
using Variety.Base;
using Variety.Damageable;

namespace Variety.Template
{
    public class BulletDataSlight : BulletDataBase
    {
        public BulletDataSlight(Target t,IDamageable damagetype, float damagerate,EffectCollection ec=null) : base(t,damagetype, damagerate, 0,0,ec)
        {
        }
    }
    public class BulletDataCommon : BulletDataBase
    {
        public BulletDataCommon(Target t, IDamageable damagetype, float damagerate,float hitback=5,EffectCollection ec = null) : base(t, damagetype, damagerate, 1, hitback,ec)
        {
        }
    }
    public class BulletDataStrike : BulletDataBase
    {
        public BulletDataStrike(Target t, IDamageable damagetype, float damagerate, float hitback=10,EffectCollection ec=null) : base(t, damagetype, damagerate, 2, hitback,ec)
        {
        }
    }
    public class BulletDataAttract : BulletDataBase
    {
        public BulletDataAttract(Target t, float damagedt, float damagerate, float attractPower,float motionActiveFor=0.2f) : base(t, new Damage_Time(damagedt), damagerate, 0, 0,null, (_, d, b) =>
        {
            var dir = (b.transform.position - d.transform.position).normalized * attractPower;
            return new MotionDir(dir, motionActiveFor, false, 0, false, false);
        })
        {
        }
        public override int FigureDamage(DynamicAttributes attacker, DynamicAttributes defenser, out bool hit, out bool strike)
        {
            strike = false;
            hit = true;

            float damage = damageRate * attacker.Gongji.Value / Mathf.Max(attacker.Gongji.Value + defenser.Fangyu.Value, 1) * attacker.Gongji.Value;

            if (UnityEngine.Random.Range(0f, 1f) > Lerp01((defenser.Renxing.Value + 100 - attacker.Baoji.Value) / 100f))
            {
                damage *= 2;
                strike = true;
            }

            damage *= (attacker.Jiashang.Value - defenser.Jianshang.Value) / 100f + 1;

            if (damage <= 1) return 1;
            return (int)damage;
        }
    }
}