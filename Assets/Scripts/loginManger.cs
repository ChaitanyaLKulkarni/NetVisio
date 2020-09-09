using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class loginManger : MonoBehaviour {

    public static loginManger instance;


    public Button Mainbtn;
    public Button Sisubtn;
    public Button gosubtn;

    public TextMeshProUGUI Title;
    public TextMeshProUGUI Errort;

    public GameObject Form;
    public GameObject FileForm;
    public GameObject Filecont;
    public GameObject Fielbtn;
    public GameObject Emailf;
    public GameObject Passwordf;
    public GameObject RePasswordf;

    private void Start()
    {
        instance = this;
        FileForm.SetActive(false);
        Form.SetActive(false);
        Sisubtn.gameObject.SetActive(false);
        gosubtn.gameObject.SetActive(false);
        Errort.gameObject.SetActive(false);
        Check();
    }

    private void Update()
    {
        if (Form.activeSelf && Input.GetKeyUp(KeyCode.Escape))
        {
            Form.SetActive(false);
            Emailf.GetComponent<TMP_InputField>().text = "";
            Passwordf.GetComponent<TMP_InputField>().text = "";
            RePasswordf.GetComponent<TMP_InputField>().text = "";
            Sisubtn.gameObject.SetActive(false);
            gosubtn.gameObject.SetActive(false);
            Errort.gameObject.SetActive(false);
        }

        if(FileForm.activeSelf && Input.GetKeyUp(KeyCode.Escape))
        {
            for (int i = 0; i < Filecont.transform.childCount; i++)
            {
                Destroy(Filecont.transform.GetChild(i).gameObject);
            }
            FileForm.SetActive(false);
        }
    }

    private void Check()
    {
        if (CookieManager.Instance.SessionCookieIsSet)
        {
            Mainbtn.GetComponentInChildren<TextMeshProUGUI>().text = "Sign out";
        }
        else
        {
            Mainbtn.GetComponentInChildren<TextMeshProUGUI>().text = "Sign in";
        }
    }

    public void ShowFiles(string[] fls)
    {
        FileForm.SetActive(true);
        for (int i = 0; i < Filecont.transform.childCount; i++)
        {
            Destroy(Filecont.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < fls.Length-1; i++)
        {
            Debug.Log("Added" + i);
            GameObject go = Instantiate(Fielbtn, Vector3.zero, Quaternion.identity, Filecont.transform);
            go.GetComponent<FileFS>().Set(fls[i].Split('/')[0], int.Parse(fls[i].Split('/')[1]));
        }
    }
    public void Mainbtncall()
    {
        if (CookieManager.Instance.SessionCookieIsSet)
        {
            Client.instance.myid = -1;
            CookieManager.Instance.ClearSessionCookie();
            Check();
        }
        else
        {
            Form.SetActive(true);
            RePasswordf.gameObject.SetActive(false);
            gosubtn.gameObject.SetActive(true);
            Sisubtn.gameObject.SetActive(true);
            Sisubtn.GetComponentInChildren<TextMeshProUGUI>().text = "Sign In";
            Title.text = "Sign in";
        }
    }

    public void GOSU()
    {
        RePasswordf.gameObject.SetActive(true);
        Title.text = "Sign Up";
        gosubtn.gameObject.SetActive(false);
        Sisubtn.GetComponentInChildren<TextMeshProUGUI>().text = "Sign Up";
    }

    public void SISU()
    {
        bool isUp=false;
        string email = Emailf.GetComponent<TMP_InputField>().text;
        string pass = Passwordf.GetComponent<TMP_InputField>().text;
        string repass = "";
        if (RePasswordf.activeSelf)
        {
            repass= RePasswordf.GetComponent<TMP_InputField>().text;
            isUp = true;
        }
        if (!isUp)
        {
            StartCoroutine(Client.instance.LogInto(email, pass));
        }
        else
        {
            StartCoroutine(Client.instance.Signup(email, pass,repass));

        }
    }



    public void OK()
    {
        FileForm.SetActive(false);
        for (int i = 0; i < Filecont.transform.childCount; i++)
        {
            Destroy(Filecont.transform.GetChild(i).gameObject);
        }
        Form.SetActive(false);
        Sisubtn.gameObject.SetActive(false);
        gosubtn.gameObject.SetActive(false);
        Check();
        Emailf.GetComponent<TMP_InputField>().text="";
        Passwordf.GetComponent<TMP_InputField>().text="";
        RePasswordf.GetComponent<TMP_InputField>().text="";

    }

    public void Error()
    {
        Errort.gameObject.SetActive(true);
        Errort.text = "Network Error!";
        Invoke("HideErr", 5f);
    }
    public void Error(string err)
    {
        Errort.gameObject.SetActive(true);
        Errort.text = err;
        Invoke("HideErr", 5f);
    }

    void HideErr()
    {
        Errort.gameObject.SetActive(false);
    }
}
