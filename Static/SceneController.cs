using LevelCreator.TargetTemplate;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController//联机状态下的生成由FightController控制
{
    public void InitScene()
    {
        DestroyLevel();
        CreateLevel(0);
        CreateUnnetPlayer();
    }

    [HideInInspector]public GameObject Player;
    [HideInInspector]public Level Level;
    
    public Dictionary<int,NonSkillPlayerData> NonSkillPlayers = new Dictionary<int,NonSkillPlayerData>();//仅仅用于存储



    //断开连接时，Corr会自动根据id移除防止null
    public Dictionary<int, Dictionary<int, Target>> Targets = new Dictionary<int, Dictionary<int, Target>>();
    public Dictionary<int,Target>FlattenTargets= new Dictionary<int,Target>();
    public void OnTargetPostcreated(Target target)
    {
        if(!Targets.ContainsKey(target.Camp))Targets.Add(target.Camp, new Dictionary<int, Target>());
        Targets[target.Camp].Add(target.ObjectId, target);
        FlattenTargets.Add(target.ObjectId, target);
    }
    public void OnTargetPredestroy(Target target)
    {
        if (Targets[target.Camp].ContainsKey(target.ObjectId))Targets[target.Camp].Remove(target.ObjectId);
        if (Targets[target.Camp].Count==0)Targets.Remove(target.Camp);
        FlattenTargets.Remove(target.ObjectId);
    }
    public Target GetTarget(int id)
    {
        if(FlattenTargets.ContainsKey(id)) return FlattenTargets[id];
        return null;
    }
    public Target GetTarget(int camp,int id)
    {
        if (Targets.ContainsKey(camp) && Targets[camp].ContainsKey(id)) return Targets[camp][id];
        return null;
    }
    public void DestroyTargetByOwner(int ownerId)
    {
        List<Target> toDetroy = new List<Target>();
        foreach (var c in Targets.Values) foreach (var t in c.Values) if (t.Owner == ownerId) toDetroy.Add(t);
        foreach (var i in toDetroy) Object.Destroy(i.gameObject);
    }

    public void CreateUnnetPlayer()
    {
        Object.Instantiate(Tool.PrefabManager.UnnetPlayer);
    }
    public void DestroyPlayer()
    {
        if (Player != null) Object.Destroy(Player);
    }
    public void DestroyNonSkillPlayer()
    {
        foreach (var i in NonSkillPlayers.Values) Object.Destroy(i.gameObject);
    }

    public void DestroyAllTargetsLocal()
    {
        foreach(var i in Targets.Values)
        {
            foreach(var j in i.Values)
            {
                Object.Destroy(j.gameObject);
            }
        }
    }
    public void CreateLevel(int index)
    {
        Level = Object.Instantiate(Tool.PrefabManager.Levels[index].gameObject).GetComponent<Level>();
    }
    public void DestroyLevel() 
    {
        if (Level != null) Object.Destroy(Level.gameObject);
    }
}