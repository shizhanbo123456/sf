using LevelCreator.TargetTemplate;

namespace LevelCreator.Executer
{
    public struct EffectExecuter
    {
        public static void Execute(ushort id,Target receiver,int adder)
        {
            EffectInfo info=Tool.LevelCreatorManager.GetEffectInfo(id);
            receiver.ApplyEffect(new AttributeSystem.Effect.EffectCollection(adder, info.effects.ToArray()));
        }
    }
}