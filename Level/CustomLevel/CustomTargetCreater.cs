public class CustomTargetCreater
{
    public enum TargetType
    {
        
    }
    public enum GraphicType
    {

    }
    public enum TargetControllerType
    {

    }
    public enum TargetSkillControllerType
    {

    }
    public enum TargetEffectControllerType
    {

    }

    private TargetType targetType;
    private GraphicType graphicType;
    public CustomTargetCreater(TargetType targetType,GraphicType graphicType)
    {
        this.targetType = targetType;
        this.graphicType = graphicType;
    }
    public void LoadSkillController()
    {
        
    }
}