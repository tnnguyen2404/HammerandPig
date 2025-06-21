using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public UIPanelAnim menuPanel;
    public UIPanelAnim titlePanel;
    
    private void Start()
    {
        Application.targetFrameRate = 120;
        MusicManager.Instance.PlayMusic("Main Menu");
        menuPanel.FlyInFromLeft();
        titlePanel.FlyInFromRight();
    }
}
