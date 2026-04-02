using LevelCreator;
using System.Collections.Generic;

[XLua.LuaCallCSharp]
public class ExecutionBuilder
{
    public static void CreateBullet(short id,int graphicType,float radius,float lifeTime,float rate,
        int liftstoiclevel = 1,float hitbackForce = 0f,short effect = -1)
    {
        BulletBuilder.Create(id);
        BulletBuilder.SetBulletParam(graphicType,radius, lifeTime, rate, liftstoiclevel, hitbackForce, effect);
        BulletBuilder.Upload();
    }


    public static void InitEffect(short id)=>EffectBuilder.Create(id);
    public static void AddEffect(int type, float value, float time)=>EffectBuilder.AddEffect(type, value, time);
    public static void UploadEffect()=>EffectBuilder.Upload(); 
    
    public static void CreateLandscape(short id,byte x, byte y)
    {
        LandscapeBuilder.Create(id);
        LandscapeBuilder.SetSize(x, y)  ;
    }
    public static void CreatOutline(byte thickness)=>
        LandscapeBuilder.CreateOutline(thickness);
    public static void CreateSolidLand(byte point1x, byte point1y, byte point2x, byte point2y) => 
        LandscapeBuilder.CreateSolidLand(point1x, point1y, point2x, point2y);
    public static void CreateLevitatingPlatform(byte centerX, byte centerY, byte width)=>
        LandscapeBuilder.CreateLevitatingPlatform(centerX, centerY, width);
    public static void CreateVelocityArea(byte point1x, byte point1y, byte point2x, byte point2y, byte velocityX, byte velocityY) => 
        LandscapeBuilder.CreateVelocityArea(point1x, point1y, point2x, point2y, velocityX, velocityY);
    public static void UploadLandscape() => LandscapeBuilder.Upload();


    public static void InitSkillOperation(short id)=>OperationBuilder.Create(id);
    public static void AddSubSkillOperator(short delay, short subSkillId)=>OperationBuilder.AddSubSkillOperator(delay, subSkillId);
    public static void ShootBullet(short delay, short bulletInfoId, short shootActId)=>OperationBuilder.ShootBullet(delay,bulletInfoId, shootActId);
    public static void DoMotion(short delay, short motionActId)=>OperationBuilder.DoMotion(delay, motionActId);
    public static void AddEffect(short delay, short effectId, float operationRadius)=>OperationBuilder.AddEffect(delay, effectId, operationRadius);
    public static void UploadOperation()=>OperationBuilder.Upload();

    public static void CreateWithoutCD(short id, short iconType, short iconIndex, string name, string des, short operationtime)=>
        SkillBuilder.Create(id,iconType,iconIndex,name,des,operationtime);
    public static void CreateWithCD(short id, short iconType, short iconIndex, string name, string des, short operationtime, short cd)=>
        SkillBuilder.Create(id, iconType,iconIndex,name,des, operationtime, cd);
    public static void CreateAsStorable(short id, short iconType, short iconIndex, string name, string des, short operationtime, short cd, short maxStoreTime)=>
        SkillBuilder.Create(id,iconType,iconIndex,name,des,operationtime, cd,maxStoreTime);
    public static void AddAction(short actionId, short delay)=>SkillBuilder.AddAction(actionId, delay);
    public static void UploadSkill()=>SkillBuilder.Upload();

    public static void CreateTarget(short id,int level, float size, string label, int targetType, int graphicType)
    {
        TargetBuilder.Create(id);
        TargetBuilder.SetBaseInfo(level, size, label, targetType, graphicType);
    }
    public static void LoadController(int controllerType)=>TargetBuilder.LoadController(controllerType);
    public static void LoadSkillController(int skillControllerType)=>TargetBuilder.LoadSkillController(skillControllerType);
    public static void LoadEffectController(int effectControllerType)=>TargetBuilder.LoadEffectController(effectControllerType);
    public static void LoadParams(Dictionary<TargetParams, string> paramsDict)=>TargetBuilder.LoadParams(paramsDict);
    public static void UploadTarget()=>TargetBuilder.Upload();
}