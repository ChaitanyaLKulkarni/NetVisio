using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReportManager : MonoBehaviour {

    public static ReportManager  instance;

    public GameObject packPlayInfo;
    public GameObject PackClick;
    public GameObject PackmInfo;
    public GameObject PackIds;
    public GameObject PackInfo;
    public GameObject Infopacks;
    public GameObject MInfopacks;
    public Scrollbar scroll;

    public Color color1;
    public Color color2;

    public bool isShown = false;

    public float sttime = 0;    
    public int totalpacks = 0;    

    public List<ReportP> reports = new List<ReportP>();

    public bool isStarted = false;
    private CanvasGroup cg;
    public int problems = 0;
    public struct Packid
    {
        public int packid;
        public int id;
    }

    List<Packid> packs=new List<Packid>();
    
	void Start () {
        instance = this;
        cg = GetComponentInParent<CanvasGroup>();
        isShown = false;
        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }

    private void Update()
    {
        if(isShown && Input.GetKeyDown(KeyCode.Escape))
        {
            isShown = false;
            cg.alpha = 0;
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }
    }

    public void Report()
    {
        if (!isStarted)
            return;
        isShown = true;
        cg.alpha = 1;
        cg.interactable = true;
        cg.blocksRaycasts = true;
        Show();
    }

    private void Show()
    {
        for (int i = 0; i < PackIds.transform.childCount; i++)
        {
            Destroy(PackIds.transform.GetChild(i).gameObject);
        }
        packs.Clear();
        GameObject Par = Manager.Instance.Par;
        for (int i = 0; i < Par.transform.childCount; i++)
        {
            Info info = Par.gameObject.transform.GetChild(i).GetComponent<Info>();
            if (info.ClassName == "Packet")
            {
                if (info.GetComponent<Packet>().parent != null)
                    continue;

                int packId = info.GetComponent<Packet>().PacketId;
                Packid p = new Packid
                {
                    id = i,
                    packid = packId
                };
                packs.Add(p);
                GameObject go = Instantiate(PackClick, PackIds.transform.position, Quaternion.identity, PackIds.transform);
                go.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Packet Id:" + packId;
                go.GetComponent<showPackInfo>().PackId = packId;
            }
        }
        int success = totalpacks - problems;
        List<string> nos = ComponentManger.Instance.Getnos();
        Infopacks.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Total No. of Packets = "+ totalpacks;
        Infopacks.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Total Packets sent successfully = " + success;
        Infopacks.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "No. of Packet Failures =" + problems;
        for (int i = 0; i < nos.Count; i++)
        {
            Infopacks.transform.GetChild(6+i).GetComponent<TextMeshProUGUI>().text = nos[i];
        }
        if(packs.Count >= 1)
            ShowPackInfo(0);
    }

    public void ShowPackInfo(int pid)
    {
        int id = packs[pid].id;
        Packet pa = Manager.Instance.Par.gameObject.transform.GetChild(id).GetComponent<Packet>();
        string srcip = pa.Para["Source IP"];
        string destip = pa.Para["Destination IP"];
        string proto = pa.Para["Protocol"];
        ReportP start=null;
        ReportP end=null;

        for (int i = 0; i < reports.Count; i++)
        {
            if(reports[i].packetID == pid)
            {
                if (start == null) {
                    start = reports[i];
                }
                else if (reports[i].currentDevice == start.currentDevice)
                {
                    end = reports[i];
                }
            }
        }
        float TotalTime = end.arrtime - start.deptTime;

        PackInfo.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Source IP : " + srcip;
        PackInfo.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Destination IP : " + destip;
        PackInfo.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Protocol : " + proto;
        PackInfo.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "Time Taken : " + TotalTime+ " s";

        for (int i = 0; i < MInfopacks.transform.childCount; i++)
        {
            Destroy(MInfopacks.transform.GetChild(i).gameObject);
        }
        int cl = 0;
        for (int i = 0; i < reports.Count; i++)
        {
            if (reports[i].packetID == pid)
            {
                string[] ss = new string[5] { reports[i].deptTime.ToString() + " s", (float.IsNaN(reports[i].arrtime)) ? reports[i].arrtime.ToString() : reports[i].arrtime.ToString() +" s", reports[i].currentDevice, reports[i].lastDevice, (float.IsNaN((reports[i].arrtime - reports[i].deptTime))) ? (reports[i].arrtime - reports[i].deptTime).ToString() : (reports[i].arrtime - reports[i].deptTime).ToString() + " s"};
                GameObject go = Instantiate(PackmInfo, MInfopacks.transform.position, Quaternion.identity, MInfopacks.transform);
                for (int j = 0; j < ss.Length; j++)
                {
                    go.transform.GetChild(j).GetComponent<TextMeshProUGUI>().text = ss[j];
                }
                if(cl%2 == 0)
                {
                    go.GetComponent<Image>().color = color1;
                }
                else
                {
                    go.GetComponent<Image>().color = color2;
                }
                cl++;
            }
            
        }
        
    }

    public void Export()
    {
        if (!isStarted)
            return;
        string path = "";
        string initpath = PlayerPrefs.GetString("initpath", "null");
        var sfd = new SaveFileDialog
        {
            Filter = "Comma-separated values|.csv",
            FileName = "Report.csv"

        };

        if (initpath != "null")
        {
            sfd.InitialDirectory = initpath;
        }
        else
        {
            string p = @"C:\NetVisio";
            if (Directory.Exists(p) == false)
            {
                Directory.CreateDirectory(p);
            }
            sfd.InitialDirectory = p;
        }

        sfd.ShowDialog(
            (form, result) =>
            {
                if (result == DialogResult.OK)
                {
                    path = sfd.FileName;
                    initpath=Path.GetDirectoryName(path);
                    PlayerPrefs.SetString("initpath", initpath);
                    Export(path);
                }
            }
        );
    }

    public void Export (string path)
    {
        if (path.Length > 0)
        {
            string op = ",,Report from File : " + PlayerPrefs.GetString("CurrentTitle", "Untitled") + "," + System.DateTime.Now + "\n\n" + ",,---Componets--" + "\n";

            GameObject Par = Manager.Instance.Par;
            for (int i = 0; i <Par.transform.childCount; i++)
            {
                Info info = Par.gameObject.transform.GetChild(i).GetComponent<Info>();
                if (info.ClassName == "Link")
                    continue;
                op += "\n ----------------------\n";
                Dictionary<string,string> para = info.GetPara();
                name = info.ClassName;
                Debug.Log(name);
                if (para.ContainsKey("DisplayName"))
                {
                    op += para["DisplayName"];
                }
                if(name == "Packet")
                {
                    op += "Packet ID :" + info.GetComponent<Packet>().PacketId;
                }
                op += " ( " + name + " ) \n";
                foreach (KeyValuePair<string,string> s in para)
                {
                    if (s.Key == "DisplayName")
                        continue;
                    op += s.Key + " : " + s.Value + ",";
                }
                op += "\n";
            }
            op += "\n ,,---Connections--- \n \n";
            int j = 0;
            foreach (LineRenderer lr in Manager.Instance.lrs)
            {
                op += lr.GetComponent<Link>().GetString();
                if (j > 4)
                {
                    op += "\n";
                    j = 0;
                }
                else
                {
                    op += ",";
                }
                j++;
            }
            op += "\n \n ,,---Packets Report--- \n";
            op += "Packet ID:,Depature Time, Arrival Time, At Device, Last Device, Total Time Taken :";
            op += GetReport();
            File.WriteAllText(path, op);
            Stop();
        }
    }

    private string GetReport()
    {
        string ret = "";
        foreach (ReportP rp in reports)
        {
            ret += "\n";
            ret += rp.packetID + ",";
            ret += rp.deptTime + " s,";
            ret += rp.arrtime+ " s,";
            ret += rp.currentDevice+ ",";
            ret += rp.lastDevice+ ",";
            ret += (rp.arrtime - rp.deptTime).ToString();
            ret += " s\n";

        }
        return ret;
    }

    public void StartR()
    {
        Router.isSync = true;
        Packet.CurrentPacket = 0;
        for (int i = 0; i < PackIds.transform.childCount; i++)
        {
            Destroy(PackIds.transform.GetChild(i).gameObject);
        }
        packs.Clear();
        isStarted = true;
        sttime = time();
        reports.Clear();
        
        problems = 0;
        totalpacks = 0;
        Camera.main.backgroundColor = new Color32(164,235,235,255);
        Manager.Instance.SetPlay(true);
        packPlayInfo.SetActive(true);
    }

    public void Stop()
    {
        Router.isSync = false;
        for (int i = 0; i < PackIds.transform.childCount; i++)
        {
            Destroy(PackIds.transform.GetChild(i).gameObject);
        }
        packs.Clear();
        isStarted = false;
        sttime = 0;
        reports.Clear();
        problems = 0;
        totalpacks = 0;
        Camera.main.backgroundColor = new Color32(241, 235, 235, 255);
        Manager.Instance.SetPlay(false);
        packPlayInfo.SetActive(false);
    }

    public float AddReport(int packetId,string s)
    {
        float t = time();
        reports.Add(new ReportP(packetId, t ,s));
        return t;
    }

    public void AddReport(int packetId,float deptime,string pv, string cd)
    {
        reports.Add(new ReportP(packetId, deptime,cd,pv,time()));
    }

    float time()
    {
        if(sttime == 0)
        {
            sttime = Time.time;
        }
        return (float)Math.Round((Time.time - sttime), 3);
    }

}

[System.Serializable]
public class ReportP
{
    public int packetID;
    public float deptTime, arrtime;
    public string lastDevice, currentDevice;
    public ReportP(int pid, float dt, string cd, string ld = null, float at = float.NaN)
    {
        packetID = pid;
        deptTime = dt;
        currentDevice = cd;
        lastDevice = ld;
        arrtime = at;        
    }
}

