using System;
using System.Collections.Generic;
namespace LevelCreator
{
    public static class EffectBuilder
    {
        private static List<SingleEffect> _effects = new List<SingleEffect>();
        private static EffectInfo _info;

        /// <summary>
        /// 初始化并录入ID标识
        /// </summary>
        public static void Create(ushort id)
        {
            _effects.Clear();
            _info = new EffectInfo(id, _effects);
        }

        /// <summary>
        /// 添加单个效果参数
        /// </summary>
        public static void AddEffect(int type, float value, float time)
        {
            _info.effects.Add(new SingleEffect((EffectType)type, value, time));
        }

        /// <summary>
        /// 将信息上传
        /// </summary>
        public static void Upload()
        {
            Tool.LevelCreatorManager.LoadInfo(_info);
        }
    }

    /// <summary>
    /// 存储EffectBuilder所有参数
    /// </summary>
    public struct EffectInfo : Info
    {
        public ushort id;
        public List<SingleEffect> effects;

        public EffectInfo(ushort id, List<SingleEffect> effects)
        {
            this.id = id;
            this.effects = effects;
        }
    }

    /// <summary>
    /// 符合标准的序列化器
    /// </summary>
    public struct EffectInfoSerializer
    {
        /// <summary>
        /// 序列化：写入字节数组，返回缓冲区是否足够
        /// </summary>
        public static bool Serialize(EffectInfo value, byte[] result, ref int indexStart)
        {
            // 序列化ID
            if (!UshortSerializer.Serialize(value.id, result, ref indexStart))
                return false;

            // 序列化效果数量
            int count = value.effects.Count;
            if (!IntSerializer.Serialize(count, result, ref indexStart))
                return false;

            // 逐个序列化效果
            foreach (var effect in value.effects)
            {
                // 类型
                if (!ByteSerializer.Serialize((byte)effect.effectType, result, ref indexStart))
                    return false;
                // 值
                if (!FloatSerializer.Serialize(effect.value, result, ref indexStart))
                    return false;
                // 时间
                if (!FloatSerializer.Serialize(effect.time, result, ref indexStart))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 反序列化：从字节数组读取，超出invalidIndex报错
        /// </summary>
        public static EffectInfo Deserialize(byte[] data, ref int indexStart, int invalidIndex)
        {
            // 读取ID
            ushort id = UshortSerializer.Deserialize(data, ref indexStart, invalidIndex);

            // 读取效果数量
            int count = IntSerializer.Deserialize(data, ref indexStart, invalidIndex);
            List<SingleEffect> effects = new List<SingleEffect>();

            for (int i = 0; i < count; i++)
            {
                // 类型
                byte typeByte = ByteSerializer.Deserialize(data, ref indexStart, invalidIndex);
                EffectType type = (EffectType)typeByte;
                // 值
                float value = FloatSerializer.Deserialize(data, ref indexStart, invalidIndex);
                // 时间
                float time = FloatSerializer.Deserialize(data, ref indexStart, invalidIndex);
                effects.Add(new SingleEffect(type, value, time));
            }
            return new EffectInfo(id, effects);
        }
    }
}