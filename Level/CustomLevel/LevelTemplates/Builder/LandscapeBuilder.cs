using System;
using System.Collections.Generic;

namespace LevelCreator
{
    public static class LandscapeBuilder
    {
        // 基础参数
        private static ushort _id;
        private static byte _sizeX;
        private static byte _sizeY;

        // 可多次添加的地形元素列表（永远非空）
        private static List<SolidLand> _solidLands;
        private static List<LevitatingPlatform> _levitatingPlatforms;
        private static List<FanWindArea> _velocityAreas;
        private static List<BrokenPlatform> _brokenPlatforms;
        private static List<Trampoline> _trampolines;
        private static List<Spike> _spikes;

        public struct SolidLand
        {
            public byte point1X;
            public byte point1Y;
            public byte point2X;
            public byte point2Y;
        }

        public struct LevitatingPlatform
        {
            public byte leftX;
            public byte leftY;
            public byte width;
        }

        public struct FanWindArea
        {
            public byte point1X;
            public byte point1Y;
            public byte point2X;
            public byte point2Y;
            public byte velocity;
        }

        public struct BrokenPlatform
        {
            public byte leftX;
            public byte leftY;
            public byte width;
        }

        public struct Trampoline
        {
            public byte leftX;
            public byte leftY;
            public byte width;
            public byte velocity;
        }

        public struct Spike
        {
            public byte point1X;
            public byte point1Y;
            public byte point2X;
            public byte point2Y;
            public int damage;
        }

        /// <summary>
        /// 标准初始化：Create(short id)
        /// </summary>
        public static void Create(ushort id)
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
            CreateSolidLand(0, 0, (byte)(_sizeX * 16 - 1), (byte)(thickness - 1));
            CreateSolidLand(0, (byte)(_sizeY * 8 - thickness), (byte)(_sizeX * 16 - 1), (byte)(_sizeY * 8 - 1));
            CreateSolidLand(0, 0, (byte)(thickness - 1), (byte)(_sizeY * 8 - 1));
            CreateSolidLand((byte)(_sizeX * 16 - thickness), 0, (byte)(_sizeX * 16 - 1), (byte)(_sizeY * 8 - 1));
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
        public static void CreateLevitatingPlatform(byte leftX, byte leftY, byte width)
        {
            _levitatingPlatforms.Add(new LevitatingPlatform
            {
                leftX = leftX,
                leftY = leftY,
                width = width
            });
        }

        /// <summary>
        /// 添加破损平台（0/1/多次）
        /// </summary>
        public static void CreateBrokenPlatform(byte leftX, byte leftY, byte width)
        {
            _brokenPlatforms.Add(new BrokenPlatform
            {
                leftX = leftX,
                leftY = leftY,
                width = width
            });
        }

        /// <summary>
        /// 添加速度区域（0/1/多次）
        /// </summary>
        public static void CreateFanWindArea(byte point1x, byte point1y, byte point2x, byte point2y, byte velocity)
        {
            _velocityAreas.Add(new FanWindArea
            {
                point1X = point1x,
                point1Y = point1y,
                point2X = point2x,
                point2Y = point2y,
                velocity = velocity
            });
        }

        /// <summary>
        /// 添加弹跳板（0/1/多次）
        /// </summary>
        public static void CreateTrampoline(byte leftX, byte leftY, byte width, byte velocity)
        {
            _trampolines.Add(new Trampoline
            {
                leftX = leftX,
                leftY = leftY,
                width = width,
                velocity = velocity
            });
        }

        /// <summary>
        /// 添加刺（0/1/多次）
        /// </summary>
        public static void CreateSpike(byte point1X, byte point1Y, byte point2X, byte point2Y, int damage)
        {
            _spikes.Add(new Spike
            {
                point1X = point1X,
                point1Y = point1Y,
                point2X = point2X,
                point2Y = point2Y,
                damage = damage
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
                windAreas = _velocityAreas,
                brokenPlatforms = _brokenPlatforms,
                trampolines = _trampolines,
                spikes = _spikes
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
            _velocityAreas = new List<FanWindArea>();
            _brokenPlatforms = new List<BrokenPlatform>();
            _trampolines = new List<Trampoline>();
            _spikes = new List<Spike>();
        }
    }

    /// <summary>
    /// 标准 LandscapeInfo
    /// </summary>
    public struct LandscapeInfo : Info
    {
        public ushort id;
        public byte sizeX;
        public byte sizeY;
        public List<LandscapeBuilder.SolidLand> solidLands;
        public List<LandscapeBuilder.LevitatingPlatform> levitatingPlatforms;
        public List<LandscapeBuilder.FanWindArea> windAreas;
        public List<LandscapeBuilder.BrokenPlatform> brokenPlatforms;
        public List<LandscapeBuilder.Trampoline> trampolines;
        public List<LandscapeBuilder.Spike> spikes;
    }

    /// <summary>
    /// 标准序列化器
    /// </summary>
    public struct LandscapeInfoSerializer
    {
        public static bool Serialize(LandscapeInfo value, byte[] result, ref int indexStart)
        {
            // ID & 尺寸
            if (!UshortSerializer.Serialize(value.id, result, ref indexStart)) return false;
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
                if (!ByteSerializer.Serialize(item.leftX, result, ref indexStart)) return false;
                if (!ByteSerializer.Serialize(item.leftY, result, ref indexStart)) return false;
                if (!ByteSerializer.Serialize(item.width, result, ref indexStart)) return false;
            }

            // 速度区域
            if (!IntSerializer.Serialize(value.windAreas.Count, result, ref indexStart)) return false;
            foreach (var item in value.windAreas)
            {
                if (!ByteSerializer.Serialize(item.point1X, result, ref indexStart)) return false;
                if (!ByteSerializer.Serialize(item.point1Y, result, ref indexStart)) return false;
                if (!ByteSerializer.Serialize(item.point2X, result, ref indexStart)) return false;
                if (!ByteSerializer.Serialize(item.point2Y, result, ref indexStart)) return false;
                if (!ByteSerializer.Serialize(item.velocity, result, ref indexStart)) return false;
            }

            // 破损平台
            if (!IntSerializer.Serialize(value.brokenPlatforms.Count, result, ref indexStart)) return false;
            foreach (var item in value.brokenPlatforms)
            {
                if (!ByteSerializer.Serialize(item.leftX, result, ref indexStart)) return false;
                if (!ByteSerializer.Serialize(item.leftY, result, ref indexStart)) return false;
                if (!ByteSerializer.Serialize(item.width, result, ref indexStart)) return false;
            }

            // 弹跳板
            if (!IntSerializer.Serialize(value.trampolines.Count, result, ref indexStart)) return false;
            foreach (var item in value.trampolines)
            {
                if (!ByteSerializer.Serialize(item.leftX, result, ref indexStart)) return false;
                if (!ByteSerializer.Serialize(item.leftY, result, ref indexStart)) return false;
                if (!ByteSerializer.Serialize(item.width, result, ref indexStart)) return false;
                if (!ByteSerializer.Serialize(item.velocity, result, ref indexStart)) return false;
            }

            // 尖刺
            if (!IntSerializer.Serialize(value.spikes.Count, result, ref indexStart)) return false;
            foreach (var item in value.spikes)
            {
                if (!ByteSerializer.Serialize(item.point1X, result, ref indexStart)) return false;
                if (!ByteSerializer.Serialize(item.point1Y, result, ref indexStart)) return false;
                if (!ByteSerializer.Serialize(item.point2X, result, ref indexStart)) return false;
                if (!ByteSerializer.Serialize(item.point2Y, result, ref indexStart)) return false;
                if (!IntSerializer.Serialize(item.damage, result, ref indexStart)) return false;
            }

            return true;
        }

        public static LandscapeInfo Deserialize(byte[] data, ref int indexStart, int invalidIndex)
        {
            LandscapeInfo info = new LandscapeInfo();

            // ID & 尺寸
            info.id = UshortSerializer.Deserialize(data, ref indexStart, invalidIndex);
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
                item.leftX = ByteSerializer.Deserialize(data, ref indexStart, invalidIndex);
                item.leftY = ByteSerializer.Deserialize(data, ref indexStart, invalidIndex);
                item.width = ByteSerializer.Deserialize(data, ref indexStart, invalidIndex);
                info.levitatingPlatforms.Add(item);
            }

            // 速度区域
            int velocityCount = IntSerializer.Deserialize(data, ref indexStart, invalidIndex);
            info.windAreas = new List<LandscapeBuilder.FanWindArea>();
            for (int i = 0; i < velocityCount; i++)
            {
                var item = new LandscapeBuilder.FanWindArea();
                item.point1X = ByteSerializer.Deserialize(data, ref indexStart, invalidIndex);
                item.point1Y = ByteSerializer.Deserialize(data, ref indexStart, invalidIndex);
                item.point2X = ByteSerializer.Deserialize(data, ref indexStart, invalidIndex);
                item.point2Y = ByteSerializer.Deserialize(data, ref indexStart, invalidIndex);
                item.velocity = ByteSerializer.Deserialize(data, ref indexStart, invalidIndex);
                info.windAreas.Add(item);
            }

            // 破损平台
            int brokenCount = IntSerializer.Deserialize(data, ref indexStart, invalidIndex);
            info.brokenPlatforms = new List<LandscapeBuilder.BrokenPlatform>();
            for (int i = 0; i < brokenCount; i++)
            {
                var item = new LandscapeBuilder.BrokenPlatform();
                item.leftX = ByteSerializer.Deserialize(data, ref indexStart, invalidIndex);
                item.leftY = ByteSerializer.Deserialize(data, ref indexStart, invalidIndex);
                item.width = ByteSerializer.Deserialize(data, ref indexStart, invalidIndex);
                info.brokenPlatforms.Add(item);
            }

            // 弹跳板
            int trampolineCount = IntSerializer.Deserialize(data, ref indexStart, invalidIndex);
            info.trampolines = new List<LandscapeBuilder.Trampoline>();
            for (int i = 0; i < trampolineCount; i++)
            {
                var item = new LandscapeBuilder.Trampoline();
                item.leftX = ByteSerializer.Deserialize(data, ref indexStart, invalidIndex);
                item.leftY = ByteSerializer.Deserialize(data, ref indexStart, invalidIndex);
                item.width = ByteSerializer.Deserialize(data, ref indexStart, invalidIndex);
                item.velocity = ByteSerializer.Deserialize(data, ref indexStart, invalidIndex);
                info.trampolines.Add(item);
            }

            // 尖刺
            int spikeCount = IntSerializer.Deserialize(data, ref indexStart, invalidIndex);
            info.spikes = new List<LandscapeBuilder.Spike>();
            for (int i = 0; i < spikeCount; i++)
            {
                var item = new LandscapeBuilder.Spike();
                item.point1X = ByteSerializer.Deserialize(data, ref indexStart, invalidIndex);
                item.point1Y = ByteSerializer.Deserialize(data, ref indexStart, invalidIndex);
                item.point2X = ByteSerializer.Deserialize(data, ref indexStart, invalidIndex);
                item.point2Y = ByteSerializer.Deserialize(data, ref indexStart, invalidIndex);
                item.damage = IntSerializer.Deserialize(data, ref indexStart, invalidIndex);
                info.spikes.Add(item);
            }

            return info;
        }
    }
}