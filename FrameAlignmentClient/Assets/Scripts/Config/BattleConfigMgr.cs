using FixedMathSharp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleConfigMgr
{
    public Vector3d blueInitV3 = new Vector3d(10, 0, 16);
    public Vector3d blueInitRotate = new Vector3d(0, 0, 0);

    public Vector3d redInitV3 = new Vector3d(-135, 0, 15);
    public Vector3d redInitRotate = new Vector3d(0, 0, 0);

    public Vector3d dummyInitV3 = new Vector3d(-63, 0, 15);
    public Vector3d dummyInitRotate = new Vector3d(0, 90, 0);


    public readonly Fixed64 FixedDeltaTime = new Fixed64(1.0 / 15.0);

    public Fixed64 reviveFrameCount = new Fixed64(15 * 10);//∏¥ªÓ ±º‰5√Î

    public string blue_guangquan = "blue_guangquan";
    public string red_guangquan = "red_guangquan";
}
