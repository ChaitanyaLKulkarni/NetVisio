using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(CanvasGroup))]
public class GenericDialog : MonoBehaviour
{

    public Text title;
    public Text message;
    public Text accept, decline,OK;
    public Button acceptButton, declineButton,OKbtn;

    private CanvasGroup cg;
    private bool IsShown = false;
    private bool isOk = false;

    void Awake()
    {
        cg = GetComponent<CanvasGroup>();
    }

    public GenericDialog SetOnAccept(string text, UnityAction action)
    {
        isOk = false;
        accept.text = text;
        acceptButton.onClick.RemoveAllListeners();
        acceptButton.onClick.AddListener(action);
        return this;
    }

    

    public GenericDialog SetOnDecline(string text, UnityAction action)
    {
        isOk = false;
        decline.text = text;
        declineButton.onClick.RemoveAllListeners();
        declineButton.onClick.AddListener(action);
        return this;
    }

    public GenericDialog SetOk(string text, UnityAction action)
    {
        isOk = true;
        OK.text = text;
        OKbtn.onClick.RemoveAllListeners();
        OKbtn.onClick.AddListener(action);
        return this;
    }

    public GenericDialog SetTitle(string title)
    {
        this.title.text = title;
        return this;
    }

    public GenericDialog SetMessage(string message)
    {
        this.message.text = message;
        return this;
    }

    // show the dialog, set it's canvasGroup.alpha to 1f or tween like here
    public void Show()
    {
        IsShown = true;
        this.transform.SetAsLastSibling();
        cg.blocksRaycasts = true;
        cg.interactable = true;
        cg.alpha = 1f;
        acceptButton.gameObject.SetActive(!isOk);
        declineButton.gameObject.SetActive(!isOk);
        OKbtn.gameObject.SetActive(isOk);
    }

    public void Hide()
    {
        IsShown = false;
        cg.interactable = false;
        cg.blocksRaycasts = false;
        cg.alpha = 0;
    }

    private static GenericDialog instance;
    public static GenericDialog Instance()
    {
        if (!instance)
        {
            instance = FindObjectOfType(typeof(GenericDialog)) as GenericDialog;
            if (!instance)
                Debug.Log("There need to be at least one active GenericDialog on the scene");
        }

        return instance;
    }
    private void Update()
    {
        if (IsShown)
        {
            if (Input.GetKeyUp(KeyCode.Return))
            {
                acceptButton.onClick.Invoke();
            }
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                declineButton.onClick.Invoke();
            }
        }
    }
}