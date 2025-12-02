public class CustomTargetCreater
{
    public enum TargetType
    {
        Player
    }
    public enum GraphicType
    {
        Player
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

    private int camp;
    private int updateAt;
    private TargetType targetType;
    private GraphicType graphicType;

    private TargetControllerType controllerType;
    private bool canFly;
    private TargetSkillControllerType skillControllerType;
    private int[] skillIndex;
    private TargetEffectControllerType effectControllerType;
    public CustomTargetCreater(int camp,TargetType targetType,GraphicType graphicType, int updateAt=-1)
    {
        this.camp = camp;
        this.updateAt = updateAt;

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
        sb.Append(camp).Append('_');
        sb.Append(updateAt).Append('_');
        sb.Append((int)targetType).Append('_');
        sb.Append((int)graphicType).Append('_');
        sb.Append((int)controllerType).Append('_');
        sb.Append(canFly?1:0).Append('_');
        sb.Append((int)skillControllerType).Append('_');
        sb.Append(Format.ArrayToString(skillIndex,'+')).Append('_');
        sb.Append((int)effectControllerType);
        return sb.ToString();
    }
    public CustomTargetCreater(string data)
    {
        var s = data.Split('_');

    }
}