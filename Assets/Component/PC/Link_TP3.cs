using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Link_TP3 : Link {

    private new void Awake()
    {
        base.Awake();
        speed = 3f;
        wtype = Wtype.Twisted;
    }
}
