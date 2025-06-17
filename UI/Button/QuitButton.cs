using UnityEngine;

public class QuitButton : BaseButton
{
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        Application.Quit();
    }
}
