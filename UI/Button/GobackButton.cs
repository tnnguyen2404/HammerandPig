using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GobackButton : BaseButton
{
    public UIPanelAnim soundAndMusicPanel;
    public UIPanelAnim settingsBoard;
    public UIPanelAnim menuPanel;
    public UIPanelAnim titlePanel;
    public UIPanelAnim levelPanel;

    public MonoBehaviour coroutineRunner;
    
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        
        if (gameObject.transform.parent.name == "Sound&Music")
        {
            soundAndMusicPanel.FlyOutToAbove();
            settingsBoard.FlyOutToBelow();
            coroutineRunner.StartCoroutine(WaitForUIAnimation(0.8f));
        } 
        else if (gameObject.transform.parent.name == "Level")
        {
            levelPanel.PopOut();
            coroutineRunner.StartCoroutine(WaitForUIAnimation(0.8f));
        }
    }
    
    IEnumerator WaitForUIAnimation(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        menuPanel.FlyInFromLeft();
        titlePanel.FlyInFromRight();
    }
}
