using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

namespace FGUIFrame
{
    /// <summary>
    /// 弹窗基类
    /// </summary>
    public class UIPopup : UIBase
    {
        /// <summary>
        /// 是否显示遮罩
        /// </summary>
        public bool showMask = true;
        /// <summary>
        /// 是否点击遮罩关闭
        /// </summary>
        public bool isMaskClickCloseThis = true;
        /// <summary>
        /// 弹窗遮罩
        /// </summary>
        public GComponent popupMask;
        /// <summary>
        /// 是否开启动画，默认开启，会修改弹窗锚点
        /// </summary>
        public bool isOpenAni = true;
        /// <summary>
        /// 被此弹窗隐藏的其他弹窗
        /// </summary>
        public List<UIPopup> thisHideOtherPopup = new List<UIPopup>();

        private GTweener openAniTweener;

        public override void Resize()
        {
            //居中适配
            float x = GRoot.inst.width / 2 - this.m_ui.width / 2;
            float y = GRoot.inst.height / 2 - this.m_ui.height / 2;
            this.m_ui.SetPosition(x, y, 0f);
        }

        public override void Opened()
        {
            base.Opened();
            this.InitMask();
            this.OpenAni();
            this.HideOtherPopup();
            GameUIFrame.Instance.uiFrame.RefreshPopupMask();
        }

        /// <summary>
        /// 打开动画
        /// </summary>
        public virtual void OpenAni()
        {
            if (this.isOpenAni)
            {
                this.m_ui.SetPivot(0.5f, 0.5f);
                this.m_ui.SetScale(0.3f, 0.3f);
                this.m_ui.touchable = false;
                openAniTweener = GTween.To(0, 1.2f, 0.2f).OnUpdate((tween) =>
                {
                    float value = tween.value.x;
                    this.m_ui.SetScale(value, value);
                }).OnComplete(() =>
                {
                    openAniTweener = GTween.To(1.2f, 1, 0.1f).OnUpdate((tween) =>
                    {
                        float value = tween.value.x;
                        this.m_ui.SetScale(value, value);
                    }).OnComplete(() =>
                    {
                        this.m_ui.touchable = true;
                    });
                });

                // 简化版本，直接设置
                this.m_ui.SetScale(1f, 1f);
            }
        }

        /// <summary>
        /// 初始化遮罩
        /// </summary>
        public virtual void InitMask()
        {
            if (this.showMask && this.popupMask == null)
            {
                if (GameUIFrame.Instance.uiFrame.popupMaskCreateFunc == null)
                {
                    Debug.LogError("弹窗背景组件创建方法未绑定");
                    Debug.LogError("使用GameUIFrame.Instance.uiFrame.BindPopupMaskCreateFunc绑定弹窗背景组件创建方法");
                    return;
                }
                this.popupMask = GameUIFrame.Instance.uiFrame.popupMaskCreateFunc();
                this.popupMask.MakeFullScreen();
                this.popupMask.x = 0;
                this.popupMask.y = 0;
                this.popupMask.alpha = UIFrameConfig.POPUP_MASK_ALPHA;
                GameUIFrame.Instance.uiFrame.GetUILayer(UILayer.popupLayer).AddChild(this.popupMask);
                if (this.isMaskClickCloseThis)
                {
                    this.popupMask.onClick.Add(() =>
                    {
                        this.CloseThis();
                    });
                }
            }
            if (this.popupMask != null) this.popupMask.visible = true;
        }

        public override void Closeed()
        {
            base.Closeed();
            if (this.popupMask != null)
            {
                this.popupMask.Dispose();
                this.popupMask = null;
            }
            this.ShowThisHideOtherPopup();
            GameUIFrame.Instance.uiFrame.RefreshPopupMask();
        }

        public override void Hide()
        {
            base.Hide();
            if (this.popupMask != null) this.popupMask.visible = false;
            GameUIFrame.Instance.uiFrame.RefreshPopupMask();
        }

        /// <summary>
        /// 隐藏其他弹窗
        /// </summary>
        private void HideOtherPopup()
        {
            //关闭其他弹窗
            var allPopup = GameUIFrame.Instance.uiFrame.GetCurPopupAll();
            for (int i = 0; i < allPopup.Count; i++)
            {
                var element = allPopup[i];
                if (element != this)
                {
                    bool isclose = false;
                    if (this.UIRegisterInfo.isSamePriorityMeanwhileOpen)
                    {
                        if (element.UIRegisterInfo.popupPriority != this.UIRegisterInfo.popupPriority)
                        {
                            isclose = true;
                        }
                    }
                    else
                    {
                        isclose = true;
                    }
                    if (isclose)
                    {
                        GameUIFrame.Instance.uiFrame.Close(element.UIID, false);
                        this.thisHideOtherPopup.Add(element);
                    }
                }
            }
        }

        /// <summary>
        /// 显示被此弹窗隐藏的其他弹窗
        /// </summary>
        private void ShowThisHideOtherPopup()
        {
            for (int i = 0; i < this.thisHideOtherPopup.Count; i++)
            {
                var element = this.thisHideOtherPopup[i];
                GameUIFrame.Instance.uiFrame.OpenUIIns(element);
            }
            this.thisHideOtherPopup.Clear();
        }

        public override void Close()
        {
            base.Close();
            openAniTweener.Kill();
            openAniTweener = null;
        }
    }
}

