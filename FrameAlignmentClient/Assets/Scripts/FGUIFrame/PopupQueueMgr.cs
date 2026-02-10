using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FGUIFrame
{
    /// <summary>
    /// 弹窗队列管理
    /// </summary>
    public class PopupQueueMgr
    {
        private List<OpenUIParam> queue = new List<OpenUIParam>();
        //private UIBase curPopup = null;

        /// <summary>
        /// 入队
        /// </summary>
        public void Push(OpenUIParam param)
        {
            if (this.queue.Any(p => p.UIID == param.UIID))
            {
                //弹窗已经在队列
                Debug.LogWarning($"ui {param.UIID} already in queue");
            }
            else if (GameUIFrame.Instance.uiFrame.GetCurPopupAll().Any(ui => ui.UIID == param.UIID))
            {
                //弹窗已打开
                Debug.LogWarning($"ui {param.UIID} already open");
            }
            var registerInfo = UIRegister.GetUIInfo(param.UIID);
            if (registerInfo == null)
            {
                //弹窗未注册
                Debug.LogWarning($"ui {param.UIID} not register");
                return;
            }
            if (registerInfo.UIType != UIType.Popup || registerInfo.popupPriority == 0)
            {
                //弹窗注册信息不正确
                Debug.LogWarning($"ui {param.UIID} popup register error");
                return;
            }
            Debug.Log($"ui {param.UIID} push queue");
            this.queue.Add(param);
            this.CheckQueue();
        }

        /// <summary>
        /// 检查队列
        /// </summary>
        public void CheckQueue()
        {
            if (this.queue.Count == 0) return;
            if (GameUIFrame.Instance.uiFrame.GetOpenings() != null) return; //如果有界面正在打开，则return
            //队列进行排队
            this.queue = this.queue.OrderByDescending(a => UIRegister.GetUIInfo(a.UIID).popupPriority).ToList();
            OpenUIParam target = null;
            UIRegisterInfo targetInfo = null;
            for (int i = 0; i < this.queue.Count; i++) //找到一个可以打开的弹窗
            {
                var popup = this.queue[i];
                var popupInfo = UIRegister.GetUIInfo(popup.UIID);
                var curPanel = GameUIFrame.Instance.uiFrame.GetCurTopPanel();
                var curPopupPriority = GameUIFrame.Instance.uiFrame.GetCurTopPopup() != null
                    ? GameUIFrame.Instance.uiFrame.GetCurTopPopup().UIRegisterInfo.popupPriority
                    : (PopupPriority)0;
                if ((popupInfo.popupPriority > curPopupPriority
                    && (popupInfo.popupDependPanel == null || popupInfo.popupDependPanel.Count == 0 || popupInfo.popupDependPanel.Contains(curPanel.UIID)))
                    || (popupInfo.popupPriority == curPopupPriority && popupInfo.isSamePriorityMeanwhileOpen))
                {
                    target = popup;
                    targetInfo = popupInfo;
                    break;
                }
            }
            if (target != null) this.Show(target);
        }

        private void Show(OpenUIParam param)
        {
            param.popuoQueueOpen = true;
            this.queue.Remove(param);
            GameUIFrame.Instance.uiFrame.Open(param);
        }
    }
}

