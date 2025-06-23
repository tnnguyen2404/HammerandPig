using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelButton : BaseButton
{
    public UIPanelAnim menuPanel;
    public UIPanelAnim titlePanel;
    public UIPanelAnim levelPanel;
    
    public UIPanelAnim gameOverTitlePanel;
    public UIPanelAnim gameOverButtonPanel;
    public UIPanelAnim gameOverScorePanel;
    
    public MonoBehaviour coroutineRunner;
    
    protected override void OnButtonClick()
    {
        base.OnButtonClick();

        if (transform.parent.name == "Main Menu")
        {
            menuPanel.FlyOutToRight();
            titlePanel.FlyOutToLeft();
        }
        else if (transform.parent.name == "Button Banner")
        {
            gameOverTitlePanel.FlyOutToAbove();
            gameOverButtonPanel.FlyOutToBelow();
            gameOverScorePanel.PopIn();
        }
        
        coroutineRunner.StartCoroutine(WaitForUIAnimation(1f));
    }
    
    IEnumerator WaitForUIAnimation(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        levelPanel.PopIn();
    }
}
