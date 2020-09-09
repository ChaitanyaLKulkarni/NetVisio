using UnityWinForms.Examples;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Windows.Forms;
using UnityEngine.EventSystems;
using System.Text;
using System;

public class Manager : MonoBehaviour {

    public static Manager Instance;
    public GameObject bus;
    public GameObject mesh;
    public protocols proto;
    public GameObject contents;
    public GameObject Comp;
    public GameObject DeleteObj;
    public LayerMask Dadcomp;
    public List<Component> comps;
    public GameObject SubPa;
    public GameObject Par;
    public GameObject circleObj;
    public List<SaveComp> SaveComps;
    public List<LineRenderer> lrs;
    public MoreInfo more;
    public GameObject cur;
    public Selectables Selected;
    private bool isSaved=false;
    private bool isSub=false;
    public bool isLink = false;
    private string currentPath="";



    private GameObject goa;
    private GameObject gop;
    public GameObject go;

    public Sprite ROimg;

    private Link currlink;
    private CameraController camC;

    private bool isPlay = false;


    public List<string> tutsdes = new List<string>();
    List<string> paths = new List<string> { "LAN", "MAN", "WAN", "BUS", "MESH", "RING", "STAR"};

    Dictionary<int, string> namess = new Dictionary<int, string> {
    {0,"Local Area Network (LAN)"},
    {1, "Metropolitan Area Network (MAN)"},
    {2,"Wide Area Network (WAN)"},
    {3,"Bus Topology"},
    {4,"Mesh Topology"},
    {5,"Ring Topology"},
    {6,"Star Topology"},
    {7,"Stop And Wait Protocol"},
    {8,"Go Back N Protocol"},
    {9,"Selective Repeat Protocol"},
    };

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            isSaved = false;
            PlayerPrefs.SetString("CurrentTitle", "NetVisio");
            for (int i = 0; i < comps.Count; i++)
            {
                comps[i].ID = i;
            }
        }
    }

    private void Start()
    {
        gop = GameObject.Find("Main Window");
        camC = CameraController.instance;
        if (MainMenu.instance != null)
        {
            if (MainMenu.instance.after == MainMenu.After.Open)
            {
                Load();
            }else if(MainMenu.instance.after == MainMenu.After.Tut)
            {
                contents.SetActive(false);
                Tutmanager.Instance.Show();
            }
        }
    }

    void Update()
    {
        Shortcuts();

        if (isPlay)
            return;

        if(Selected !=null && Selected.isCreatable)
        {
            Vector2 mous = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            cur.transform.position = mous;
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !isLink)
        {
            if (Selected != null)
            {
                Deselect();
            }
        }

        if (Input.GetMouseButtonUp(0) && Selected != null && Selected.isCreatable && ReportManager.instance.isShown == false && isLink == false)
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            goa = Instantiate(go, gop.transform.position, Quaternion.identity, gop.transform);
            System.Type mType = System.Type.GetType(Selected.gameObject.name);
            goa.AddComponent(mType);
            
            Vector2 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            goa.transform.position = mousepos;
            mType = System.Type.GetType("Drag");
            goa.AddComponent(mType);
            
            if (Selected.gameObject.name.Split('_')[0] == "Link")
            {
                goa.GetComponent<Info>().ClassName = Selected.ClassName;
                goa.GetComponent<Info>().ClassIndex = Selected.ClassIndex;
            }
            else
            {
                if(Selected.gameObject.name != "Disturbance")
                {
                    goa.GetComponent<ClampName>().placed = true;
                }
                goa.GetComponent<SpriteRenderer>().sprite = Selected.GetComponent<Image>().sprite;
                goa.GetComponent<Info>().ClassName = Selected.ClassName;
                goa.GetComponent<Info>().ClassIndex = Selected.ClassIndex;
                goa.GetComponent<Selectables>().ClassName = Selected.ClassName;
                goa.GetComponent<Selectables>().ClassIndex = Selected.ClassIndex;

            }
            goa = null;
        }
    }

    private void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0) && ReportManager.instance.isShown == false && isPlay == false)
        {
            CheckForClick();
        }

    }

    public void SetPlay(bool p)
    {
        if (p)
        {
            if (isLink)
            {
                currlink.Deselect(true);
            }
            Deselect();
        }
        isPlay = p;
    }

    private void CheckForClick()
    {
        RaycastHit2D rayhit;
        if (rayhit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Camera.main.ScreenToWorldPoint(Input.mousePosition),Mathf.Infinity,Dadcomp))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            if(isLink && rayhit.collider.GetComponent<Selectables>().isCreatable)
            {
                currlink.Deselect(true);
            }

            if (rayhit.collider.GetComponent<Selectables>() && !isLink)//||true)
            {
                if (Selected != null )
                {
                    Deselect();
                }

                Selected = rayhit.collider.GetComponent<Selectables>();
                Selected.Selected();
                more.ShowInfo(comps[Selected.ClassIndex]); 
                if (Selected.isCreatable)
                {
                    cur.SetActive(true);
                    cur.GetComponent<Image>().sprite = Selected.GetComponent<Image>().sprite;
                    isLink = (Selected.gameObject.name.Split('_')[0] == "Link");


                    if (isLink)
                    {
                        goa = Instantiate(go, gop.transform.position, Quaternion.identity, gop.transform);
                        System.Type mType = System.Type.GetType(Selected.gameObject.name);
                        goa.AddComponent(mType);
                        currlink = goa.GetComponent<Link>();
                        Vector2 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        goa.transform.position = Vector2.zero;// = mousepos;
                        goa.GetComponent<Info>().ClassName = Selected.ClassName;
                        goa.GetComponent<Info>().ClassIndex = Selected.ClassIndex;
                        goa = null;
                    }
                }
            }
            else
            {
                if (Selected != null && !isLink)
                {
                    if (!Selected.isDiscomp)
                    {
                        Deselect();
                    }
                }
            }
            
        }
        else
        {
            if (Selected != null && !isLink)
            {
                if (!Selected.isDiscomp)
                {
                    Deselect();
                }
            }
        }

    }

    public void LinkComplete()
    {
        Deselect();
    }

    private void Deselect()
    {
        if(Selected != null)
            Selected.Deselect();
        isLink = false;
        Selected = null;
        more.Default();
        cur.SetActive(false);
        isSub = false;
        currlink = null;
        SubPa.SetActive(isSub);
        for (int i = 0; i < SubPa.transform.childCount; i++)
        {
            Destroy(SubPa.transform.GetChild(i).gameObject);
        }
    }

    private void Shortcuts()
    {
        if (Selected != null)
        {
            if (Selected.isCreatable == false)
            {
                if (ShortInput.GetKey("Copy"))
                {
                    Copy(false);
                }
                if (ShortInput.GetKey("Paste"))
                {
                    Paste();
                }
                if (ShortInput.GetKey("Cut"))
                {
                    Copy(true);
                }
            }
        }
        
        if (ShortInput.GetKey("Save"))
        {
            Save();
        }
        if (ShortInput.GetKey("SaveAs"))
        {
            isSaved = false;
            Save();
        }
        if (ShortInput.GetKey("Open"))
        {
            Load();
        }
        if (ShortInput.GetKey("New"))
        {
            Load(true);
        }
        if (ShortInput.GetKey("Exit"))
        {
            UnityEngine.Application.Quit();
        }

        if (ShortInput.GetKey("Export"))
        {
            ReportManager.instance.Export();
        }
        if (ShortInput.GetKey("Play"))
        {
            ReportManager.instance.StartR();
        }
        if (ShortInput.GetKey("Stop"))
        {
            ReportManager.instance.Stop();
        }
        if (ShortInput.GetKey("LoadS"))
        {
            LoadFromSer();
        }
    }

    private void Paste()
    {
        for (int i = 0; i < contents.transform.childCount; i++)
        {
            if (contents.transform.GetChild(i).GetComponent<Selectables>().ClassIndex == Selected.ClassIndex)
            {
                Deselect();
                Selected = contents.transform.GetChild(i).GetComponent<Selectables>();
                break;
            }
        }
        more.ShowInfo(comps[Selected.ClassIndex]);
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        goa = Instantiate(go, gop.transform.position, Quaternion.identity, gop.transform);
        System.Type mType = System.Type.GetType(Selected.gameObject.name);
        goa.AddComponent(mType);

        Vector2 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        goa.transform.position = mousepos;
        mType = System.Type.GetType("Drag");
        goa.AddComponent(mType);
        if (Selected.gameObject.name != "Disturbance")
        {
            goa.GetComponent<SpriteRenderer>().sprite = Selected.GetComponent<Image>().sprite;
            goa.GetComponent<ClampName>().placed = true;
        }

        goa.GetComponent<Info>().ClassName = Selected.ClassName;
        goa.GetComponent<Info>().ClassIndex = Selected.ClassIndex;

        goa.GetComponent<Selectables>().ClassName = Selected.ClassName;
        goa.GetComponent<Selectables>().ClassIndex = Selected.ClassIndex;
        Deselect();
        goa.GetComponent<Selectables>().Selected();
        Selected = goa.GetComponent<Selectables>();
        goa = null;
    }

    private void Copy(bool t)
    {
        for (int i = 0; i < contents.transform.childCount; i++)
        {
            if (contents.transform.GetChild(i).GetComponent<Selectables>().ClassIndex == Selected.ClassIndex)
            {
                if (t)
                {
                    Destroy(Selected.gameObject);
                }
                Deselect();
                Selected = contents.transform.GetChild(i).GetComponent<Selectables>();
                break;
            }
        }
        more.ShowInfo(comps[Selected.ClassIndex]);
        if (Selected.isCreatable)
        {
            cur.SetActive(true);
            cur.GetComponent<Image>().sprite = Selected.GetComponent<Image>().sprite;
            isLink = (Selected.gameObject.name.Split('_')[0] == "Link");
        }

    }

    public int AddLink(LineRenderer lr)
    {
        lrs.Add(lr);
        return lrs.Count-1;
    }

    public void Save()
    {
        if (isSaved == false)
        {
            string path = "";
            var sfd = new SaveFileDialog
            {
                Filter = "NetVisio|.nvs",
                FileName = "untitled.nvs",
            };
            string initpath = PlayerPrefs.GetString("initpath", "null");
            if(initpath != "null")
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
                        initpath = Path.GetDirectoryName(path);
                        PlayerPrefs.SetString("initpath", initpath);
                        Save(path);
                    }
                });
        }
        else
        {
            Save(currentPath);
        }
    }

    private void Save(string path)
    {
        if (path.Length != 0)
        {
            int n = path.LastIndexOf(".");
            if (n < 0)
            {
                path += ".nvs";
            }
            else if (path.Substring(n, path.Length - n) != ".nvs")
            {
                path += ".nvs";
            }

            SaveComps = new List<SaveComp>();
            Dictionary<string, string> para;
            string name;
            int index;
            Vector2 pos;
            Dictionary<int, int> Linkids;
            for (int i = 0; i < Par.transform.childCount; i++)
            {
                Info info = Par.gameObject.transform.GetChild(i).GetComponent<Info>();
                para = info.GetPara();
                name = info.ClassName;
                index = info.ClassIndex;
                pos = info.transform.position;
                Linkids = info.GetLinkIds();
                SaveComps.Add(new SaveComp(name, index, pos.x, pos.y, para, Linkids));
            }
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            int nn = path.LastIndexOf('/');
            string namen = path.Substring(nn + 1, path.Length - nn - 1);
            using (FileStream fileStream = File.Open(path, FileMode.Create))
            {
                binaryFormatter.Serialize(fileStream, SaveComps);
            }
            WinTitles.ChangeTitle(namen + " - NetVisio");
            isSaved = true;
            if(currentPath != path)
            {
                currentPath = path;
                SolExp.instance.Change(currentPath);
                ComponentManger.Instance.SetNos();
            }
        }
    }

    public void SaveSer()
    {
        if (isSaved)
        {
            string path = currentPath;
            if (path.Length != 0)
            {
                int n = path.LastIndexOf(".");
                if (n < 0)
                {
                    path += ".nvs";
                }
                else if (path.Substring(n, path.Length - n) != ".nvs")
                {
                    path += ".nvs";
                }

                SaveComps = new List<SaveComp>();
                Dictionary<string, string> para;
                string name;
                int index;
                Vector2 pos;
                Dictionary<int, int> Linkids;
                for (int i = 0; i < Par.transform.childCount; i++)
                {
                    Info info = Par.gameObject.transform.GetChild(i).GetComponent<Info>();
                    para = info.GetPara();
                    name = info.ClassName;
                    index = info.ClassIndex;
                    pos = info.transform.position;
                    Linkids = info.GetLinkIds();
                    SaveComps.Add(new SaveComp(name, index, pos.x, pos.y, para, Linkids));
                }
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                int nn = path.LastIndexOf('/');
                string namen = path.Substring(nn + 1, path.Length - nn - 1);
                using (FileStream fileStream = File.Open(path, FileMode.Create))
                {
                    binaryFormatter.Serialize(fileStream, SaveComps);
                }
                using (FileStream fileStream = File.Open(path, FileMode.Open))
                {
                    StartCoroutine(Client.instance.Upload(ReadFully(fileStream), namen));
                }
            }
        }
        else
        {
            GenericDialog dialog = GenericDialog.Instance();
            dialog.SetTitle("Can't Do that!");
            dialog.SetMessage("Please first Save to Local machine");
            dialog.SetOk("OK", () =>
            {
                dialog.Hide();
            });
            dialog.Show();
        }
    }

    public static byte[] ReadFully(Stream input)
    {
        byte[] buffer = new byte[16 * 1024];
        using (MemoryStream ms = new MemoryStream())
        {
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, read);
            }
            return ms.ToArray();
        }
    }

    public void Load()
    {
        string path = "";
        var ofd = new OpenFileDialog
        {
            Filter = "NetVisio|.nvs"
        };
        string initpath = PlayerPrefs.GetString("initpath", "null");
        if (initpath != "null")
        {
            ofd.InitialDirectory = initpath;
        }
        else
        {
            string p = @"C:\NetVisio";
            if (Directory.Exists(p) == false)
            {
                Directory.CreateDirectory(p);
            }
            ofd.InitialDirectory = p;
        }
        ofd.ShowDialog(
            (form, result) =>
            {
                if (result == DialogResult.OK)
                {
                    path = ofd.FileName;
                    initpath = Path.GetDirectoryName(path);
                    PlayerPrefs.SetString("initpath", initpath);
                    Load(path);
                }
            }
        );
        
    }

    public void LoadFromSer()
    {
        StartCoroutine(Client.instance.GetFiles());
    }

    public void LoadFromBytes(byte[] data,string path)
    {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (Stream fileStream = new MemoryStream(data))
            {
                SaveComps = (List<SaveComp>)binaryFormatter.Deserialize(fileStream);
            }

            GenericDialog dialog = GenericDialog.Instance();
            dialog.SetTitle("Load Components");
            dialog.SetMessage("Loading Components will remove all existing compoents. Do you Want To load?");
            dialog.SetOnAccept("Yes", () =>
            {
                ComponentManger.Instance.Default();
                ShowObj();
                int n = path.LastIndexOf('/');
                string name = path.Substring(n + 1, path.Length - n - 1);
                WinTitles.ChangeTitle(name + " - NetVisio");
                dialog.Hide();
                isSaved = false;
                SolExp.instance.Change(path);
            });
            dialog.SetOnDecline("No", () =>
            {
                dialog.Hide();
            });
            dialog.Show();

    }

    public void Load(bool t)
    {
        if (t)
        {
            GenericDialog dialog = GenericDialog.Instance();
            dialog.SetTitle("Create new Canvas");
            dialog.SetMessage("Creating New Canvas will remove all existing components. Create New ?");
            dialog.SetOnAccept("Yes", () =>
            {
                SaveComps.Clear();
                ShowObj();
                string name = "Untitled.nvs";
                WinTitles.ChangeTitle(name + " - NetVisio");
                isSaved = false;
                dialog.Hide();
            });
            dialog.SetOnDecline("No", () =>
            {
                TasksManager.instance.StopT();
                Tutmanager.Instance.Hide();
                dialog.Hide();
            });
            dialog.Show();
        }
    }

    public void Load(string path)
    {

        if (File.Exists(path) && Tutmanager.isTut == false)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (FileStream fileStream = File.Open(path, FileMode.Open))
            {
                SaveComps = (List<SaveComp>)binaryFormatter.Deserialize(fileStream);
            }

            GenericDialog dialog = GenericDialog.Instance();
            dialog.SetTitle("Load Components");
            dialog.SetMessage("Loading Components will remove all existing compoents. Do you Want To load?");
            dialog.SetOnAccept("Yes", () =>
            {
                ComponentManger.Instance.Default();
                ShowObj();

                if(currentPath != path)
                {
                    int n = path.LastIndexOf('/');
                    string name = path.Substring(n + 1, path.Length - n - 1);
                    WinTitles.ChangeTitle(name + " - NetVisio");
                    isSaved = false;
                    SolExp.instance.Change(path);
                }
                isSaved = true;
                currentPath = path;
                dialog.Hide();
            });
            dialog.SetOnDecline("No", () =>
            {
                dialog.Hide();
            });
            dialog.Show();

        }
        else
        {
            Debug.Log("Not");
        }
    }

    public void Load(int id)
    {
        if (id == -1)
        {
            camC.Fit();
            ReportManager.instance.Stop();
            proto.Stop();
            bus.GetComponent<Animator>().SetBool("isrun", false);
            mesh.GetComponent<Animator>().SetBool("isrun", false);
            foreach (Transform child in Par.transform)
            {
                Destroy(child.gameObject);
            }
            return;
        }
        if (id == 2)
        {
            camC.Zoom(-10);
        }
        else
        {
            camC.Fit();
        }
        proto.Stop();
        bus.GetComponent<Animator>().SetBool("isrun", false);
        mesh.GetComponent<Animator>().SetBool("isrun", false);
        foreach (Transform child in Par.transform)
        {
            Destroy(child.gameObject);
        }
        switch (id)
        {
            case 3:
                Debug.Log("BUS");
                bus.GetComponent<Animator>().SetBool("isrun",true);
                break;
            case 4:
                Debug.Log("MESH");
                mesh.GetComponent<Animator>().SetBool("isrun", true);
                break;
            case 7:
                proto.Wait();
                break;
            case 8:
                proto.GoBack();
                break;
            case 9:
                proto.Selective();
                break;
            default:
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                TextAsset pathTxt = (TextAsset)Resources.Load(paths[id], typeof(TextAsset));
                using (Stream s = new MemoryStream(pathTxt.bytes))
                {
                    SaveComps = (List<SaveComp>)binaryFormatter.Deserialize(s);
                }
                ComponentManger.Instance.Default();
                ShowObj();
                break;
        }

        
        more.ShowS(namess[id], tutsdes[id]);
    }

    private void ShowObj()
    {
        int i = 0;
        Info info;
        foreach (Transform child in Par.transform)
        {
            Destroy(child.gameObject);
        }

        lrs.Clear();
        foreach (SaveComp comp in SaveComps)
        {
            GameObject goa = Instantiate(Comp, Par.transform);
            goa.transform.position = new Vector2(comp.x, comp.y);
            System.Type mType;
            mType = System.Type.GetType(comp.ClassName);
            goa.AddComponent(mType);
            info = goa.GetComponent<Info>();
            info.ClassName = comp.ClassName;
            info.ClassIndex = comp.ClassIndex;

            if (comp.ClassName.Split('_')[0] != "Link")
            {
                goa.GetComponent<SpriteRenderer>().sprite = comps[comp.ClassIndex].img;
                (goa.GetComponent(mType) as BaseComp).Para = comp.para;
                mType = System.Type.GetType("Drag");
                goa.AddComponent(mType);
                info.SetLinks(comp.LinkeIds);
                goa.GetComponent<ClampName>().placed = true;
            }
            else
            {
                Link ln = goa.GetComponent<Link>();
                ln.Loaded();
                
            }
            i++;
        }
    }
    
    public int GetIndex(string n)
    {
        int ret=0;
        for (int i = 0; i < comps.Count; i++)
        {
            if(comps[i].Comp_class.name == n)
            {
                ret = i;
            }
        }
        return ret;
    }

    public void GetBymenu(string menu)
    {
        Debug.Log(menu);
        switch(menu)
        {
            case "New":
                Load(true);
                break;
            case "Open":
                Load();
                break;
            case "Open From Server":
                LoadFromSer();
                break;
            case "Save":
                Save();
                break;
            case "SaveAs..":
                isSaved = false;
                Save();
                break;
            case "Save To Server":
                SaveSer();
                break;
            case "Exit":
                UnityEngine.Application.Quit();
                break;
            case "Play":
                ReportManager.instance.StartR();
                break;
            case "Stop":
                ReportManager.instance.Stop();
                break;
            case "Export":
                ReportManager.instance.Export();
                break;
            case "Genrate Report":
                ReportManager.instance.Report();
                break;
            case "Play Game":
                TasksManager.instance.StartT();
                break;
            case "Stop Game":
                TasksManager.instance.StopT();
                break;
            case "Next":
                TasksManager.instance.NextT();
                break;
            case "Previous":
                TasksManager.instance.NextT();
                break;
            case "Zoom In":
                camC.Zoom(1);
                break;
            case "Zoom Out":
                camC.Zoom(-1);
                break;
            case "Fit Screen":
                camC.Fit();
                break;
            case "Start Tutorial":
                contents.SetActive(false);
                Tutmanager.Instance.Show();
                break;
            case "Stop Tutorial":
                Tutmanager.Instance.Hide();
                break;
        }
    }

}
