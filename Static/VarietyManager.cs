using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Variety.Base;
using vs = Variety.Skill;

public static class VarietyManager
{
    public static List<string> SkillPackageName = new List<string>()
    {
        "技能组1","技能组2","技能组3"
    };
    public static List<string> SkillPackageDes = new List<string>()
    {
        "均衡、中射程","破霸体、范围伤害","高爆发、Debuff"
    };
    private static List<Func<Target, RepeatContent>> BossRepeatContents = new List<Func<Target, RepeatContent>>()
    {
        static t=>{return null; },
        static t=>{return new vs.Boss2.RepeatBoss2(t); },
        static t=>{return new vs.Boss3.RepeatBoss3(t); },
        static t=>{return new vs.Boss4.RepeatBoss4(t); },
        static t=>{return new vs.Boss5.RepeatBoss5(t); },
        static t=>{return null; },
        static t=>{return null; },
        static t=>{return new vs.Boss8.RepeatBoss8(t); },
        static t=>{return null; },
        static t=>{return null; },
        static t=>{return new vs.Boss11.RepeatBoss(t); },
        static t=>{return null; },
        static t=>{return new vs.Boss13.RepeatBoss(t); },
        static t=>{return new vs.Boss14.RepeatBoss(t); },
        static t=>{return new vs.Boss15.RepeatBoss(t); },
        static t=>{return new vs.Boss16.RepeatBoss(t); },
        static t=>{return new vs.Boss17.RepeatBoss(t); },
        static t=>{return new vs.Boss18.RepeatBoss(t); },
    };
    public static List<List<SkillBase>> PlayerSkills = new()
    {
        new List<SkillBase>()
        {
            new vs.PackageA.Skill0(),
            new vs.PackageA.Skill1(),
            new vs.PackageA.Skill2(),
            new vs.PackageA.Skill3(),
            new vs.PackageA.Skill4(),
            new vs.PackageA.Skill5(),
        },
        new List<SkillBase>()
        {
            new vs.PackageB.Skill0(),
            new vs.PackageB.Skill1(),
            new vs.PackageB.Skill2(),
            new vs.PackageB.Skill3(),
            new vs.PackageB.Skill4(),
            new vs.PackageB.Skill5(),
        },
        new List<SkillBase>()
        {
            new vs.PackageC.Skill0(),
            new vs.PackageC.Skill1(),
            new vs.PackageC.Skill2(),
            new vs.PackageC.Skill3(),
            new vs.PackageC.Skill4(),
            new vs.PackageC.Skill5(),
        }
    };
    public static List<List<SkillBase>> BossSkills = new()
    {
        new List<SkillBase>()
        {
            new vs.Boss1.Skill0(),
            new vs.Boss1.Skill1(),
            new vs.Boss1.Skill2(),
            new vs.Boss1.Skill3(),
            new vs.Boss1.Skill4(),
            new vs.Boss1.Skill5(),
        },
        new List<SkillBase>()
        {
            new vs.Boss2.Skill0(),
            new vs.Boss2.Skill1(),
            new vs.Boss2.Skill2(),
            new vs.Boss2.Skill3(),
            new vs.Boss2.Skill4(),
            new vs.Boss2.Skill5(),
        },
        new List<SkillBase>()
        {
            new vs.Boss3.Skill0(),
            new vs.Boss3.Skill1(),
            new vs.Boss3.Skill2(),
            new vs.Boss3.Skill3(),
            new vs.Boss3.Skill4(),
            new vs.Boss3.Skill5(),
        },
        new List<SkillBase>()
        {
            new vs.Boss4.Skill0(),
            new vs.Boss4.Skill1(),
            new vs.Boss4.Skill2(),
            new vs.Boss4.Skill3(),
            new vs.Boss4.Skill4(),
            new vs.Boss4.Skill5(),
        },
        new List<SkillBase>()
        {
            new vs.Boss5.Skill0(),
            new vs.Boss5.Skill1(),
            new vs.Boss5.Skill2(),
            new vs.Boss5.Skill3(),
            new vs.Boss5.Skill4(),
            new vs.Boss5.Skill5(),
        },
        new List<SkillBase>()
        {
            new vs.Boss6.Skill0(),
            new vs.Boss6.Skill1(),
            new vs.Boss6.Skill2(),
            new vs.Boss6.Skill3(),
            new vs.Boss6.Skill4(),
            new vs.Boss6.Skill5(),
        },
        new List<SkillBase>()
        {
            new vs.Boss7.Skill0(),
            new vs.Boss7.Skill1(),
            new vs.Boss7.Skill2(),
            new vs.Boss7.Skill3(),
            new vs.Boss7.Skill4(),
            new vs.Boss7.Skill5(),
        },
        new List<SkillBase>()
        {
            new vs.Boss8.Skill0(),
            new vs.Boss8.Skill1(),
            new vs.Boss8.Skill2(),
            new vs.Boss8.Skill3(),
            new vs.Boss8.Skill4(),
            new vs.Boss8.Skill5(),
        },
        new List<SkillBase>()
        {
            new vs.Boss9.Skill0(),
            new vs.Boss9.Skill1(),
            new vs.Boss9.Skill2(),
            new vs.Boss9.Skill3(),
            new vs.Boss9.Skill4(),
            new vs.Boss9.Skill5(),
        },
        new List<SkillBase>()
        {
            new vs.Boss10.Skill0(),
            new vs.Boss10.Skill1(),
            new vs.Boss10.Skill2(),
            new vs.Boss10.Skill3(),
            new vs.Boss10.Skill4(),
            new vs.Boss10.Skill5(),
        },
        new List<SkillBase>()
        {
            new vs.Boss11.Skill0(),
            new vs.Boss11.Skill1(),
            new vs.Boss11.Skill2(),
            new vs.Boss11.Skill3(),
            new vs.Boss11.Skill4(),
            new vs.Boss11.Skill5(),
        },
        new List<SkillBase>()
        {
            new vs.Boss12.Skill0(),
            new vs.Boss12.Skill1(),
            new vs.Boss12.Skill2(),
            new vs.Boss12.Skill3(),
            new vs.Boss12.Skill4(),
            new vs.Boss12.Skill5(),
        },
        new List<SkillBase>()
        {
            new vs.Boss13.Skill0(),
            new vs.Boss13.Skill1(),
            new vs.Boss13.Skill2(),
            new vs.Boss13.Skill3(),
            new vs.Boss13.Skill4(),
            new vs.Boss13.Skill5(),
        },
        new List<SkillBase>()
        {
            new vs.Boss14.Skill0(),
            new vs.Boss14.Skill1(),
            new vs.Boss14.Skill2(),
            new vs.Boss14.Skill3(),
            new vs.Boss14.Skill4(),
            new vs.Boss14.Skill5(),
        },
        new List<SkillBase>()
        {
            new vs.Boss15.Skill0(),
            new vs.Boss15.Skill1(),
            new vs.Boss15.Skill2(),
            new vs.Boss15.Skill3(),
            new vs.Boss15.Skill4(),
            new vs.Boss15.Skill5(),
        },
        new List<SkillBase>()
        {
            new vs.Boss16.Skill0(),
            new vs.Boss16.Skill1(),
            new vs.Boss16.Skill2(),
            new vs.Boss16.Skill3(),
            new vs.Boss16.Skill4(),
            new vs.Boss16.Skill5(),
        },
        new List<SkillBase>()
        {
            new vs.Boss17.Skill0(),
            new vs.Boss17.Skill1(),
            new vs.Boss17.Skill2(),
            new vs.Boss17.Skill3(),
            new vs.Boss17.Skill4(),
            new vs.Boss17.Skill5(),
        },
        new List<SkillBase>()
        {
            new vs.Boss18.Skill0(),
            new vs.Boss18.Skill1(),
            new vs.Boss18.Skill2(),
            new vs.Boss18.Skill3(),
            new vs.Boss18.Skill4(),
            new vs.Boss18.Skill5(),
        }
    };
    public static RepeatContent GetBossRepeatContent(Monster m)
    {
        return BossRepeatContents[(int)m.Type].Invoke(m);
    }
    private static Dictionary<int, SkillBase> FlattenSkillCollection;
    public static SkillBase GetSkill(int index)
    {
        if (FlattenSkillCollection == null)
        {
            FlattenSkillCollection= new Dictionary<int, SkillBase>();
            foreach(var i in PlayerSkills)foreach(var j in i)FlattenSkillCollection.Add(j.GetHashCode(), j);
            foreach(var i in BossSkills)foreach(var j in i)FlattenSkillCollection.Add(j.GetHashCode(), j);
        }
        return FlattenSkillCollection[index];
    }
}
