using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentManger : MonoBehaviour {

    public static ComponentManger Instance;

    public List<PC> pcs = new List<PC>();
    public List<Switch> switches = new List<Switch>();
    public List<Hub> hubs = new List<Hub>();
    public List<Modem> modems = new List<Modem>();
    public List<Router> routers = new List<Router>();
    public List<Packet> packets = new List<Packet>();

    public bool isChanged = false;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void SetNos()
    { 
        List<string> ret = new List<string>()
        {
            "PC x " + pcs.Count,
            "Switch x " + switches.Count,
            "Hub x " + hubs.Count,
            "Modem x " + modems.Count,
            "Router x " + routers.Count
        };
        if(!TasksManager.instance.isStarted)
            ShowCompNo.instance.Set(ret);
    }

    public List<string> Getnos()
    {
        List<string> ret = new List<string>()
        {
            "PC x " + pcs.Count,
            "Switch x " + switches.Count,
            "Hub x " + hubs.Count,
            "Modem x " + modems.Count,
            "Router x " + routers.Count
        };
        return ret;
    }

	public string AddGetName(Object obj)
    {
        string compname = obj.GetType().ToString();
        int myIndex = 0;
        switch (compname)
        {
            case "PC":
                pcs.Add((PC)obj);
                myIndex = pcs.Count - 1;
                break;
            case "Switch":
                switches.Add((Switch)obj);
                myIndex = switches.Count - 1;
                break;
            case "Hub":
                hubs.Add((Hub)obj);
                myIndex = hubs.Count - 1;
                break;
            case "Modem":
                modems.Add((Modem)obj);
                myIndex = modems.Count - 1;
                break;
            case "Router":
                routers.Add((Router)obj);
                myIndex = routers.Count - 1;
                break;
            case "Packet":
                packets.Add((Packet)obj);
                myIndex = routers.Count - 1;
                break;
        }
        string ret = compname + myIndex;

        SetNos();

        return ret;
    }


    public void ClearDeleted(Object obj)
    {
        if(pcs.Contains(obj as PC))
        {
            pcs.Remove(obj as PC);
        }
        else if (switches.Contains(obj as Switch))
        {
            switches.Remove(obj as Switch);
        }
        else if (hubs.Contains(obj as Hub))
        {
            hubs.Remove(obj as Hub);
        }
        else if (modems.Contains(obj as Modem))
        {
            modems.Remove(obj as Modem);
        }
        else if (routers.Contains(obj as Router))
        {
            routers.Remove(obj as Router);
        }
        else if (packets.Contains(obj as Packet))
        {
            packets.Remove(obj as Packet);
        }

        SetNos(); 
    }

    public void Default()
    {
        pcs.Clear();
        switches.Clear();
        hubs.Clear();
        modems.Clear();
        routers.Clear();
        SetNos();
    }

    public bool CheckForIP(string ip)
    {
        bool ret = false;
        foreach (PC pc in pcs)
        {
            if(pc.Para["IPv4 Address"] == ip)
            {
                Debug.LogWarning("Already");
                ret = true;
            }
        }
        foreach (Router router in routers)
        {
            if (router.Para["IPv4 Address"] == ip)
            {
                Debug.LogWarning("Already");
                ret = true;
            }
        }
        return ret;
    }

    public void ClearDeleted()
    {
        #region ToClearDeleted
        List<PC> tpc = new List<PC>();
        tpc = pcs;
        foreach (PC bc in pcs)
        {
            if (bc == null)
            {
                tpc.Remove(bc);
            }
        }
        pcs = tpc;


        List<Switch> ts = switches;
        foreach (Switch bc in switches)
        {
            if (bc == null)
            {
                ts.Remove(bc);
            }
        }
        switches = ts;


        List<Hub> th = hubs;
        foreach (Hub bc in hubs)
        {
            if (bc == null)
            {
                th.Remove(bc);
            }
        }
        hubs = th;


        


        List<Modem> tm = modems;
        foreach (Modem bc in modems)
        {
            if (bc == null)
            {
                tm.Remove(bc);
            }
        }
        modems = tm;

        List<Router> tr = routers;
        foreach (Router bc in routers)
        {
            if (bc == null)
            {
                tr.Remove(bc);
            }
        }
        routers = tr;
        SetNos();
        #endregion

    }
}
