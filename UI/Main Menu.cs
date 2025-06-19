using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public UIPanelAnim menuPanel;
    public UIPanelAnim titlePanel;
    
    private void Start()
    {
        MusicManager.Instance.PlayMusic("Main Menu");
        menuPanel.FlyInFromLeft();
        titlePanel.FlyInFromRight();
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
