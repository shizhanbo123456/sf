using LevelCreator;
using LevelCreator.TargetTemplate;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TimeLineWork : MonoBehaviour
{
    private Target target;
    private List<(uint, OperationBuilder.SubSkillOperator)> suboperation =new();
    private List<(uint, OperationBuilder.BulletShoot)> bulletShoot =new();
    private List<(uint, OperationBuilder.MotionAction)> doMotion =new();
    private List<(uint,OperationBuilder.EffectOperation)> addEffect =new();
    private void Awake()
    {
        target = GetComponent<Target>();
    }
    private void Update()
    {
        BulletSystemCommon.CurrentShooter = target;
        if(suboperation.Count>0)UpdateForList(suboperation);
        if(bulletShoot.Count>0)UpdateForList(bulletShoot);
        if(doMotion.Count>0)UpdateForList(doMotion);
        if(addEffect.Count>0)UpdateForList(addEffect);
    }
    public void AddEvent(float delay,OperationBuilder.SubSkillOperator actor)
        => Insert(delay, actor, suboperation);
    public void AddEvent(float delay, OperationBuilder.BulletShoot actor)
        => Insert(delay, actor, bulletShoot);
    public void AddEvent(float delay,  OperationBuilder.MotionAction actor)
        => Insert(delay, actor, doMotion);
    public void AddEvent(float delay, OperationBuilder.EffectOperation actor)
        => Insert(delay, actor, addEffect);


    private void Insert<T>(float delay,T actor, List<(uint, T)> list)
    {
        uint t = (uint)((delay + Time.time) * 1000);
        int index = 0;
        while (index<list.Count && list[index].Item1 < t)
        {
            index++;
        }
        list.Insert(index,(t, actor));
        enabled = true;
    }
    private void UpdateForList<T>(List<(uint, T)> list) where T : ITimelineActor
    {
        var first = list[0];
        if (Time.time * 1000 > first.Item1)
        {
            first.Item2.Act(target);
            list.RemoveAt(0);
        }
    }
    public void Interrupted()
    {
        suboperation.Clear();
        bulletShoot.Clear();
        doMotion.Clear();
        addEffect.Clear();
    }
}
public interface ITimelineActor
{
    void Act(Target t);
}