using AttributeSystem.Attributes;
using AttributeSystem.Effect;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Variety.Base;


public class Bullet : MonoBehaviour
{
    public static Dictionary<int, Dictionary<int, Bullet>> Bullets = new Dictionary<int, Dictionary<int, Bullet>>();

    //Detector
    public float radius = 2;
    [HideInInspector] public Vector3 LastFramePos;
    

    //basicInfo
    // 对于中立物体或敌对boss，Camp填-1
    [HideInInspector]public Target Shooter;
    public int Camp => Shooter.Camp;
    [HideInInspector] public int bulletType;
    private BulletParticleController particleController;

    //Info
    [HideInInspector]public float damageRate;
    [HideInInspector]public int liftStoicLevel;
    public EffectCollection Effects;
    public Func<Vector3,Vector3,Vector2> hitbackForce;//BulletPos,TargetPos,Power

    public Action<Bullet> ReleaseBulletSystemReference;
    public Action<Bullet> ReleaseDamageableReference;
    public Func<Target, Bullet, bool> CanDamage;

    private void Awake()
    {
        particleController = GetComponent<BulletParticleController>();
        particleController.Stop();
    }
    public void Init(float rate=1,int liftstoiclevel=1,EffectCollection ec=null, Func<Vector3, Vector3, Vector2>hitback=null)
    {
        damageRate= rate;
        liftStoicLevel = liftstoiclevel;
        Effects = ec;
        if (hitback == null) hitbackForce = (b,t)=>FigureHitBackForce(5+2*rate,b,t);
        else hitbackForce = hitback;
    }
    public void Shoot()
    {
        Shooter = BulletSystemCommon.CurrentShooter;

        SpriteManager.ColorType ct = liftStoicLevel switch
        {
            0 => SpriteManager.ColorType.Bullet1,
            1 => SpriteManager.ColorType.Bullet2,
            2 => SpriteManager.ColorType.Bullet3,
            _ => SpriteManager.ColorType.Bullet2,
        };
        particleController.ChangeColor(Tool.SpriteManager.TargetToColor(Shooter,ct));
        particleController.Play();

        UpdateDetectors();

        if (!Bullets.ContainsKey(Camp))Bullets.Add(Camp, new Dictionary<int, Bullet>());
        Bullets[Camp].Add(GetInstanceID(),this);
    }
    public void DestroyBullet()
    {
        ReleaseBulletSystemReference?.Invoke(this);
        ReleaseDamageableReference?.Invoke(this);

        particleController.Stop();

        Bullets[Camp].Remove(GetInstanceID());
        if (Bullets[Camp].Count==0)Bullets.Remove(Camp);

        Tool.BulletManager.ReturnBullet(this);
    }
    public void UpdateDetectors()
    {
        LastFramePos = transform.position;
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

    public static Vector2 FigureHitBackForce(float power,Vector3 bullet,Vector3 target)
    {
        return new Vector2((target.x > bullet.x) ? 0.5f : -0.5f, 1)*power+Vector2.up*3.2f;
    }
    public static Vector2 FigureAttractForce(Vector3 bullet,Vector3 target, float power=20)
    {
        float maxPower = (bullet - target).magnitude;
        return (bullet - target).normalized * Mathf.Min(maxPower*3,power)+Vector3.up*0.1962f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius * transform.lossyScale.x);
    }
}
