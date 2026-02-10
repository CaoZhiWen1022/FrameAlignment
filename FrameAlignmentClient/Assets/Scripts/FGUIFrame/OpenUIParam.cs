using System;

namespace FGUIFrame
{
    /// <summary>
    /// 打开UI参数
    /// </summary>
    public class OpenUIParam
    {
        /// <summary>
        /// UI ID
        /// </summary>
        public int UIID;
        /// <summary>
        /// 参数
        /// </summary>
        public object data;
        /// <summary>
        /// 打开回调 只执行一次
        /// </summary>
        public Action openCall;
        /// <summary>
        /// 关闭回调
        /// </summary>
        public Action closeCall;
        /// <summary>
        /// 打开失败回调
        /// </summary>
        public Action errorCall;
        /// <summary>
        /// 已打开状态下是否重新打开
        /// </summary>
        public bool reOpen;
        /// <summary>
        /// 标记为弹窗队列打开
        /// </summary>
        public bool popuoQueueOpen;

        /// <summary>
        /// 初始化 OpenUIParam 实例。
        /// 所有参数均为可选，方便直接 new OpenUIParam() 或带部分参数调用。
        /// </summary>
        public OpenUIParam(int uiid = 0, object data = null, Action openCall = null, Action closeCall = null, Action errorCall = null, bool reOpen = false, bool popuoQueueOpen = false)
        {
            this.UIID = uiid;
            this.data = data;
            this.openCall = openCall;
            this.closeCall = closeCall;
            this.errorCall = errorCall;
            this.reOpen = reOpen;
            this.popuoQueueOpen = popuoQueueOpen;
        }
    }
}

