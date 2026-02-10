using Assets.Scripts.FGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoad : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //œ‘ æLoadPanel
        GameUIFrame.Instance.uiFrame.Open(new FGUIFrame.OpenUIParam((int)UIID.LoadPanel));
    }
}
