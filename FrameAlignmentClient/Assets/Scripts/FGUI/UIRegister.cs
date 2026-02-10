using Battle;
using FairyGUI;
using FGUIFrame;
using GameLaunch;
using Hero;
using Home;
using JumpScene;
using Login;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Assets.Scripts.FGUI
{
    public class UIRegister
    {
        public static void RegisterAll()
        {
            BindAll();

            //全屏界面
            RegisterInfo(UIID.LoadPanel, Load.UI_LoadPanel.CreateInstance, typeof(LoadPanel), UIType.Panel, UILayer.panelLayer, new List<string> { "Load" });
            RegisterInfo(UIID.GameLaunchPanel, UI_GameLaunchPanel.CreateInstance, typeof(GameLaunchPanel), UIType.Panel, UILayer.panelLayer, new List<string> { "GameLaunch" });
            RegisterInfo(UIID.JumpScenePanel, UI_JumpScenePanel.CreateInstance, typeof(JumpScenePanel), UIType.Panel, UILayer.JumpSceneLayer, new List<string> { "JumpScene" });
            RegisterInfo(UIID.LoginPanel, UI_LoginPanel.CreateInstance, typeof(LoginPanel), UIType.Panel, UILayer.panelLayer, new List<string> { "Login" });
            RegisterInfo(UIID.HomePanel, UI_HomePanel.CreateInstance, typeof(HomePanel), UIType.Panel, UILayer.panelLayer, new List<string> { "Home" });
            RegisterInfo(UIID.MatchPanel, UI_MatchPanel.CreateInstance, typeof(MatchPanel), UIType.Panel, UILayer.panelLayer, new List<string> { "Home" });
            RegisterInfo(UIID.HeroPanel, UI_HeroPanel.CreateInstance, typeof(HeroPanel), UIType.Panel, UILayer.panelLayer, new List<string> { "Hero" });
            RegisterInfo(UIID.BattleInitPanel, UI_BattleInitPanel.CreateInstance, typeof(BattleInitPanel), UIType.Panel, UILayer.panelLayer, new List<string> { "Battle" });
            RegisterInfo(UIID.BattlePanel, UI_BattlePanel.CreateInstance, typeof(BattlePanel), UIType.Panel, UILayer.panelLayer, new List<string> { "Battle" });



            //弹窗界面
            RegisterInfo(UIID.RegisterPopup, UI_RegisterPopup.CreateInstance, typeof(RegisterPopup), UIType.Popup, UILayer.popupLayer, new List<string> { "Login" }, true, PopupPriority.Normal);
        }

        private static void RegisterInfo(UIID uiid, Func<GComponent> createInstance, Type _class, UIType uIType, UILayer uILayer, List<string> uiPackage, bool isSamePriorityMeanwhileOpen = true, PopupPriority popupPriority = PopupPriority.Normal, List<int> popupDependPanel = null, int popupTimeout = 30)
        {
            UIRegisterInfo uiRegisterInfo = new UIRegisterInfo();
            uiRegisterInfo.UIID = (int)uiid;
            uiRegisterInfo.createInstance = createInstance;
            uiRegisterInfo._class = _class;
            uiRegisterInfo.UIType = uIType;
            uiRegisterInfo.UILayer = uILayer;
            uiRegisterInfo.uiPackage = uiPackage;
            uiRegisterInfo.popupPriority = popupPriority;
            uiRegisterInfo.isSamePriorityMeanwhileOpen = isSamePriorityMeanwhileOpen;
            uiRegisterInfo.popupDependPanel = popupDependPanel;
            uiRegisterInfo.popupTimeout = popupTimeout;
            GameUIFrame.Instance.uiFrame.RegisterUI(uiRegisterInfo);
        }

        private static void BindAll()
        {
            Load.LoadBinder.BindAll();
            Login.LoginBinder.BindAll();
            Home.HomeBinder.BindAll();
            Common.CommonBinder.BindAll();
            GameLaunchBinder.BindAll();
            JumpScene.JumpSceneBinder.BindAll();
            Hero.HeroBinder.BindAll();
            Battle.BattleBinder.BindAll();
        }
    }
}