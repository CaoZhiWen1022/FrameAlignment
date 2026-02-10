using Assets.Scripts.FGUI;
using FairyGUI;
using FGUIFrame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLunch : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("游戏启动....");
        this.InitGame();
    }

    void InitGame()
    {
        Debug.Log("开始初始化游戏");
        this.InitFgui();
    }

    void InitFgui()
    {
        GameUIFrame.Instance.Init();
        GameUIFrame.Instance.InitUIFrameConfig(1920, 1080, new List<string>() { "Common", "Home", "Login" }, new List<string>(), 10, 0.5f);
        Assets.Scripts.FGUI.UIRegister.RegisterAll();

        GameUIFrame.Instance.uiFrame.Open(new FGUIFrame.OpenUIParam((int)UIID.GameLaunchPanel));
    }
}
