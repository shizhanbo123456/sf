using System;
using System.Collections.Generic;
using UnityEngine;

namespace LevelCreator.Internal
{
    public class InternalPackageLoader
    {
        private static readonly Dictionary<string, Action> Packages = new()
        {
            {"SmallShotgunPellets", Package1.Load1 }
        };
        public static void Load(string packageName)
        {
            if (Packages.ContainsKey(packageName))
            {
                Packages[packageName].Invoke();
            }
            else
            {
                Debug.LogError("未找到内置包");
            }
        }
    }
}