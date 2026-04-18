using System;
using System.Collections.Generic;
using UnityEngine;

namespace LevelCreator.Internal
{
    public class InternalPackageLoader
    {
        private static readonly Dictionary<string, Action> Packages = new()
        {
            {"ShotgunPellets", Package1.Load },
            {"SkillPackage1", Package2.Load },
        };
        public static void Load(string packageName)
        {
            if (Packages.ContainsKey(packageName))
            {
                Packages[packageName].Invoke();
                Debug.Log("加载了内置包："+packageName);
            }
            else
            {
                Debug.LogError("未找到内置包");
            }
        }
    }
}