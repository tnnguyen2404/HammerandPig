using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        MusicManager.Instance.PlayMusic("Main Menu");
    }

    public void PlayGame()
    {
        MusicManager.Instance.PlayMusic("Level 1");
    }

    public void Settings()
    {
        
    }
    
    public void Quit()
    {
        Application.Quit();
    }
}
