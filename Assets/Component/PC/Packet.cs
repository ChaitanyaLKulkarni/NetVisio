using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Packet : BaseComp
{

    /// <summary>
    /// Static int for getting the unique id of packet
    /// </summary>
    public static int CurrentPacket = 0;

    /// <summary>
    /// Start position of packet
    /// </summary>
    private Vector2 startMarker;

    /// <summary>
    /// End position of packet
    /// </summary>
    private Vector2 endMarker;

    /// <summary>
    /// Speed of packet, to be get from link
    /// </summary>
    public float speed = 5.0F;

    /// <summary>
    /// Starting time of the packet
    /// </summary>
    private float startTime;

    /// <summary>
    /// how much journey has been completed
    /// </summary>
    private float journeyLength;

    /// <summary>
    /// Data in the packet
    /// </summary>
    public string Data;

    /// <summary>
    /// Is packet has been started or not
    /// </summary>
    public bool journeyStarted = false;

    /// <summary>
    /// Reference to LineRender comnponent
    /// </summary>
    public LineRenderer lr;

    /// <summary>
    /// gameobject under which all the components will be placed 
    /// </summary>
    public GameObject gop;

    /// <summary>
    /// Current link id on which packet is beign transfered
    /// </summary>
    public int Currentlinkid;

    /// <summary>
    /// does sending began
    /// </summary>
    public bool issend = false;

    /// <summary>
    /// Can it be send without goin in play mode
    /// </summary>
    public bool notAffByPlay = false;

    /// <summary>
    /// Unique packet id get from currentpacket
    /// </summary>
    public int PacketId;

    /// <summary>
    /// from which conmponet on link packet has been send
    /// </summary>
    public int Srcint;

    /// <summary>
    /// Types of Packet
    /// </summary>
    public enum PType
    {
        NOR,
        ACK,
        NCK
    }

    /// <summary>
    /// Type of Current Packet
    /// </summary>
    public PType pType = PType.NOR;

    /// <summary>
    /// The Parent Packet that created this packet
    /// </summary>
    public GameObject parent;

    /// <summary>
    /// Is Packet has currupted
    /// </summary>
    public bool scrambled = false;

    /// <summary>
    /// Depature time 
    /// </summary>
    public float depttime;

    /// <summary>
    /// Previous Device where packet has come
    /// </summary>
    public string prevdev;

    /// <summary>
    /// Is Done with transmission
    /// </summary>
    private bool isDone = false;

    /// <summary>
    /// Called when object is created
    /// </summary>
    void Start()
    {
        base.AddParas(new string[] { "Protocol", "Source IP", "Destination IP" });

        Dictionary<string, string> Disabled = new Dictionary<string, string>()
        { {"Type" , "Data"},
          {"Size" , "1000 Bytes"},
          {"TTL" , "64"}
        };
        base.SetDisablePara(Disabled);

        if (transform.childCount > 0)
        {
            Destroy(transform.GetChild(0).gameObject);
            Destroy(GetComponent<ClampName>());
        }

        GetComponent<SpriteRenderer>().sortingOrder = 0;

        if (pType == Packet.PType.ACK)
        {
            GetComponent<SpriteRenderer>().color = new Color32(0, 255, 0, 255);
        }
        else if (pType == Packet.PType.NCK)
        {
            GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 255);
        }
        else
        {
            GetComponent<SpriteRenderer>().color = new Color32(0, 137, 208, 255);
        }


        if (base.Para["Protocol"] == null)
        {
            base.Para["Protocol"] = "PING";
        }

        ComponentManger.Instance.AddGetName(this);
    }

    
    /// <summary>
    /// Called Each frame
    /// </summary>
    void Update()
    {
        if (ReportManager.instance.isStarted && !issend && !isDone)
        {
            StartP();
        }
        if (journeyStarted)
        {
            float distCovered = (Time.time - startTime) * speed;
            float fracJourney = distCovered / journeyLength;
            transform.position = Vector2.Lerp(startMarker, endMarker, fracJourney);

            if (new Vector2(transform.position.x, transform.position.y) == endMarker)
            {
                journeyStarted = false;
                SetNext();
            }
        }
        if (ReportManager.instance.isStarted == false && notAffByPlay == false)
        {
            isDone = false;
            if (parent)
            {
                parent.GetComponent<Packet>().isDone = false;
                parent.SetActive(true);
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// Start Sending of Packet
    /// </summary>
    private void StartP()
    {
        if (parent == null)
        {
            PacketId = CurrentPacket;
            CurrentPacket += 1;

            gop = GameObject.Find("Main Window");

            bool isAvailable = false;
            foreach (Transform goa in gop.transform)
            {
                if (goa.GetComponent<PC>())
                {
                    if (goa.GetComponent<PC>().Para["IPv4 Address"] == Para["Source IP"])
                    {
                        isAvailable = true;
                        break;
                    }
                }

            }
            if (isAvailable == false)
            {
                isDone = true;
                return;
            }

            parent = this.gameObject;

            GameObject go = Instantiate(Manager.Instance.Comp, Manager.Instance.Par.transform);
            System.Type mType = System.Type.GetType("Packet");
            go.AddComponent(mType);
            go.GetComponent<Info>().ClassName = "Packet";
            go.GetComponent<Info>().ClassIndex = Manager.Instance.GetIndex("Packet");
            go.GetComponent<SpriteRenderer>().sprite = Manager.Instance.comps[go.GetComponent<Info>().ClassIndex].img;
            Packet pa = go.GetComponent<Packet>();
            pa.Para["Source IP"] = this.Para["Source IP"];
            pa.Para["Destination IP"] = this.Para["Destination IP"];
            pa.parent = this.parent;
            pa.PacketId = this.PacketId;
            pa.SendPacket();
            parent.SetActive(false);
        }
    }

    /// <summary>
    /// Send The Packet
    /// </summary>
    public void SendPacket()
    {
        gop = GameObject.Find("Main Window");
        foreach (Transform go in gop.transform)
        {
            if (go.GetComponent<PC>())
            {
                if (go.GetComponent<PC>().Para["IPv4 Address"] == Para["Source IP"])
                {
                    go.GetComponent<PC>().Sendpacket(this);
                    issend = true;
                    break;
                }
            }

        }
    }


    /// <summary>
    /// used to send ack of packet
    /// </summary>
    /// <param name="my">id of packet</param>
    public void SendPacket(int my)
    {
        gop = GameObject.Find("Main Window");
        foreach (Transform go in gop.transform)
        {
            if (go.GetComponent<PC>())
            {
                if (go.GetComponent<PC>().Para["IPv4 Address"] == Para["Source IP"])
                {
                    go.GetComponent<PC>().Sendpacket(this, my);
                    issend = true;
                    break;
                }
            }
        }
    }

    /// <summary>
    /// set the link for the packet 
    /// </summary>
    /// <param name="_lr">line render of the given ink</param>
    /// <param name="src">source id of sender</param>
    /// <param name="linkid">unique linkid of link</param>
    public void SetLink(LineRenderer _lr, int src, int linkid)
    {
        startTime = Time.time;
        lr = _lr;
        speed = lr.GetComponent<Link>().speed;
        Srcint = src;
        Currentlinkid = linkid;
        startMarker = lr.GetPosition(src);
        endMarker = lr.GetPosition((src + 1) % 2);
        journeyLength = Vector2.Distance(startMarker, endMarker);
        journeyStarted = true;
        string s = (Srcint == 0) ? lr.GetComponent<Link>().FirstComp.GetPara("DisplayName") : lr.GetComponent<Link>().SecondComp.GetPara("DisplayName");
        if(notAffByPlay == false){
            depttime = ReportManager.instance.AddReport(PacketId, s);
            ReportManager.instance.totalpacks++;
        }
        prevdev = s;
    }

    /// <summary>
    /// set the link for the packet 
    /// </summary>
    /// <param name="_lr">line render of the given ink</param>
    /// <param name="src">source id of sender</param>
    /// <param name="linkid">unique linkid of link</param>
    /// <param name="pakid">parent packet id</param>
    public void SetLink(LineRenderer _lr, int src, int linkid, int pakid)
    {
        PacketId = pakid;
        SetLink(_lr, src, linkid);
    }

    public void SetLink()
    {
        startTime = Time.time;
        Srcint = (Srcint + 1) % 2;
        startMarker = lr.GetPosition(Srcint);
        endMarker = lr.GetPosition((Srcint + 1) % 2);
        journeyLength = Vector2.Distance(startMarker, endMarker);
        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered / journeyLength;
        transform.position = Vector2.Lerp(startMarker, endMarker, fracJourney);
        string s = (Srcint == 0) ? lr.GetComponent<Link>().FirstComp.GetPara("DisplayName") : lr.GetComponent<Link>().SecondComp.GetPara("DisplayName");
        if (notAffByPlay == false)
        {
            depttime = ReportManager.instance.AddReport(PacketId, s);
            ReportManager.instance.totalpacks++;
        }
        prevdev = s;
        journeyStarted = true;
    }


    /// <summary>
    /// Call accept packet after the journey completed
    /// </summary>
    public void SetNext()
    {

        Link ln = Manager.Instance.lrs[Currentlinkid].GetComponent<Link>();
        string s = (Srcint == 1) ? ln.FirstComp.GetPara("DisplayName") : ln.SecondComp.GetPara("DisplayName");
        if (notAffByPlay == false)
        {
            ReportManager.instance.AddReport(PacketId, depttime, prevdev, s);
        }
        if (Srcint == 1)
        {
            ln.FirstComp.Acceptpack(this);
        }
        else
        {
            ln.SecondComp.Acceptpack(this);
        }
    }

    /// <summary>
    /// Destroy the packet and enable parent packet after 3 sec
    /// </summary>
    /// <returns></returns>
    public IEnumerator End()
    {
        yield return new WaitForSecondsRealtime(3f);
        parent.SetActive(true);
        parent.GetComponent<Packet>().isDone = true;
        Destroy(gameObject);
    }

    private void OnEnable()
    {
        parent = null;
    }

    void OnDestroy()
    {
        ComponentManger.Instance.ClearDeleted(this);
    }

    /// <summary>
    /// Collision detection for the noise
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.tag != "Noise")
            return;
        Debug.Log("scramble");
        if (pType == PType.NOR && journeyStarted && !scrambled && !notAffByPlay)
        {
            scrambled = true;
            ReportManager.instance.problems++;
            GetComponent<SpriteRenderer>().color = new Color32(100, 100, 100, 255);
        }
    }

}
