using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Info : MonoBehaviour
{

    public string ClassName;
    public int ClassIndex;

    public List<Links> links = new List<Links>();

    public Dictionary<string, string> paras = new Dictionary<string, string>();

    public Dictionary<string, string> GetPara()
    {
        System.Type mType = System.Type.GetType(ClassName);
        if (ClassName.Split('_')[0]=="Link"){
            return paras;
        }
        paras = (gameObject.GetComponent(mType) as BaseComp).Para;
        return paras;
    }

    public string GetPara(string key)
    {
        System.Type mType = System.Type.GetType(ClassName);
        paras = (gameObject.GetComponent(mType) as BaseComp).Para;
        if (!paras.ContainsKey(key))
            return null;
        string value = paras[key];
        return value;
    }
    public string GetDisplayName()
    {
        System.Type mType = System.Type.GetType(ClassName);
        return (gameObject.GetComponent(mType) as BaseComp).Para["DisplayName"];
    }

    public bool AddLink(int i,int linkid,LineRenderer lr)
    {
        if(links.Count >= int.Parse(GetPara("Ports")))
        {
            return false;
        }
        links.Add(new Links(i,linkid,lr));
        return true;
    }

    public void Moved()
    {
        if (links.Count > 0)
        {
            foreach (Links l in links)
            {
                if (Manager.Instance.lrs.Contains(l.lr))
                {
                    l.lr.SetPosition(l.index, transform.position);
                }
                else
                {
                    links.Remove(l);
                }
            }
        }
    }
    public Dictionary<int ,int> GetLinkIds()
    {
        Dictionary<int,int> ret = new Dictionary<int, int>();
        for (int i = 0; i < links.Count; i++)
        {
            ret.Add(links[i].linkid, links[i].index);
        }
        return ret;
    }
    public void Deleted()
    {
        for (int i = 0; i < links.Count; i++)
        {
            if (links[i].lr.gameObject != null)
            {
                Destroy(links[i].lr.gameObject);
                Manager.Instance.lrs.Remove(links[i].lr);
            }
        }
    }

    public void SetLinks(Dictionary<int, int> linkids)
    {
        StartCoroutine(Wait(linkids));
    }
    public void SetLinks(Dictionary<int, int> linkids, bool b)
    {
        foreach(KeyValuePair<int,int> link in linkids)
        {
            links.Add(new Links(link.Value, link.Key, Manager.Instance.lrs[link.Key]));
        }
        Moved();
    }
    private IEnumerator Wait(Dictionary<int, int> linkids)
    {
        yield return new WaitForSeconds(0.5f);
        SetLinks(linkids, true);
    }
    public void Acceptpack(Packet p)
    {
        System.Type mType = System.Type.GetType(ClassName);
        if (p.scrambled || p.pType == Packet.PType.NCK)
        {
            (gameObject.GetComponent(mType) as BaseComp).SendPack(p);
        }
        else
        {
            (gameObject.GetComponent(mType) as BaseComp).Acceptpack(p);
        }
    }
    public List<IPDatabase> GetIPData(int linkfrom)
    {
        List<IPDatabase> ipds = new List<IPDatabase>();
        for (int i = 0; i < links.Count; i++)
        {
            if(links[i].linkid == linkfrom)
            {
                continue;
            }
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
                    case "PC":
                        ipds.Add(new IPDatabase(i, other.GetPara("IPv4 Address"), 0, links[i].lr.GetComponent<Link>().linkId));
                        break;

                    default:
                        ipds = other.GetIPData(links[i].linkid);
                        for (int j = 0; j < ipds.Count; j++)
                        {
                            ipds[j].linkid = i;
                        }
                        break;
                }
            }
        }
        foreach (IPDatabase ips in ipds)
        {
            ips.numLinks++;
        }
        return ipds;
    }

    public void Collide()
    {
        Debug.Log("Coollide");
    }
}

public class Links
{
    public int index;
    public LineRenderer lr;
    public int linkid;
    public Links(int i,int link ,LineRenderer l)
    {
        index = i;
        linkid = link;
        lr = l;
    }
}