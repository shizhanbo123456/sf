using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour//联机状态下的生成由FightController控制
{
    private void Awake()
    {
        Tool.SceneController = this;
        Invoke(nameof(LateInit), 0.3f);
    }
    private void LateInit()
    {
        CreateUnnetPlayer();
    }


    public enum LevelType
    {
        Home,Prepare,Luandou,Gongfang,
        PVE1
    }
    public GameObject Player;
    public Level Level;
    [Space]
    public Dictionary<int,NonSkillPlayerData> NonSkillPlayers = new Dictionary<int,NonSkillPlayerData>();//仅仅用于存储



    //断开连接时，Corr会自动根据id移除防止null
    public Dictionary<int,PlayerData>Players=new Dictionary<int,PlayerData>();
    public Dictionary<int, Dictionary<int, Target>> Targets = new Dictionary<int, Dictionary<int, Target>>();
    public void OnTargetPostcreated(Target target)
    {
        if(!Targets.ContainsKey(target.Camp))Targets.Add(target.Camp, new Dictionary<int, Target>());
        Targets[target.Camp].Add(target.ObjectId, target);

        if (target is PlayerData p) if (!Players.ContainsKey(p.Owner)) Players.Add(p.Owner,target as PlayerData);
    }
    public void OnTargetPredestroy(Target target)
    {
        if (Targets[target.Camp].ContainsKey(target.ObjectId))Targets[target.Camp].Remove(target.ObjectId);
        if (Targets[target.Camp].Count==0)Targets.Remove(target.Camp);

        if (target is PlayerData p) if(Players.ContainsKey(p.Owner))Players.Remove(p.Owner);
    }
    public Target GetTarget(int id)
    {
        foreach(var i in Targets.Values)
        {
            if (i.ContainsKey(id)) return i[id];
        }
        return null;
    }
    public Target GetTarget(int camp,int id)
    {
        if (Targets.ContainsKey(camp) && Targets[camp].ContainsKey(id)) return Targets[camp][id];
        return null;
    }

    public void CreateUnnetPlayer()
    {
        Player= Instantiate(Tool.PrefabManager.UnnetPlayer);
        if (Level == null) Player.transform.position = new Vector3();
        else
        {
            Player.transform.position = Level.GetSpawnPlace();
        }
    }
    public void DestroyPlayer()
    {
        if (Player != null) Destroy(Player);
    }
    public void DestroyNonSkillPlayer()
    {
        foreach (var i in NonSkillPlayers.Values) i.DestroyLocal();
    }
    public void DestroyNetPlayer()
    {
        foreach (var i in Players.Values) i.targetDataSync.DestroyLocal();
    }

    public void DestroyAllTargetsLocal()
    {
        foreach(var i in Targets.Values)
        {
            foreach(var j in i.Values)
            {
                j.targetDataSync.DestroyLocal();
            }
        }
    }

    public LevelType ModeToLevel(string modeList)
    {
        if (modeList[0] == '0') return LevelType.Luandou;
        else if (modeList[0] == '1') return LevelType.Gongfang;
        else if (modeList[0] == '2') return (LevelType)(int.Parse(modeList[1].ToString())+(int)LevelType.PVE1);
        return default;
    }
    public void CreateLevel(LevelType type)
    {
        Level = Instantiate(GetLevel(type).gameObject).GetComponent<Level>();
    }
    public Level GetLevel(LevelType type)
    {
        Level level = null;
        switch (type)
        {
            case LevelType.Home: level = Tool.PrefabManager.Home; break;
            case LevelType.Prepare: level = Tool.PrefabManager.Prepare; break;
            case LevelType.Luandou: level = Tool.PrefabManager.Battle; break;
            case LevelType.Gongfang: level = Tool.PrefabManager.Guard; break;
        }
        if (level == null)
        {
            level = Tool.PrefabManager.LevelPVE[(int)type - (int)LevelType.PVE1];
        }
        if (level == null) Debug.LogError("未找到关卡");
        return level;
    }
    public void DestroyLevel() 
    {
        if (Level != null) Destroy(Level.gameObject);
    }
}
