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

    /// <param name="speedFactor">0-9</param>
    public static int Aim(int speedFactor)
    {     
        var id=aim.minId + speedFactor;
        if (id < aim.minId || id > aim.maxId) throw new System.Exception("id计算错误");
        return id;
    }
    /// <param name="speedFactor">0-9</param>
    /// <param name="angleFactor">0-9</param>
    public static int Angle(int speedFactor,int angleFactor)
    {
        var id = angle.minId +10*angleFactor+ speedFactor;
        if (id < angle.minId || id > angle.maxId) throw new System.Exception("id计算错误");
        return id;
    }
    /// <param name="speedFactor">0-4</param>
    /// <param name="angleFactor">0-19</param>
    public static int AngleNonFacing(int speedFactor, int angleFactor)
    {
        var id = anglenonfacing.minId + angleFactor + speedFactor*20;
        if (id < anglenonfacing.minId || id > anglenonfacing.maxId) throw new System.Exception("id计算错误");
        return id;
    }
    /// <param name="speedFactor">0-4</param>
    /// <param name="angleFactor">0-19</param>
    public static int AngleNonFacing2(int speedFactor, int angleFactor)
    {
        var id = anglenonfacing2.minId + angleFactor + speedFactor * 20;
        if (id < anglenonfacing2.minId || id > anglenonfacing2.maxId) throw new System.Exception("id计算错误");
        return id;
    }
    public static int Follow()
    {
        var id = follow.minId;
        if (id < aim.minId || id > aim.maxId) throw new System.Exception("id计算错误");
        return id;
    }
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
    /// <param name="angleOffsetFactor">0-7</param>
    /// <param name="orbitSpeedFactor">0-9</param>
    /// <param name="radiusFactor">0-9</param>
    public static int Orbit(int angleOffsetFactor,int orbitSpeedFactor,int radiusFactor)
    {
        var id = orbit.minId + angleOffsetFactor*100+orbitSpeedFactor*10+radiusFactor;
        if (id < orbit.minId || id > orbit.maxId) throw new System.Exception("id计算错误");
        return id;
    }
    /// <param name="angleOffsetFactor">0-7</param>
    /// <param name="orbitSpeedFactor">0-9</param>
    /// <param name="radiusFactor">0-9</param>
    public static int OrbitWorld(int angleOffsetFactor, int orbitSpeedFactor, int radiusFactor,bool atNearestEnemyOrSelf)
    {
        var id = orbitworld.minId + angleOffsetFactor * 100 + orbitSpeedFactor * 10 + radiusFactor+(atNearestEnemyOrSelf?1000:0);
        if (id < orbitworld.minId || id > orbitworld.maxId) throw new System.Exception("id计算错误");
        return id;
    }
    /// <param name="horizontalSpeedFactor">1-9</param>
    /// <param name="verticalSpeedFactor">0-9</param>
    public static int Projectile(int horizontalSpeedFactor, int verticalSpeedFactor)
    {
        var id = projectile.minId + horizontalSpeedFactor * 10 + verticalSpeedFactor;
        if (id < projectile.minId || id > projectile.maxId) throw new System.Exception("id计算错误");
        return id;
    }
    /// <param name="startAngleFactor">0-7</param>
    /// <param name="startSpeedFactor">0-9</param>
    /// <param name="hitTimeFactor">0-9</param>
    public static int ProjectileAim(int startAngleFactor,int startSpeedFactor,int hitTimeFactor)
    {
        var id = projectileaim.minId + startAngleFactor*100+startSpeedFactor*10+hitTimeFactor;
        if (id < projectileaim.minId || id > projectileaim.maxId) throw new System.Exception("id计算错误");
        return id;
    }
    /// <param name="horizontalOffset">1-9</param>
    /// <param name="verticalOffset">1-9</param>
    public static int Static(int horizontalOffset,int verticalOffset)
    {
        var id = _static.minId + horizontalOffset*10+verticalOffset;
        if (id < _static.minId || id > _static.maxId) throw new System.Exception("id计算错误");
        return id;
    }
    /// <param name="horizontalOffset">1-9</param>
    /// <param name="verticalOffset">1-9</param>
    public static int Static2(int horizontalOffset, int verticalOffset)
    {
        var id = _static2.minId + horizontalOffset * 10 + verticalOffset;
        if (id < _static2.minId || id > _static2.maxId) throw new System.Exception("id计算错误");
        return id;
    }
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