using LevelCreator.TargetTemplate;
using System.Collections.Generic;
using System.Linq;

[XLua.LuaCallCSharp]
//添加Targets,筛选目标Targets,执行指令
public class TargetCommand//如果不是筛选后立即执行指令并清除筛选缓冲区，请获取id数组并存储而不是等待之后再执行
{
    private static Dictionary<int, Target> Selected=new();
    public static void ClearTargetBuffer()
    {
        Selected.Clear();
    }
    public static void AddAllTargets()
    {
        foreach(var i in Tool.SceneController.FlattenTargets)
        {
            Selected.Add(i.Key,i.Value);
        }
        RemoveNull();
    }
    public static void AddTargets(int[] ids)
    {
        foreach (var i in ids)
        {
            if (Tool.SceneController.FlattenTargets.TryGetValue(i,out var t))
            {
                Selected.Add(i, t);
            }
        }
    }
    public static void AddTarget(int id)
    {
        if (Tool.SceneController.FlattenTargets.TryGetValue(id, out var t))
        {
            Selected.Add(id, t);
        }
    }


    private static List<int> ToRemove = new();
    private static void Remove()
    {
        foreach(var i in ToRemove)
        {
            if(Selected.ContainsKey(i))Selected.Remove(i);
        }
    }
    private static void RemoveNull()
    {
        foreach(var i in Selected)
        {
            if(i.Value==null)ToRemove.Add(i.Key);
        }
        Remove();
    }


    public static void SelectByLabel(string label)
    {
        foreach(var i in Selected)
        {
            if(i.Value.Info.label!=label)ToRemove.Add(i.Key);
        }
        Remove();
    }
    public static void SelectByHealth(int value,bool largerOrEqual)//false表示smallerOrEqual
    {
        foreach (var i in Selected)
        {
            if (largerOrEqual)
            {
                if (i.Value.Shengming < value) ToRemove.Add(i.Key);
            }
            else
            {
                if (i.Value.Shengming > value) ToRemove.Add(i.Key);
            }
        }
        Remove();
    }
    public static void SelectByPos(float centerX,float centerY,float radius)
    {
        float sqrd= radius * radius;
        var pos=new UnityEngine.Vector3(centerX, centerY);
        foreach (var i in Selected)
        {
            if ((i.Value.transform.position - pos).sqrMagnitude > sqrd) ToRemove.Add(i.Key);
        }
        Remove();
    }
    public static void SelectByCamp(int camp)
    {
        foreach (var i in Selected)
        {
            if (i.Value.Info.camp!=camp) ToRemove.Add(i.Key);
        }
        Remove();
    }
    public static void SelectByLevel(int value,bool largerOrEqual)//false表示smallerOrEqual
    {
        foreach (var i in Selected)
        {
            if (largerOrEqual)
            {
                if (i.Value.Info.level<value) ToRemove.Add(i.Key);
            }
            else
            {
                if (i.Value.Info.level > value) ToRemove.Add(i.Key);
            }
        }
        Remove();
    }

    public static int[] GetObjectId()
    {
        return Selected.Keys.ToArray();
    }


    public static void Spawn(ushort id)
    {
        foreach(var i in Selected.Values)
        {
            LevelLogic.CreateTarget(id, i.Info.name, i.Info.camp, i.Info.owner, i.transform.position.x, i.transform.position.y);
        }
    }
    public static void Teleport(float x,float y)=>Tool.NetworkCorrespondent.TeleportCommandRpc(Selected,x,y);
    public static void AddEffect(ushort id)=>Tool.NetworkCorrespondent.AddEffectCommandRpc(Selected, id);
    public static void DoOperation(ushort id)=>Tool.NetworkCorrespondent.DoOperationCommandRpc(Selected, id);
    public static void Damage(int value)=>Tool.NetworkCorrespondent.DamageCommandRpc(Selected, value);
}