using System.Collections.Generic;
using static Level;

public static class CustomLevelAPIManifest
{
    public static List<int> KillCount => Tool.FightController.KillCount;
    public static List<int> KilledCount => Tool.FightController.KilledCount;
    public static float TimeUsed => CustomLevel.FightTime;
    public static void CreateLevel(LevelType type)=>Tool.SceneController.CreateLevel(type);
    public static void DestroyLevel() => Tool.SceneController.DestroyLevel();
}