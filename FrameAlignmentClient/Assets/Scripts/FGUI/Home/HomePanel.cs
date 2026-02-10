using FairyGUI;
using Home;
using UnityEngine;


public class HomePanel : FGUIFrame.UIPanel
{
    private UI_HomePanel ui { get { return base.m_ui as UI_HomePanel; } }
    private Controller heroRootButtonController;
    private bool isAllowRotate = false;
    private float mouseX;
    private Vector3 heroRotateV3 = new Vector3(0, -180, 0);
    private GameObject heroObj;
    private float rotationSensitivity = 2f; // degrees per pixel
    //[SerializeField] private float rotationSmoothTime = 0.05f;
    //private float rotationSmoothVelocity = 0f;

    int curShowHero = -1;

    public override void Opened()
    {
        base.Opened();
        Debug.Log("HomePanel Opened");

        ShowHero();
        //监听控制器
        heroRootButtonController = ui.m_heroRoot.GetController("button");
        heroRootButtonController.onChanged.Add(HeroRootOnChanged);

        //开启一个update
        GameUIFrame.Instance.timerMgr.Loop(0.033f, Update, this);
        ui.m_heroRoot.onClick.Add(OpenHeroPanel);
        ui.m_battle.onClick.Add(OpenMatchPanel);

        //监听
        GameEventMgr.Instance.On(GameEventType._修改英雄信息成功_, ShowHero);
    }

    public void ShowHero()
    {
        Debug.Log("UseHeroId:" + HeroSocket.heroInfo.UseHeroId);
        if (curShowHero != HeroSocket.heroInfo.UseHeroId)
        {
            HeroCfg cfg = ConfigMgr.heroCfg.GetHeroById(HeroSocket.heroInfo.UseHeroId);
            if (cfg == null) return;
            Object prefab = Resources.Load("HeroModel/" + cfg.prefabName + "/" + cfg.prefabName);
            heroObj = (GameObject)Object.Instantiate(prefab);
            heroObj.transform.position = new Vector3(ui.m_heroRoot.width / 2, -ui.m_heroRoot.height, -200);
            heroObj.transform.localScale = new Vector3(200, 200, 200);
            heroObj.transform.localRotation = Quaternion.Euler(heroRotateV3);
            GoWrapper wrapper = new GoWrapper(heroObj);
            if (ui.m_heroRoot.m_root.displayObject != null) ui.m_heroRoot.m_root.displayObject.Dispose();
            ui.m_heroRoot.m_root.SetNativeObject(wrapper);
            curShowHero = HeroSocket.heroInfo.UseHeroId;
        }
    }

    public void HeroRootOnChanged(EventContext context)
    {
        int index = heroRootButtonController.selectedIndex;
        if (index == 1)
        {
            //允许旋转
            isAllowRotate = true;
            mouseX = Input.mousePosition.x;
        }
        else
        {
            //不允许旋转
            isAllowRotate = false;
        }
    }

    private void OpenHeroPanel()
    {
        GameUIFrame.Instance.uiFrame.OpenByUiid(Assets.Scripts.FGUI.UIID.HeroPanel);
    }

    private void OpenMatchPanel()
    {
        GameUIFrame.Instance.uiFrame.OpenByUiid(Assets.Scripts.FGUI.UIID.MatchPanel);
    }
    private void Update()
    {
        if (!isAllowRotate) return;
        if (heroObj == null) return;

        float curMouseX = Input.mousePosition.x;
        float delta = curMouseX - mouseX;
        // Negative delta (move left) should increase Y similar to previous behavior
        float targetY = heroRotateV3.y + (-delta) * rotationSensitivity;

        // Smoothly interpolate the angle to avoid jumps
        heroRotateV3.y = targetY;
        heroObj.transform.localRotation = Quaternion.Euler(heroRotateV3);

        mouseX = curMouseX;
    }

    public override void Closeed()
    {
        base.Closeed();
        GameUIFrame.Instance.timerMgr.Clear(Update, this);

    }
}
