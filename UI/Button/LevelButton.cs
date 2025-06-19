using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelButton : BaseButton
{
    public UIPanelAnim menuPanel;
    public UIPanelAnim titlePanel;
    public UIPanelAnim levelPanel;
    
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
        levelPanel.Rotate();
    }
}
