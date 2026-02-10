using FairyGUI;
using System;
using System.Collections;
using UnityEngine;

public class JumpScenePanel : FGUIFrame.UIPanel
{

    public override void Opened()
    {
        base.Opened();
        Debug.Log("JumpScenePanel Opened");
    }

    public void closeAni(Action action = null)
    {
        GTween.To(1, 0, 0.5f).OnUpdate((GTweener tweener) =>
        {
            float value = tweener.value.x;
            this.m_ui.alpha = value;
        }).OnComplete(() =>
        {
            action?.Invoke();
            this.CloseThis();
        });
    }
}