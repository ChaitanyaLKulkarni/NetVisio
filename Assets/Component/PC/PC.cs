using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PC : BaseComp {

    public void Start()
    {
        base.AddParas(new string[] { "DisplayName","Ports", "IPv4 Address", "Subnet Mask", "DefaultG", "MAC Address" });
        Dictionary<string, string> Disabled = new Dictionary<string, string>
        {
            { "Processor", "i5-6500k" },
            { "RAM", "8GB" },
            { "Hard Disk", "500TB" }
        };

        base.SetDisablePara(Disabled);
        SetDisplayname(this);

    }

    /// <summary>
    /// Sending next packet in list
    /// </summary>
    /// <param name="go">packet need to be send</param>
    public void Sendpacket(Packet go)
    {   
        packs.Add(go);
        StartCoroutine(SendNext(packs.Count / (10 + packs.Count), go));
    }

    /// <summary>
    /// Send next packet after specifed amount of time
    /// </summary>
    /// <param name="wait">Time to wait before send</param>
    /// <param name="go">packet to be send</param>
    /// <returns></returns>
    IEnumerator SendNext(float wait,Packet go)
    {
        yield return new WaitForSecondsRealtime(wait);
        Info info = GetComponent<Info>();
        go.SetLink(info.links[0].lr, info.links[0].index, info.links[0].linkid);
    }

    /// <summary>
    /// Send the packet from pc
    /// </summary>
    /// <param name="go">packet</param>
    /// <param name="my">linkid</param>
    public void Sendpacket(Packet go,int my)
    {
        Info info = GetComponent<Info>();
        packs.Add(go);
        int j=0;
        List<Links> links = info.links;
        for (int i = 0; i < links.Count; i++)
        {
            if(links[i].linkid == my)
            {
                j = i;
                break;
            }
        }
        go.SetLink(links[j].lr,links[j].index, links[j].linkid);
    }

    bool CheckForDupli(Packet p)
    {
        if (packs.Contains(p))
        {
            return true;
        }
        return false;
    }

    public override void Acceptpack(Packet p)
    {
        
        if (p.Para["Destination IP"] == base.Para["IPv4 Address"])
        {
            
            if (p.pType == Packet.PType.ACK)
            {
                packs.Remove(p);
                p.issend = false;
                p.journeyStarted = false;
                p.transform.position = transform.position + new Vector3(1, 1, 0);
                StartCoroutine(p.End());
            }
            else if(p.pType == Packet.PType.NOR)
            {
                p.GetComponent<SpriteRenderer>().color = new Color32(0, 255, 0, 150);
                StartCoroutine(SendAck(p));
            }
        }
        else
        {
            List<Links> links = GetComponent<Info>().links;
            if(links.Count > 1)
            {
                for (int i = 0; i < links.Count; i++)
                {
                    if (links[i].linkid != p.Currentlinkid)
                    {
                        SendPack(p, TO.SINGLE, false, links[i]);
                    }
                }
            }
            
            p.GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 150);
            Destroy(p.gameObject,1f);
        }
        
    }

    /// <summary>
    /// Send back the Ack of correctly recieved packet
    /// </summary>
    /// <param name="p">packet</param>
    /// <returns></returns>
    IEnumerator SendAck(Packet p)
    {
        yield return new WaitForSeconds(1f);
        p.GetComponent<SpriteRenderer>().color = new Color32(0, 255, 0, 255);
        string destip = p.Para["Destination IP"];
        p.Para["Destination IP"] = p.Para["Source IP"];
        p.Para["Source IP"] = destip;
        p.pType = Packet.PType.ACK;
        p.SendPacket(p.Currentlinkid);
    }

    void OnDestroy()
    {
        ComponentManger.Instance.ClearDeleted(this);
    }

}
