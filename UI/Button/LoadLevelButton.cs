using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadLevelButton : BaseButton
{
    public int levelIndex;
    
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        LevelManager.Instance.LoadScene("Level " + levelIndex, "CircleWipe");
    }
}
