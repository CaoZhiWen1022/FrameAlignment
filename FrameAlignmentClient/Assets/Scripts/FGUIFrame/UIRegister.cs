using System;
using System.Collections.Generic;
using System.Linq;
using FairyGUI;

namespace FGUIFrame
{
    /// <summary>
    /// UI注册信息
    /// </summary>
    public class UIRegisterInfo
    {
        /// <summary>
        /// uiid
        /// </summary>
        public int UIID;
        /// <summary>
        /// 创建实例
        /// </summary>
        public Func<GComponent> createInstance;
        /// <summary>
        /// ui脚本类型
        /// </summary>
        public Type _class;
        /// <summary>
        /// ui类型
        /// </summary>
        public UIType UIType;
        /// <summary>
        /// ui层级
        /// </summary>
        public UILayer UILayer;
        /// <summary>
        /// ui包依赖
        /// </summary>
        public List<string> uiPackage;
        /// <summary>
        /// 是否允许和同权重的弹窗同时打开
        /// </summary>
        public bool isSamePriorityMeanwhileOpen;
        /// <summary>
        /// 弹窗权重
        /// </summary>
        public PopupPriority popupPriority;
        /// <summary>
        /// 弹窗的依赖Panel,存在依赖界面才打开
        /// </summary>
        public List<int> popupDependPanel;
        /// <summary>
        /// 弹窗超时时间，等不到依赖界面时多久清理
        /// </summary>
        public float popupTimeout;
    }

    /// <summary>
    /// UI注册管理
    /// </summary>
    public class UIRegister
    {
        public static List<UIRegisterInfo> ALLUIINFO = new List<UIRegisterInfo>();

        public static void Register(UIRegisterInfo info)
        {
            //避免重复注册
            if (GetUIInfo(info.UIID) != null)
            {
                UnityEngine.Debug.LogError($"uiID:{info.UIID} 已注册");
                return;
            }
            ALLUIINFO.Add(info);
        }

        public static UIRegisterInfo GetUIInfo(int uiID)
        {
            return ALLUIINFO.FirstOrDefault(info => info.UIID == uiID);
        }
    }
}

