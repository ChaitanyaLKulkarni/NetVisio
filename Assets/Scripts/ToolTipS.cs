using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ToolTipS : MonoBehaviour {

    public static ToolTipS instance { get; private set; }

    public CanvasGroup cg;

    public TMP_Text txtComp;
    public Vector3 off;
    private float cap;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        cg = GetComponent<CanvasGroup>();
        cap = (Screen.currentResolution.width / 100) * 62;
    }
    public void Show(string s)
    {
        cg.alpha = 1;
        Vector2 mospos = Input.mousePosition;
        if(mospos.x > cap)
        {
            GetComponent<RectTransform>().pivot = new Vector2(1, 1);
            transform.position = Input.mousePosition;
        }
        else
        {
            GetComponent<RectTransform>().pivot = new Vector2(0, 1);
            transform.position = Input.mousePosition + off;
        }
        Vector2 textSize = txtComp.GetPreferredValues(s);
        txtComp.text = s;
    }

    public void Hide()
    {
        cg.alpha = 0;
    }
}
