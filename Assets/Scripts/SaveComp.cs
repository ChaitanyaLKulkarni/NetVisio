using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveComp {

    public string ClassName;
    public int ClassIndex;
    public float x, y;
    public Dictionary<string, string> para;
    public Dictionary<int,int> LinkeIds;
    
    public SaveComp(string _name, int _index, float _x,float _y, Dictionary<string, string> _para, Dictionary<int, int> _LinkeIds)
    {
        ClassName = _name;
        ClassIndex = _index;
        x = _x;
        y = _y;
        para = _para;
        LinkeIds = _LinkeIds;
    }
}
