using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Selectables : MonoBehaviour {

    public bool isDiscomp = false;
    public bool isCreatable = false;
    public bool isSelected=false;
    private bool isChanged = false;

    public string ClassName;
    public int ClassIndex;
    bool isOver = false;
    float t;
    readonly float tb = 0.5f;
    // Update is called once per frame
    void Update () {

        if (isOver && isCreatable==false)
        {
            t -= Time.deltaTime;
            if (t < 0)
            {
                t = 0;
                ToolTipS.instance.Show(GetComponent<BaseComp>().ToolTip());
                isOver = false;
            }
        }
        if (isChanged)
        {
            if (isDiscomp)
            {
                Color c = GetComponent<Image>().color;
                Color cl = new Color(c.r, c.g, c.b, (isSelected) ? 0.5f : 1);
                GetComponent<Image>().color = cl;
            }
            else
            {
                Color c = GetComponent<SpriteRenderer>().color;
                Color cl = new Color(c.r, c.g, c.b, (isSelected) ? 0.5f : 1);
                GetComponent<SpriteRenderer>().color = cl;
            }
            isChanged = false;
        }
        if (!isCreatable)
        {
            if (Input.GetKeyUp(KeyCode.Delete))
            {
                GetComponent<Drag>().Delete();
            }
        }
	}

    public void Selected()
    {
        isSelected = true;
        isChanged = true;
    }

    public void Deselect()
    {
        isSelected = false;
        isChanged = true;
    }

    public void Set(int index, string name)
    {
        ClassIndex = index;
        ClassName = name;
    }

    private void OnMouseEnter()
    {
        t = tb;
        isOver = true;
    }

    private void OnMouseExit()
    {
        isOver = false;
        ToolTipS.instance.Hide();

    }
}
