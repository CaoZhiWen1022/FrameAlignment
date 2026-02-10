using FixedMathSharp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLogicBase
{
    // Âß¼­Î»ÖÃ
    public Vector3d posV3;
    // Âß¼­Ðý×ª
    public Vector3d rotate;

    public bool isDispose = false;

    public ObjectLogicBase(Vector3d posV3, Vector3d rotate)
    {
        this.posV3 = posV3;
        this.rotate = rotate;
    }

    public virtual void OnUpdate()
    {

    }
}
