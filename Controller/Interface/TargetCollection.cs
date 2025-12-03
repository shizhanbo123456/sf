public class TargetCollection : EnsBehaviourCollection
{
    protected override void Init(string data)
    {
        CustomTargetCreater creater = new CustomTargetCreater(data);
        creater.ApplyForTarget(gameObject,out var grphic,out var target,out var controller,out var skillcontroller,out var effectcontroller);
    }
}