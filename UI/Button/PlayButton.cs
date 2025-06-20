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
        LevelManager.Instance.LoadScene("Level 1", "CircleWipe");
        MusicManager.Instance.PlayMusic("Level 1");
        
        /*menuPanel.FlyOutToRight();
        titlePanel.FlyOutToLeft();
        coroutineRunner.StartCoroutine(WaitForUIAnimation(1f));*/
    }

    /*IEnumerator WaitForUIAnimation(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
    }*/
}
