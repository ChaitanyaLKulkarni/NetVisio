using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragAndDrop : MonoBehaviour{

    private bool Selected = false;
    public GameObject go;
    public GameObject gop;
    public GameObject scroll;
    public string ClassName;
    public int ClassIndex;

    private GameObject goa;
    // Update is called once per frame

    void Start()
    {
        gop = GameObject.Find("Main Window");
        scroll = GameObject.Find("Scroll View");
    }

	void Update () {
        if (Selected)
        {
            Vector2 mousepos = Input.mousePosition;
            mousepos.x = Mathf.Clamp(mousepos.x, 200f, Camera.main.pixelWidth-20f);
            mousepos.y = Mathf.Clamp(mousepos.y, 24f, Camera.main.pixelHeight-20f);
            Vector2 pos = Camera.main.ScreenToWorldPoint(mousepos);
            goa.transform.position = new Vector2(pos.x, pos.y);

            if (Input.GetMouseButtonUp(0))
            {
                Selected = false;
                pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                System.Type mType = System.Type.GetType("Drag");
                goa.AddComponent(mType);
                goa.GetComponent<Info>().ClassName = ClassName;
                goa.GetComponent<Info>().ClassIndex = ClassIndex;
                goa.GetComponent<ClampName>().placed = true;
                goa = null;
                scroll.GetComponent<ScrollRect>().vertical = true;

            }
        }
    }

    void OnMouseOver()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            scroll.GetComponent<ScrollRect>().vertical = false;
            goa = Instantiate(go, gop.transform.position, Quaternion.identity, gop.transform);
            System.Type mType = System.Type.GetType(gameObject.name);
            goa.AddComponent(mType);
            
            if (gameObject.name.Split('_')[0] == "Link")
            {
                scroll.GetComponent<ScrollRect>().vertical = true;
                goa.GetComponent<Info>().ClassName = ClassName;
                goa.GetComponent<Info>().ClassIndex = ClassIndex;
                Selected = false;
                goa = null;
                
            }
            else
            {
                Selected = true;
                goa.GetComponent<SpriteRenderer>().sprite = GetComponent<Image>().sprite;
            }
        }
    }

}
