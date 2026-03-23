using LevelCreator;
using LevelCreator.Skills;
using LevelCreator.TargetTemplate;

public class LevelCreatorManager : EnsBehaviour
{
    private void Awake()
    {
        Tool.LevelCreatorManager = this;
    }
    public void LoadInfo<T>(T info)where T:Info
    {
        if(info is TargetInfo)
        {

        }
    }
    public T GetInfo<T>(int index)where T:Info
    {
        throw new System.Exception();
    }
    public SkillBase GetSkill(int index)
    {
        return null;
    }
}
namespace LevelCreator
{
    public interface Info 
    {

    }
}