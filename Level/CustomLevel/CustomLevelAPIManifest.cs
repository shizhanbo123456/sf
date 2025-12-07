using static CustomTargetCreater;
using static Level;

//创建关卡时需要手动创建关卡地形、创建玩家、敌人等
//退出关卡时会自动销毁地形和所有物体
public static class CustomLevelAPIManifest
{
    public static float TimeUsed => CustomLevel.FightTime;
    public static void CreateLevel(LevelType type)=>Tool.SceneController.CreateLevel(type);
    public static void DestroyLevel() => Tool.SceneController.DestroyLevel();
    public static void SetScoreboardActive(bool active)=>Tool.NetworkCorrespondent.SetScoreboardActiveRpc(active);
    public static void SetScoreBoardText(int x, int y, string data)=>Tool.NetworkCorrespondent.SetScoreboardTextRpc(x, y, data);

    private static CustomTargetCreater creater;
    public static void LoadCreater(TargetInfo info, TargetType targetType, GraphicType graphicType) 
        => creater = new CustomTargetCreater(info, targetType, graphicType);
    public static void LoadController(TargetControllerType controllertype, bool canFly)
        => creater.LoadController(controllertype, canFly);
    public static void LoadSkillController(TargetSkillControllerType skillcontrollertype, int[] skillIndex, int repeatContentIndex)
        =>creater.LoadSkillController(skillcontrollertype, skillIndex, repeatContentIndex);//repeatContentIndex<0时即视为无效
    public static void LoadEffectController(TargetEffectControllerType effectcontrollertype)
        =>creater.LoadEffectController(effectcontrollertype);
    public static void Create() => creater.Create();

    public static int SkillMapper(int type,int index)//用于获取技能的index，type=0时为玩家技能组，=1为boss技能组
    {
        if (type == 0)
        {
            return VarietyManager.PlayerSkills[0][0].GetHashCode() + index;
        }
        else if (type == 1)
        {
            return VarietyManager.BossSkills[0][0].GetHashCode() + index;
        }
        return 0;
    }
}
/*
public class Level
{
    public enum LevelType
    {
        Home, Prepare, Luandou, Gongfang,
        PVE1, PVE2, PVE3, PVE4, PVE5, PVE6, PVE7
    }
}
public class CustomTargetCreater
{
    public enum TargetType
    {
        Player,Ore,Lantern,Monster
    }
    public enum GraphicType
    {
        Player,Ore,Lantern,
        Monster1,Monster2,Monster3,Monster4,Monster5,Monster6,Monster7,Monster8,Monster9,
        Monster10,Monster11,Monster12,Monster13,Monster14,Monster15,Monster16,Monster17,Monster18
    }
    public enum TargetControllerType
    {
        None,Player,Monster
    }
    public enum TargetSkillControllerType
    {
        None,Player,Monster
    }
    public enum TargetEffectControllerType
    {
        None,Default
    }
}
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