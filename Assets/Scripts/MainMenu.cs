using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {


    public GameObject panel;
    public GameObject About;

    public static MainMenu instance;

    public enum After
    {
        None,
        Open,
        Game,
        Tut
    }
    public After after;

    private void Start()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void CreateNw()
    {
        panel.SetActive(true);
    }

    public void GameMode()
    {
        panel.SetActive(false);
        after = After.Game;
        
        SceneManager.LoadScene(1);
    }

    public void Help()
    {
        panel.SetActive(false);
        after = After.Tut;
        SceneManager.LoadScene(1);
    }

    public void AboutUs()
    {
        panel.SetActive(false);
        About.SetActive(true);
    }

    public void MainMenuB()
    {
        About.SetActive(false);
    }

    public void NewNw()
    {
        panel.SetActive(false);
        after = After.None;
        
        SceneManager.LoadScene(1);
    }

    public void OpeNw()
    {
        panel.SetActive(false);
        after = After.Open;
        
        SceneManager.LoadScene(1);
    }



}
