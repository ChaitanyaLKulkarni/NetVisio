using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShowProperties : MonoBehaviour
{

    public static ShowProperties Instance;
        
    public GameObject Property;
    public GameObject Title;
    public BaseComp Current;
    private bool exited = false;

    public Color sho;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

	// Use this for initialization
	void Start () {
        Hide();
	}

    void Update()
    {
        if(Input.GetMouseButtonDown(0)|| (Input.GetMouseButtonDown(1))){
            if (exited)
            {
                Hide();
            }
        }
    }
	
    public void ShowPorp(ref BaseComp comp)
    {
        Current = comp;
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        GetComponent<Image>().color = sho;
        GetComponent<Image>().raycastTarget= true;

        GameObject go = Instantiate(Title, gameObject.transform);

        if (comp.Para.ContainsKey("DisplayName")) 
            go.GetComponent<Text>().text = comp.Para["DisplayName"];
        else
            go.GetComponent<Text>().text = comp.GetComponent<Info>().ClassName;

        foreach (KeyValuePair<string,string> para in comp.Para)
        {
            go = Instantiate(Property, gameObject.transform);
            go.transform.GetChild(0).GetComponent<Text>().text = para.Key;
            go.transform.GetChild(1).GetComponent<InputField>().text = para.Value;
        }
        foreach(KeyValuePair<string,string> para in comp.DisablePara)
        {
            go = Instantiate(Property, gameObject.transform);
            go.transform.GetChild(0).GetComponent<Text>().text = para.Key;
            go.transform.GetChild(1).GetComponent<InputField>().text = para.Value;
            go.transform.GetChild(1).GetComponent<InputField>().enabled=false;
            
        }
    }


    
    public void Entered()
    {
        exited = false; 
    }

    public void Exited()
    {
        exited = true;
    }

    public void Hide()
    {
        GetComponent<Image>().color = new Color(0, 0, 0, 0);
        GetComponent<Image>().raycastTarget= false;

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        Current = null;
    }
}
