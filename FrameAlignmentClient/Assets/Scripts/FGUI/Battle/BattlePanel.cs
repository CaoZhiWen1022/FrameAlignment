using Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePanel : FGUIFrame.UIPanel
{
    public UI_BattlePanel ui { get { return m_ui as UI_BattlePanel; } }

    public override void Opened()
    {
        base.Opened();
    }
}
