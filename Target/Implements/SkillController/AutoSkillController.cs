using System.Collections.Generic;
using UnityEngine;

namespace LevelCreator.TargetTemplate
{
    public class AutoSkillController : TargetSkillController
    {
        private int skillIndex;
        private float interval;
        private float useSkillCD;

        public override void Init(Target data, Dictionary<TargetParams, string> param)
        {
            base.Init(data, param);
            if (param.ContainsKey(TargetParams.AutoSkillCD)) interval = float.Parse(param[TargetParams.AutoSkillCD]);
            else interval = 5f;
        }
        protected override void Update()
        {
            base.Update();
            useSkillCD -= Time.deltaTime;
            if (useSkillCD < 0) UseSkill();
        }
        private void UseSkill()
        {
            if (Skills.Count == 0) return;
            if (skillIndex >= Skills.Count) skillIndex = 0;
            if (target.GetNearestEnemy(20f,true))
            {
                if(!CanUseSkill())
                {
                    useSkillCD = 0.2f;
                }
                else
                {
                    UseSkillByIndex(skillIndex);
                    useSkillCD = interval;
                }
            }
            skillIndex++;
        }
    }
}