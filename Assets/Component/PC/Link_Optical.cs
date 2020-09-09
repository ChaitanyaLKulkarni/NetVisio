using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Link_Optical : Link {

    private new void Awake()
    {
        base.Awake();
        speed=15f;
        wtype = Wtype.Optical;
    }
}
