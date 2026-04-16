using LevelCreator;
using LevelCreator.TargetTemplate;
using System.Collections.Generic;
using XLua;

//创建关卡时需要手动创建关卡地形、创建玩家、敌人等
//退出关卡时会自动销毁地形和所有物体
[LuaCallCSharp]
public class LevelLogic//地图坐标中，左下角为（0，0）,右上角为（1，1）
{
    public static float TimeUsed => CustomLevel.FightTime;
    public static short[] ClientIds => ServerDataContainer.GetAllKeys();
    public static int ClientCount=>ServerDataContainer.GetAllKeys().Length;
    public static short RoomOwnerId => EnsInstance.LocalClientId;
    public static string NullName=>TargetGraphic.NullName;//名字设为此值时自动隐藏
    public static string GetName(short clientid)
    {
        if(ServerDataContainer.TryGet(clientid ,out var data))
        {
            return data.name;
        }
        UnityEngine.Debug.LogError("检测到未知Id：" + clientid);
        return "未知玩家";
    }

    public static void CreateLevel(ushort id,float minimapScale)=>Tool.NetworkCorrespondent.CreateLevelRpc(id,minimapScale);
    public static void DestroyLevel() => Tool.NetworkCorrespondent.DestroyLevelRpc();
    public static void CreateTarget(ushort templateId, string name, int camp,int owner,float spawnX,float spawnY)
    {
        var sb = Tool.stringBuilder;
        sb.Clear();
        sb.Append(templateId).Append('/');
        sb.Append(camp).Append('/');
        sb.Append(owner).Append('/');
        sb.Append(name).Append('/');
        sb.Append(spawnX).Append('/');
        sb.Append(spawnY);
        EnsInstance.EnsSpawner.CreateServerRpc(SendTo.Everyone, Tool.PrefabManager.TargetCollection.CollectionId,sb.ToString());
    }//camp为0/1/2分别表示红/绿/蓝队
    public static void ShowScoreboard(string columnHeader1, string columnHeader2, string rowHeader1, string rowHeader2, string rowHeader3) 
        =>Tool.NetworkCorrespondent.ShowScoreboardRpc(columnHeader1, columnHeader2, rowHeader1, rowHeader2,rowHeader3);
    public static void HideScoreboard() => Tool.NetworkCorrespondent.HideScoreboardRpc();
    public static void SetScoreboardText(int x, int y, string data)=>Tool.NetworkCorrespondent.SetScoreboardTextRpc(x, y, data);
    public static void ShowSelection(string label, string[] messages) => Tool.NetworkCorrespondent.ShowSelectionRpc(label, messages);
    public static void HideSelection()=>Tool.NetworkCorrespondent.HideSelectionRpc();
    public static void ShowTitle(string title) => Tool.NetworkCorrespondent.ShowTitleRpc(title);
    public static void ShowSubtitle(string subtitle)=>Tool.NetworkCorrespondent.ShowSubtitleRpc(subtitle);
}