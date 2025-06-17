using UnityEngine.SceneManagement;

public class PlayButton : BaseButton
{
    public UIPanelFlyAnim menuPanel;
    public UIPanelFlyAnim titlePanel;
    
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        SceneManager.LoadScene("Level 1");
        MusicManager.Instance.PlayMusic("Level 1");
        menuPanel.FlyOutToRight();
        titlePanel.FlyOutToLeft();
    }
}
