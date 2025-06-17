using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        MusicManager.Instance.PlayMusic("Main Menu");
    }

    public void PlayGame()
    {
        
    }

    public void Settings()
    {
        
    }
    
    public void Quit()
    {
        Application.Quit();
    }
}
