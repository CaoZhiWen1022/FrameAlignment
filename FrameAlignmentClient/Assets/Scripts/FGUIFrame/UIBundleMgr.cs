using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FairyGUI;

namespace FGUIFrame
{
    /// <summary>
    /// UI Package管理
    /// </summary>
    public class UIBundleMgr
    {
        /// <summary>
        /// 加载完成的package
        /// </summary>
        private static List<string> m_loadedPackage = new List<string>();
        /// <summary>
        /// 包的引用计数
        /// </summary>
        private static Dictionary<string, int> refCountMap = new Dictionary<string, int>();

        /// <summary>
        /// 加载Package列表（同步）
        /// </summary>
        public static bool LoadBundlePackage(List<string> packageNames)
        {
            bool allSuccess = true;
            foreach (var packageName in packageNames)
            {
                if (!LoadPackage(packageName))
                {
                    allSuccess = false;
                }
            }
            return allSuccess;
        }

        /// <summary>
        /// 加载package（同步）
        /// </summary>
        private static bool LoadPackage(string packageName)
        {
            if (m_loadedPackage.Contains(packageName))
            {
                return true;
            }

            try
            {
                // 使用 UIPackage.AddPackage 同步加载包
                UIPackage.AddPackage("FGUI/" + packageName + "/" + packageName);
                m_loadedPackage.Add(packageName);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"加载package失败 {packageName}: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// 增加引用计数
        /// </summary>
        public static void AddRefCount(List<string> packageNames)
        {
            foreach (var packageName in packageNames)
            {
                if (refCountMap.ContainsKey(packageName))
                {
                    refCountMap[packageName] = refCountMap[packageName] + 1;
                }
                else
                {
                    refCountMap[packageName] = 1;
                }
            }
        }

        /// <summary>
        /// 减少引用计数
        /// </summary>
        public static void RemoveRefCount(List<string> packageNames)
        {
            foreach (var packageName in packageNames)
            {
                if (refCountMap.ContainsKey(packageName))
                {
                    int count = refCountMap[packageName];
                    int newCount = count - 1;
                    refCountMap[packageName] = newCount;
                }
            }
            CheckAllowUnloadPackage();
        }

        /// <summary>
        /// 检查是否允许卸载的包
        /// </summary>
        private static void CheckAllowUnloadPackage()
        {
            int max = UIFrameConfig.MAX_PKGS + UIFrameConfig.PERMANENT_PKGS.Count;
            //卸载引用计数为0的非常驻包
            var keysToRemove = new List<string>();
            foreach (var kvp in refCountMap)
            {
                if (m_loadedPackage.Count <= max) break;
                if (kvp.Value == 0 && !UIFrameConfig.PERMANENT_PKGS.Contains(kvp.Key))
                {
                    UnBundlePackage(kvp.Key);
                    keysToRemove.Add(kvp.Key);
                }
            }
            foreach (var key in keysToRemove)
            {
                refCountMap.Remove(key);
            }
        }

        /// <summary>
        /// 卸载包
        /// </summary>
        private static void UnBundlePackage(string packageName)
        {
            if (m_loadedPackage.Contains(packageName))
            {
                // FairyGUI 卸载
                UIPackage.RemovePackage(packageName);
                m_loadedPackage.Remove(packageName);
            }
        }

    }
}

