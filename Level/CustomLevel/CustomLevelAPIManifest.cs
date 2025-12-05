using System.Collections.Generic;
using static Level;

public static class CustomLevelAPIManifest
{
    public static float TimeUsed => CustomLevel.FightTime;
    public static void CreateLevel(LevelType type)=>Tool.SceneController.CreateLevel(type);
    public static void DestroyLevel() => Tool.SceneController.DestroyLevel();
    public static void SetScoreboardActive(bool active)=>Tool.NetworkCorrespondent.SetScoreboardActiveRpc(active);
    public static void SetScoreBoardText(int x, int y, string data)=>Tool.NetworkCorrespondent.SetScoreboardTextRpc(x, y, data);
}