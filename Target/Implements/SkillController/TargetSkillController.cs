using LevelCreator.Skills;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Variety.Base;

namespace LevelCreator.TargetTemplate
{
    public class TargetSkillController : MonoBehaviour
    {
        protected Target target;
        public List<SkillControllerBase> Skills = new List<SkillControllerBase>();
        private HashSet<int> UseSkillCommandBuffer = new HashSet<int>();
        private float TimeNeeded = 0;

        public LockChain SkillLock;

        private bool Initialized = false;

        public virtual void Init(Target data, Dictionary<TargetParams, string> param)
        {
            target = data;

            if (param.ContainsKey(TargetParams.Skill))
            {
                var skillIndex = Format.StringToArray(param[TargetParams.Skill], ushort.Parse);
                CreateSkillControllers(skillIndex);
            }
            SkillLock = data.SkillLock.GetChain();

            TimeNeeded = 0;

            Initialized = true;
        }
        public virtual void CreateSkillControllers(ushort[] ids)
        {
            foreach(var i in ids)
            {
                var controller = SkillExecuter.CreateSkillController(i, target);
                Skills.Add(controller);
            }
        }
        public virtual void ClearSkillControllers()
        {
            Skills.Clear();
        }
        protected virtual void OnDestroy()
        {
            ClearSkillControllers();
        }
        protected virtual void Update()
        {
            if (!Initialized) return;
            PreUpdate();

            foreach (SkillControllerBase i in Skills)
            {
                i.Update();
            }

            if (TimeNeeded > 0)
            {
                TimeNeeded -= Time.deltaTime;
                SkillLock.Locked = true;
            }
            else SkillLock.Locked = false;

            if (target.SkillLock.LockedInHierechy)
            {
                UseSkillCommandBuffer.Clear();
            }
            else
            {
                foreach (var i in UseSkillCommandBuffer)
                {
                    if (UseSkillByOwnedIndex(i)) break;
                }
            }
            UseSkillCommandBuffer.Clear();
        }
        private bool UseSkillByOwnedIndex(int x)
        {
            if (SkillLock.LockedInHierechy) return false;
            if (x >= Skills.Count) return false;
            var s = Skills[x];
            if (!s.CanUse()) return false;
            var skill = Tool.LevelCreatorManager.GetSkillInfo(s.SkillIndex);
            TimeNeeded = skill.operationtime * 0.001f;
            target.UseSkillRpc(s.SkillIndex);
            s.OnUse();
            return true;
        }
        public bool CanUseSkill() => !target.SkillLock.LockedInHierechy;
        public virtual void PreUpdate()
        {

        }
        public void UseSkillByIndex(int index)
        {
            if (index < 0 || index >= Skills.Count) return;
            if (!UseSkillCommandBuffer.Contains(index))
            {
                UseSkillCommandBuffer.Add(index);
            }
        }
        public void UseSkillById(int id)
        {
            if (!UseSkillCommandBuffer.Contains(id)) UseSkillCommandBuffer.Add(id);
        }
    }
}