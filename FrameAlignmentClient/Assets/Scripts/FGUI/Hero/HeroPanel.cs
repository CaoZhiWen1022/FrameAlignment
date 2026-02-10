using FairyGUI;
using Hero;
using System.Collections;
using System.Collections.Generic;
using Unity.Loading;
using UnityEngine;

public class HeroPanel : FGUIFrame.UIPanel
{
    private UI_HeroPanel ui { get { return base.m_ui as UI_HeroPanel; } }

    public override void Opened()
    {
        base.Opened();
        InitList();
        ui.m_close.onClick.Add(CloseThis);
        ui.m_useBtn.onClick.Add(UseHero);
        //监听
        GameEventMgr.Instance.On(GameEventType._修改英雄信息成功_, refHero);
    }

    private void InitList()
    {
        ui.m_heroList.itemRenderer = HeroListItemRenderer;
        ui.m_heroList.numItems = ConfigMgr.heroCfg.GetAllHero().Count;

        ui.m_heroList.onClickItem.Add(HeroListClickItem);

        ui.m_heroList.selectedIndex = 0;
        refHero();
    }

    private void HeroListItemRenderer(int index, GObject item)
    {
        UI_HeroItem heroItem = item as UI_HeroItem;
        HeroCfg cfg = ConfigMgr.heroCfg.GetAllHero()[index];
        heroItem.m_name.text = cfg.name;
        //初始化Hero
        Object prefab = Resources.Load("HeroModel/" + cfg.prefabName + "/" + cfg.prefabName);
        GameObject heroObj = (GameObject)Object.Instantiate(prefab);
        heroObj.transform.position = new Vector3(heroItem.m_root.width / 2, -heroItem.m_root.height, -200);
        heroObj.transform.localScale = new Vector3(100, 100, 100);
        Vector3 heroRotateV3 = new Vector3(0, 180, 0);
        heroObj.transform.localRotation = Quaternion.Euler(heroRotateV3);
        GoWrapper wrapper = new GoWrapper(heroObj);
        heroItem.m_root.SetNativeObject(wrapper);
        //停掉动画
        Animator ani = heroObj.GetComponent<Animator>();
        if (ani) ani.enabled = false;
    }

    private void HeroListClickItem()
    {
        refHero();
    }

    private void refHero()
    {
        int curIndex = ui.m_heroList.selectedIndex;
        HeroCfg cfg = ConfigMgr.heroCfg.GetAllHero()[curIndex];
        Object prefab = Resources.Load("HeroModel/" + cfg.prefabName + "/" + cfg.prefabName);
        GameObject heroObj = (GameObject)Object.Instantiate(prefab);
        heroObj.transform.position = new Vector3(ui.m_heroRoot.width / 2, -ui.m_heroRoot.height, -200);
        heroObj.transform.localScale = new Vector3(200, 200, 200);
        Vector3 heroRotateV3 = new Vector3(0, 180, 0);
        heroObj.transform.localRotation = Quaternion.Euler(heroRotateV3);
        GoWrapper wrapper = new GoWrapper(heroObj);
        if (ui.m_heroRoot.displayObject != null) ui.m_heroRoot.displayObject.Dispose();
        ui.m_heroRoot.SetNativeObject(wrapper);

        //刷新状态
        if (cfg.id == HeroSocket.heroInfo.UseHeroId) ui.m_state.selectedIndex = 0;
        else ui.m_state.selectedIndex = 1;
    }

    private void UseHero()
    {
        if (ui.m_state.selectedIndex != 1) return;
        int index = ui.m_heroList.selectedIndex;
        HeroCfg cfg = ConfigMgr.heroCfg.GetAllHero()[index];
        int heroId = cfg.id;
        HeroSocket.SetUseHeroReq(heroId);
    }

    public override void Closeed()
    {
        base.Closeed();
        GameEventMgr.Instance.Off(GameEventType._修改英雄信息成功_, refHero);
    }
}
