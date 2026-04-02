using LevelCreator.TargetTemplate;
using UnityEngine;

public class SkillStorableController : SkillControllerBase
{
    private int maxStoreTime;
    private float cd;
    private float storeTime;
    public override void Update()
    {
        storeTime += Time.deltaTime / cd;
        if (storeTime > maxStoreTime) storeTime = maxStoreTime;
        if (skill != null)
        {
            skill.SetAvailableTime(storeTime);
        }
    }
    public override bool CanUse()
    {
        return storeTime >= 0.999f && base.CanUse();
    }
    public override void OnUse()
    {
        storeTime -= 1f;
        base.OnUse();
    }
    public static SkillControllerBase Create(short index, Target t, int maxStoreTime, float cd)
    {
        var r = new SkillStorableController();
        r.target = t;
        r.SkillIndex = index;
        r.maxStoreTime = maxStoreTime;
        r.cd = cd;
        r.storeTime = 1;
        return r;
    }
}