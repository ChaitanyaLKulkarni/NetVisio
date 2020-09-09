using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContentShower : MonoBehaviour {



    public GameObject disObj;
    public GameObject Compobj;

    public static ContentShower instance;
    private void Awake()
    {
        instance = this;
    }
    // Use this for initialization
    void Start ()
    {
        List<Component> comps = new List<Component>(Manager.Instance.comps.ToArray());
        comps.Sort();
        for (int i = 0; i < comps.Count; i++)
        {
            if (comps[i].parentid == -1)
            {
                GameObject go = Instantiate(disObj, transform.position, Quaternion.identity, transform);
                go.gameObject.name = comps[i].Comp_class.name;
                go.GetComponent<Image>().sprite = comps[i].img;
                go.GetComponentInChildren<TextMeshProUGUI>().text = comps[i].DisplayN;
                Selectables dad = go.GetComponent<Selectables>();
                dad.ClassName = comps[i].Comp_class.name;
                dad.ClassIndex = comps[i].ID;
            }
        }
	}
	
}