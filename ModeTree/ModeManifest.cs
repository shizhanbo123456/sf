using System;
using System.Collections.Generic;
using System.Linq;

namespace ModeTree
{
    public class ModeManifest
    {
        private static readonly List<ModeNode> DifficultyModeNode = new List<ModeNode>()
                            {
            new ModeInfo(){ ModeName = "新手", Level = 10 },
            new ModeInfo(){ ModeName = "简单", Level = 20 },
            new ModeInfo(){ ModeName = "普通", Level = 30 },
            new ModeInfo(){ ModeName = "困难", Level = 40 },
            new ModeInfo(){ ModeName = "试炼", Level = 50 },
            new ModeInfo(){ ModeName = "噩梦", Level = 60 }
        };
        public static readonly ModeNode Modes = new ModeNode()
        {
            ContentName = "模式选择",
            Submodes = new List<ModeNode>()
            {
                // 乱斗模式
                new ModeNode()
                {
                    ModeName = "乱斗模式",
                    ContentName = "结束条件",
                    Submodes = new List<ModeNode>()
                    {
                        new ModeInfo() { ModeName = "单队30击杀", Level = 10 },
                        new ModeInfo() { ModeName = "单队60击杀", Level = 10 },
                        new ModeInfo() { ModeName = "单队100击杀", Level = 10 },
                        new ModeInfo() { ModeName = "300秒时限", Level = 10 },
                        new ModeInfo() { ModeName = "450秒时限", Level = 10 },
                        new ModeInfo() { ModeName = "600秒时限", Level = 10 }
                    }
                },
                // 非对称攻防
                new ModeNode()
                {
                    ModeName = "非对称攻防",
                    ContentName = "结束条件",
                    Submodes = new List<ModeNode>()
                    {
                        new ModeInfo() { ModeName = "蓝方的矿石被摧毁/蓝方被击杀", Level = 10 },
                        new ModeInfo() { ModeName = "蓝方的矿石被摧毁/360秒时限", Level = 10 },
                        new ModeInfo() { ModeName = "蓝方的矿石被摧毁", Level = 10 },
                        new ModeInfo() { ModeName = "蓝方的矿石被摧毁/蓝方被击杀/360秒时限", Level = 10 }
                    }
                },
                // PVE
                new ModeNode()
                {
                    ModeName = "PVE",
                    ContentName = "目标地图",
                    Submodes = new List<ModeNode>()
                    {
                        new ModeNode()
                        {
                            ModeName = "『猎羊山原』",
                            ContentName = "难度选择",
                            Submodes = DifficultyModeNode
                        },
                        new ModeNode()
                        {
                            ModeName = "『霜熔之地』",
                            ContentName = "难度选择",
                            Submodes = DifficultyModeNode
                        },
                        new ModeNode()
                        {
                            ModeName = "『爻砂之地』",
                            ContentName = "难度选择",
                            Submodes = DifficultyModeNode
                        },
                        new ModeNode()
                        {
                            ModeName = "『斗兽场』",
                            ContentName = "难度选择",
                            Submodes = DifficultyModeNode
                        },
                        new ModeNode()
                        {
                            ModeName = "『登霄峰』",
                            ContentName = "难度选择",
                            Submodes = DifficultyModeNode
                        },
                        new ModeNode()
                        {
                            ModeName = "『幽影峡』",
                            ContentName = "难度选择",
                            Submodes = DifficultyModeNode
                        },
                        new ModeNode()
                        {
                            ModeName = "『梦魇之渊』",
                            ContentName = "难度选择",
                            Submodes = DifficultyModeNode
                        }
                    }
                },
            }
        };
        public static string ModeListToString(string modeList)
        {
            // 转换为索引列表（每个字符对应一个层级的0基索引）
            List<int> indices = modeList.Select(c => c - '0').ToList();
            ModeNode currentNode = Modes;
            List<string> namePath = new List<string>();

            foreach (int index in indices)
            {
                // 校验当前层级索引有效性
                if (currentNode.Submodes == null || index < 0 || index >= currentNode.Submodes.Count)
                    return string.Empty;

                // 进入下一级节点并记录名称
                currentNode = currentNode.Submodes[index];
                string nodeName = string.IsNullOrEmpty(currentNode.ModeName)
                    ? currentNode.ContentName
                    : currentNode.ModeName;
                namePath.Add(nodeName);
            }

            // 拼接模式路径（排除根节点"模式选择"）
            return namePath.FirstOrDefault() == "模式选择"
                ? string.Join("-", namePath.Skip(1))
                : string.Join("-", namePath);
        }
        public static string ModeName(int id)
        {
            return Modes.Submodes[id].ModeName;
        }
        public static int GetLevel(string modeList)
        {
            // 转换为索引列表（每个字符对应一个层级的0基索引）
            List<int> indices = modeList.Select(c => c - '0').ToList();
            ModeNode currentNode = Modes;

            foreach (int index in indices)
            {
                // 校验当前层级索引有效性
                if (currentNode.Submodes == null || index < 0 || index >= currentNode.Submodes.Count)
                    throw new Exception(modeList);

                // 进入下一级节点并记录名称
                currentNode = currentNode.Submodes[index];
            }

            // 拼接模式路径（排除根节点"模式选择"）
            return (currentNode as ModeInfo).Level;
        }
    }
}