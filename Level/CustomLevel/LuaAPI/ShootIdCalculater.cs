using LevelCreator.BulletShootTemplate;
using System.Collections.Generic;
using XLua;

[LuaCallCSharp]
public class ShootIdCalclater
{
    private static ShootActBase aim = new LevelCreator.BulletShootTemplate.Aim();
    private static ShootActBase angle = new LevelCreator.BulletShootTemplate.Angle();
    private static ShootActBase anglenonfacing = new LevelCreator.BulletShootTemplate.AngleNonFacing();
    private static ShootActBase anglenonfacing2 = new LevelCreator.BulletShootTemplate.AngleNonFacing2();
    private static ShootActBase follow = new LevelCreator.BulletShootTemplate.Follow();
    private static ShootActBase fromto = new LevelCreator.BulletShootTemplate.FromTo();
    private static ShootActBase orbit = new LevelCreator.BulletShootTemplate.Orbit();
    private static ShootActBase orbitworld = new LevelCreator.BulletShootTemplate.OrbitWorld();
    private static ShootActBase projectile = new LevelCreator.BulletShootTemplate.Projectile();
    private static ShootActBase projectileaim = new LevelCreator.BulletShootTemplate.ProjectileAim();
    private static ShootActBase _static = new LevelCreator.BulletShootTemplate.Static();
    private static ShootActBase _static2 = new LevelCreator.BulletShootTemplate.Static2();
    private static ShootActBase staticscalechange = new LevelCreator.BulletShootTemplate.StaticScaleChange();

    ///子弹朝向最近敌人发射，无敌人则默认水平向前
    /// <param name="speedFactor">0-9</param>
    public static int Aim(int speedFactor)
    {     
        var id=aim.minId + speedFactor;
        if (id < aim.minId || id > aim.maxId) throw new System.Exception("id计算错误");
        return id;
    }
    ///子弹朝向前方发射，角度为相对于水平的角度，越大则越向上，越小则越向下
    /// <param name="speedFactor">0-9</param>
    /// <param name="angleFactor">0-9</param>
    public static int Angle(int speedFactor,int angleFactor)
    {
        var id = angle.minId +10*angleFactor+ speedFactor;
        if (id < angle.minId || id > angle.maxId) throw new System.Exception("id计算错误");
        return id;
    }
    ///子弹以水平向右为0角度，角度增大会使发射方向逆时针旋转
    /// <param name="speedFactor">0-4</param>
    /// <param name="angleFactor">0-19</param>
    public static int AngleNonFacing(int speedFactor, int angleFactor)
    {
        var id = anglenonfacing.minId + angleFactor + speedFactor*20;
        if (id < anglenonfacing.minId || id > anglenonfacing.maxId) throw new System.Exception("id计算错误");
        return id;
    }
    ///子弹以朝向最近敌人为0角度(无敌人则为水平向右为0角度)，角度增大会使发射方向逆时针旋转
    /// <param name="speedFactor">0-4</param>
    /// <param name="angleFactor">0-19</param>
    public static int AngleNonFacing2(int speedFactor, int angleFactor)
    {
        var id = anglenonfacing2.minId + angleFactor + speedFactor * 20;
        if (id < anglenonfacing2.minId || id > anglenonfacing2.maxId) throw new System.Exception("id计算错误");
        return id;
    }
    ///子弹跟随发射者
    public static int Follow()
    {
        var id = follow.minId;
        if (id < aim.minId || id > aim.maxId) throw new System.Exception("id计算错误");
        return id;
    }
    ///子弹从相对于玩家的某个位置发射，直线运动
    /// <param name="startx">1-9</param>
    /// <param name="starty">1-9</param>
    /// <param name="endx">1-9</param>
    /// <param name="endy">1-9</param>
    public static int FromTo(int startx,int starty,int endx,int endy)
    {
        var id = fromto.minId + startx*1000+starty*100+endx*10+endy;
        if (id < fromto.minId || id > fromto.maxId) throw new System.Exception("id计算错误");
        return id;
    }
    ///子弹环绕玩家运动
    /// <param name="angleOffsetFactor">0-7</param>
    /// <param name="orbitSpeedFactor">0-9</param>
    /// <param name="radiusFactor">0-9</param>
    public static int Orbit(int angleOffsetFactor,int orbitSpeedFactor,int radiusFactor)
    {
        var id = orbit.minId + angleOffsetFactor*100+orbitSpeedFactor*10+radiusFactor;
        if (id < orbit.minId || id > orbit.maxId) throw new System.Exception("id计算错误");
        return id;
    }
    ///子弹环绕运动，环绕中心位置不会改变
    /// <param name="angleOffsetFactor">0-7</param>
    /// <param name="orbitSpeedFactor">0-9</param>
    /// <param name="radiusFactor">0-9</param>
    public static int OrbitWorld(int angleOffsetFactor, int orbitSpeedFactor, int radiusFactor,bool atNearestEnemyOrSelf)
    {
        var id = orbitworld.minId + angleOffsetFactor * 100 + orbitSpeedFactor * 10 + radiusFactor+(atNearestEnemyOrSelf?1000:0);
        if (id < orbitworld.minId || id > orbitworld.maxId) throw new System.Exception("id计算错误");
        return id;
    }
    ///子弹以抛物线运动，发射后会受到重力影响
    /// <param name="horizontalSpeedFactor">1-9</param>
    /// <param name="verticalSpeedFactor">0-9</param>
    public static int Projectile(int horizontalSpeedFactor, int verticalSpeedFactor)
    {
        var id = projectile.minId + horizontalSpeedFactor * 10 + verticalSpeedFactor;
        if (id < projectile.minId || id > projectile.maxId) throw new System.Exception("id计算错误");
        return id;
    }
    ///不受重力影响，子弹以抛物线运动，一段时间后命中最近敌人位置(附近无敌人则为自身位置)
    ///此类子弹的lifetime最好只比hitTime略微大一点，否则子弹命中后会不断加速，玩家难以躲避
    /// <param name="startAngleFactor">0-7</param>
    /// <param name="startSpeedFactor">0-9</param>
    /// <param name="hitTimeFactor">0-9</param>
    public static int ProjectileAim(int startAngleFactor,int startSpeedFactor,int hitTimeFactor)
    {
        var id = projectileaim.minId + startAngleFactor*100+startSpeedFactor*10+hitTimeFactor;
        if (id < projectileaim.minId || id > projectileaim.maxId) throw new System.Exception("id计算错误");
        return id;
    }
    ///此类子弹发射时产生在相对于玩家的某个位置，发射后静止不动，存在时机极短的子弹(例如爆炸)可使用此类
    /// <param name="horizontalOffset">1-9</param>
    /// <param name="verticalOffset">1-9</param>
    public static int Static(int horizontalOffset,int verticalOffset)
    {
        var id = _static.minId + horizontalOffset*10+verticalOffset;
        if (id < _static.minId || id > _static.maxId) throw new System.Exception("id计算错误");
        return id;
    }
    ///此类子弹发射时产生在相对于最近敌人(附近无敌人则为玩家自身)的某个位置，发射后静止不动，存在时机极短的子弹(例如爆炸)可使用此类
    /// <param name="horizontalOffset">1-9</param>
    /// <param name="verticalOffset">1-9</param>
    public static int Static2(int horizontalOffset, int verticalOffset)
    {
        var id = _static2.minId + horizontalOffset * 10 + verticalOffset;
        if (id < _static2.minId || id > _static2.maxId) throw new System.Exception("id计算错误");
        return id;
    }
    ///此类子弹发射时产生在相对于玩家的某个位置，发射后位置不动，但半径会改变
    /// <param name="startScaleFactor">0-4</param>
    /// <param name="endScaleFactor">0-4</param>
    /// <param name="horizontalOffset">1-9</param>
    /// <param name="verticalOffset">1-9</param>
    public static int StaticScaleChange(int startScaleFactor,int endScaleFactor,int horizontalOffset, int verticalOffset)
    {
        var id = staticscalechange.minId + (startScaleFactor*5+endScaleFactor)*100+horizontalOffset*10+verticalOffset;
        if (id < staticscalechange.minId || id > staticscalechange.maxId) throw new System.Exception("id计算错误");
        return id;
    }
}