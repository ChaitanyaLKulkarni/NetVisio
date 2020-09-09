using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Link : MonoBehaviour
{

    /// <summary>
    /// Type of the cable
    /// </summary>
    public enum Wtype
    {
        Twisted,
        Optical,
        Coaxial
    }
    
    /// <summary>
    /// Unique ID to Identify every Link
    /// </summary>
    public int linkId;

    /// <summary>
    /// To Check how many component has been connected 
    /// </summary>
    public bool FirstDone, SecondDone;

    /// <summary>
    /// Line Render that renders the line between the component
    /// </summary>
    private LineRenderer lr;

    /// <summary>
    /// Width of the line that has to be Rendered
    /// </summary>
    public float LineWidth;

    /// <summary>
    /// Referenced Info of the first component that has connected  
    /// </summary>
    public Info FirstComp;

    /// <summary>
    /// Referenced Info of the Second component that has connected  
    /// </summary>
    public Info SecondComp;

    /// <summary>
    /// Materials that needed to rendered for each type of wire
    /// </summary>
    private static Material mat_twisted = null, mat_optical = null, mat_coaxial = null;

    /// <summary>
    /// Texture that needed show if the connection is to be made
    /// </summary>
    private static Texture2D cursor = null;

    private bool reseted=false;

    /// <summary>
    /// Speed of this link can be overwritten to make different type of links
    /// </summary>
    public  float speed = 5f;

    /// <summary>
    /// Set a Default type of the link i.e. twisted
    /// </summary>
    public Wtype wtype = Wtype.Twisted;


    public void Awake() {
        if (GetComponent<Drag>() != null)
        {
            Destroy(GetComponent<Drag>());
        }
    }

	void Start () {
        if(cursor == null)
        {
            cursor = Resources.Load("link", typeof(Texture2D)) as Texture2D;
        }
        Destroy(GetComponent<BoxCollider2D>());
        lr = gameObject.AddComponent<LineRenderer>();
        Material mat= null ;
        float s = 0.3f;
        if (wtype == Wtype.Twisted)
        { 
            if (mat_twisted == null)
            {
                mat_twisted = Resources.Load("Twisted", typeof(Material)) as Material;
            }
            mat = mat_twisted;
        }
        else if(wtype == Wtype.Optical)
        {
            if (mat_optical == null)
            {
                mat_optical = Resources.Load("Glass", typeof(Material)) as Material;
            }
            mat = mat_optical;
            s = 0.2f;
        }
        else if (wtype == Wtype.Coaxial)
        {
            if (mat_coaxial == null)
            {
                mat_coaxial = Resources.Load("Coax", typeof(Material)) as Material;
            }
            mat = mat_coaxial;
            s = 0.2f;
        }
        lr.material = mat;
        lr.startWidth=s;
        lr.endWidth = s;
        linkId = Manager.Instance.AddLink(lr);
        Invoke("Init", 0.1f);
    }

    /// <summary>
    /// Initialization of the link 
    /// </summary>
    void Init()
    {
        
        if (!(FirstDone && SecondDone))
        {
            lr.SetPositions(new Vector3[0]);
            Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
        }
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape) && !SecondDone)
        {
            Deselect(true);
        }

        if (Input.GetMouseButtonUp(0) && !SecondDone)
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (hit)
            {
                if (hit.transform.tag == "Comp" && hit.transform.gameObject.GetComponent<Info>())
                {
                    
                    Vector2 pos = hit.transform.position;
                    Info info = hit.transform.gameObject.GetComponent<Info>();
                    if (!FirstDone)
                    {
                        if(info.AddLink(0,linkId, lr))
                        {
                            lr.SetPosition(0, pos);
                            FirstComp = info;
                            FirstDone = true;
                            reseted = false;
                        }
                        else
                        {
                            Debug.LogError("Ports Are full");
                        }
                        
                    }
                    else
                    {
                        if (info.AddLink(1, linkId, lr))
                        {
                            lr.SetPosition(1, pos);
                            SecondComp = info;
                            SecondDone = true;
                        }   
                        else
                        {
                            Debug.LogError("Ports Are full");
                        }
                        
                    }
                }
            }
        }
        if (FirstDone && !SecondDone)
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            lr.SetPosition(1, pos);
        }
        if (SecondDone) {
            if (!reseted)
            {
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                reseted = true;
                Deselect(false);
            }

        }
    }
    
    /// <summary>
    /// Fuction called if the file is opened to indicate that
    /// </summary>
    public void Loaded()
    {
        FirstDone = true;
        SecondDone = true;
        Invoke("Set", 0.2f);
    }

    /// <summary>
    /// USed to set the link to and from the components if file is opened
    /// </summary>
    void Set()
    {
        for (int i = 0; i < Manager.Instance.SaveComps.Count; i++)
        {
            if (Manager.Instance.SaveComps[i].LinkeIds.ContainsKey(linkId))
            {
                Info info = Manager.Instance.Par.transform.GetChild(i).GetComponent<Info>();
                if (Manager.Instance.SaveComps[i].LinkeIds[linkId] == 0)
                {
                    FirstComp = info;
                }
                else
                {
                    SecondComp = info;
                }
            }
        }
    }

    /// <summary>
    /// Get the Link info in the form of text
    /// </summary>
    /// <returns>String that represents the link or connection</returns>
    public string GetString()
    {
        string ret = "";
        ret += FirstComp.GetDisplayName();
        ret += " <--> " + SecondComp.GetDisplayName();
        return ret;
    }

    /// <summary>
    /// Deselect the link
    /// </summary>
    /// <param name="t">true if want to Delete the link and Deselct</param>
    public void Deselect(bool t)
    {
        Manager.Instance.LinkComplete();
        if (!t)
            return;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        Manager.Instance.lrs.Remove(lr);
        Destroy(gameObject);
    }
}
