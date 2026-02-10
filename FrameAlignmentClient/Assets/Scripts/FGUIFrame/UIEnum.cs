namespace FGUIFrame
{
    /// <summary>
    /// UI层级
    /// </summary>
    public enum UILayer
    {
        /// <summary>
        /// 全屏界面层
        /// </summary>
        panelLayer,
        /// <summary>
        /// 弹窗层
        /// </summary>
        popupLayer,
        /// <summary>
        /// 引导层
        /// </summary>
        GuideLayer,
        /// <summary>
        /// 全屏遮罩层
        /// </summary>
        FullScreenMask,
        /// <summary>
        /// tips
        /// </summary>
        Tips,
        /// <summary>
        /// fly
        /// </summary>
        Fly,
        /// <summary>
        /// JumpSceneLayer
        /// </summary>
        JumpSceneLayer,
    }

    /// <summary>
    /// UI类型
    /// </summary>
    public enum UIType
    {
        /// <summary>
        /// 全屏
        /// </summary>
        Panel = 0,
        /// <summary>
        /// 弹窗
        /// </summary>
        Popup = 1,
    }

    /// <summary>
    /// 弹窗权重 同权重按照入队顺序打开，高权重进入队列时关闭低权重并打开，等待高权重关闭后再打开被关闭的界面
    /// </summary>
    public enum PopupPriority
    {
        /// <summary>
        /// 普通
        /// </summary>
        Normal = 1,
        /// <summary>
        /// 中
        /// </summary>
        Middle = 2,
        /// <summary>
        /// 高
        /// </summary>
        High = 3,
        /// <summary>
        /// 最高
        /// </summary>
        Highest = 4,
    }
}

