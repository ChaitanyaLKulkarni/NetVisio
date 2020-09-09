using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Link_Twisted : Link {

    private new void Awake()
    {
        base.Awake();
        speed = 6f;
        wtype = Wtype.Twisted;
    }
}