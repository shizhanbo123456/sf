using LevelCreator;

namespace LevelCreator.TargetTemplate
{
    public class TargetCollection : EnsBehaviourCollection
    {
        protected override void Init(string data)
        {
            var creater=Tool.LevelCreatorManager.GetInfo<TargetInfo>(int.Parse(data));
            creater.ApplyForTarget(gameObject);
        }
    }
}