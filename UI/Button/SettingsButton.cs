using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsButton : BaseButton
{
    public UIPanelAnim soundAndMusicPanel;
    public UIPanelAnim settingsBoard;
    public UIPanelAnim menuPanel;
    public UIPanelAnim titlePanel;

    public MonoBehaviour coroutineRunner;

    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        menuPanel.FlyOutToRight();
        titlePanel.FlyOutToLeft();
        coroutineRunner.StartCoroutine(WaitForUIAnimation(0.8f));
    }

    IEnumerator WaitForUIAnimation(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        soundAndMusicPanel.FlyInFromAbove();
        settingsBoard.FlyInFromBelow();
    }
}
