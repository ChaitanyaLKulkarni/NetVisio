using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Toturial : MonoBehaviour {

    public GameObject panel;

    public List<Steps> steps;

    private int currentId = 0;
    private bool allDone = false;

	// Use this for initialization
	void Start () {
        panel.SetActive(false);
        steps.Sort();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Next();
        }
	}

    private void Next()
    {
        if (currentId >= steps.Count)
        {
            panel.SetActive(false);
            return;
        }

        panel.SetActive(true);

        GameObject g = steps[currentId].rect.gameObject;
        //panel.transform.SetParent(g.transform.parent.transform, false);
        RectTransform rectTransform = g.GetComponent<RectTransform>();
        RectTransform objRectTransform = panel.GetComponent<RectTransform>();
        objRectTransform.anchorMin = rectTransform.anchorMin;
        objRectTransform.anchorMax = rectTransform.anchorMax;
        objRectTransform.anchoredPosition = rectTransform.anchoredPosition;
        objRectTransform.sizeDelta = rectTransform.sizeDelta;
        objRectTransform.localPosition += steps[currentId].scale;

        currentId++;
    }
}

[System.Serializable]
public class Steps : IComparable<Steps>
{
    public int id;
    public RectTransform rect;
    public string info;
    public Vector3 scale;

    public int CompareTo(Steps other)
    {
        return id - other.id;
    }
}