using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variety.Base;
using vs = Variety.Skill;

public class VarietyManager
{
    public int SkillCount { get { return GetSkillBaseCollection.Count; } }
    private List<string> SkillPackageName = new List<string>()
    {
        "技能组1","技能组2","技能组3"
    };
    private List<string> SkillPackageDes = new List<string>()
    {
        "均衡、中射程","破霸体、范围伤害","高爆发、Debuff"
    };
    private List<List<Func<Target,SkillBase>>> GetSkillBaseCollection = new List<List<Func<Target,SkillBase>>>()
    {
        new List<Func<Target, SkillBase>>()
        {
            static t =>{ return new vs.PackageA.Skill0(t); },
            static t =>{ return new vs.PackageA.Skill1(t); },
            static t =>{ return new vs.PackageA.Skill2(t); },
            static t =>{ return new vs.PackageA.Skill3(t); },
            static t =>{ return new vs.PackageA.Skill4(t); },
            static t =>{ return new vs.PackageA.Skill5(t); },
        },
        new List<Func<Target, SkillBase>>()
        {
            static t =>{ return new vs.PackageB.Skill0(t); },
            static t =>{ return new vs.PackageB.Skill1(t); },
            static t =>{ return new vs.PackageB.Skill2(t); },
            static t =>{ return new vs.PackageB.Skill3(t); },
            static t =>{ return new vs.PackageB.Skill4(t); },
            static t =>{ return new vs.PackageB.Skill5(t); },
        },
        new List<Func<Target, SkillBase>>()
        {
            static t =>{ return new vs.PackageC.Skill0(t); },
            static t =>{ return new vs.PackageC.Skill1(t); },
            static t =>{ return new vs.PackageC.Skill2(t); },
            static t =>{ return new vs.PackageC.Skill3(t); },
            static t =>{ return new vs.PackageC.Skill4(t); },
            static t =>{ return new vs.PackageC.Skill5(t); },
        }
    };
    private List<List<Func<Target, SkillBase>>> GetBossSkill = new List<List<Func<Target, SkillBase>>>()
    {
        new List<Func<Target, SkillBase>>()
        {
            static t=>new vs.Boss1.Skill0(t),
            static t=>new vs.Boss1.Skill1(t),
            static t=>new vs.Boss1.Skill2(t),
            static t=>new vs.Boss1.Skill3(t),
            static t=>new vs.Boss1.Skill4(t),
            static t=>new vs.Boss1.Skill5(t),
        },
        new List<Func<Target, SkillBase>>()
        {
            static t=>new vs.Boss2.Skill0(t),
            static t=>new vs.Boss2.Skill1(t),
            static t=>new vs.Boss2.Skill2(t),
            static t=>new vs.Boss2.Skill3(t),
            static t=>new vs.Boss2.Skill4(t),
            static t=>new vs.Boss2.Skill5(t),
        },
        new List<Func<Target, SkillBase>>()
        {
            static t=>new vs.Boss3.Skill0(t),
            static t=>new vs.Boss3.Skill1(t),
            static t=>new vs.Boss3.Skill2(t),
            static t=>new vs.Boss3.Skill3(t),
            static t=>new vs.Boss3.Skill4(t),
            static t=>new vs.Boss3.Skill5(t),
        },
        new List<Func<Target, SkillBase>>()
        {
            static t=>new vs.Boss4.Skill0(t),
            static t=>new vs.Boss4.Skill1(t),
            static t=>new vs.Boss4.Skill2(t),
            static t=>new vs.Boss4.Skill3(t),
            static t=>new vs.Boss4.Skill4(t),
            static t=>new vs.Boss4.Skill5(t),
        },
        new List<Func<Target, SkillBase>>()
        {
            static t=>new vs.Boss5.Skill0(t),
            static t=>new vs.Boss5.Skill1(t),
            static t=>new vs.Boss5.Skill2(t),
            static t=>new vs.Boss5.Skill3(t),
            static t=>new vs.Boss5.Skill4(t),
            static t=>new vs.Boss5.Skill5(t),
        },
        new List<Func<Target, SkillBase>>()
        {
            static t=>new vs.Boss6.Skill0(t),
            static t=>new vs.Boss6.Skill1(t),
            static t=>new vs.Boss6.Skill2(t),
            static t=>new vs.Boss6.Skill3(t),
            static t=>new vs.Boss6.Skill4(t),
            static t=>new vs.Boss6.Skill5(t),
        },
        new List<Func<Target, SkillBase>>()
        {
            static t=>new vs.Boss7.Skill0(t),
            static t=>new vs.Boss7.Skill1(t),
            static t=>new vs.Boss7.Skill2(t),
            static t=>new vs.Boss7.Skill3(t),
            static t=>new vs.Boss7.Skill4(t),
            static t=>new vs.Boss7.Skill5(t),
        },
        new List<Func<Target, SkillBase>>()
        {
            static t=>new vs.Boss8.Skill0(t),
            static t=>new vs.Boss8.Skill1(t),
            static t=>new vs.Boss8.Skill2(t),
            static t=>new vs.Boss8.Skill3(t),
            static t=>new vs.Boss8.Skill4(t),
            static t=>new vs.Boss8.Skill5(t),
        },
        new List<Func<Target, SkillBase>>()
        {
            static t=>new vs.Boss9.Skill0(t),
            static t=>new vs.Boss9.Skill1(t),
            static t=>new vs.Boss9.Skill2(t),
            static t=>new vs.Boss9.Skill3(t),
            static t=>new vs.Boss9.Skill4(t),
            static t=>new vs.Boss9.Skill5(t),
        },
        new List<Func<Target, SkillBase>>()
        {
            static t=>new vs.Boss10.Skill0(t),
            static t=>new vs.Boss10.Skill1(t),
            static t=>new vs.Boss10.Skill2(t),
            static t=>new vs.Boss10.Skill3(t),
            static t=>new vs.Boss10.Skill4(t),
            static t=>new vs.Boss10.Skill5(t),
        },
        new List<Func<Target, SkillBase>>()
        {
            static t=>new vs.Boss11.Skill0(t),
            static t=>new vs.Boss11.Skill1(t),
            static t=>new vs.Boss11.Skill2(t),
            static t=>new vs.Boss11.Skill3(t),
            static t=>new vs.Boss11.Skill4(t),
            static t=>new vs.Boss11.Skill5(t),
        },
        new List<Func<Target, SkillBase>>()
        {
            static t=>new vs.Boss12.Skill0(t),
            static t=>new vs.Boss12.Skill1(t),
            static t=>new vs.Boss12.Skill2(t),
            static t=>new vs.Boss12.Skill3(t),
            static t=>new vs.Boss12.Skill4(t),
            static t=>new vs.Boss12.Skill5(t),
        },
        new List<Func<Target, SkillBase>>()
        {
            static t=>new vs.Boss13.Skill0(t),
            static t=>new vs.Boss13.Skill1(t),
            static t=>new vs.Boss13.Skill2(t),
            static t=>new vs.Boss13.Skill3(t),
            static t=>new vs.Boss13.Skill4(t),
            static t=>new vs.Boss13.Skill5(t),
        },
        new List<Func<Target, SkillBase>>()
        {
            static t=>new vs.Boss14.Skill0(t),
            static t=>new vs.Boss14.Skill1(t),
            static t=>new vs.Boss14.Skill2(t),
            static t=>new vs.Boss14.Skill3(t),
            static t=>new vs.Boss14.Skill4(t),
            static t=>new vs.Boss14.Skill5(t),
        },
        new List<Func<Target, SkillBase>>()
        {
            static t=>new vs.Boss15.Skill0(t),
            static t=>new vs.Boss15.Skill1(t),
            static t=>new vs.Boss15.Skill2(t),
            static t=>new vs.Boss15.Skill3(t),
            static t=>new vs.Boss15.Skill4(t),
            static t=>new vs.Boss15.Skill5(t),
        },
        new List<Func<Target, SkillBase>>()
        {
            static t=>new vs.Boss16.Skill0(t),
            static t=>new vs.Boss16.Skill1(t),
            static t=>new vs.Boss16.Skill2(t),
            static t=>new vs.Boss16.Skill3(t),
            static t=>new vs.Boss16.Skill4(t),
            static t=>new vs.Boss16.Skill5(t),
        },
        new List<Func<Target, SkillBase>>()
        {
            static t=>new vs.Boss17.Skill0(t),
            static t=>new vs.Boss17.Skill1(t),
            static t=>new vs.Boss17.Skill2(t),
            static t=>new vs.Boss17.Skill3(t),
            static t=>new vs.Boss17.Skill4(t),
            static t=>new vs.Boss17.Skill5(t),
        },
        new List<Func<Target, SkillBase>>()
        {
            static t=>new vs.Boss18.Skill0(t),
            static t=>new vs.Boss18.Skill1(t),
            static t=>new vs.Boss18.Skill2(t),
            static t=>new vs.Boss18.Skill3(t),
            static t=>new vs.Boss18.Skill4(t),
            static t=>new vs.Boss18.Skill5(t),
        }
    };
    private List<Func<Target, RepeatContent>> BossRepeatContents = new List<Func<Target, RepeatContent>>()
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
    public RepeatContent GetBossRepeatContent(Monster m)
    {
        return BossRepeatContents[(int)m.Type].Invoke(m);
    }
    public List<SkillBase> GetSkill(Target t)
    {
        if (t is PlayerData pd)
        {
            ServerDataContainer.TryGet(pd.id, out var data);
            var skillindex = data.vocation;
            return GetSkillBaseCollection[skillindex].Select(f=>f.Invoke(t)).ToList();
        }
        else if(t is Monster m)
        {
            var r = GetBossRepeatContent(m);
            if(r!=null)m.RepeatWork.Add(r);

            var coll = GetBossSkill[(int)m.Type];
            int indexmax = Tool.AttributesManager.GetLevel()/10;
            List<SkillBase>s=new List<SkillBase>();
            for(int i = 0; i < indexmax&&i<coll.Count; i++)
            {
                s.Add(coll[i].Invoke(t));
            }
            return s;
        }
        return null;
    }
    public List<List<SkillBase>> GetAllSkills()
    {
        return GetSkillBaseCollection.Select(t => t.Select(r=>r.Invoke(null)).ToList()).ToList();
    }
    public List<string> GetSkillPackageName()
    {
        return SkillPackageName;
    }
    public List<string> GetSkillPackageDes()
    {
        return SkillPackageDes;
    }
}
