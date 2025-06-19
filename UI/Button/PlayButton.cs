using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : BaseButton
{
    public UIPanelAnim menuPanel;
    public UIPanelAnim titlePanel;

    public MonoBehaviour coroutineRunner;
    
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        menuPanel.FlyOutToRight();
        titlePanel.FlyOutToLeft();
        coroutineRunner.StartCoroutine(WaitForUIAnimation(1f));
    }

    IEnumerator WaitForUIAnimation(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        SceneManager.LoadScene("Level 1");
        MusicManager.Instance.PlayMusic("Level 1");
    }
}
