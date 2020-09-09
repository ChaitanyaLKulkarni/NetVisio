using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseComp : MonoBehaviour{

    /// <summary>
    /// properties of that component, key is name of the property and value is value of that property
    /// </summary>
    public Dictionary<string, string> Para=new Dictionary<string, string>();

    /// <summary>
    /// Properties which canot been changed i.e Disabled
    /// </summary>
    public Dictionary<string, string> DisablePara = new Dictionary<string, string>();

    /// <summary>
    /// Buffer for the packet
    /// </summary>
    public List<Packet> packs = new List<Packet>();

    private void Awake()
    {
        try
        {
            GetComponent<SpriteRenderer>().sortingOrder = 1;
        }
        catch (System.Exception)
        {

        }
    }

    /// <summary>
    /// To choose to Send to a Single or all device
    /// </summary>
    public enum TO {ALL,SINGLE};

    /// <summary>
    /// Adding Parameters to that component Only a property name
    /// </summary>
    /// <param name="Keys">Array of all the keys i.e. property names</param>
    public void AddParas(string[] Keys)
    {
        foreach (string Key in Keys)
        {
            if(!Para.ContainsKey(Key))
                Para.Add(Key, null);
        }
        if (Para.ContainsKey("Ports") && Para["Ports"]==null)
        {
            Para["Ports"] = "1";
        }
        if (Para.ContainsKey("MAC Address"))
        {
            Para["MAC Address"] = GetRandomMacAddress();
        }
    }
	
    /// <summary>
    /// To Set the value for the specified property
    /// </summary>
    /// <param name="key">Name of the Property</param>
    /// <param name="value">Value to be set</param>
    public void SetParaValue(string key, string value)
    {
        if (Para.ContainsKey(key))
        {
            if(Para[key] == value)
            {
                return; 
            }
            if(key == "IPv4 Address" && value != "")
            {
                string[] ips = value.Split('.');
                if(ips.Length != 4)
                {
                    Debug.LogError("Wrong");
                    GenericDialog dialog = GenericDialog.Instance();
                    dialog.SetTitle("Wrong IP entered");
                    dialog.SetMessage("Enter Valid IP address");
                    dialog.SetOk("OK", () =>
                    {
                        dialog.Hide();
                    });
                    dialog.Show();
                    return;
                }

                foreach (string s in ips)
                {
                    int i;
                    try
                    {
                        i = int.Parse(s);
                        if(i<0 || i > 255)
                        {
                            Debug.LogWarning("Out of range");
                            GenericDialog dialog = GenericDialog.Instance();
                            dialog.SetTitle("Wrong IP entered");
                            dialog.SetMessage("Enter Valid IP address, \n Out of range");
                            dialog.SetOk("OK", () =>
                            {
                                dialog.Hide();
                            });
                            dialog.Show();
                            return;
                        }
                    }
                    catch (System.Exception)
                    {
                        Debug.LogWarning("Not a number");
                        GenericDialog dialog = GenericDialog.Instance();
                        dialog.SetTitle("Wrong IP entered");
                        dialog.SetMessage("Enter Valid IP address, \n Enter only numbers");
                        dialog.SetOk("OK", () =>
                        {
                            dialog.Hide();
                        });
                        dialog.Show();
                        return;
                    }
                }
                if (ComponentManger.Instance.CheckForIP(value))
                {
                    GenericDialog dialog = GenericDialog.Instance();
                    dialog.SetTitle("Wrong IP entered");
                    dialog.SetMessage("Enter Valid IP address, \n Already in use");
                    dialog.SetOk("OK", () =>
                    {
                        dialog.Hide();
                    });
                    dialog.Show();
                    return;
                }
                if(int.Parse(ips[0]) <= 126)
                {
                    SetParaValue("Subnet Mask", "255.0.0.0");
                }else if (int.Parse(ips[0]) <=191)
                {
                    SetParaValue("Subnet Mask", "255.255.0.0");
                }
                else if (int.Parse(ips[0]) <= 223)
                {
                    SetParaValue("Subnet Mask", "255.255.255.0");
                }
                else if (int.Parse(ips[0]) <= 255)
                {
                    SetParaValue("Subnet Mask", "255.255.255.255");
                }
            }

            Para[key] = value;
        }
        else
        {
            Debug.LogWarning("Not found Key " + key);
        }
    }

    /// <summary>
    /// To Retrive any properties value with the help of its name
    /// </summary>
    /// <param name="Key">Name of the Property</param>
    /// <returns>Value of that property</returns>
    public string GetParaValue(string Key)
    {
        string ret = null;
        if (Para.ContainsKey(Key))
        {
            ret = Para[Key];
        }
        return ret;
    }

    /// <summary>
    /// To set those property which values can not be changed by user
    /// </summary>
    /// <param name="Disabled">Dictionary of Disabled properties</param>
    public void SetDisablePara(Dictionary<string, string> Disabled)
    {
        DisablePara = Disabled;

    }

    /// <summary>
    /// To set The increamental Display name to component
    /// </summary>
    /// <param name="obj">Object that want the name</param>
    public void SetDisplayname(UnityEngine.Object obj)
    {
        if (Para.ContainsKey("DisplayName") == false)
            return;
        string myname = ComponentManger.Instance.AddGetName(obj);
        if (GetParaValue("DisplayName") == null)
        {
            SetParaValue("DisplayName", myname);
        }
    }

    /// <summary>
    /// Accept the incoming packet and implement the algorithm of Desired component
    /// </summary>
    /// <param name="p">Packet that has arrived</param>
    public virtual void Acceptpack(Packet p)
    {    }

    /// <summary>
    /// Send the packet to other connected device
    /// </summary>
    /// <param name="p">Packet that need to be send</param>
    /// <param name="s">To All or To Single</param>
    /// <param name="ToSave">true if want to save to the listof sent</param>
    /// <param name="ln">Link on which to send pakcet, only used when want to send to single</param>
    public void SendPack(Packet p,TO s, bool ToSave =false,Links ln =null)
    {
        if(s == TO.ALL)
        {
            Packet ori = p;
            foreach (Links l in GetComponent<Info>().links)
            {
                if (l.lr != p.lr)
                {
                    GameObject go = Instantiate(Manager.Instance.Comp, Manager.Instance.Par.transform);
                    System.Type mType = System.Type.GetType("Packet");
                    go.AddComponent(mType);
                    go.GetComponent<Info>().ClassName = "Packet";
                    go.GetComponent<Info>().ClassIndex = Manager.Instance.GetIndex("Packet");
                    go.GetComponent<SpriteRenderer>().sprite = Manager.Instance.comps[go.GetComponent<Info>().ClassIndex].img;
                    Packet pa = go.GetComponent<Packet>();
                    pa.Para["Source IP"] = ori.Para["Source IP"];
                    pa.Para["Destination IP"] = ori.Para["Destination IP"];
                    pa.issend = true;
                    pa.parent = ori.parent;
                    pa.SetLink(l.lr, l.index, l.linkid, ori.PacketId);
                    pa.pType = p.pType;
                    if (pa.pType == Packet.PType.ACK)
                    {
                        pa.GetComponent<SpriteRenderer>().color = new Color32(0, 255, 0, 255);
                    }
                    else if (pa.pType == Packet.PType.NCK)
                    {
                        pa.GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 255);
                    }
                    else
                    {
                        pa.GetComponent<SpriteRenderer>().color = new Color32(0, 137, 208, 255);
                    }
                    if (ToSave)
                        packs.Add(pa);
                }
            }
        }
        else
        {
            if (ln != null)
            {
                Packet ori = p;
                GameObject go = Instantiate(Manager.Instance.Comp, Manager.Instance.Par.transform);
                System.Type mType = System.Type.GetType("Packet");
                go.AddComponent(mType);
                go.GetComponent<Info>().ClassName = "Packet";
                go.GetComponent<Info>().ClassIndex = Manager.Instance.GetIndex("Packet");
                go.GetComponent<SpriteRenderer>().sprite = Manager.Instance.comps[go.GetComponent<Info>().ClassIndex].img;
                Packet pa = go.GetComponent<Packet>();
                pa.Para["Source IP"] = ori.Para["Source IP"];
                pa.Para["Destination IP"] = ori.Para["Destination IP"];
                pa.issend = true;
                pa.parent = ori.parent;
                pa.pType = p.pType;
                if(pa.pType == Packet.PType.ACK)
                {
                    pa.GetComponent<SpriteRenderer>().color = new Color32(0, 255, 0, 255);
                }
                else if(pa.pType == Packet.PType.NCK)
                {
                    pa.GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 255);
                }
                else
                {
                    pa.GetComponent<SpriteRenderer>().color = new Color32(0, 137, 208, 255);
                }
                pa.SetLink(ln.lr, ln.index, ln.linkid, ori.PacketId);
            }
        }
    }

    /// <summary>
    /// Sending back the corrupted packet or requesting the corect packet
    /// </summary>
    /// <param name="p">Packet that is corrupted</param>
    public void SendPack(Packet p)
    {
        List<Links> links = GetComponent<Info>().links;
        for (int i = 0; i < links.Count; i++)
        {
            if (links[i].linkid == p.Currentlinkid)
            {
                Packet ori = p;
                GameObject go = Instantiate(Manager.Instance.Comp, Manager.Instance.Par.transform);
                System.Type mType = System.Type.GetType("Packet");
                go.AddComponent(mType);
                go.GetComponent<Info>().ClassName = "Packet";
                go.GetComponent<Info>().ClassIndex = Manager.Instance.GetIndex("Packet");
                go.GetComponent<SpriteRenderer>().sprite = Manager.Instance.comps[go.GetComponent<Info>().ClassIndex].img;
                Packet pa = go.GetComponent<Packet>();
                pa.issend = true;
                pa.parent = ori.parent;
                if (p.pType == Packet.PType.NCK)
                {
                    for (int j = 0; j <packs.Count; j++)
                    {
                        if(p.PacketId == packs[j].PacketId)
                        {
                            pa.Para["Source IP"] = packs[j].Para["Source IP"];
                            pa.Para["Destination IP"] = packs[j].Para["Destination IP"];
                            break;
                        }
                    }
                }
                else
                {
                    pa.pType = Packet.PType.NCK;
                    go.GetComponent<SpriteRenderer>().color = new Color32(235, 57, 74, 255);
                }
                pa.SetLink(links[i].lr, links[i].index, links[i].linkid, ori.PacketId);
                Destroy(p.gameObject);
                break;
            }
        }
    }

    /// <summary>
    /// used to Get Random Mac address for the components at start
    /// </summary>
    /// <returns>The Random Mac Address in XX-XX-XX-XX-XX-XX</returns>
    public static string GetRandomMacAddress()
    {
        var random = new System.Random();
        var buffer = new byte[6];
        random.NextBytes(buffer);
        var result = String.Concat(buffer.Select(x => string.Format("{0}-", x.ToString("X2"))).ToArray());
        return result.TrimEnd('-');
    }
   
    /// <summary>
    /// Used to Get the info for the ToolTip of component
    /// </summary>
    /// <returns>ToolTip in simplified string form</returns>
    public string ToolTip()
    {
        string ret = "";
        if (Para.ContainsKey("DisplayName"))
        {
            ret += "Name :  " + Para["DisplayName"] +"\n" ;
        }
        if (Para.ContainsKey("Channel"))
        {
            ret += "Channel : " + Para["Channel"] + "\n";
        }
        if (Para.ContainsKey("Protocol"))
        {
            ret += "Protocol : " + Para["Protocol"] + "\n";
        }
        if (Para.ContainsKey("IPv4 Address"))
        {
            ret += "IP : " + Para["IPv4 Address"] + "\n";
        }
        if (Para.ContainsKey("SubnetM"))
        {
            ret += "Subnet Mask : " + Para["SubnetM"] + "\n";
        }
        if (Para.ContainsKey("MACAddr"))
        {
            ret += "MAC Addr : " + Para["MACAddr"] + "\n";
        }
        if (Para.ContainsKey("Source IP"))
        {
            ret += "Source IP : " + Para["Source IP"] + "\n";
        }
        if (Para.ContainsKey("Destination IP"))
        {
            ret += "Destination IP : " + Para["Destination IP"];
        }
        
        return ret;
    }

}
