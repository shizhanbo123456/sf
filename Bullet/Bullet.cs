using AttributeSystem.Attributes;
using AttributeSystem.Effect;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Variety.Base;
using Variety.Template;


public class Bullet : MonoBehaviour
{
    public List<BulletCollisionDetecter> collisionDetectors = new List<BulletCollisionDetecter>();
    public int Camp
    {
        get
        {
            return Shooter.Camp;
        }
    }
    [HideInInspector]public Target Shooter;

    private BulletControllerBase BulletController;
    private BulletDataBase BulletData;

    private BulletParticleController particleController;

    private Vector3 StartPosition;
    private bool FaceRight;

    private float ScaleFactor;

    public static Dictionary<int,Dictionary<int,Bullet>>Bullets = new Dictionary<int,Dictionary<int,Bullet>>();

    public Bullet Init(BulletControllerBase bulletController,BulletDataBase bulletData)
    {
        Target owner = bulletController.shooter;
        FaceRight = owner.FacingRight();
        Shooter = owner;

        BulletController = bulletController;
        BulletData = bulletData;

        ScaleFactor=transform.localScale.x;

        particleController = GetComponent<BulletParticleController>();
        particleController.ChangeColor(Tool.SpriteManager.TargetToColor(Shooter));
        particleController.Stop();

        if (BulletController.BulletMoveSpace == BulletControllerBase.MoveSpace.Local)
            StartPosition = Shooter.transform.position + 
                (FaceRight ? BulletController.GetPosition() : BulletController.GetPositionL());
        transform.localScale = BulletController.GetScale() * ScaleFactor * Vector3.one;
        UpdatePosition();

        foreach (var i in collisionDetectors)
        {
            i.LastFramePos = i.transform.position;
        }
        return this;
    }
    public void Shoot()
    {
        if(!Bullets.ContainsKey(Camp))Bullets.Add(Camp, new Dictionary<int, Bullet>());
        Bullets[Camp].Add(GetInstanceID(),this);

        particleController.Play();
        Destroy(gameObject, BulletController.LifeTime);
    }
    private void OnDestroy()
    {
        Bullets[Camp].Remove(GetInstanceID());
        if (Bullets[Camp].Count==0)Bullets.Remove(Camp);
    }
    private void Update()
    {
        BulletController.SpawnTime += Time.deltaTime;
        if (Shooter == null) Destroy(gameObject);

        foreach (var i in collisionDetectors)
        {
            i.LastFramePos = i.transform.position;
        }
        UpdatePosition();
        
        transform.localScale = BulletController.GetScale()*ScaleFactor * Vector3.one;
    }
    private void UpdatePosition()
    {
        if (BulletController is BulletStaticScaleChange) return;
        switch (BulletController.BulletMoveSpace)
        {
            case BulletControllerBase.MoveSpace.Local:
                transform.position = StartPosition +
                    (FaceRight ? BulletController.GetPosition() : BulletController.GetPositionL());
                break;
            case BulletControllerBase.MoveSpace.Player:
                transform.position = Shooter.transform.position +
                    (FaceRight ? BulletController.GetPosition() : BulletController.GetPositionL());
                break;
            case BulletControllerBase.MoveSpace.World:
                transform.position = BulletController.GetPosition();
                break;
        }
    }

    /// <summary>
    /// 对于中立物体或敌对boss，Camp填-1
    /// </summary>
    public bool CanDamage(int id, int camp)
    {
        if (camp == Camp) return false;
        return BulletData.DamageType.OnDamage(id);
    }
    public int FigureDamage(DynamicAttributes defenser, out bool hit, out bool strike)//受击者数据
    {
        return BulletData.FigureDamage(Shooter.DedicatedAttributes, defenser, out hit, out strike);
    }
    public void GetHitBackInfo(out int liftStoicLevel,out float hitbackForce)
    {
        liftStoicLevel = BulletData.liftStoicLevel;
        hitbackForce = BulletData.hitbackForce;
    }
    public EffectCollection GetEffects()=> BulletData.Effects;
    public Func<Target, Target, Bullet, MotionBase> GetMotionFunc() => BulletData.ApplyMotion;

}
