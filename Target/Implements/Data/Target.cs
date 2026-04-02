using AttributeSystem.Attributes;
using AttributeSystem.Effect;
using System;
using System.Collections.Generic;
using UnityEngine;
using Variety.Base;
using static WorldTextController;

namespace LevelCreator.TargetTemplate
{
    /// <summary>
    /// 需要添加TargetDataSync,TargetControllerSync
    /// </summary>
    public abstract partial class Target : MonoBehaviour
    {
        public const int RegenerationAdderId = -10000;
        public const int SceneEffectId = -10001;

        public int ObjectId => targetDataSync.ObjectId % 10000;

        public TargetIdentify Info;
        public int Camp => Info.camp;
        public int Level => Info.level;
        public int Owner => Info.owner;
        public string Name => Info.name;

        public GameTimeAttributes BaseAttributes;
        public GameTimeAttributes FloatingAttributes;
        public DedicateSyncAttributes DedicatedAttributes => targetDataSync.DedicatedAttributes;

        [HideInInspector] public TargetDataSync targetDataSync;
        [HideInInspector] public TargetControllerSync targetControllerSync;

        [HideInInspector] public TargetGraphic graphic;
        [HideInInspector] public TargetController controller;
        [HideInInspector] public TargetEffectController effectController;
        [HideInInspector] public TargetSkillController skillController;

        [HideInInspector] public TimeLineWork TimeLineWork;

        public bool FaceRight => targetControllerSync.FaceRight;
        public Vector3 Front => FaceRight ? new Vector3(1, 0) : new Vector3(-1, 0);
        public virtual int Shengming
        {
            get { return FloatingAttributes.Shengming.Value; }
            set
            {
                FloatingAttributes.Shengming.Value = Mathf.Clamp(value, 0, BaseAttributes.Shengming.Value);
            }
        }
        public virtual bool UpdateLocally => EnsInstance.HasAuthority;

        public LockChain OperationLock = LockChain.CreateLock();
        public LockChain SkillLock = LockChain.CreateLock();

        /// <summary>
        /// 分配信息，设置位置，获取组件，注册OnCreate。注册属性同步需要额外调用<br/>
        /// 需要手动调用InitNameAndBar(所有客户端)和RegistSyncAttributes(拥有者)
        /// </summary>
        /// <param name="info"></param>
        public virtual void Init(TargetIdentify info, Dictionary<TargetParams, string> param)
        {
            Info = info;
            enabled = EnsInstance.LocalClientId == info.owner;

            graphic.transform.localScale = info.size * Vector3.one;

            transform.position = Tool.SceneController.Level.GetPos(info.spawnX, info.spawnY) +
                graphic.SpawnOffset * graphic.transform.localScale.y * Vector3.up;
            graphic.transform.localPosition = Vector3.zero;

            if (param.ContainsKey(TargetParams.BodySize)) graphic.transform.localScale = Vector3.one * float.Parse(param[TargetParams.BodySize]);

            TimeLineWork = gameObject.AddComponent<TimeLineWork>();
            if (!TryGetComponent(out targetControllerSync)) Debug.LogError("未找到同步");
            if (TryGetComponent(out targetDataSync)) targetDataSync.Init(this, param);
            else Debug.LogError("未找到信息同步");

            RegistOnCreated();
        }
        protected virtual void InitNameAndBar()
        {
            graphic.SetName(Name, Tool.SpriteManager.TargetToColor(Camp));
        }
        protected virtual void RegistSyncAttributes()
        {
            void SyncShengming()
            {
                targetDataSync.SyncShengming(BaseAttributes.Shengming.Value, FloatingAttributes.Shengming.Value);
            }
            //生命每过一段时间同步(脏标记)
            //攻击类属性远程同步
            //防御类属性本地同步
            BaseAttributes.Shengming.OnValueChanged += _ => SyncShengming();
            FloatingAttributes.Shengming.OnValueChanged += _ => SyncShengming();
            FloatingAttributes.Gongji.OnValueChanged += v => targetDataSync.SyncGongji(v);
            FloatingAttributes.Mingzhong.OnValueChanged += v => targetDataSync.SyncMingzhong(v);
            FloatingAttributes.Baoji.OnValueChanged += v => targetDataSync.SyncBaoji(v);
            FloatingAttributes.Jiashang.OnValueChanged += v => targetDataSync.SyncJiashang(v);

            DedicatedAttributes.Shengming.OnValueChanged += v => graphic.SetBarValue(v.Item2 / (float)v.Item1);
        }

        protected virtual void RegistOnCreated()
        {
            Tool.SceneController.OnTargetPostcreated(this);
        }
        protected virtual void RegistOnDestroy()
        {
            Tool.SceneController.OnTargetPredestroy(this);
            GlobalEffectManager.OnTargetDestroyed?.Invoke(ObjectId);

            OperationLock?.Discard();
            SkillLock?.Discard();
            OperationLock = null;
            SkillLock = null;

            BaseAttributes?.Release();
            BaseAttributes = null; ;
            FloatingAttributes?.Release();
            FloatingAttributes = null;
        }
        private void OnDestroy()
        {
            RegistOnDestroy();
        }

        protected virtual HashSet<Bullet> DetectBullet() => graphic.bulletDetector.DetectBullet();

        protected virtual void Update()
        {
            foreach (var b in DetectBullet())
            {
                DamageByBullet(b);
                if (Shengming == 0) break;
            }
        }
        protected virtual bool DamageByBullet(Bullet b)
        {
            if (b.Camp == Camp) return false;
            if (b.CanDamage == null) return false;
            if (!b.CanDamage.Invoke(this, b)) return false;

            int d = b.FigureDamage(FloatingAttributes, out bool hit, out bool strike);
            ShowDamageText(d, hit, strike);

            if (hit)
            {
                Shengming -= d;
                if (Shengming <= 0)
                {
                    Shengming = 0;
                    OnKilled(b.Shooter);
                }
                else
                {
                    OnHitBack(b);
                    ApplyEffects(b);
                }
            }
            return true;
        }
        protected void ShowDamageText(int value, bool hit, bool strike)
        {
            if (!hit) Tool.WorldTextController.ShowTextRpc("miss", transform.position, TextColor.Blue);
            else if (!strike) Tool.WorldTextController.ShowTextRpc('-' + value.ToString(), transform.position, TextColor.Orange);
            else Tool.WorldTextController.ShowTextRpc('-' + value.ToString(), transform.position, TextColor.Red);
        }
        protected virtual void OnHitBack(Bullet b)
        {
            if (controller != null) controller.OnHitBack(b);
        }
        protected virtual void ApplyEffects(Bullet b)
        {
            if (effectController == null) return;
            var effs = b.GetEffects();
            if (!effs.IsEmpty())
            {
                effectController.AddEffect(effs);
            }
        }
        public virtual void ApplyMotion(MotionBase m)
        {
            if (controller != null) controller.ApplyMotion(m);
        }
        public virtual void ApplyEffect(EffectCollection collection)
        {
            if (effectController != null) effectController.AddEffect(collection);
        }
        /// <summary>
        /// 被非对象击杀则传入null
        /// </summary>
        public virtual void OnKilled(Target killer)
        {
            Tool.NetworkCorrespondent.TargetKilledRpc(killer, this);
            targetDataSync.DestroyRpc();
        }

        public void UseSkillRpc(short index) => targetDataSync.UseSkillRpc(index);
        public void SyncEffectIconRpc(HashSet<(EffectType, int)> values) => targetDataSync.SyncEffectIconRpc(values);
        public void Interrupt() => TimeLineWork.Interrupted();
    }
}