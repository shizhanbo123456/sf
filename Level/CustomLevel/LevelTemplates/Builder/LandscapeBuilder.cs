using System;
using System.Collections.Generic;
namespace LevelCreator
{
    public static class LandscapeBuilder
    {
        // 基础参数
        private static short _id;
        private static byte _sizeX;
        private static byte _sizeY;
        // 可多次添加的地形元素列表（永远非空）
        private static List<SolidLand> _solidLands;
        private static List<LevitatingPlatform> _levitatingPlatforms;
        private static List<VelocityArea> _velocityAreas;

        // 内部数据结构
        public struct SolidLand
        {
            public byte point1X;
            public byte point1Y;
            public byte point2X;
            public byte point2Y;
        }
        public struct LevitatingPlatform
        {
            public byte centerX;
            public byte centerY;
            public byte width;
        }
        public struct VelocityArea
        {
            public byte point1X;
            public byte point1Y;
            public byte point2X;
            public byte point2Y;
            public byte velocityX;
            public byte velocityY;
        }

        /// <summary>
        /// 标准初始化：Create(short id)
        /// </summary>
        public static void Create(short id)
        {
            Reset();
            _id = id;
        }

        /// <summary>
        /// 设置地图尺寸（x,y 1~15，size = x*16, y*8）
        /// </summary>
        public static void SetSize(byte x, byte y)
        {
            _sizeX = x;
            _sizeY = y;
        }

        public static void CreateOutline(byte thickness)
        {
            CreateSolidLand(0, 0, (byte)(_sizeX * 16 - 1), (byte)(thickness-1));
            CreateSolidLand(0, (byte)(_sizeY*8-thickness), (byte)(_sizeX * 16 - 1), (byte)(_sizeY * 8 - 1));
            CreateSolidLand(0, 0, (byte)(thickness - 1), (byte)(_sizeY * 8 - 1));
            CreateSolidLand((byte)(_sizeX*16-thickness), 0, (byte)(_sizeX * 16 - 1), (byte)(_sizeY * 8 - 1));
        }

        /// <summary>
        /// 添加实心地面（0/1/多次）
        /// </summary>
        public static void CreateSolidLand(byte point1x, byte point1y, byte point2x, byte point2y)
        {
            _solidLands.Add(new SolidLand
            {
                point1X = point1x,
                point1Y = point1y,
                point2X = point2x,
                point2Y = point2y
            });
        }

        /// <summary>
        /// 添加悬浮平台（0/1/多次）
        /// </summary>
        public static void CreateLevitatingPlatform(byte centerX, byte centerY, byte width)
        {
            _levitatingPlatforms.Add(new LevitatingPlatform
            {
                centerX = centerX,
                centerY = centerY,
                width = width
            });
        }

        /// <summary>
        /// 添加速度区域（0/1/多次）
        /// </summary>
        public static void CreateVelocityArea(byte point1x, byte point1y, byte point2x, byte point2y, byte velocityX, byte velocityY)
        {
            _velocityAreas.Add(new VelocityArea
            {
                point1X = point1x,
                point1Y = point1y,
                point2X = point2x,
                point2Y = point2y,
                velocityX = velocityX,
                velocityY = velocityY
            });
        }

        /// <summary>
        /// 标准上传：仅此处创建 LandscapeInfo
        /// </summary>
        public static void Upload()
        {
            LandscapeInfo info = new LandscapeInfo
            {
                id = _id,
                sizeX = _sizeX,
                sizeY = _sizeY,
                solidLands = _solidLands,
                levitatingPlatforms = _levitatingPlatforms,
                velocityAreas = _velocityAreas
            };
            Tool.LevelCreatorManager.LoadInfo(info);
        }

        /// <summary>
        /// 重置所有参数
        /// </summary>
        private static void Reset()
        {
            _id = 0;
            _sizeX = 0;
            _sizeY = 0;
            _solidLands = new List<SolidLand>();
            _levitatingPlatforms = new List<LevitatingPlatform>();
            _velocityAreas = new List<VelocityArea>();
        }
    }

    /// <summary>
    /// 标准 LandscapeInfo
    /// </summary>
    public struct LandscapeInfo : Info
    {
        public short id;
        public byte sizeX;
        public byte sizeY;
        public List<LandscapeBuilder.SolidLand> solidLands;
        public List<LandscapeBuilder.LevitatingPlatform> levitatingPlatforms;
        public List<LandscapeBuilder.VelocityArea> velocityAreas;
    }

    /// <summary>
    /// 标准序列化器
    /// </summary>
    public struct LandscapeInfoSerializer
    {
        public static bool Serialize(LandscapeInfo value, byte[] result, ref int indexStart)
        {
            // ID & 尺寸
            if (!ShortSerializer.Serialize(value.id, result, ref indexStart)) return false;
            if (!ByteSerializer.Serialize(value.sizeX, result, ref indexStart)) return false;
            if (!ByteSerializer.Serialize(value.sizeY, result, ref indexStart)) return false;

            // 实心地面
            if (!IntSerializer.Serialize(value.solidLands.Count, result, ref indexStart)) return false;
            foreach (var item in value.solidLands)
            {
                if (!ByteSerializer.Serialize(item.point1X, result, ref indexStart)) return false;
                if (!ByteSerializer.Serialize(item.point1Y, result, ref indexStart)) return false;
                if (!ByteSerializer.Serialize(item.point2X, result, ref indexStart)) return false;
                if (!ByteSerializer.Serialize(item.point2Y, result, ref indexStart)) return false;
            }

            // 悬浮平台
            if (!IntSerializer.Serialize(value.levitatingPlatforms.Count, result, ref indexStart)) return false;
            foreach (var item in value.levitatingPlatforms)
            {
                if (!ByteSerializer.Serialize(item.centerX, result, ref indexStart)) return false;
                if (!ByteSerializer.Serialize(item.centerY, result, ref indexStart)) return false;
                if (!ByteSerializer.Serialize(item.width, result, ref indexStart)) return false;
            }

            // 速度区域
            if (!IntSerializer.Serialize(value.velocityAreas.Count, result, ref indexStart)) return false;
            foreach (var item in value.velocityAreas)
            {
                if (!ByteSerializer.Serialize(item.point1X, result, ref indexStart)) return false;
                if (!ByteSerializer.Serialize(item.point1Y, result, ref indexStart)) return false;
                if (!ByteSerializer.Serialize(item.point2X, result, ref indexStart)) return false;
                if (!ByteSerializer.Serialize(item.point2Y, result, ref indexStart)) return false;
                if (!ByteSerializer.Serialize(item.velocityX, result, ref indexStart)) return false;
                if (!ByteSerializer.Serialize(item.velocityY, result, ref indexStart)) return false;
            }

            return true;
        }

        public static LandscapeInfo Deserialize(byte[] data, ref int indexStart, int invalidIndex)
        {
            LandscapeInfo info = new LandscapeInfo();

            // ID & 尺寸
            info.id = ShortSerializer.Deserialize(data, ref indexStart, invalidIndex);
            info.sizeX = ByteSerializer.Deserialize(data, ref indexStart, invalidIndex);
            info.sizeY = ByteSerializer.Deserialize(data, ref indexStart, invalidIndex);

            // 实心地面
            int solidCount = IntSerializer.Deserialize(data, ref indexStart, invalidIndex);
            info.solidLands = new List<LandscapeBuilder.SolidLand>();
            for (int i = 0; i < solidCount; i++)
            {
                var item = new LandscapeBuilder.SolidLand();
                item.point1X = ByteSerializer.Deserialize(data, ref indexStart, invalidIndex);
                item.point1Y = ByteSerializer.Deserialize(data, ref indexStart, invalidIndex);
                item.point2X = ByteSerializer.Deserialize(data, ref indexStart, invalidIndex);
                item.point2Y = ByteSerializer.Deserialize(data, ref indexStart, invalidIndex);
                info.solidLands.Add(item);
            }

            // 悬浮平台
            int platformCount = IntSerializer.Deserialize(data, ref indexStart, invalidIndex);
            info.levitatingPlatforms = new List<LandscapeBuilder.LevitatingPlatform>();
            for (int i = 0; i < platformCount; i++)
            {
                var item = new LandscapeBuilder.LevitatingPlatform();
                item.centerX = ByteSerializer.Deserialize(data, ref indexStart, invalidIndex);
                item.centerY = ByteSerializer.Deserialize(data, ref indexStart, invalidIndex);
                item.width = ByteSerializer.Deserialize(data, ref indexStart, invalidIndex);
                info.levitatingPlatforms.Add(item);
            }

            // 速度区域
            int velocityCount = IntSerializer.Deserialize(data, ref indexStart, invalidIndex);
            info.velocityAreas = new List<LandscapeBuilder.VelocityArea>();
            for (int i = 0; i < velocityCount; i++)
            {
                var item = new LandscapeBuilder.VelocityArea();
                item.point1X = ByteSerializer.Deserialize(data, ref indexStart, invalidIndex);
                item.point1Y = ByteSerializer.Deserialize(data, ref indexStart, invalidIndex);
                item.point2X = ByteSerializer.Deserialize(data, ref indexStart, invalidIndex);
                item.point2Y = ByteSerializer.Deserialize(data, ref indexStart, invalidIndex);
                item.velocityX = ByteSerializer.Deserialize(data, ref indexStart, invalidIndex);
                item.velocityY = ByteSerializer.Deserialize(data, ref indexStart, invalidIndex);
                info.velocityAreas.Add(item);
            }

            return info;
        }
    }
}