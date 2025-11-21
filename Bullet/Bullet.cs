using AttributeSystem.Attributes;
using AttributeSystem.Effect;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Variety.Base;
using Variety.Damageable;


public class Bullet : MonoBehaviour
{
    public static Dictionary<int, Dictionary<int, Bullet>> Bullets = new Dictionary<int, Dictionary<int, Bullet>>();

    public List<BulletCollisionDetecter> collisionDetectors = new List<BulletCollisionDetecter>();
    private BulletParticleController particleController;
    [HideInInspector] public int bulletType;
    public int Camp=>Shooter.Camp;
    [HideInInspector]public Target Shooter;


    public IDamageable DamageType;
    [HideInInspector]public float damageRate;
    [HideInInspector]public int liftStoicLevel;
    public EffectCollection Effects;
    public Func<Vector3,Vector3,Vector2> hitbackForce;//BulletPos,TargetPos,Power

    public void Init(float rate=1,int liftstoiclevel=1,EffectCollection ec=null, Func<Vector3, Vector3, Vector2>hitback=null)
    {
        damageRate= rate;
        liftStoicLevel = liftstoiclevel;
        Effects = ec;
        if (hitback == null) hitbackForce = FigureHitBackForce;
        else hitbackForce = hitback;
    }
    public void Shoot(Target shooter)
    {
        Shooter = shooter;

        if (!particleController)particleController = GetComponent<BulletParticleController>();
        particleController.ChangeColor(Tool.SpriteManager.TargetToColor(Shooter));

        UpdateDetectors();

        if (!Bullets.ContainsKey(Camp))Bullets.Add(Camp, new Dictionary<int, Bullet>());
        Bullets[Camp].Add(GetInstanceID(),this);
        particleController.Play();
    }
    public void DestroyBullet()
    {
        particleController.Stop();

        Bullets[Camp].Remove(GetInstanceID());
        if (Bullets[Camp].Count==0)Bullets.Remove(Camp);

        Tool.BulletManager.ReturnBullet(this);
    }
    public void UpdateDetectors()
    {
        foreach (var i in collisionDetectors)
        {
            i.LastFramePos = i.transform.position;
        }
    }
    /// <summary>
    /// 对于中立物体或敌对boss，Camp填-1
    /// </summary>
    public bool CanDamage(int id, int camp)
    {
        if (camp == Camp) return false;
        return DamageType.OnDamage(id);
    }
    public int FigureDamage(DynamicAttributes defenser, out bool hit, out bool strike)//受击者数据
    {
        return FigureDamage(Shooter.DedicatedAttributes, defenser,damageRate, out hit, out strike);
    }

    public EffectCollection GetEffects()=> Effects;
    public static int FigureDamage(DynamicAttributes attacker, DynamicAttributes defenser,float damageRate, out bool hit, out bool strike)
    {
        strike = false;
        hit = true;
        if (UnityEngine.Random.Range(0f, 1f) < Lerp01((defenser.Shanbi.Value - attacker.Mingzhong.Value) / 100f))
        {
            hit = false;
            return 0;
        }
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
    protected static float Lerp01(float x)
    {
        if (x < 0) return 0.1f / (1 - x);
        if (x < 1) return 0.8f * x + 0.1f;
        return 1 - 0.1f / x;
    }

    public static Vector2 FigureHitBackForce(Vector3 bullet, Vector3 target)
    {
        return FigureHitBackForce(5, bullet, target);
    }
    public static Vector2 FigureHitBackForce(float power,Vector3 bullet,Vector3 target)
    {
        return new Vector2((target.x > bullet.x) ? 0.5f : -0.5f, 1)*power;
    }
    public static Vector2 HitBackBulletAttracitve(float power,Vector3 bullet,Vector3 target)
    {
        return (bullet - target).normalized * power;
    }
}
