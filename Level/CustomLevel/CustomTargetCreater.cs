using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using XLua;

[LuaCallCSharp]
[Serializable]
public struct TargetInfo
{
    private static readonly StringBuilder stringBuilder=new StringBuilder();
    public int camp;
    public int owner;
    public int level;
    public string name;
    public float spawnX;
    public float spawnY;
    public float size;
    public string label;
    public TargetInfo(int camp, int owner, int level, string name, float spawnX,float spawnY,float size, string label)
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
        label = s[7];
    }
    public override string ToString()
    {
        var sb = stringBuilder;
        sb.Clear();
        sb.Append(camp).Append('/');
        sb.Append(owner).Append('/');
        sb.Append(level).Append('/');
        sb.Append(name).Append('/');
        sb.Append(spawnX.ToString("F1")).Append('/');
        sb.Append(spawnY.ToString("F1")).Append('/');
        sb.Append(size.ToString("F1")).Append('/');
        sb.Append(label);
        return sb.ToString();
    }
}
[LuaCallCSharp]
public class CustomTargetCreater
{
    private static Dictionary<string, string> EmptyParam = new();
    private TargetInfo info;
    private int targetType;//Player,Ore,Lantern,Monster
    private int graphicType;

    private int controllerType;//None,Player,Monster
    private int skillControllerType;//None,Player,Monster
    private int effectControllerType;//None,Default
    private Dictionary<string, string> Params=EmptyParam;
    public CustomTargetCreater(TargetInfo info,int targetType,int graphicType)
    {
        this.info = info;

        this.targetType = targetType;
        this.graphicType = graphicType;
    }
    public void LoadController(int controllertype)
    {
        controllerType = controllertype;
    }
    public void LoadSkillController(int skillcontrollertype)
    {
        skillControllerType= skillcontrollertype;
    }
    public void LoadEffectController(int effectcontrollertype)
    {
        effectControllerType= effectcontrollertype;
    }
    public void LoadParams(Dictionary<string, string> Params)
    {
        this.Params = Params;
    }
    public override string ToString()
    {
        var sb = Tool.stringBuilder;
        sb.Append(info.ToString()).Append('_');
        sb.Append(targetType).Append('_');
        sb.Append(graphicType).Append('_');
        sb.Append(controllerType).Append('_');
        sb.Append(skillControllerType).Append('_');
        sb.Append(effectControllerType);
        if(Params!=null)sb.Append('_').Append(Format.DictionaryToString(Params));
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
        skillControllerType = int.Parse(s[index++]);
        effectControllerType=int.Parse(s[index++]);
        if (s.Length > index) Params = Format.StringToDictionary(s[index++], t => t, t => t);
    }

    public void Create()
    {
        EnsInstance.EnsSpawner.CreateServerRpc(Tool.PrefabManager.TargetCollection.CollectionId, EnsBehaviour.SendTo.Everyone, ToString(), Delivery.Reliable);
    }
    public void ApplyForTarget(GameObject obj)
    {
        bool isLocalPlayer = info.owner == EnsInstance.LocalClientId;
        TargetGraphic graphic;
        Target target;
        TargetController controller=null;
        TargetSkillController skillcontroller=null;
        TargetEffectController effectController= null;

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
        target.graphic = graphic;

        if (isLocalPlayer)
        {
            switch (controllerType)
            {
                case 0: controller = null; break;
                case 1: controller = obj.AddComponent<PlayerController>(); break;
                case 2: controller = obj.AddComponent<MonsterController>(); break;
                default: controller = null; break;
            }
            switch (skillControllerType)
            {
                case 0: skillcontroller = null; break;
                case 1: skillcontroller = obj.AddComponent<PlayerSkillController>(); break;
                case 2: skillcontroller = obj.AddComponent<MonsterSkillController>(); break;
                default: skillcontroller = null; break;
            }
            switch (effectControllerType)
            {
                case 0: effectController = null; break;
                case 1: effectController = obj.AddComponent<TargetEffectController>(); break;
                default: effectController = null; break;
            }
            target.controller = controller;
            target.effectController = effectController;
            target.skillController = skillcontroller;
        }

        target.Init(info,Params);
        if (isLocalPlayer)
        {
            if (controller) controller.Init(target, Params);
            if (skillcontroller) skillcontroller.Init(target, Params);
            if (effectController) effectController.Init(target, Params);
        }
        graphic.Init(obj);
    }
}