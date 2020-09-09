using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TasksManager : MonoBehaviour {

    public int currentLevel = 0;
    public GameObject Textg;
    public TextMeshProUGUI tasktext;
    public GameObject test;
    public GameObject par;

    public GameObject Title;
    public GameObject Sln;

    public bool isStarted = false;

    public static TasksManager instance;

    public enum TODO
    {
        AddTwoPC,
        DeleteOnePC,
        SetPC,
        RepeatPC,
        AddLinkPCs,
        AddSetPacket,
        SimulatePackPCs
    }

    public TODO currentTodo = TODO.AddTwoPC;
    public bool currentDone;

    public List<string> todo = new List<string>();
    


    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start () {
        currentDone = true;
        todo.Clear();
        todo.Add("Add Two PCs from the Components on the left side.");
        todo.Add("Delete one PC by draging it to the upper right hand side of screen.");
        todo.Add("Right click on Any PC to set Display Name to My and its IP Address to 192.168.10.1");
        todo.Add("Add another PC and set its IP Address to 192.168.10.2");
        todo.Add("Now Connect these two PCs through link. Click on any type of link from components and then first connect the PC with the name 'My' to the other PC.");
        todo.Add("Add Packet to the canvas and set its Soruce IP same as 'My' PCs IP Address and Destination IP same as the other PCs IP Address.");
        todo.Add("Now Simulate the reated network in the simulation mode by pressing F5.");
        if(MainMenu.instance != null)
        {
            if(MainMenu.instance.after == MainMenu.After.Game)
            {
                StartT();
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown(KeyCode.F2) && !isStarted)
        {
            StartT();
        }
        if(Input.GetKeyDown(KeyCode.F3) && isStarted)
        {
            StopT();
        }
        if (!isStarted)
            return;

        if (Input.GetKeyDown(KeyCode.UpArrow) && currentDone)
        {
            NextT();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) && currentDone)
        {
            PrevT();
        }
        if (!currentDone)
        {
            CheckForTask();

        }
    }

    public void StartT()
    {
        isStarted = true;
        Title.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Game Mode";
        Sln.GetComponent<SolExp>().enabled = false;
        tasktext = Instantiate(Textg, Sln.transform, false).GetComponent<TextMeshProUGUI>();
        Manager.Instance.Load(true);
        currentLevel = 0;
        currentTodo = TODO.AddTwoPC;
        NextT();
    }

    public void StopT()
    {
        Destroy(Sln.transform.GetChild(0));
        isStarted = false;
        Title.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Solution Explorer";
        Sln.GetComponent<SolExp>().enabled = true;
        currentDone = true;
    }

    public void NextT()
    {
        if (isStarted == false)
            return;
        if (currentLevel < todo.Count)
        {
            Debug.Log("Next");
            tasktext.text = todo[currentLevel];
            currentDone = false;
        }
        else
        {
            currentDone = false;
            GenericDialog dialog = GenericDialog.Instance();
            dialog.SetTitle("Game Finished!");
            dialog.SetMessage("You Have already completed all the levels.");
            dialog.SetOk("OK", () =>
            {
                dialog.Hide();
            });
            dialog.Show();
        }
    }

    public void PrevT()
    {
        if (isStarted == false)
            return;
        if (currentLevel > 0)
        {
            Debug.Log("Prev");
            currentLevel--;
            currentTodo--;
            tasktext.text = todo[currentLevel];
            currentDone = false;
        }
        else
        {
            GenericDialog dialog = GenericDialog.Instance();
            dialog.SetTitle("Reached Start !");
            dialog.SetMessage("Your have reached the first level");
            dialog.SetOk("OK", () =>
            {
                dialog.Hide();
            });
            dialog.Show();
        }
    }

    void CheckForTask()
    {
        switch (currentTodo)
        {
            case TODO.AddTwoPC:
                
                if (ComponentManger.Instance.pcs.Count == 2)
                {
                    LevelUp();
                }
                
                break;

            case TODO.DeleteOnePC:
                if (ComponentManger.Instance.pcs.Count == 1)
                {
                    LevelUp();
                }
                break;
            case TODO.SetPC:
                if (ComponentManger.Instance.pcs[0].GetParaValue("DisplayName") == "My" && ComponentManger.Instance.pcs[0].GetParaValue("IPv4 Address") == "192.168.10.1")
                {
                    LevelUp();
                }
                break;
            case TODO.RepeatPC:
                if (ComponentManger.Instance.pcs.Count == 2)
                {
                    if (ComponentManger.Instance.pcs[1].GetParaValue("IPv4 Address") == "192.168.10.2")
                    {
                        LevelUp();
                    }
                }
                break;
            case TODO.AddLinkPCs:
                if (Manager.Instance.lrs.Count == 1 && Manager.Instance.lrs[0].GetComponent<Link>().FirstDone && Manager.Instance.lrs[0].GetComponent<Link>().SecondDone)
                {
                    if (Manager.Instance.lrs[0].GetComponent<Link>().FirstComp.GetPara()["IPv4 Address"] == "192.168.10.1" && Manager.Instance.lrs[0].GetComponent<Link>().SecondComp.GetPara()["IPv4 Address"] == "192.168.10.2")
                    {
                        LevelUp();
                    }
                }
                break;
            case TODO.AddSetPacket:
                if (ComponentManger.Instance.packets.Count == 1)
                {
                    if (ComponentManger.Instance.packets[0].GetParaValue("Source IP") == "192.168.10.1" && ComponentManger.Instance.packets[0].GetParaValue("Destination IP") == "192.168.10.2")
                    {
                        LevelUp();
                    }
                }
                break;
            case TODO.SimulatePackPCs:
                int c = ReportManager.instance.reports.Count;
                if (c < 1)
                {
                    break;
                }
                if (ReportManager.instance.reports[c - 1].arrtime != float.NaN && ReportManager.instance.reports[c - 1].lastDevice == "PC1" && ReportManager.instance.reports[c - 1].currentDevice == "My")
                {
                    LevelUp();
                }
                break;
        }
    }

    private void LevelUp()
    {
        currentLevel++;
        currentDone = true;
        currentTodo++;
        tasktext.text = "";
        Debug.Log("Level up " + currentLevel);

        GenericDialog dialog = GenericDialog.Instance();
        dialog.SetTitle("Congrats !");
        dialog.SetMessage("Click Next to get the next task or Cancel to stop");
        dialog.SetOnAccept("Next", () =>
        {
            dialog.Hide();
            NextT();
        });
        dialog.SetOnDecline("Cancel", () =>
        {
            dialog.Hide();
        });
        dialog.Show();
    }
}

