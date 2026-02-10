using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.FGUI;
using FairyGUI;
using UnityEngine;

namespace FGUIFrame
{
    /// <summary>
    /// UI框架核心类
    /// </summary>
    public class UIFrame
    {
        public PopupQueueMgr popupQueueMgr;

        private Dictionary<UILayer, GComponent> uiLayerMap = new Dictionary<UILayer, GComponent>();
        /// <summary>
        /// 打开中的UI
        /// </summary>
        private List<int> openings = new List<int>();
        /// <summary>
        /// 打开的UI
        /// </summary>
        private List<UIBase> openUIs = new List<UIBase>();
        /// <summary>
        /// 被隐藏的ui
        /// </summary>
        private List<UIBase> concealUIs = new List<UIBase>();
        /// <summary>
        /// 全屏遮罩组件
        /// </summary>
        private UIFullMask fullMaskUI;
        /// <summary>
        /// 弹窗背景组件的创建方法
        /// </summary>
        public Func<GComponent> popupMaskCreateFunc;

        public UIFrame()
        {
            this.popupQueueMgr = new PopupQueueMgr();
        }

        /// <summary>
        /// 游戏启动时初始化
        /// </summary>
        public void GameLaunchInit()
        {
            //GRoot.inst.SetContentScaleFactor(UIFrameConfig.FRAME_WIDTH, UIFrameConfig.FRAME_HEIGHT);
            this.InitUILayer();
        }

        private void InitUILayer()
        {
            foreach (UILayer layer in Enum.GetValues(typeof(UILayer)))
            {
                var com = new GComponent();
                com.name = layer.ToString();
                GRoot.inst.AddChild(com);
                this.uiLayerMap[layer] = com;
            }
        }

        /// <summary>
        /// 加载界面时初始化
        /// </summary>
        public bool GameLoadInit()
        {
            return UIBundleMgr.LoadBundlePackage(UIFrameConfig.INIT_LOAD_PKGS);
        }

        /// <summary>
        /// 初始化全屏遮罩组件
        /// </summary>
        public void InitFullMask(int UIID, Action<bool> callback)
        {
            //获取UI注册信息
            var uiRegisterInfo = UIRegister.GetUIInfo(UIID);
            if (uiRegisterInfo == null)
            {
                Debug.LogError($"全屏遮罩初始化失败，UIID:{UIID} 未注册UI信息");
                callback?.Invoke(false);
                return;
            }
            if (uiRegisterInfo.UIType != UIType.Panel)
            {
                Debug.LogError($"全屏遮罩初始化失败，UIID:{UIID} 不是面板类型");
                callback?.Invoke(false);
                return;
            }
            if (uiRegisterInfo.UILayer != UILayer.FullScreenMask)
            {
                Debug.LogError($"全屏遮罩初始化失败，UIID:{UIID} 不是全屏遮罩层级");
                callback?.Invoke(false);
                return;
            }
            this.Open(new OpenUIParam
            {
                UIID = UIID,
                openCall = () =>
                {
                    this.fullMaskUI = this.GetUIInstance(UIID) as UIFullMask;
                    this.fullMaskUI.m_ui.visible = false;
                    this.openUIs.Remove(this.fullMaskUI);
                    callback?.Invoke(true);
                }
            });
        }

        /// <summary>
        /// 绑定弹窗遮罩创建方法
        /// </summary>
        public void BindPopupMaskCreateFunc(Func<GComponent> func)
        {
            this.popupMaskCreateFunc = func;
        }

        /// <summary>
        /// open的简单实现
        /// </summary>
        public void OpenByUiid(UIID uiid)
        {
            this.Open(new OpenUIParam((int)uiid));
        }

        /// <summary>
        /// 打开ui
        /// </summary>
        public void Open(OpenUIParam param)
        {
            var UIRegisterInfo = UIRegister.GetUIInfo(param.UIID);
            if (this.openings.Contains(param.UIID))
            {
                Debug.LogWarning($"ui {param.UIID} is opening");
                return;
            }
            if (UIRegisterInfo.UIType == UIType.Popup && !param.popuoQueueOpen)
            {
                //该弹窗不是通过弹窗队列打开的
                Debug.LogWarning($"ui {param.UIID} is not popup queue open");
                return;
            }
            if (this.openUIs.Any(ui => ui.UIID == param.UIID))
            {
                if (param.reOpen)
                {
                    this.Close(param.UIID);
                }
                else
                {
                    Debug.LogWarning($"ui {param.UIID} is open");
                    return;
                }
            }
            this.openings.Add(param.UIID);
            this.SetFullScreenMaskPanelVisible(true);
            //加载依赖
            bool isLoadSuccess = UIBundleMgr.LoadBundlePackage(UIRegisterInfo.uiPackage);
            if (!isLoadSuccess)
            {
                Debug.Log($"ui {param.UIID} load package failed");
                this.SetFullScreenMaskPanelVisible(false);
                this.openings.Remove(param.UIID);
                param.errorCall?.Invoke();
                return;
            }

            var ui = Activator.CreateInstance(UIRegisterInfo._class) as UIBase;
            bool openSuccess = ui.Open(param);
            if (openSuccess)
            {
                this.openUIs.Add(ui);
                this.openings.Remove(param.UIID);
                ui.Opened();
                var layer = this.GetUILayer(UIRegisterInfo.UILayer);
                layer.AddChild(ui.m_ui);
                refPanelShow();
            }
            else
            {
                Debug.LogError($"ui {param.UIID} open failed");
                this.openings.Remove(param.UIID);
                param.errorCall?.Invoke();
            }
            this.SetFullScreenMaskPanelVisible(false);
        }

        /// <summary>
        /// 刷新全屏界面的显示，保留一个顶层界面显示
        /// </summary>
        private void refPanelShow()
        {
            bool show = true;
            for (int i = openUIs.Count - 1; i > -1; i--)
            {
                UIBase ui = openUIs[i];
                if (ui.UIRegisterInfo.UIType == UIType.Panel && ui.UIRegisterInfo.UILayer == UILayer.panelLayer)
                {
                    if (show) { show = false; ui.Show(); }
                    else ui.Hide();
                }
            }
        }

        /// <summary>
        /// 打开ui实例,作用只是显示并添加到打开列表，并执行openSuccess
        /// </summary>
        public void OpenUIIns(UIBase ui)
        {
            if (ui == null || ui.isDisposed) return;
            this.openUIs.Add(ui);
            ui.Show();
            this.concealUIs.Remove(ui);
        }

        /// <summary>
        /// 获取ui实例
        /// </summary>
        public UIBase GetUIInstance(int UIID)
        {
            return this.openUIs.FirstOrDefault(ui => ui.UIID == UIID);
        }

        /// <summary>
        /// 关闭ui
        /// </summary>
        /// <param name="UIID"></param>
        /// <param name="dispose">是否销毁，销毁才会执行closeCall，不销毁只是隐藏且从打开列表中移除</param>
        public void Close(int UIID, bool dispose = true)
        {
            var uiins = this.openUIs.FirstOrDefault(ui => ui.UIID == UIID);
            if (uiins != null && !uiins.isDisposed)
            {
                this.openUIs.Remove(uiins);
                if (dispose)
                {
                    uiins.allowClose = true;
                    uiins.Close();
                }
                else
                {
                    uiins.Hide();
                    this.concealUIs.Add(uiins);
                }
            }
            else
            {
                Debug.LogWarning($"ui close error {UIID} not found or disposed");
            }
            refPanelShow();
        }

        /// <summary>
        /// 关闭所有
        /// </summary>
        /// <param name="exclude">不需要关闭的UI</param>
        public void CloseAll(List<int> exclude = null)
        {
            if (exclude == null) exclude = new List<int>();
            var uiArr = this.openUIs.Where(ui => !exclude.Contains(ui.UIID)).ToList();
            for (int i = 0; i < uiArr.Count; i++)
            {
                var element = uiArr[i];
                this.Close(element.UIID);
            }
        }

        /// <summary>
        /// 获取ui层级实例
        /// </summary>
        public GComponent GetUILayer(UILayer layer)
        {
            return this.uiLayerMap[layer];
        }

        /// <summary>
        /// 设置全屏遮罩
        /// </summary>
        public void SetFullScreenMaskPanelVisible(bool visible)
        {
            if (this.fullMaskUI != null) this.fullMaskUI.SetMaskVisible(visible);
        }

        /// <summary>
        /// 获取当前正在打开的界面(包含加载中)
        /// </summary>
        public List<int> GetOpenings()
        {
            return this.openings.Count > 0 ? this.openings : null;
        }

        /// <summary>
        /// 获取当前最上层打开的弹窗,(不包含加载中)
        /// </summary>
        public UIPopup GetCurTopPopup()
        {
            UIPopup curPopup = null;
            foreach (var ui in this.openUIs)
            {
                if (ui.UIRegisterInfo.UIType == UIType.Popup)
                {
                    curPopup = ui as UIPopup;
                }
            }
            return curPopup;
        }

        /// <summary>
        /// 获取当前打开的所有弹窗
        /// </summary>
        public List<UIPopup> GetCurPopupAll()
        {
            return this.openUIs.Where(ui => ui.UIRegisterInfo.UIType == UIType.Popup)
                .Cast<UIPopup>().ToList();
        }

        /// <summary>
        /// 刷新弹窗遮罩，只有最上层弹窗有遮罩
        /// </summary>
        public void RefreshPopupMask()
        {
            var popAll = this.GetCurPopupAll();
            for (int i = 0; i < popAll.Count; i++)
            {
                if (i < popAll.Count - 1 && popAll[i].popupMask != null)
                {
                    popAll[i].popupMask.visible = false;
                }
                else if (i == popAll.Count - 1 && popAll[i].popupMask != null)
                {
                    popAll[i].popupMask.visible = true;
                }
            }
        }

        /// <summary>
        /// 获取当前打开的最上层全屏界面，(不包含加载中)
        /// </summary>
        public UIPanel GetCurTopPanel()
        {
            UIPanel curPanel = null;
            foreach (var ui in this.openUIs)
            {
                if (ui.UIRegisterInfo.UIType == UIType.Panel)
                {
                    curPanel = ui as UIPanel;
                }
            }
            return curPanel;
        }

        /// <summary>
        /// 注册UI
        /// </summary>
        public void RegisterUI(UIRegisterInfo info)
        {
            UIRegister.Register(info);
        }
    }
}

