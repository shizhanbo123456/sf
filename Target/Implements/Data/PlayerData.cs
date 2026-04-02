using AttributeSystem.Attributes;
using AttributeSystem.Effect;
using SF.UI.Bar;
using System.Collections.Generic;
using UnityEngine;

namespace LevelCreator.TargetTemplate
{
    public class PlayerData : Target
    {
        public bool isLocalPlayer => FightController.localPlayerId == Owner;
        public override bool UpdateLocally => isLocalPlayer;

        public Bar bar;

        public override void Init(TargetIdentify info, Dictionary<TargetParams, string> param)
        {
            base.Init(info, param);
            if (isLocalPlayer)
            {
                float healthRate = param.ContainsKey(TargetParams.HealthScale) ? float.Parse(param[TargetParams.HealthScale]) : 1;
                BaseAttributes = TargetAttributes.GetGameTimeAttributes(Level, healthRate);
                FloatingAttributes = BaseAttributes.Clone();
                float reg = 0.01f;
                if (param.ContainsKey(TargetParams.RegenerationRate)) reg = float.Parse(param[TargetParams.RegenerationRate]);
                ApplyEffect(new EffectCollection(RegenerationAdderId,
                    new SingleEffect[] 
                    { 
                        new SingleEffect(EffectType.HealthRegeneration, reg * BaseAttributes.Shengming.Value, 100000) 
                    }
                ));
                RegistSyncAttributes();

                CameraInstance.instance.Init(transform);
                Tool.SceneController.Player = gameObject;
            }
            InitNameAndBar();
        }
        protected override void InitNameAndBar()
        {
            base.InitNameAndBar();

            if (isLocalPlayer)
            {
                bar = PlayModeController.Instance.CreateBar();
                bar.SetScale(1f);
                bar.SetColor(new Color(1f, 0.4f, 0.4f, 1f));

                BaseAttributes.Shengming.OnValueChanged += _ => UpdateBar();
                FloatingAttributes.Shengming.OnValueChanged += _ => UpdateBar();

                BaseAttributes.Shengming.OnValueChanged.Invoke(BaseAttributes.Shengming.Value);
                FloatingAttributes.Shengming.OnValueChanged.Invoke(FloatingAttributes.Shengming.Value);
            }
        }
        private void UpdateBar()
        {
            bar.SetValue(FloatingAttributes.Shengming.Value, BaseAttributes.Shengming.Value);
        }
        protected override void RegistOnDestroy()
        {
            base.RegistOnDestroy();
            if (bar != null)
            {
                PlayModeController.Instance.DestroyBar(bar);
            }
        }
        protected override bool DamageByBullet(Bullet b)
        {
            if (!base.DamageByBullet(b)) return false;

            CameraInstance.instance.ShakeCamera();
            return true;
        }
        public override void OnKilled(Target killer)
        {
            PlayModeController.Instance.ShowKilledSignal();
            base.OnKilled(killer);
        }
    }
}