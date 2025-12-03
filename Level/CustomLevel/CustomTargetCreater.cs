using UnityEngine;

public class CustomTargetCreater
{
    public struct TargetInfo
    {
        public int camp;
        public int owner;
        public int level;
        public string name;
        public Vector2 spawnPos;
        public TargetInfo(int camp,int owner,int level,string name,Vector2 spawnPos)
        {
            this.camp = camp;
            this.owner = owner;
            this.level = level;
            this.name = name;
            this.spawnPos = spawnPos;
        }
        public TargetInfo(string info)
        {
            string[]s=info.Split('/');
            camp=int.Parse(s[0]);
            owner=int.Parse(s[1]);
            level=int.Parse(s[2]);
            name=s[3];
            spawnPos = new Vector2(float.Parse(s[4]), float.Parse(s[5]));
        }
        public override string ToString()
        {
            var sb=Tool.stringBuilder;
            sb.Clear();
            sb.Append(camp).Append('/');
            sb.Append(owner).Append('/');
            sb.Append(level).Append('/');
            sb.Append(name).Append('/');
            sb.Append(spawnPos.x.ToString("F1")).Append('/');
            sb.Append(spawnPos.y.ToString("F1"));
            return sb.ToString();
        }
    }
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

    private TargetInfo info;
    private TargetType targetType;
    private GraphicType graphicType;

    private TargetControllerType controllerType;
    private bool canFly=false;
    private TargetSkillControllerType skillControllerType;
    private int[] skillIndex;
    private TargetEffectControllerType effectControllerType;
    public CustomTargetCreater(TargetInfo info,TargetType targetType,GraphicType graphicType)
    {
        this.info = info;

        this.targetType = targetType;
        this.graphicType = graphicType;
    }
    public void LoadController(TargetControllerType controllertype,bool canFly)
    {
        controllerType = controllertype;
        this.canFly = canFly;
    }
    public void LoadSkillController(TargetSkillControllerType skillcontrollertype, int[] skillIndex)
    {
        skillControllerType= skillcontrollertype;
        if(skillIndex!=null&&skillIndex.Length>0)this.skillIndex = skillIndex;
    }
    public void LoadEffectController(TargetEffectControllerType effectcontrollertype)
    {
        effectControllerType= effectcontrollertype;
    }

    public override string ToString()
    {
        var sb = Tool.stringBuilder;
        sb.Clear();
        sb.Append(info).Append('_');
        sb.Append((int)targetType).Append('_');
        sb.Append((int)graphicType).Append('_');
        sb.Append((int)controllerType).Append('_');
        sb.Append(canFly?1:0).Append('_');
        sb.Append((int)skillControllerType).Append('_');
        sb.Append(skillIndex==null||skillIndex.Length==0?"null":Format.ArrayToString(skillIndex,'+')).Append('_');
        sb.Append((int)effectControllerType);
        return sb.ToString();
    }
    public CustomTargetCreater(string data)
    {
        var s = data.Split('_');
        int index = 0;
        info = new TargetInfo(s[index++]);
        targetType=(TargetType)int.Parse(s[index++]);
        graphicType = (GraphicType)int.Parse(s[index++]);
        controllerType=(TargetControllerType)int.Parse(s[index++]);
        canFly=int.Parse(s[index++])==1;
        skillControllerType = (TargetSkillControllerType)int.Parse(s[index++]);
        string skill = s[index++];
        skillIndex = skill == "null" ? null : Format.StringToArray(skill, int.Parse);
        effectControllerType=(TargetEffectControllerType)int.Parse(s[index++]);
    }

    public void Create()
    {

    }
    public void ApplyForTarget(GameObject obj,out TargetGraphic graphic,out Target target,out TargetController controller,
        out TargetSkillController skillcontroller,out TargetEffectController effectController)
    {
        graphic = Object.Instantiate(Tool.PrefabManager.GraphicCollection[(int)graphicType].gameObject, 
            Vector3.zero, Quaternion.identity, obj.transform).GetComponent<TargetGraphic>();
        switch (targetType)
        {
            case TargetType.Player:target = obj.AddComponent<PlayerData>();break;
            case TargetType.Lantern:target = obj.AddComponent<Lantern>();break;
            case TargetType.Ore:target = obj.AddComponent<Ore>();break;
            case TargetType.Monster:target = obj.AddComponent<Monster>();break;
            default:target = null;break;
        }
        switch (controllerType)
        {
            case TargetControllerType.None:controller = null;break;
            case TargetControllerType.Player:controller = obj.AddComponent<PlayerController>();break;
            case TargetControllerType.Monster:controller=obj.AddComponent<MonsterController>();break;
            default:controller = null;break;
        }
        switch (skillControllerType)
        {
            case TargetSkillControllerType.None:skillcontroller = null;break;
            case TargetSkillControllerType.Player:skillcontroller = obj.AddComponent<PlayerSkillController>();break;
            case TargetSkillControllerType.Monster:skillcontroller = obj.AddComponent<MonsterSkillController>();break;
            default :skillcontroller = null;break;
        }
        switch (effectControllerType)
        {
            case TargetEffectControllerType.None:effectController = null;break;
            case TargetEffectControllerType.Default:effectController = obj.AddComponent<TargetEffectController>();break;
            default:effectController=null;break;
        }
        target.graphic = graphic;
        target.controller= controller;
        target.effectController= effectController;
        target.skillController = skillcontroller;

        target.Init(info);
        if(controller)controller.Init(target, canFly);
        if(skillcontroller)skillcontroller.Init(target, skillIndex);
        if(effectController)effectController.Init(target);
        graphic.Init(obj);
    }
}