using System;
using UnityEngine;
using XLua;

[LuaCallCSharp]
[Serializable]
public struct TargetInfo
{
    public int camp;
    public int owner;
    public int level;
    public string name;
    public float spawnX;
    public float spawnY;
    public float size;
    public string label;
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
    public TargetInfo(string info)
    {
        string[] s = info.Split('/');
        camp = int.Parse(s[0]);
        owner = int.Parse(s[1]);
        level = int.Parse(s[2]);
        name = s[3];
        spawnX=float.Parse(s[4]);
        spawnY=float.Parse(s[5]);
        size = float.Parse(s[6]);
        if (s.Length > 7) label = s[7];
        else label = "";
    }
    public override string ToString()
    {
        var sb = Tool.stringBuilder;
        sb.Clear();
        sb.Append(camp).Append('/');
        sb.Append(owner).Append('/');
        sb.Append(level).Append('/');
        sb.Append(name).Append('/');
        sb.Append(spawnX.ToString("F1")).Append('/');
        sb.Append(spawnY.ToString("F1")).Append('/');
        sb.Append(size.ToString("F1"));
        if (label != "") sb.Append('/').Append(label);
        return sb.ToString();
    }
}
[LuaCallCSharp]
public class CustomTargetCreater
{
    private TargetInfo info;
    private int targetType;//Player,Ore,Lantern,Monster
    private int graphicType;

    private int controllerType;//None,Player,Monster
    private bool canFly=false;
    private int skillControllerType;//None,Player,Monster
    private int[] skillIndex;
    private int repeatContentIndex;
    private int effectControllerType;//None,Default
    public CustomTargetCreater(TargetInfo info,int targetType,int graphicType)
    {
        this.info = info;

        this.targetType = targetType;
        this.graphicType = graphicType;
    }
    public void LoadController(int controllertype,bool canFly)
    {
        controllerType = controllertype;
        this.canFly = canFly;
    }
    public void LoadSkillController(int skillcontrollertype, int[] skillIndex,int repeatContentIndex)
    {
        skillControllerType= skillcontrollertype;
        if (skillIndex != null && skillIndex.Length == 0) Debug.LogError("未装载正确的技能");
        this.skillIndex = skillIndex;
        this.repeatContentIndex = repeatContentIndex;
    }
    public void LoadEffectController(int effectcontrollertype)
    {
        effectControllerType= effectcontrollertype;
    }

    public override string ToString()
    {
        var sb = Tool.stringBuilder;
        sb.Clear();
        sb.Append(info).Append('_');
        sb.Append(targetType).Append('_');
        sb.Append(graphicType).Append('_');
        sb.Append(controllerType).Append('_');
        sb.Append(canFly?1:0).Append('_');
        sb.Append(skillControllerType).Append('_');
        sb.Append(skillIndex==null||skillIndex.Length==0?"null":Format.ArrayToString(skillIndex,'+')).Append('_');
        sb.Append(repeatContentIndex).Append('_');
        sb.Append(effectControllerType);
        return sb.ToString();
    }
    public CustomTargetCreater(string data)
    {
        var s = data.Split('_');
        int index = 0;
        info = new TargetInfo(s[index++]);
        targetType=int.Parse(s[index++]);
        graphicType = int.Parse(s[index++]);
        controllerType=int.Parse(s[index++]);
        canFly=int.Parse(s[index++])==1;
        skillControllerType = int.Parse(s[index++]);
        string skill = s[index++];
        skillIndex = skill == "null" ? null : Format.StringToArray(skill, int.Parse,'+');
        repeatContentIndex=int.Parse(s[index++]);
        effectControllerType=int.Parse(s[index++]);
    }

    public void Create()
    {
        EnsInstance.EnsSpawner.CreateServerRpc(Tool.PrefabManager.TargetCollection.CollectionId, EnsBehaviour.SendTo.Everyone, ToString(), KeyLibrary.KeyFormatType.DisorderConfirm);
    }
    public void ApplyForTarget(GameObject obj,out TargetGraphic graphic,out Target target,out TargetController controller,
        out TargetSkillController skillcontroller,out TargetEffectController effectController)
    {
        graphic = UnityEngine.Object.Instantiate(Tool.PrefabManager.GraphicCollection[graphicType].gameObject, 
            Vector3.zero, Quaternion.identity, obj.transform).GetComponent<TargetGraphic>();
        switch (targetType)
        {
            case 0:target = obj.AddComponent<PlayerData>();break;
            case 1:target = obj.AddComponent<Lantern>();break;
            case 2:target = obj.AddComponent<Ore>();break;
            case 3:target = obj.AddComponent<Monster>();break;
            default:target = null;break;
        }
        switch (controllerType)
        {
            case 0:controller = null;break;
            case 1:controller = obj.AddComponent<PlayerController>();break;
            case 2:controller=obj.AddComponent<MonsterController>();break;
            default:controller = null;break;
        }
        switch (skillControllerType)
        {
            case 0:skillcontroller = null;break;
            case 1:skillcontroller = obj.AddComponent<PlayerSkillController>();break;
            case 2:skillcontroller = obj.AddComponent<MonsterSkillController>();break;
            default :skillcontroller = null;break;
        }
        switch (effectControllerType)
        {
            case 0:effectController = null;break;
            case 1:effectController = obj.AddComponent<TargetEffectController>();break;
            default:effectController=null;break;
        }
        target.graphic = graphic;
        target.controller= controller;
        target.effectController= effectController;
        target.skillController = skillcontroller;

        target.Init(info);
        if(controller)controller.Init(target, canFly);
        if(skillcontroller)skillcontroller.Init(target, skillIndex,repeatContentIndex);
        if(effectController)effectController.Init(target);
        graphic.Init(obj);
    }
}