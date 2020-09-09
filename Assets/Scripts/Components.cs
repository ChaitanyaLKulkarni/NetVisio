using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Component : IComparable<Component>
{
    public int ID;
    public int SortID;
    public int parentid = -1;
    public string DisplayN; 
    public string Title; 
    public UnityEngine.Object Comp_class;
    public Sprite img;
    public Sprite oriimg;
    public string info;
    public string wikilink;

    public int CompareTo(Component other)
    {
        return SortID - other.SortID;
    }

}
