using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Xml.Serialization;

public class Router : BaseComp {

    public List<IPDatabase> iPDatabases = new List<IPDatabase>();
    public float CurrentLoad;    public float interval = 5f;
    public List<Router> routers = new List<Router>();
    private bool isSetted = false;
    public enum Protocol { RIP,OSPF};
    public Protocol protocol;

    public static bool isSync=false;

    public void Start()
    {
        base.AddParas(new string[] { "DisplayName", "Protocol", "Ports", "IPv4 Address", "Subnet Mask", "DefaultG", "MAC Address" });
        Dictionary<string, string> Disabled = new Dictionary<string, string>
        {
            { "Data Rate", "300Mbps" },
            { "Integrated Firewall", "Yes" },
        };
        base.SetParaValue("Protocol", "RIP");
        base.SetDisablePara(Disabled);
        SetDisplayname(this);
        CurrentLoad = UnityEngine.Random.Range(0f, 0.001f);
        //Invoke("Intervals" , 0.5f);
    }
    void Update()
    {
       
        protocol = (base.GetParaValue("Protocol") == "RIP") ? Protocol.RIP : Protocol.OSPF;
        if(isSync == true && isSetted == false)
        {
            Intervals();
            isSetted = true;
        }else if( isSync == false && isSetted == true)
        {
            isSetted = false;
        }

    }
    private void FixedUpdate()
    {
        if(CurrentLoad > 0.1f)
        {
            CurrentLoad -= UnityEngine.Random.Range(0.001f, 0.01f);
        }
    }



    private void Intervals()
    {
        Info info = GetComponent<Info>();
        List<Links> links = info.links;
        for (int i = 0; i < links.Count; i++)
        {
            Info other = null;
            int src = links[i].index;
            int ot = (src + 1) % 2;
            if (ot == 0)
            {
                other = links[i].lr.GetComponent<Link>().FirstComp;
            }
            else
            {
                other = links[i].lr.GetComponent<Link>().SecondComp;
            }

            if (other != null)
            {

                switch (other.ClassName)
                {
                    case "Router":
                        if (routers.Contains(other.GetComponent<Router>()) == false)
                        {
                            routers.Add(other.GetComponent<Router>());
                        }
                        break;
                    case "PC":
                        IPDatabase ipd = new IPDatabase(i, other.GetPara("IPv4 Address"), 0, links[i].lr.GetComponent<Link>().linkId);
                        if(IPDatabase.NotPresent(iPDatabases, ipd))
                        {
                            iPDatabases.Add(ipd);
                        }
                        break;

                    default:
                        List<IPDatabase> ipds = other.GetIPData(links[i].linkid);
                        foreach (IPDatabase ip in ipds)
                        {
                            if (IPDatabase.NotPresent(iPDatabases, ip))
                            {
                                ip.linkid = i;
                                iPDatabases.Add(ip);
                            }
                        }
                        break;
                }
            }
        }
        string data = SerializeObject(iPDatabases);
        protocol = (base.GetParaValue("Protocol") == "RIP") ? Protocol.RIP : Protocol.OSPF ;
        SendRIPorOSPF(data);
        //Invoke("Intervals", interval);
    }

    private void SendRIPorOSPF(string data)
    {
        Info info = GetComponent<Info>();
        List<Links> links = info.links;
        for (int i = 0; i < links.Count; i++)
        {
            Info other = null;
            int src = links[i].index;
            int ot = (src + 1) % 2;
            if (ot == 0)
            {
                other = links[i].lr.GetComponent<Link>().FirstComp;
            }
            else
            {
                other = links[i].lr.GetComponent<Link>().SecondComp;
            }

            if (other != null)
            {
                if (other.ClassName == "Router")
                {
                    Router ro = other.GetComponent<Router>();
                    GameObject go = Instantiate(Manager.Instance.Comp, Manager.Instance.Par.transform);
                    System.Type mType = System.Type.GetType("Packet");
                    go.AddComponent(mType);
                    go.GetComponent<Info>().ClassName = "Packet";
                    go.GetComponent<Info>().ClassIndex = Manager.Instance.GetIndex("Packet");
                    go.GetComponent<SpriteRenderer>().sprite = Manager.Instance.ROimg;
                    Packet pa = go.GetComponent<Packet>();
                    pa.Para["Protocol"] = base.GetParaValue("Protocol");
                    pa.Para["Source IP"] = base.Para["IPv4 Address"];
                    pa.Para["Destination IP"] = ro.Para["IPv4 Address"];
                    pa.Data = data;
                    pa.notAffByPlay = true;
                    pa.issend = true;
                    pa.parent = pa.gameObject;
                    pa.SetLink(links[i].lr,src,links[i].linkid);
                }
            }
        }
    } 
    public string SerializeObject(List<IPDatabase> toSerialize)
    {
        XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());

        using (StringWriter textWriter = new StringWriter())
        {
            xmlSerializer.Serialize(textWriter, toSerialize);
            return textWriter.ToString();
        }
    }

    public override void Acceptpack(Packet p)
    {
        CurrentLoad += 5f;
        if(p.GetParaValue("Protocol") == "RIP" || p.GetParaValue("Protocol") == "OSPF")
        {
            var serializer = new XmlSerializer(typeof(List<IPDatabase>));
            List<IPDatabase> ipds = serializer.Deserialize(new StringReader(p.Data)) as List<IPDatabase>;
            foreach (IPDatabase ip in ipds)
            {
                if (IPDatabase.NotPresent(iPDatabases, ip))
                {
                    Info info = GetComponent<Info>();
                    List<Links> links = info.links;
                    for (int i = 0; i < links.Count; i++)
                    {
                        if (links[i].linkid == p.Currentlinkid)
                        {
                            ip.linkid = i;
                            ip.numLinks++;
                            iPDatabases.Add(ip);
                        }
                    }     
                }
            }
            Destroy(p.gameObject);
        }
        else if(p.GetParaValue("Protocol") == "PING") //For PING
        {
            DoRIPorOSPF(p);
        }
    }

    void DoRIPorOSPF(Packet p)
    {
        IPDatabase ip;
        if (protocol == Protocol.RIP)
        {
            ip = getBestIPRIP(iPDatabases, p.Para["Destination IP"]);
        }
        else
        {
            ip = getBestIPOSPF(iPDatabases, p.Para["Destination IP"]);
        }

        if (ip != null)
        {
            if (p.pType == Packet.PType.NOR)
            {
                Packet ori = p;
                packs.Add(ori);
                List<Links> ln = GetComponent<Info>().links;
                Links l = ln[ip.linkid];
                SendPack(ori, TO.SINGLE, false, l);

            }
            else
            {
                int id = p.PacketId;
                List<Packet> temp = packs;
                foreach (Packet pa in temp)
                {
                    if (pa)
                    {
                        if (pa.PacketId == id)
                        {
                            Destroy(pa.gameObject);
                        }
                    }
                    else
                    {
                        Remove();
                    }
                }
                List<Links> ln = GetComponent<Info>().links;
                Links l = ln[ip.linkid];
                SendPack(p, TO.SINGLE, false, l);
                Destroy(p.gameObject);
                Remove();
            }
        }
    }


    void Remove()
    {
        List<Packet> temp = new List<Packet>();
        foreach (Packet pa in packs)
        {
            if (pa != null)
            {
                temp.Add(pa);
            }
        }
        packs = temp;
    }
    public  IPDatabase getBestIPRIP(List<IPDatabase> ipds, string ip)
    {
        IPDatabase ret = null;
        for (int i = 0; i < ipds.Count; i++)
        {
            if (ipds[i].DestIp == ip)
            {
                if (ret == null)
                    ret = ipds[i];
                else
                {
                    if (ret.numLinks > ipds[i].numLinks)
                        ret = ipds[i];
                }
            }
        }
        return ret;
    }

    public  IPDatabase getBestIPOSPF(List<IPDatabase> ipds, string ip)
    {
        IPDatabase ret = null;
        Info info = GetComponent<Info>();
        List<Links> links = info.links;
        for (int i = 0; i < ipds.Count; i++)
        {
            if (ipds[i].DestIp == ip)
            {
                Info other = null;
                float mul = 1f;
                int src = links[ipds[i].linkid].index;
                int ot = (src + 1) % 2;
                if (ot == 0)
                {
                    other = links[ipds[i].linkid].lr.GetComponent<Link>().FirstComp;
                }
                else
                {
                    other = links[ipds[i].linkid].lr.GetComponent<Link>().SecondComp;
                }

                if (ret == null) {
                    ret = ipds[i];
                    if (other.ClassName == "Router")
                    {
                        mul = other.GetComponent<Router>().CurrentLoad;
                        ret.Load = mul;
                    }

                }
                else
                {
                    
                    if (other.ClassName == "Router")
                    {
                        mul = other.GetComponent<Router>().CurrentLoad;
                    }
                    if ((ret.numLinks * ret.Load) > (ipds[i].numLinks * mul)) {
                        ret = ipds[i];
                        ret.Load =  mul;
                    }
                }
            }
        }
        return ret;
    }
    void OnDestroy()
    {
        ComponentManger.Instance.ClearDeleted(this);
    }
}

[System.Serializable]
public class IPDatabase : IComparable<IPDatabase>
{
    public int linkid, numLinks;
    public string DestIp;
    public int Linkeid;
    public float Load;
    public IPDatabase()
    {

    }


    public IPDatabase(int _linkid, string _DestIp,int _numLinks , int _linkedid)
    {
        linkid = _linkid;
        DestIp = _DestIp;
        numLinks = _numLinks;
        Linkeid = _linkedid;
    }

    public static bool NotPresent(List<IPDatabase> al , IPDatabase b)
    {
        foreach (IPDatabase a in al)
        {

            if (a.linkid == b.linkid && a.DestIp == b.DestIp && a.numLinks == b.numLinks || a.Linkeid == b.Linkeid)
            {
                return false ;
            }
        }
       return true;
    }

    

    public int CompareTo(IPDatabase other)
    {
        return numLinks - other.numLinks;
    }
}