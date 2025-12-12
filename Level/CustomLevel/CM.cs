using XLua;

//创建关卡时需要手动创建关卡地形、创建玩家、敌人等
//退出关卡时会自动销毁地形和所有物体
[LuaCallCSharp]
public static class CM//CustomLevelAPIManifest
{
    public static float TimeUsed => CustomLevel.FightTime;
    public static void CreateLevel(int type)=>Tool.NetworkCorrespondent.CreateLevelRpc(type);
    public static void DestroyLevel() => Tool.NetworkCorrespondent.DestroyLevelRpc();
    public static void SetScoreboardActive(bool active)=>Tool.NetworkCorrespondent.SetScoreboardActiveRpc(active);
    public static void SetScoreBoardText(int x, int y, string data)=>Tool.NetworkCorrespondent.SetScoreboardTextRpc(x, y, data);

    private static CustomTargetCreater creater;
    public static void LoadCreater(TargetInfo info, int targetType, int graphicType)
        => creater = new CustomTargetCreater(info, targetType, graphicType);
    public static void LoadController(int controllertype, bool canFly)
        => creater.LoadController(controllertype, canFly);
    public static void LoadSkillController(int skillcontrollertype, int[] skillIndex, int repeatContentIndex)
        => creater.LoadSkillController(skillcontrollertype, skillIndex, repeatContentIndex);//repeatContentIndex<0时即视为无效
    public static void LoadEffectController(int effectcontrollertype)
        => creater.LoadEffectController(effectcontrollertype);
    public static void Create()=> creater.Create();
}
/*
TargetType:Player,Ore,Lantern,Monster

GraphicType:Player,Ore,Lantern,
    Monster1,Monster2,Monster3,Monster4,Monster5,Monster6,Monster7,Monster8,Monster9,
    Monster10,Monster11,Monster12,Monster13,Monster14,Monster15,Monster16,Monster17,Monster18

TargetControllerType: None,Player,Monster

TargetSkillControllerType:None,Player,Monster

TargetEffectControllerType:None,Default

public struct TargetInfo
{
    public TargetInfo(int camp, int owner, int level, string name, float spawnX,float spawnY,float size, string label = "")
    {
        this.camp = camp;
        this.owner = owner;
        this.level = level;
        this.name = name;
        this.spawnX = spawnX;
        this.spawnY = spawnY;
        this.size = size;
        this.label = label;
    }
}
*/