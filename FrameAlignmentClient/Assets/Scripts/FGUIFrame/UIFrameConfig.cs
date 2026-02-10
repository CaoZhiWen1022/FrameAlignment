using System.Collections.Generic;

namespace FGUIFrame
{
    /// <summary>
    /// UI框架配置
    /// </summary>
    public class UIFrameConfig
    {
        /// <summary>
        /// 设计分辨率宽度
        /// </summary>
        public static int FRAME_WIDTH = 750;
        /// <summary>
        /// 设计分辨率高度
        /// </summary>
        public static int FRAME_HEIGHT = 1334;
        /// <summary>
        /// 初始化加载包
        /// </summary>
        public static List<string> INIT_LOAD_PKGS = new List<string> { "Common" };
        /// <summary>
        /// 常驻内存包
        /// </summary>
        public static List<string> PERMANENT_PKGS = new List<string> { "Common" };
        /// <summary>
        /// 最大包数量-不包含常驻
        /// </summary>
        public static int MAX_PKGS = 5;
        /// <summary>
        /// 弹窗遮罩透明度
        /// </summary>
        public static float POPUP_MASK_ALPHA = 0.6f;

        /// <summary>
        /// 初始化配置
        /// </summary>
        public static void Init(int? frameWidth = null, int? frameHeight = null,
            List<string> initLoadPkgs = null, List<string> permanentPkgs = null,
            int? maxPkgs = null, float? popupMaskAlpha = null)
        {
            if (frameWidth.HasValue) FRAME_WIDTH = frameWidth.Value;
            if (frameHeight.HasValue) FRAME_HEIGHT = frameHeight.Value;
            if (initLoadPkgs != null) INIT_LOAD_PKGS = initLoadPkgs;
            if (permanentPkgs != null) PERMANENT_PKGS = permanentPkgs;
            if (maxPkgs.HasValue) MAX_PKGS = maxPkgs.Value;
            if (popupMaskAlpha.HasValue) POPUP_MASK_ALPHA = popupMaskAlpha.Value;
        }
    }
}

