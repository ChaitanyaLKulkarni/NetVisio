using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Client : MonoBehaviour {

    public static Client instance;

    public string host = "http://netvisio.dx.am";
    public int myid = -1;

    public GameObject Loadingimg;
    private void Start()
    {
        instance = this;
    }
    public  string data = "";

    public  IEnumerator  Upload(byte[] datas,string f)
    {
        if (myid == -1)
        {
            GenericDialog dialog = GenericDialog.Instance();
            dialog.SetTitle("Not Logged In");
            dialog.SetMessage("Please Login to use this feature");
            dialog.SetOk("OK", () =>
            {
                dialog.Hide();
            });
            dialog.Show();
            yield break;
        }

        string fileName = f;
        fileName = fileName.ToUpper();
        WWWForm form = new WWWForm();

        form.AddField("id", myid);

        form.AddField("file", "file");
        form.AddBinaryData("file", datas, fileName, "application/octet-stream");
        WWW w = new WWW(host+"/upload.php", form);
        Loadingimg.SetActive(true);
        yield return w;
        Loadingimg.SetActive(false);
        if (w.error != null)
        {
            print("error");
            print(w.error);
        }
        else
        {
            if (w.uploadProgress == 1 && w.isDone)
            {
                if(w.text == "-1")
                {
                    GenericDialog dialog = GenericDialog.Instance();
                    dialog.SetTitle("File Already exits !");
                    dialog.SetMessage("To overwrite that press Yes or cancle");
                    dialog.SetOnAccept("Yes", () =>
                    {
                        StartCoroutine(UploadOver(datas, f));
                        dialog.Hide();
                    });
                    dialog.SetOnDecline("No", () =>
                    {
                        dialog.Hide();
                    });
                    dialog.Show();
                }
                if (w.text == "0")
                {
                    GenericDialog dialog = GenericDialog.Instance();
                    dialog.SetTitle("Uploaded");
                    dialog.SetMessage("Successfully Uploaded");
                    dialog.SetOk("OK", () =>
                    {
                        dialog.Hide();
                    });
                    dialog.Show();
                }
                else
                {
                    Debug.Log("Not");
                    Debug.Log(w.text);
                }
            }
        }
    }

    private IEnumerator UploadOver(byte[] datas, string f)
    {
        string fileName = f;
        fileName = fileName.ToUpper();
        WWWForm form = new WWWForm();

        form.AddField("id", myid);
        form.AddField("over", "Yes");

        form.AddField("file", "file");
        form.AddBinaryData("file", datas, fileName, "application/octet-stream");
        WWW w = new WWW(host+"/upload.php", form);

        Loadingimg.SetActive(true);
        yield return w;
        Loadingimg.SetActive(false);
        if (w.error != null)
        {
            print("error");
            print(w.error);
        }
        else
        {
            if (w.uploadProgress == 1 && w.isDone)
            {
                if (w.text == "0")
                {
                    GenericDialog dialog = GenericDialog.Instance();
                    dialog.SetTitle("Uploaded");
                    dialog.SetMessage("Successfully Uploaded");
                    dialog.SetOk("OK", () =>
                    {
                        dialog.Hide();
                    });
                    dialog.Show();
                }
            }
        }
    }

    public  IEnumerator GetFile(string fileName, int id)
    {
        Debug.Log("In GetFIl");
        WWWForm form = new WWWForm();
        form.AddField("fid", id);
        WWW w2 = new WWW(host+"/show.php" , form);
        Loadingimg.SetActive(true);
        yield return w2;
        Loadingimg.SetActive(false);
        if (w2.error != null)
        {
            print("error 2");
            print(w2.error);
        }
        else
        {
            if (w2.text != null && w2.text != "")
            {
                Debug.Log(w2.text);
                if (w2.text == "-1")
                {
                    GenericDialog dialog = GenericDialog.Instance();
                    dialog.SetTitle("Error!");
                    dialog.SetMessage("Unknow Error Has Occured");
                    dialog.SetOk("OK", () =>
                    {
                        dialog.Hide();
                    });
                    dialog.Show();
                    yield break;
                }
                loginManger.instance.OK();
                Manager.Instance.LoadFromBytes(w2.bytes, fileName);
            }
        }
    }

    public IEnumerator GetFiles()
    {
        if (myid == -1)
        {
            GenericDialog dialog = GenericDialog.Instance();
            dialog.SetTitle("Not Logged In");
            dialog.SetMessage("Please Login to use this feature");
            dialog.SetOk("OK", () =>
            {
                dialog.Hide();
            });
            dialog.Show();
            yield break;
        }
        WWWForm form = new WWWForm();
        form.AddField("id", myid);
        WWW w2 = new WWW(host+"/show.php", form);
        Loadingimg.SetActive(true);
        yield return w2;
        Loadingimg.SetActive(false);
        if (w2.error != null)
        {
            print("error 2");
            print(w2.error);
        }
        else
        {
            if (w2.text != null && w2.text != "")
            {
                Debug.Log(w2.text);
                if (w2.text == "-1")
                {
                    GenericDialog dialog = GenericDialog.Instance();
                    dialog.SetTitle("Error!");
                    dialog.SetMessage("Error while Getting info from Server");
                    dialog.SetOk("OK", () =>
                    {
                        dialog.Hide();
                    });
                    dialog.Show();
                    yield break;
                }else if(w2.text == "-2")
                {
                    GenericDialog dialog = GenericDialog.Instance();
                    dialog.SetTitle("No Files");
                    dialog.SetMessage("No files found in Server \n Use Save to Server");
                    dialog.SetOk("OK", () =>
                    {
                        dialog.Hide();
                    });
                    dialog.Show();
                    yield break;
                }
                else
                {
                    string[] st = w2.text.Split('\t');
                    loginManger.instance.ShowFiles(st);
                }
            }
        }
    }

    public IEnumerator LogInto(string email,string pass)
    {
        Debug.Log("login");

        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("pass", pass);
        WWW w2 = new WWW(host+"/login.php", form);
        Loadingimg.SetActive(true);
        yield return w2;
        Loadingimg.SetActive(false);
        if (w2.error != null)
        {
            print("error 2");
            print(w2.error);
            loginManger.instance.Error();
        }
        else
        {
            if (w2.text != null && w2.text != "")
            {

                try
                {
                    myid = int.Parse(w2.text);
                }
                catch (System.Exception)
                {
                    loginManger.instance.Error(w2.text);
                    myid = -1;
                    yield break;
                }
                CookieManager.Instance.ClearSessionCookie();
                CookieManager.Instance.SetSessionCookie(myid.ToString());
                loginManger.instance.OK();
            }
        }
    }

    public IEnumerator Signup(string email,string pass,string repass)
    {
        if(pass != repass)
        {
            loginManger.instance.Error("Passwords do not Match");
            yield break;
        }
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("pass", pass);

        WWW w2 = new WWW(host+"/register.php", form);

        Loadingimg.SetActive(true);
        yield return w2;
        Loadingimg.SetActive(false);
        if (w2.error != null)
        {
            print("error 2");
            print(w2.error);
            loginManger.instance.Error();
        }
        else
        {
            if (w2.text != null && w2.text != "")
            {
                try
                {
                    myid = int.Parse(w2.text);
                }
                catch (System.Exception)
                {
                    loginManger.instance.Error(w2.text);
                    myid = -1;
                    yield break;
                }
                CookieManager.Instance.ClearSessionCookie();
                CookieManager.Instance.SetSessionCookie(myid.ToString());
                loginManger.instance.OK();
            }
        }
    }
}
