using AttributeSystem.Attributes;
using SF.UI.Bar;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelCreator.TargetTemplate
{
    public class BossTarget : Target
    {
        private const int LayerMax = 100;

        private BossBar Bar;

        private int BloodPerM;


        public override void Init(TargetIdentify info, Dictionary<TargetParams, string> param)
        {
            base.Init(info, param);

            if (UpdateLocally)
            {
                float healthRate = param.ContainsKey(TargetParams.HealthScale) ? float.Parse(param[TargetParams.HealthScale]) : 1;
                BaseAttributes = TargetAttributes.GetGameTimeAttributes(info.level, healthRate);
                BloodPerM = BaseAttributes.Shengming.Value / LayerMax;
                BaseAttributes.Shengming.Value = LayerMax * BloodPerM;
                FloatingAttributes = BaseAttributes.Clone();

                RegistSyncAttributes();
            }
            InitNameAndBar();
        }
        protected override void InitNameAndBar()
        {
            base.InitNameAndBar();

            Bar = PlayModeController.Instance.CreateBossBar();
            Bar.SetName(Name);
            Bar.SetValue(DedicatedAttributes.Shengming.Value.Item1, DedicatedAttributes.Shengming.Value.Item2, LayerMax);
        }
        protected override void RegistSyncAttributes()
        {
            base.RegistSyncAttributes();
            DedicatedAttributes.Shengming.OnValueChanged += v => Bar.SetValue(v.Item2, v.Item1, LayerMax);
        }
        protected override void RegistOnCreated()
        {
            base.RegistOnCreated();
        }
        protected override void RegistOnDestroy()
        {
            base.RegistOnDestroy();
            if (Bar != null)
            {
                PlayModeController.Instance.DestroyBossBar(Bar);
            }
        }
    }
}