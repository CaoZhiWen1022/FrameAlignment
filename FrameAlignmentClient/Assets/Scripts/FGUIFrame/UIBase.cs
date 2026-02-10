using FairyGUI;
using UnityEngine;

namespace FGUIFrame
{
    /// <summary>
    /// UI基类
    /// </summary>
    public class UIBase
    {
        public bool allowClose = false;
        public UIRegisterInfo UIRegisterInfo;
        public int UIID = -1;
        public OpenUIParam openParam;
        public GComponent m_ui;
        public object data;

        public bool isDisposed => m_ui == null || m_ui.isDisposed;

        /// <summary>
        /// 打开
        /// </summary>
        public virtual bool Open(OpenUIParam openParam)
        {
            this.UIID = openParam.UIID;
            this.openParam = openParam;
            this.data = openParam.data;
            this.UIRegisterInfo = UIRegister.GetUIInfo(openParam.UIID);
            if (this.UIRegisterInfo == null)
            {
                Debug.LogError($"ui {this.UIID} not found");
                return false;
            }
            this.m_ui = this.UIRegisterInfo.createInstance();
            this.m_ui.name = this.GetType().Name;
            if (this.m_ui == null)
            {
                Debug.LogError($"ui {this.UIID} not found");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 适配
        /// </summary>
        public virtual void Resize()
        {
            this.m_ui.MakeFullScreen();
        }

        /// <summary>
        /// 打开成功，仅首次打开执行
        /// </summary>
        public virtual void Opened()
        {
            if (this.openParam.openCall != null)
            {
                this.openParam.openCall();
                this.openParam.openCall = null;
            }
            this.Resize();
            //this.Showed();
            GameUIFrame.Instance.uiFrame.popupQueueMgr.CheckQueue();
            //增加包体引用计数
            UIBundleMgr.AddRefCount(this.UIRegisterInfo.uiPackage);
        }

        /// <summary>
        /// 关闭-此时UI已销毁
        /// </summary>
        public virtual void Close()
        {
            if (!this.allowClose) return;
            if (this.isDisposed) return;
            this.m_ui.Dispose();
            this.Closeed();
        }

        /// <summary>
        /// 关闭成功
        /// </summary>
        public virtual void Closeed()
        {
            if (this.openParam.closeCall != null) this.openParam.closeCall();
            // Unity 中使用 DOTween 或 Unity Tween，这里使用简单的停止方式
            // DOTween.Kill(this.m_ui);
            GameUIFrame.Instance.uiFrame.popupQueueMgr.CheckQueue();
            //减少包体引用计数
            UIBundleMgr.RemoveRefCount(this.UIRegisterInfo.uiPackage);
        }

        /// <summary>
        /// 关闭自身
        /// </summary>
        public void CloseThis()
        {
            GameUIFrame.Instance.uiFrame.Close(this.UIID);
        }

        /// <summary>
        /// 隐藏
        /// </summary>
        public virtual void Hide()
        {
            this.m_ui.visible = false;
            this.Hideed();
        }

        /// <summary>
        /// 隐藏成功
        /// </summary>
        public virtual void Hideed()
        {
        }

        /// <summary>
        /// 显示
        /// </summary>
        public virtual void Show()
        {
            this.m_ui.visible = true;
            this.Showed();
        }

        /// <summary>
        /// 显示成功
        /// </summary>
        public virtual void Showed()
        {
        }
    }
}

