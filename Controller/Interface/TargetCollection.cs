public class TargetCollection : EnsBehaviourCollection
{
    protected override void Init(string data)
    {
        CustomTargetCreater creater = new CustomTargetCreater(data);
        creater.ApplyForTarget(gameObject);
    }
}