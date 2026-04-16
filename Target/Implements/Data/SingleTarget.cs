using AttributeSystem.Attributes;
using System.Collections;
using System.Collections.Generic;
using Variety.Base;

namespace LevelCreator.TargetTemplate
{
    public class SingleTarget : Target
    {
        public override void Init(TargetIdentify info, Dictionary<TargetParams, string> param)
        {
            base.Init(info, param);

            if (UpdateLocally)
            {
                float healthRate = param.ContainsKey(TargetParams.HealthScale) ? float.Parse(param[TargetParams.HealthScale]) : 1;
                BaseAttributes = TargetAttributes.GetGameTimeAttributes(info.level, healthRate);
                if (param.TryGetValue(TargetParams.DefaultResistance, out string str) && int.TryParse(str, out int resistance))
                    BaseAttributes.Kangjitui.Value = resistance;
                FloatingAttributes = BaseAttributes.Clone();
                RegistSyncAttributes();
            }
            InitNameAndBar();
        }
    }
}