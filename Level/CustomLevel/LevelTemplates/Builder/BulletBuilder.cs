using System;

namespace LevelCreator
{
    /// <summary>
    /// 子弹构建器（遵循Builder标准：参数独立存储，Upload时创建Info）
    /// </summary>
    public static class BulletBuilder
    {
        // 独立存储所有参数，不存储BulletInfo对象
        private static ushort _id;
        private static int _graphicType;
        private static float _radius;
        private static float _lifeTime;
        private static float _rate;
        private static byte _liftStoiclevel;
        private static float _hitBackForce;
        private static ushort _effect;

        /// <summary>
        /// 标准初始化方法
        /// </summary>
        public static void Create(ushort id)
        {
            Reset();
            _id = id;
        }

        /// <summary>
        /// 设置子弹完整参数
        /// </summary>
        public static void SetBulletParam(
            int graphicType,
            float radius,
            float lifeTime,
            float rate,
            byte liftstoiclevel = 1,
            float hitbackForce = 0f,
            ushort effect = 0)
        {
            _graphicType = graphicType;
            _radius = radius;
            _lifeTime = lifeTime;
            _rate = rate;
            _liftStoiclevel = liftstoiclevel;
            _hitBackForce = hitbackForce;
            _effect = effect;
        }

        /// <summary>
        /// 标准上传：仅在此处创建BulletInfo并提交
        /// </summary>
        public static void Upload()
        {
            BulletInfo info = new BulletInfo(
                _id,
                _graphicType,
                _radius,
                _lifeTime,
                _rate,
                _liftStoiclevel,
                _hitBackForce,
                _effect
            );
            Tool.LevelCreatorManager.LoadInfo(info);
        }

        /// <summary>
        /// 重置所有参数
        /// </summary>
        private static void Reset()
        {
            _id = 0;
            _graphicType = 0;
            _radius = 0f;
            _lifeTime = 0f;
            _rate = 0f;
            _liftStoiclevel = 0;
            _hitBackForce = 0f;
            _effect = 0;
        }
    }

    /// <summary>
    /// 子弹信息结构体（标准Info）
    /// </summary>
    public struct BulletInfo : Info
    {
        public ushort id;
        public int graphicType;
        public float radius;
        public float lifeTime;
        public float rate;
        public byte liftStoicLevel;
        public float hitBackForce;
        public ushort effect;

        public BulletInfo(ushort id, int graphicType, float radius, float lifeTime, float rate, byte liftStoicLevel, float hitBackForce, ushort effect)
        {
            this.id = id;
            this.graphicType = graphicType;
            this.radius = radius;
            this.lifeTime = lifeTime;
            this.rate = rate;
            this.liftStoicLevel = liftStoicLevel;
            this.hitBackForce = hitBackForce;
            this.effect = effect;
        }
    }

    /// <summary>
    /// 标准序列化器
    /// </summary>
    public struct BulletInfoSerializer
    {
        public static bool Serialize(BulletInfo value, byte[] result, ref int indexStart)
        {
            if (!UshortSerializer.Serialize(value.id, result, ref indexStart)) return false;
            if (!IntSerializer.Serialize(value.graphicType, result, ref indexStart)) return false;
            if (!FloatSerializer.Serialize(value.radius, result, ref indexStart)) return false;
            if (!FloatSerializer.Serialize(value.lifeTime, result, ref indexStart)) return false;
            if (!FloatSerializer.Serialize(value.rate, result, ref indexStart)) return false;
            if (!ByteSerializer.Serialize(value.liftStoicLevel, result, ref indexStart)) return false;
            if (!FloatSerializer.Serialize(value.hitBackForce, result, ref indexStart)) return false;
            if (!UshortSerializer.Serialize(value.effect, result, ref indexStart)) return false;
            return true;
        }

        public static BulletInfo Deserialize(byte[] data, ref int indexStart, int invalidIndex)
        {
            BulletInfo info = new BulletInfo();
            info.id = UshortSerializer.Deserialize(data, ref indexStart, invalidIndex);
            info.graphicType = IntSerializer.Deserialize(data, ref indexStart, invalidIndex);
            info.radius = FloatSerializer.Deserialize(data, ref indexStart, invalidIndex);
            info.lifeTime = FloatSerializer.Deserialize(data, ref indexStart, invalidIndex);
            info.rate = FloatSerializer.Deserialize(data, ref indexStart, invalidIndex);
            info.liftStoicLevel = ByteSerializer.Deserialize(data, ref indexStart, invalidIndex);
            info.hitBackForce = FloatSerializer.Deserialize(data, ref indexStart, invalidIndex);
            info.effect = UshortSerializer.Deserialize(data, ref indexStart, invalidIndex);
            return info;
        }
    }
}