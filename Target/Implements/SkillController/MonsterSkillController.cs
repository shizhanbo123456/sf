using System.Collections.Generic;
using UnityEngine;

namespace LevelCreator.TargetTemplate
{
    public class MonsterSkillController : TargetSkillController
    {
        private int skillIndex;
        private float interval;
        private float useSkillCD;

        public override void Init(Target data, Dictionary<TargetParams, string> param)
        {
            base.Init(data, param);
            if (param.ContainsKey(TargetParams.MonsterSkillCD)) interval = float.Parse(param[TargetParams.MonsterSkillCD]);
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
            if (target.GetNearestEnemy(20f))
            {
                var b = UseSkillByOwnedIndex(skillIndex);
                if (!b)
                {
                    useSkillCD = 0.2f;
                }
                else
                {
                    useSkillCD = interval;
                }
            }
            skillIndex++;
        }
    }
}