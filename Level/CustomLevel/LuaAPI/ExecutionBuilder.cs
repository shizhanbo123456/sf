using LevelCreator;
using System.Collections.Generic;
using XLua;

[XLua.LuaCallCSharp]
public class ExecutionBuilder
{
    //加载内置数据集
    public static void LoadInternal(string packageName)
        =>Tool.LevelCreatorManager.LoadInternalPackage(packageName);
    //加载其它模板数据集
    public static void LoadTemplate(string templateName)
    {
        if (TemplateLoader.LevelInfoMap.TryGetValue(templateName, out var levelInfoMap))
        {
            using(LuaEnv env = new())
            {
                try
                {
                    env.DoString(levelInfoMap);
                    LuaFunction loadTemplates = env.Global.Get<LuaFunction>("LoadTemplates");
                    loadTemplates.Action(0);
                    loadTemplates.Dispose();
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.LogError($"{templateName}加载失败:{e}");
                }
            }
        }
        else
        {
            UnityEngine.Debug.LogError("未找到模板数据集："+templateName);
        }
    }

    //子弹模板
    //hitBackForce为0时表示使用标准击退(会根据子弹倍率自动计算)
    //effect可使用未定义的buff id，此时程序认为该子弹无buff
    //graphicType：0:爆炸 1:火焰 2:普通 3:刀光 4:狼形 5:骷髅
    //liftStoicLevel只能为0/1，0表示不破霸体(一般的攻击都是这个)，1表示破霸体(强力/特殊的攻击)
    public static void CreateBullet(ushort id,int graphicType,float radius,float lifeTime,float rate,
        byte liftStoiclevel = 0,float hitBackForce = 0f,ushort effect = 0)
    {
        BulletBuilder.Create(id);
        BulletBuilder.SetBulletParam(graphicType,radius, lifeTime, rate, liftStoiclevel, hitBackForce, effect);
        BulletBuilder.Upload();
    }

    //buff效果模板
    //EffectType
    //{
    //    HealthRegeneration, Burning, Speed, Slowness, JumpBoost, AgileBoost, AccuracyBoost, AttackBoost, DefenseBoost,
    //    AgileDecrease, AccuracyDecrease, AttackDecrease, DefenseDecrease, ArmorFortify, ArmorShatter, DamageBoost, DamageDecrease,
    //    LifeSteal, Luck, BadLuck, Freeze, Stun, Sticky, Silence, Paralysis
    //}
    public static void InitEffect(ushort id)=>EffectBuilder.Create(id);
    public static void AddEffect(int type, float value, float time)=>EffectBuilder.AddEffect(type, value, time);
    public static void UploadEffect()=>EffectBuilder.Upload();

    //地形模板
    //landscapeSize=(x*16,y*8),x和y不小于1，不大于15，通常x和y在1-3之间就够常规地形，切忌太大
    public static void CreateLandscape(ushort id,byte x, byte y)
    {
        LandscapeBuilder.Create(id);
        LandscapeBuilder.SetSize(x, y);
    }
    public static void CreateOutline(byte thickness)=>
        LandscapeBuilder.CreateOutline(thickness);
    public static void CreateSolidLand(byte point1x, byte point1y, byte point2x, byte point2y) =>
        LandscapeBuilder.CreateSolidLand(point1x, point1y, point2x, point2y);
    public static void CreateLevitatingPlatform(byte centerX, byte centerY, byte width) =>
        LandscapeBuilder.CreateLevitatingPlatform(centerX, centerY, width);
    public static void CreateBrokenPlatform(byte centerX, byte centerY, byte width)=>
        LandscapeBuilder.CreateBrokenPlatform(centerX, centerY, width);
    public static void CreateFanWindArea(byte point1x, byte point1y, byte point2x, byte point2y, byte velocity)=>
        LandscapeBuilder.CreateFanWindArea(point1x,point1y, point2x,point2y, velocity);
    public static void CreateTrampoline(byte leftX, byte leftY, byte width, byte velocity)=>
        LandscapeBuilder.CreateTrampoline(leftX,leftY,width,velocity);
    public static void CreateSpike(byte point1X, byte point1Y, byte point2X, byte point2Y, int damage)=>
        LandscapeBuilder.CreateSpike(point1X,point1Y,point2X,point2Y, damage);//会自动辨认哪些位置下方是地面，并只在那些位置生成
    public static void CreateCheckPoint(byte x, byte y, byte id)=>
        LandscapeBuilder.CreateCheckPoint(x, y, id);
    public static void CreateSelectablePoint(byte x, byte y, byte id)=>
        LandscapeBuilder.CreateSelectablePoint(x, y, id);
    public static void UploadLandscape() => LandscapeBuilder.Upload();


    //操作模板
    //delay单位为ms
    public static void InitSkillOperation(ushort id)=>OperationBuilder.Create(id);
    public static void AddSubSkillOperator(ushort delay, ushort subSkillId)=>OperationBuilder.AddSubSkillOperator(delay, subSkillId);
    public static void ShootBullet(ushort delay, ushort bulletInfoId, ushort shootActId)=>OperationBuilder.ShootBullet(delay,bulletInfoId, shootActId);
    public static void DoMotion(ushort delay, ushort motionActId)=>OperationBuilder.DoMotion(delay, motionActId);
    public static void AddEffect(ushort delay, ushort effectId, float operationRadius)=>OperationBuilder.AddEffect(delay, effectId, operationRadius);
    public static void UploadOperation()=>OperationBuilder.Upload();

    //技能模板
    public static void CreateWithoutCD(ushort id, short icon, string name, string des, short operationtime)=>
        SkillBuilder.Create(id,icon,name,des,operationtime);
    public static void CreateWithCD(ushort id, short icon, string name, string des, short operationtime, short cd)=>
        SkillBuilder.Create(id, icon,name,des, operationtime, cd);
    public static void CreateAsStorable(ushort id, short icon, string name, string des, short operationtime, short cd, short maxStoreTime)=>
        SkillBuilder.Create(id,icon,name,des,operationtime, cd,maxStoreTime);
    public static void AddAction(ushort actionId, ushort delay)=>SkillBuilder.AddAction(actionId, delay);
    public static void UploadSkill()=>SkillBuilder.Upload();

    //单位模板
    //TargetType:0 Single   1 Player(拥有者屏幕上会显示玩家血条，死亡会有提示)   2 Boss(所有玩家都能看到血条)
    //GraphicType:0 玩家 其它为怪物
    //TargetController:0 无  1 Player  2 Auto(主动靠近最近的敌人)
    //SkillController:0 无  1 Player  2 Auto(在攻击范围内主动释放)
    //EffectController:0 无(不受buff影响)  1 标准(正常受到buff影响)
    public static void CreateTarget(ushort id,int level, float size, string label, int targetType, int graphicType)
    {
        TargetBuilder.Create(id);
        TargetBuilder.SetBaseInfo(level, size, label, targetType, graphicType);
    }
    public static void LoadController(int controllerType)=>TargetBuilder.LoadController(controllerType);
    public static void LoadSkillController(int skillControllerType)=>TargetBuilder.LoadSkillController(skillControllerType);
    public static void LoadEffectController(int effectControllerType)=>TargetBuilder.LoadEffectController(effectControllerType);
    /* 通过Param对有需要的情况细致调整单位
    0
    仅限TargetType=Player
    float:每秒回血的比例(默认0.01)

    1
    仅限SkillController=Auto
    float:使用技能的cd(默认5.0)

    2
    int:默认霸体等级，0为无霸体，1为霸体，2为完全霸体(默认为0)

    3
    int:0步行 1飞行(默认0)

    4
    技能编号的集合，形如 1/2/3/4/5/6
    最多装载6个技能
    默认无技能

    5
    float:控制单位相对于标准属性的生命值倍率(默认1)

    6
    int:可见性，1为可见，0为隐身(默认1)
    */
    public static void AddParam(byte key,string value)=>TargetBuilder.AddParam(key,value);
    public static void UploadTarget()=>TargetBuilder.Upload();
}