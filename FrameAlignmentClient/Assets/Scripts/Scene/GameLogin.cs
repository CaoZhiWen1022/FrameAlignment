using Assets.Scripts.FGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogin : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //œ‘ æµ«¬ΩΩÁ√Ê
        GameUIFrame.Instance.uiFrame.Open(new FGUIFrame.OpenUIParam((int)UIID.LoginPanel));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
