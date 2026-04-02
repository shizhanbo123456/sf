using System;
using System.Collections.Generic;
using UnityEngine;
namespace LevelCreator
{
    /// <summary>
    /// 技能构建器（标准Builder，所有Create均带id标识）
    /// </summary>
    public static class SkillBuilder
    {
        // 核心标识（所有Create必传）
        private static short _id;
        // 新增图标参数
        private static short _iconType;
        private static short _iconIndex;
        // 基础参数
        private static string _name;
        private static string _des;
        private static short _operationTime;
        private static short _cd;
        private static short _maxStoreTime;
        // 动作列表（可0/1/多次添加）
        private static List<short> _actionIds;
        private static List<short> _actionDelays;

        // --------------- 所有Create方法 均包含 id + iconType + iconIndex ---------------
        /// <summary>
        /// 创建技能（基础：id + 图标参数 + 名称 + 描述）
        /// </summary>
        public static void Create(short id, short iconType, short iconIndex, string name, string des,short operationtime)
        {
            Reset();
            _id = id;
            _iconType = iconType;
            _iconIndex = iconIndex;
            _name = name;
            _des = des;
            _operationTime = operationtime;
            _cd = 0;
            _maxStoreTime = 0;
        }

        /// <summary>
        /// 创建技能（id + 图标参数 + 名称 + 描述 + CD）
        /// </summary>
        public static void Create(short id, short iconType, short iconIndex, string name, string des, short operationtime, short cd)
        {
            Reset();
            _id = id;
            _iconType = iconType;
            _iconIndex = iconIndex;
            _name = name;
            _des = des;
            _operationTime = operationtime;
            _cd = cd;
            _maxStoreTime = 0;
        }

        /// <summary>
        /// 创建技能（完整：id + 图标参数 + 名称 + 描述 + CD + 最大存储时间）
        /// </summary>
        public static void Create(short id, short iconType, short iconIndex, string name, string des, short operationtime, short cd, short maxStoreTime)
        {
            Reset();
            _id = id;
            _iconType = iconType;
            _iconIndex = iconIndex;
            _name = name;
            _des = des;
            _operationTime = operationtime;
            _cd = cd;
            _maxStoreTime = maxStoreTime;
        }

        // --------------- 添加动作（可0/1/多次调用）---------------
        public static void AddAction(short actionId, short delay)
        {
            _actionIds.Add(actionId);
            _actionDelays.Add(delay);
        }

        // --------------- 标准上传 ---------------
        public static void Upload()
        {
            // 构建Info（仅此处创建）
            SkillInfo info = new SkillInfo
            {
                id = _id,
                iconType = _iconType,
                iconIndex = _iconIndex,
                name = _name,
                des = _des,
                cd = _cd,
                operationtime = _operationTime,
                maxStoreTime = _maxStoreTime,
                actionIds = _actionIds,
                actionDelays = _actionDelays
            };
            // 提交给管理器
            Tool.LevelCreatorManager.LoadInfo(info);
        }

        // --------------- 重置所有参数 ---------------
        private static void Reset()
        {
            _id = 0;
            _iconType = 0;
            _iconIndex = 0;
            _name = string.Empty;
            _des = string.Empty;
            _operationTime = 0;
            _cd = 0;
            _maxStoreTime = 0;
            // 初始化空列表，永远不为null
            _actionIds = new List<short>();
            _actionDelays = new List<short>();
        }
    }

    // --------------- 标准 Info 结构体 ---------------
    public struct SkillInfo : Info
    {
        public short id;
        public short iconType;
        public short iconIndex;
        public string name;
        public string des;
        public short operationtime;
        public short cd;
        public short maxStoreTime;
        public List<short> actionIds;
        public List<short> actionDelays;
        public Vector2Int sprite => new(iconType, iconIndex);
    }

    // --------------- 标准序列化器 ---------------
    public struct SkillInfoSerializer
    {
        public static bool Serialize(SkillInfo value, byte[] result, ref int indexStart)
        {
            // ID
            if (!ShortSerializer.Serialize(value.id, result, ref indexStart)) return false;
            // 图标参数
            if (!ShortSerializer.Serialize(value.iconType, result, ref indexStart)) return false;
            if (!ShortSerializer.Serialize(value.iconIndex, result, ref indexStart)) return false;
            // 字符串
            if (!StringSerializer.Serialize(value.name, result, ref indexStart)) return false;
            if (!StringSerializer.Serialize(value.des, result, ref indexStart)) return false;
            // 数值
            if (!ShortSerializer.Serialize(value.operationtime, result, ref indexStart)) return false;
            if (!ShortSerializer.Serialize(value.cd, result, ref indexStart)) return false;
            if (!ShortSerializer.Serialize(value.maxStoreTime, result, ref indexStart)) return false;
            // 动作列表（兼容空列表）
            if (!IntSerializer.Serialize(value.actionIds.Count, result, ref indexStart)) return false;
            for (int i = 0; i < value.actionIds.Count; i++)
            {
                if (!ShortSerializer.Serialize(value.actionIds[i], result, ref indexStart)) return false;
                if (!ShortSerializer.Serialize(value.actionDelays[i], result, ref indexStart)) return false;
            }
            return true;
        }

        public static SkillInfo Deserialize(byte[] data, ref int indexStart, int invalidIndex)
        {
            SkillInfo info = new SkillInfo();
            // ID
            info.id = ShortSerializer.Deserialize(data, ref indexStart, invalidIndex);
            // 图标参数
            info.iconType = ShortSerializer.Deserialize(data, ref indexStart, invalidIndex);
            info.iconIndex = ShortSerializer.Deserialize(data, ref indexStart, invalidIndex);
            // 字符串
            info.name = StringSerializer.Deserialize(data, ref indexStart, invalidIndex);
            info.des = StringSerializer.Deserialize(data, ref indexStart, invalidIndex);
            // 数值
            info.operationtime = ShortSerializer.Deserialize(data, ref indexStart, invalidIndex);
            info.cd = ShortSerializer.Deserialize(data, ref indexStart, invalidIndex);
            info.maxStoreTime = ShortSerializer.Deserialize(data, ref indexStart, invalidIndex);
            // 动作列表
            int count = IntSerializer.Deserialize(data, ref indexStart, invalidIndex);
            info.actionIds = new List<short>();
            info.actionDelays = new List<short>();
            for (int i = 0; i < count; i++)
            {
                short actionId = ShortSerializer.Deserialize(data, ref indexStart, invalidIndex);
                short delay = ShortSerializer.Deserialize(data, ref indexStart, invalidIndex);
                info.actionIds.Add(actionId);
                info.actionDelays.Add(delay);
            }
            return info;
        }
    }
}