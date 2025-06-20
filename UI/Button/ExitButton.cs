using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitButton : BaseButton
{
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        Application.Quit();
    }
}
