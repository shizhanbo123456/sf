using System;
using System.Collections.Generic;

namespace LevelCreator
{
    public static class TargetBuilder
    {
        // 标准：仅存储独立参数，不存 Info；拆分 TargetIdentify 为基础字段
        private static ushort _id;
        private static int _level;
        private static float _size;
        private static string _label;
        private static int _targetType;
        private static int _graphicType;
        private static int _controllerType;
        private static int _skillControllerType;
        private static int _effectControllerType;
        private static Dictionary<TargetParams, string> _params;

        /// <summary>
        /// 标准初始化：带 ID 标识
        /// </summary>
        public static void Create(ushort id)
        {
            Reset();
            _id = id;
        }

        /// <summary>
        /// 拆分式设置（方便编辑器/脚本调用）：替换原 SetTargetIdentify，直接设置基础字段
        /// </summary>
        public static void SetBaseInfo(int level,float size, string label, int targetType, int graphicType)
        {
            _level = level;
            _size = size;
            _label = label;
            _targetType = targetType;
            _graphicType = graphicType;
        }

        /// <summary>
        /// 加载控制器
        /// </summary>
        public static void LoadController(int controllerType)
        {
            _controllerType = controllerType;
        }

        public static void LoadSkillController(int skillControllerType)
        {
            _skillControllerType = skillControllerType;
        }

        public static void LoadEffectController(int effectControllerType)
        {
            _effectControllerType = effectControllerType;
        }

        /// <summary>
        /// 加载参数
        /// </summary>
        public static void LoadParams(Dictionary<TargetParams, string> paramsDict)
        {
            _params = new Dictionary<TargetParams, string>(paramsDict);
        }

        /// <summary>
        /// 标准上传：仅此处创建 Info，直接使用拆解后的字段
        /// </summary>
        public static void Upload()
        {
            TargetInfo info = new TargetInfo
            {
                id = _id,
                // 直接赋值拆解后的字段
                level = _level,
                size = _size,
                label = _label,
                targetType = _targetType,
                graphicType = _graphicType,
                controllerType = _controllerType,
                skillControllerType = _skillControllerType,
                effectControllerType = _effectControllerType,
                param = _params
            };

            Tool.LevelCreatorManager.LoadInfo(info);
        }

        /// <summary>
        /// 标准重置：包含拆解后的所有字段
        /// </summary>
        private static void Reset()
        {
            _id = 0;
            // 重置拆解后的 TargetIdentify 字段
            _level = 0;
            _size = 0f;
            _label = string.Empty;
            // 重置原有基础字段
            _targetType = 0;
            _graphicType = 0;
            _controllerType = 0;
            _skillControllerType = 0;
            _effectControllerType = 0;
            _params = new Dictionary<TargetParams, string>();
        }
    }

    /// <summary>
    /// 标准 Info 结构体：移除 TargetIdentify，替换为独立字段
    /// </summary>
    public struct TargetInfo : Info
    {
        public ushort id;
        // 原 TargetIdentify 拆解后的独立字段
        public int level;
        public float size;
        public string label;
        // 原有基础字段
        public int targetType;
        public int graphicType;
        public int controllerType;
        public int skillControllerType;
        public int effectControllerType;
        public Dictionary<TargetParams, string> param;
    }

    /// <summary>
    /// 标准序列化器：适配拆解后的字段，移除 TargetIdentify 序列化逻辑
    /// </summary>
    public struct TargetInfoSerializer
    {
        public static bool Serialize(TargetInfo value, byte[] result, ref int indexStart)
        {
            // ID
            if (!UshortSerializer.Serialize(value.id, result, ref indexStart)) return false;

            // 原 TargetIdentify 拆解后的字段序列化（保持原有顺序和类型）
            if (!IntSerializer.Serialize(value.level, result, ref indexStart)) return false;
            if (!FloatSerializer.Serialize(value.size, result, ref indexStart)) return false;
            if (!StringSerializer.Serialize(value.label, result, ref indexStart)) return false;

            // 基础信息
            if (!IntSerializer.Serialize(value.targetType, result, ref indexStart)) return false;
            if (!IntSerializer.Serialize(value.graphicType, result, ref indexStart)) return false;
            if (!IntSerializer.Serialize(value.controllerType, result, ref indexStart)) return false;
            if (!IntSerializer.Serialize(value.skillControllerType, result, ref indexStart)) return false;
            if (!IntSerializer.Serialize(value.effectControllerType, result, ref indexStart)) return false;

            // 字典
            if (!IntSerializer.Serialize(value.param.Count, result, ref indexStart)) return false;
            foreach (var p in value.param)
            {
                if (!IntSerializer.Serialize((int)p.Key, result, ref indexStart)) return false;
                if (!StringSerializer.Serialize(p.Value, result, ref indexStart)) return false;
            }

            return true;
        }

        public static TargetInfo Deserialize(byte[] data, ref int indexStart, int invalidIndex)
        {
            TargetInfo info = new TargetInfo();
            // ID 反序列化
            info.id = UshortSerializer.Deserialize(data, ref indexStart, invalidIndex);

            // 原 TargetIdentify 拆解后的字段反序列化（移除错误的 spawnX/spawnY 逻辑）
            info.level = IntSerializer.Deserialize(data, ref indexStart, invalidIndex);
            info.size = FloatSerializer.Deserialize(data, ref indexStart, invalidIndex);
            info.label = StringSerializer.Deserialize(data, ref indexStart, invalidIndex);

            // 基础信息反序列化
            info.targetType = IntSerializer.Deserialize(data, ref indexStart, invalidIndex);
            info.graphicType = IntSerializer.Deserialize(data, ref indexStart, invalidIndex);
            info.controllerType = IntSerializer.Deserialize(data, ref indexStart, invalidIndex);
            info.skillControllerType = IntSerializer.Deserialize(data, ref indexStart, invalidIndex);
            info.effectControllerType = IntSerializer.Deserialize(data, ref indexStart, invalidIndex);

            // 字典反序列化
            int count = IntSerializer.Deserialize(data, ref indexStart, invalidIndex);
            info.param = new Dictionary<TargetParams, string>();
            for (int i = 0; i < count; i++)
            {
                TargetParams key = (TargetParams)IntSerializer.Deserialize(data, ref indexStart, invalidIndex);
                string val = StringSerializer.Deserialize(data, ref indexStart, invalidIndex);
                info.param[key] = val;
            }

            return info;
        }
    }
}