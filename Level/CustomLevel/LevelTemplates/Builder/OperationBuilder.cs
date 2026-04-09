using LevelCreator.Executer;
using LevelCreator.TargetTemplate;
using System;
using System.Collections.Generic;

namespace LevelCreator
{
    /// <summary>
    /// 技能操作构建器（标准Builder：参数独立存储，Upload时创建Info）
    /// 兼容：0次/1次/多次添加操作，无空引用异常
    /// </summary>
    public static class OperationBuilder
    {
        // 基础ID参数
        private static ushort _skillOpId;

        // 列表参数：初始化时直接创建空列表，确保永远不为null
        private static List<SubSkillOperator> _subSkillOperators;
        private static List<BulletShoot> _bulletShoots;
        private static List<MotionAction> _motionActions;
        private static List<EffectOperation> _effectOperations;

        // 内部子结构（仅构建器使用）
        public struct SubSkillOperator:ITimelineActor
        {
            public ushort delay;
            public ushort subSkillId;

            public void Act(Target t)
            {
                OperationExecuter.Execute(subSkillId, delay, t);
            }
        }

        public struct BulletShoot : ITimelineActor
        {
            public ushort delay;
            public ushort bulletInfoId;
            public ushort shootActId;

            public void Act(Target t)
            {
                BulletShootExecuter.ExecuteBulletShootAct(t, Tool.LevelCreatorManager.GetBulletInfo(bulletInfoId), shootActId);
            }
        }

        public struct MotionAction : ITimelineActor
        {
            public ushort delay;
            public ushort motionActId;

            public void Act(Target t)
            {
                MotionExecuter.Execute(motionActId, t);
            }
        }

        public struct EffectOperation : ITimelineActor
        {
            public ushort delay;
            public ushort effectId;
            public float operationRadius;

            private static UnityEngine.Vector3 pos;
            public void Act(Target t)
            {
                var absr=MathF.Abs(operationRadius);
                if (absr < 0.1f)
                {
                    EffectExecuter.Execute(effectId, t, t.ObjectId);
                    return;
                }
                List<Target> targets;
                if (operationRadius > 0)
                {
                    targets = t.GetPartnerInRange(absr % 1000);
                }
                else
                {
                    targets = t.GetEnemyInRange(absr % 1000);
                }
                if (absr > 1000)
                {
                    int count = (int)(absr / 1000);
                    pos = t.transform.position;
                    targets.Sort((a,b) =>
                    {
                        float dista = (a.transform.position - EffectOperation.pos).sqrMagnitude;
                        float distb = (b.transform.position - EffectOperation.pos).sqrMagnitude;
                        return dista.CompareTo(distb);
                    });
                    if (targets.Count > count)
                    {
                        targets.RemoveRange(count, targets.Count - count);
                    }
                }
                foreach(var i in targets) EffectExecuter.Execute(effectId, i, t.ObjectId);
            }
        }

        /// <summary>
        /// 标准初始化：自动创建空列表，杜绝null
        /// </summary>
        public static void Create(ushort id)
        {
            Reset();
            _skillOpId = id;
        }

        /// <summary>
        /// 添加子技能操作（支持0/1/多次调用）
        /// </summary>
        public static void AddSubSkillOperator(ushort delay, ushort subSkillId)
        {
            _subSkillOperators.Add(new SubSkillOperator
            {
                delay = delay,
                subSkillId = subSkillId
            });
        }

        /// <summary>
        /// 添加子弹发射（支持0/1/多次调用）
        /// </summary>
        public static void ShootBullet(ushort delay, ushort bulletInfoId, ushort shootActId)
        {
            _bulletShoots.Add(new BulletShoot
            {
                delay = delay,
                bulletInfoId = bulletInfoId,
                shootActId = shootActId
            });
        }

        /// <summary>
        /// 添加动作执行（支持0/1/多次调用）
        /// </summary>
        public static void DoMotion(ushort delay, ushort motionActId)
        {
            _motionActions.Add(new MotionAction
            {
                delay = delay,
                motionActId = motionActId
            });
        }

        /// <summary>
        /// 添加效果操作（支持0/1/多次调用）<br/>
        /// abs(r)<0.1自己，abs(r)%1000=范围，abs(r)//1000=作用目标数量(小于1000为所有)，>0为队友，<0为敌方
        /// </summary>
        public static void AddEffect(ushort delay, ushort effectId, float operationRadius)
        {
            _effectOperations.Add(new EffectOperation
            {
                delay = delay,
                effectId = effectId,
                operationRadius = operationRadius
            });
        }

        /// <summary>
        /// 标准上传：仅此处创建Info，永远安全（空列表也正常）
        /// </summary>
        public static void Upload()
        {
            OperationInfo info = new OperationInfo
            {
                id = _skillOpId,
                subSkillOperators = _subSkillOperators,
                bulletShoots = _bulletShoots,
                motionActions = _motionActions,
                effectOperations = _effectOperations
            };

            Tool.LevelCreatorManager.LoadInfo(info);
        }

        /// <summary>
        /// 标准重置：列表永远为非空空列表，无null风险
        /// </summary>
        public static void Reset()
        {
            _skillOpId = 0;
            _subSkillOperators = new List<SubSkillOperator>();
            _bulletShoots = new List<BulletShoot>();
            _motionActions = new List<MotionAction>();
            _effectOperations = new List<EffectOperation>();
        }
    }

    /// <summary>
    /// 技能操作信息结构体（标准Info）
    /// </summary>
    public struct OperationInfo : Info
    {
        public ushort id;
        public List<OperationBuilder.SubSkillOperator> subSkillOperators;
        public List<OperationBuilder.BulletShoot> bulletShoots;
        public List<OperationBuilder.MotionAction> motionActions;
        public List<OperationBuilder.EffectOperation> effectOperations;
    }

    /// <summary>
    /// 技能操作序列化器（标准序列化，兼容空列表）
    /// </summary>
    public struct OperationInfoSerializer
    {
        public static bool Serialize(OperationInfo value, byte[] result, ref int indexStart)
        {
            // 主ID
            if (!UshortSerializer.Serialize(value.id, result, ref indexStart))
                return false;

            // 子技能列表（空列表Count=0，正常序列化）
            if (!ShortSerializer.Serialize((short)value.subSkillOperators.Count, result, ref indexStart))
                return false;
            foreach (var item in value.subSkillOperators)
            {
                if (!UshortSerializer.Serialize(item.delay, result, ref indexStart)) return false;
                if (!UshortSerializer.Serialize(item.subSkillId, result, ref indexStart)) return false;
            }

            // 子弹发射列表
            if (!ShortSerializer.Serialize((short)value.bulletShoots.Count, result, ref indexStart))
                return false;
            foreach (var item in value.bulletShoots)
            {
                if (!UshortSerializer.Serialize(item.delay, result, ref indexStart)) return false;
                if (!UshortSerializer.Serialize(item.bulletInfoId, result, ref indexStart)) return false;
                if (!UshortSerializer.Serialize(item.shootActId, result, ref indexStart)) return false;
            }

            // 动作列表
            if (!ShortSerializer.Serialize((short)value.motionActions.Count, result, ref indexStart))
                return false;
            foreach (var item in value.motionActions)
            {
                if (!UshortSerializer.Serialize(item.delay, result, ref indexStart)) return false;
                if (!UshortSerializer.Serialize(item.motionActId, result, ref indexStart)) return false;
            }

            // 效果列表
            if (!ShortSerializer.Serialize((short)value.effectOperations.Count, result, ref indexStart))
                return false;
            foreach (var item in value.effectOperations)
            {
                if (!UshortSerializer.Serialize(item.delay, result, ref indexStart)) return false;
                if (!UshortSerializer.Serialize(item.effectId, result, ref indexStart)) return false;
                if (!FloatSerializer.Serialize(item.operationRadius, result, ref indexStart)) return false;
            }

            return true;
        }

        public static OperationInfo Deserialize(byte[] data, ref int indexStart, int invalidIndex)
        {
            OperationInfo info = new OperationInfo();

            // 主ID
            info.id = UshortSerializer.Deserialize(data, ref indexStart, invalidIndex);

            // 子技能（Count=0时不进入循环，完美兼容）
            short subSkillCount = ShortSerializer.Deserialize(data, ref indexStart, invalidIndex);
            info.subSkillOperators = new List<OperationBuilder.SubSkillOperator>();
            for (int i = 0; i < subSkillCount; i++)
            {
                var item = new OperationBuilder.SubSkillOperator();
                item.delay = UshortSerializer.Deserialize(data, ref indexStart, invalidIndex);
                item.subSkillId = UshortSerializer.Deserialize(data, ref indexStart, invalidIndex);
                info.subSkillOperators.Add(item);
            }

            // 子弹发射
            short bulletCount = ShortSerializer.Deserialize(data, ref indexStart, invalidIndex);
            info.bulletShoots = new List<OperationBuilder.BulletShoot>();
            for (int i = 0; i < bulletCount; i++)
            {
                var item = new OperationBuilder.BulletShoot();
                item.delay = UshortSerializer.Deserialize(data, ref indexStart, invalidIndex);
                item.bulletInfoId = UshortSerializer.Deserialize(data, ref indexStart, invalidIndex);
                item.shootActId = UshortSerializer.Deserialize(data, ref indexStart, invalidIndex);
                info.bulletShoots.Add(item);
            }

            // 动作
            short motionCount = ShortSerializer.Deserialize(data, ref indexStart, invalidIndex);
            info.motionActions = new List<OperationBuilder.MotionAction>();
            for (int i = 0; i < motionCount; i++)
            {
                var item = new OperationBuilder.MotionAction();
                item.delay = UshortSerializer.Deserialize(data, ref indexStart, invalidIndex);
                item.motionActId = UshortSerializer.Deserialize(data, ref indexStart, invalidIndex);
                info.motionActions.Add(item);
            }

            // 效果
            short effectCount = ShortSerializer.Deserialize(data, ref indexStart, invalidIndex);
            info.effectOperations = new List<OperationBuilder.EffectOperation>();
            for (int i = 0; i < effectCount; i++)
            {
                var item = new OperationBuilder.EffectOperation();
                item.delay = UshortSerializer.Deserialize(data, ref indexStart, invalidIndex);
                item.effectId = UshortSerializer.Deserialize(data, ref indexStart, invalidIndex);
                item.operationRadius = FloatSerializer.Deserialize(data, ref indexStart, invalidIndex);
                info.effectOperations.Add(item);
            }

            return info;
        }
    }
}