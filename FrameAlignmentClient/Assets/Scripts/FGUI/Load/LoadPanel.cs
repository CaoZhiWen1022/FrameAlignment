using Common;
using FairyGUI;
using Load;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LoadPanel : FGUIFrame.UIPanel
{
    private UI_LoadPanel _ui { get { return base.m_ui as UI_LoadPanel; } }


    public override async void Opened()
    {
        base.Opened();
        await InitGameSocket();
        StartLoad();
    }

    private async Task InitGameSocket()
    {
        //初始化游戏Socket
        await GameSocketMgr.Instance.InitSocket();
    }

    private void StartLoad()
    {
        //加载必要包
        GameUIFrame.Instance.uiFrame.GameLoadInit();
        //设置弹窗mask
        GameUIFrame.Instance.uiFrame.BindPopupMaskCreateFunc(UI_PopupMask.CreateInstance);

        UpdateBarValue();
    }

    private void UpdateBarValue()
    {
        GTween.To(0, 100, 3).OnUpdate((GTweener tweener) =>
        {
            float value = tweener.value.x;
            this._ui.m_bar.value = value;
        }).OnComplete(() =>
        {
            SceneMgr.Instance.StartAsyncLoadScene("Login");
        });
    }

}
