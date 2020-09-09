using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Link_TP7 : Link {

    private new void Awake()
    {
        base.Awake();
        speed = 9f;
        wtype = Wtype.Twisted;
    }
}
