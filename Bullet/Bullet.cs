using AttributeSystem.Attributes;
using AttributeSystem.Effect;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Variety.Base;


public class Bullet : MonoBehaviour
{
    public static readonly Dictionary<int, Dictionary<int, Bullet>> Bullets = new Dictionary<int, Dictionary<int, Bullet>>();

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


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius * transform.lossyScale.x);
    }
    private void Awake()
    {
        particleController = GetComponent<BulletParticleController>();
        particleController.Stop();
    }
    public void Init(float rate=1,int liftstoiclevel=1,EffectCollection ec=default, Func<Vector3, Vector3, Vector2>hitback=null)
    {
        damageRate= rate;
        liftStoicLevel = liftstoiclevel;
        Effects = ec;
        if (hitback == null) hitbackForce = (b,t)=>FigureHitBackForce(5+2*rate,b,t);
        else hitbackForce = hitback;
    }
    public void Shoot()
    {
        if (ReleaseBulletSystemReference == null) Debug.LogError("未在轨迹系统注册");
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
        particleController.Stop();

        ReleaseBulletSystemReference?.Invoke(this);
        ReleaseDamageableReference?.Invoke(this);

        try
        {
            if (Bullets.ContainsKey(Camp))
            {
                Bullets[Camp].Remove(GetInstanceID());
                if (Bullets[Camp].Count == 0) Bullets.Remove(Camp);
            }
        }
        catch(System.Exception e)
        {
            Debug.LogException(e);
        }

        Tool.BulletManager.ReturnBullet(this);
    }
    public void UpdateDetectors()
    {
        LastFramePos = transform.position;
    }
    public int FigureDamage(GameTimeAttributes defenser, out bool hit, out bool strike)//受击者数据
    {
        return FigureDamage(Shooter.DedicatedAttributes, defenser,damageRate, out hit, out strike);
    }

    public EffectCollection GetEffects()=> Effects;
    public static int FigureDamage(DedicateSyncAttributes attacker, GameTimeAttributes defenser,float damageRate, out bool hit, out bool strike)
    {
        strike = false;
        hit = true;
        if (UnityEngine.Random.Range(0f, 1f) < RateFigure(defenser.Shanbi.Value,attacker.Mingzhong))
        {
            hit = false;
            return 0;
        }
        float damage = 2*damageRate * attacker.Gongji / Mathf.Max(attacker.Gongji + defenser.Fangyu.Value, 1) * attacker.Gongji;

        if (UnityEngine.Random.Range(0f, 1f) < RateFigure(attacker.Baoji, defenser.Renxing.Value))
        {
            damage *= 2;
            strike = true;
        }

        damage *= (attacker.Jiashang - defenser.Jianshang.Value) / 100f + 1;
        damage *= UnityEngine.Random.value * 0.4f + 0.8f;
        if (damage <= 1) return 1;
        return (int)damage;
    }
    private static float RateFigure(int triggerValue,int negativeValue)
    {
        if (negativeValue <= 0) return 1;
        if (triggerValue <= negativeValue) return 0;
        if (triggerValue >= negativeValue * 2) return 1;
        return triggerValue / (float)negativeValue - 1f;
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
}
