using GameLaunch;
using System.Collections;
using UnityEngine;

public class GameLaunchPanel : FGUIFrame.UIPanel
{
    private UI_GameLaunchPanel ui
    {
        get
        {
            return m_ui as UI_GameLaunchPanel;
        }
    }

    public override void Opened()
    {
        base.Opened();
        Debug.Log("GameLaunchPanel opened.");
        ui.m_t0.Play(GotoLoadScene);
    }

    public void GotoLoadScene()
    {
        SceneMgr.Instance.StartAsyncLoadScene("LoadScene");
    }
}