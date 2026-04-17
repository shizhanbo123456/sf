using LevelCreator.BulletShootTemplate;
using System.Collections.Generic;
using XLua;

[LuaCallCSharp]
public class ShootIdCalculator//辅助根据需要计算出对应的编码Id
{
    private static ShootActBase aim = new Aim();
    private static ShootActBase angle = new Angle();
    private static ShootActBase anglenonfacing = new AngleNonFacing();
    private static ShootActBase anglenonfacing2 = new AngleNonFacing2();
    private static ShootActBase follow = new Follow();
    private static ShootActBase fromto = new FromTo();
    private static ShootActBase orbit = new Orbit();
    private static ShootActBase orbitworld = new OrbitWorld();
    private static ShootActBase projectile = new Projectile();
    private static ShootActBase projectileaim = new ProjectileAim();
    private static ShootActBase _static = new Static();
    private static ShootActBase _static2 = new Static2();
    private static ShootActBase staticscalechange = new StaticScaleChange();

    /// <summary>
    /// 子弹朝向最近敌人发射，无敌人则默认水平向前
    /// </summary>
    /// <param name="speedFactor">速度因子：0-9（最终速度=speedFactor*6+3）</param>
    /// <returns>计算后的射击ID</returns>
    public static int Aim(int speedFactor)
    {
        if (speedFactor < 0 || speedFactor > 9) throw new System.Exception("speedFactor超出范围(0-9)");
        var id = aim.minId + global::LevelCreator.BulletShootTemplate.Aim.mod.Encode(speedFactor);
        if (id < aim.minId || id > aim.maxId) throw new System.Exception("id计算错误，id=" + id);
        return id;
    }

    /// <summary>
    /// 子弹朝向前方发射，角度为相对于水平的角度，越大则越向上，越小则越向下
    /// </summary>
    /// <param name="speedFactor">速度因子：0-4（最终速度=speedFactor*4+4）</param>
    /// <param name="angleFactor">角度因子：0-179（最终角度=angleFactor*2）</param>
    /// <returns>计算后的射击ID</returns>
    public static int Angle(int speedFactor, int angleFactor)
    {
        if (speedFactor < 0 || speedFactor > 4) throw new System.Exception("speedFactor超出范围(0-4)");
        if (angleFactor < 0 || angleFactor > 179) throw new System.Exception("angleFactor超出范围(0-179)");
        var id = angle.minId + global::LevelCreator.BulletShootTemplate.Angle.mod.Encode(angleFactor, speedFactor);
        if (id < angle.minId || id > angle.maxId) throw new System.Exception("id计算错误，id=" + id);
        return id;
    }

    /// <summary>
    /// 子弹以水平向右为0角度，角度增大会使发射方向逆时针旋转，不受发射者朝向影响
    /// </summary>
    /// <param name="speedFactor">速度因子：0-4（最终速度=speedFactor*4+4）</param>
    /// <param name="angleFactor">角度因子：0-179（最终角度=angleFactor*2）</param>
    /// <returns>计算后的射击ID</returns>
    public static int AngleNonFacing(int speedFactor, int angleFactor)
    {
        if (speedFactor < 0 || speedFactor > 4) throw new System.Exception("speedFactor超出范围(0-4)");
        if (angleFactor < 0 || angleFactor > 179) throw new System.Exception("angleFactor超出范围(0-179)");
        var id = anglenonfacing.minId + global::LevelCreator.BulletShootTemplate.AngleNonFacing.mod.Encode(angleFactor, speedFactor);
        if (id < anglenonfacing.minId || id > anglenonfacing.maxId) throw new System.Exception("id计算错误，id=" + id);
        return id;
    }

    /// <summary>
    /// 子弹瞄准前方最近敌人，以该敌人为0角度(无敌人则为水平向右为0角度)，角度增大会使发射方向逆时针旋转
    /// </summary>
    /// <param name="speedFactor">速度因子：0-4（最终速度=speedFactor*4+4）</param>
    /// <param name="angleFactor">角度因子：0-179（最终角度=angleFactor*2）</param>
    /// <returns>计算后的射击ID</returns>
    public static int AngleNonFacing2(int speedFactor, int angleFactor)
    {
        if (speedFactor < 0 || speedFactor > 4) throw new System.Exception("speedFactor超出范围(0-4)");
        if (angleFactor < 0 || angleFactor > 179) throw new System.Exception("angleFactor超出范围(0-179)");
        var id = anglenonfacing2.minId + global::LevelCreator.BulletShootTemplate.AngleNonFacing2.mod.Encode(angleFactor, speedFactor);
        if (id < anglenonfacing2.minId || id > anglenonfacing2.maxId) throw new System.Exception("id计算错误，id=" + id);
        return id;
    }

    /// <summary>
    /// 子弹跟随发射者运动
    /// </summary>
    /// <returns>固定射击ID（2999）</returns>
    public static int Follow()
    {
        var id = follow.minId;
        if (id < follow.minId || id > follow.maxId) throw new System.Exception("id计算错误，id=" + id);
        return id;
    }

    /// <summary>
    /// 子弹从相对于玩家的某个位置发射，直线运动
    /// </summary>
    /// <param name="startx">起始X偏移因子：0-10（最终偏移=startx*4-20）</param>
    /// <param name="starty">起始Y偏移因子：0-10（最终偏移=starty*4-20）</param>
    /// <param name="endx">终点X偏移因子：0-10（最终偏移=endx*4-20）</param>
    /// <param name="endy">终点Y偏移因子：0-10（最终偏移=endy*4-20）</param>
    /// <returns>计算后的射击ID</returns>
    public static int FromTo(int startx, int starty, int endx, int endy)
    {
        if (startx < 0 || startx > 10 || starty < 0 || starty > 10 || endx < 0 || endx > 10 || endy < 0 || endy > 10)
            throw new System.Exception("偏移因子超出范围(0-10)");
        var id = fromto.minId + global::LevelCreator.BulletShootTemplate.FromTo.mod.Encode(startx, starty, endx, endy);
        if (id < fromto.minId || id > fromto.maxId) throw new System.Exception("id计算错误，id=" + id);
        return id;
    }

    /// <summary>
    /// 子弹环绕玩家运动
    /// </summary>
    /// <param name="radiusFactor">半径因子：0-9（最终半径=radiusFactor+1）</param>
    /// <param name="orbitSpeedFactor">旋转速度因子：0-9（最终速度=orbitSpeedFactor*30）</param>
    /// <param name="angleOffsetFactor">初始偏移角度因子：0-39（最终偏移=angleOffsetFactor*9）</param>
    /// <returns>计算后的射击ID</returns>
    public static int Orbit(int radiusFactor, int orbitSpeedFactor, int angleOffsetFactor)
    {
        if (radiusFactor < 0 || radiusFactor > 9) throw new System.Exception("radiusFactor超出范围(0-9)");
        if (orbitSpeedFactor < 0 || orbitSpeedFactor > 9) throw new System.Exception("orbitSpeedFactor超出范围(0-9)");
        if (angleOffsetFactor < 0 || angleOffsetFactor > 39) throw new System.Exception("angleOffsetFactor超出范围(0-39)");
        var id = orbit.minId + global::LevelCreator.BulletShootTemplate.Orbit.mod.Encode(radiusFactor, orbitSpeedFactor, angleOffsetFactor);
        if (id < orbit.minId || id > orbit.maxId) throw new System.Exception("id计算错误，id=" + id);
        return id;
    }

    /// <summary>
    /// 子弹环绕运动，环绕中心位置不会改变
    /// </summary>
    /// <param name="radiusFactor">半径因子：0-9（最终半径=radiusFactor+1）</param>
    /// <param name="orbitSpeedFactor">旋转速度因子：0-9（最终速度=orbitSpeedFactor*30）</param>
    /// <param name="angleOffsetFactor">初始偏移角度因子：0-39（最终偏移=angleOffsetFactor*9）</param>
    /// <param name="atSelf">是否以自身为中心：true=自身位置，false=最近敌人位置</param>
    /// <returns>计算后的射击ID</returns>
    public static int OrbitWorld(int radiusFactor, int orbitSpeedFactor, int angleOffsetFactor, bool atSelf)
    {
        if (radiusFactor < 0 || radiusFactor > 9) throw new System.Exception("radiusFactor超出范围(0-9)");
        if (orbitSpeedFactor < 0 || orbitSpeedFactor > 9) throw new System.Exception("orbitSpeedFactor超出范围(0-9)");
        if (angleOffsetFactor < 0 || angleOffsetFactor > 39) throw new System.Exception("angleOffsetFactor超出范围(0-39)");
        // 第四维度因子：0=自身，1=最近敌人（对应mod的v4）
        int centerFactor = atSelf ? 0 : 1;
        var id = orbitworld.minId + global::LevelCreator.BulletShootTemplate.OrbitWorld.mod.Encode(radiusFactor, orbitSpeedFactor, angleOffsetFactor, centerFactor);
        if (id < orbitworld.minId || id > orbitworld.maxId) throw new System.Exception("id计算错误，id=" + id);
        return id;
    }

    /// <summary>
    /// 子弹以抛物线运动，发射后会受到重力影响
    /// </summary>
    /// <param name="horizontalSpeedFactor">水平速度因子：0-24（最终速度=horizontalSpeedFactor*4-48）</param>
    /// <param name="verticalSpeedFactor">垂直速度因子：0-19（最终速度=verticalSpeedFactor*2）</param>
    /// <returns>计算后的射击ID</returns>
    public static int Projectile(int horizontalSpeedFactor, int verticalSpeedFactor)
    {
        if (horizontalSpeedFactor < 0 || horizontalSpeedFactor > 24) throw new System.Exception("horizontalSpeedFactor超出范围(0-24)");
        if (verticalSpeedFactor < 0 || verticalSpeedFactor > 19) throw new System.Exception("verticalSpeedFactor超出范围(0-19)");
        var id = projectile.minId + global::LevelCreator.BulletShootTemplate.Projectile.mod.Encode(horizontalSpeedFactor, verticalSpeedFactor);
        if (id < projectile.minId || id > projectile.maxId) throw new System.Exception("id计算错误，id=" + id);
        return id;
    }

    /// <summary>
    /// 不受重力影响，子弹以抛物线运动，一段时间后命中最近敌人位置(附近无敌人则为自身位置)
    /// 此类子弹的lifetime最好只比hitTime略微大一点，否则子弹命中后会不断加速，玩家难以躲避
    /// </summary>
    /// <param name="speedFactor">初始速度因子：0-14（最终速度=speedFactor+1）</param>
    /// <param name="angleFactor">初始角度因子：0-9（最终角度=angleFactor*36）</param>
    /// <param name="hitTimeFactor">命中时间因子：0-19（最终时间=hitTimeFactor*0.5+0.5）</param>
    /// <returns>计算后的射击ID</returns>
    public static int ProjectileAim(int speedFactor, int angleFactor, int hitTimeFactor)
    {
        if (speedFactor < 0 || speedFactor > 14) throw new System.Exception("speedFactor超出范围(0-14)");
        if (angleFactor < 0 || angleFactor > 9) throw new System.Exception("angleFactor超出范围(0-9)");
        if (hitTimeFactor < 0 || hitTimeFactor > 19) throw new System.Exception("hitTimeFactor超出范围(0-19)");
        var id = projectileaim.minId + global::LevelCreator.BulletShootTemplate.ProjectileAim.mod.Encode(speedFactor, angleFactor, hitTimeFactor);
        if (id < projectileaim.minId || id > projectileaim.maxId) throw new System.Exception("id计算错误，id=" + id);
        return id;
    }

    /// <summary>
    /// 此类子弹发射时产生在相对于玩家的某个位置，发射后静止不动，存在时机极短的子弹(例如爆炸)可使用此类
    /// </summary>
    /// <param name="horizontalOffset">水平偏移因子：0-44（最终偏移=horizontalOffset*2.5-55）</param>
    /// <param name="verticalOffset">垂直偏移因子：0-44（最终偏移=verticalOffset*2.5-55）</param>
    /// <returns>计算后的射击ID</returns>
    public static int Static(int horizontalOffset, int verticalOffset)
    {
        if (horizontalOffset < 0 || horizontalOffset > 44) throw new System.Exception("horizontalOffset超出范围(0-44)");
        if (verticalOffset < 0 || verticalOffset > 44) throw new System.Exception("verticalOffset超出范围(0-44)");
        var id = _static.minId + global::LevelCreator.BulletShootTemplate.Static.mod.Encode(horizontalOffset, verticalOffset);
        if (id < _static.minId || id > _static.maxId) throw new System.Exception("id计算错误，id=" + id);
        return id;
    }

    /// <summary>
    /// 此类子弹发射时产生在相对于最近敌人(附近无敌人则为玩家自身)的某个位置，发射后静止不动，存在时机极短的子弹(例如爆炸)可使用此类
    /// </summary>
    /// <param name="horizontalOffset">水平偏移因子：0-44（最终偏移=horizontalOffset*2.5-55）</param>
    /// <param name="verticalOffset">垂直偏移因子：0-44（最终偏移=verticalOffset*2.5-55）</param>
    /// <returns>计算后的射击ID</returns>
    public static int Static2(int horizontalOffset, int verticalOffset)
    {
        if (horizontalOffset < 0 || horizontalOffset > 44) throw new System.Exception("horizontalOffset超出范围(0-44)");
        if (verticalOffset < 0 || verticalOffset > 44) throw new System.Exception("verticalOffset超出范围(0-44)");
        var id = _static2.minId + global::LevelCreator.BulletShootTemplate.Static2.mod.Encode(horizontalOffset, verticalOffset);
        if (id < _static2.minId || id > _static2.maxId) throw new System.Exception("id计算错误，id=" + id);
        return id;
    }

    /// <summary>
    /// 此类子弹发射时产生在相对于玩家的某个位置，发射后位置不动，但半径会改变
    /// </summary>
    /// <param name="startScaleFactor">初始尺寸因子：0-4（最终倍率=startScaleFactor*0.5）</param>
    /// <param name="endScaleFactor">结束尺寸因子：0-4（最终倍率=endScaleFactor*0.5）</param>
    /// <param name="horizontalOffset">水平偏移因子：0-8（最终偏移=horizontalOffset*2.5-10）</param>
    /// <param name="verticalOffset">垂直偏移因子：0-8（最终偏移=verticalOffset*2.5-10）</param>
    /// <returns>计算后的射击ID</returns>
    public static int StaticScaleChange(int startScaleFactor, int endScaleFactor, int horizontalOffset, int verticalOffset)
    {
        if (startScaleFactor < 0 || startScaleFactor > 4) throw new System.Exception("startScaleFactor超出范围(0-4)");
        if (endScaleFactor < 0 || endScaleFactor > 4) throw new System.Exception("endScaleFactor超出范围(0-4)");
        if (horizontalOffset < 0 || horizontalOffset > 8) throw new System.Exception("horizontalOffset超出范围(0-8)");
        if (verticalOffset < 0 || verticalOffset > 8) throw new System.Exception("verticalOffset超出范围(0-8)");
        var id = staticscalechange.minId + global::LevelCreator.BulletShootTemplate.StaticScaleChange.mod.Encode(horizontalOffset, verticalOffset, startScaleFactor, endScaleFactor);
        if (id < staticscalechange.minId || id > staticscalechange.maxId) throw new System.Exception("id计算错误，id=" + id);
        return id;
    }
}