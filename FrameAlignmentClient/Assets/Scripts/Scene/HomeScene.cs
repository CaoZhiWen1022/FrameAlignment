using Assets.Scripts.FGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameUIFrame.Instance.uiFrame.Open(new FGUIFrame.OpenUIParam((int)UIID.HomePanel));
    }
}
