using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnOffButton : BaseButton
{
    public Image checkImage;
    public Image uncheckImage;
    private bool isOn = true;
    
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        isOn = !isOn;
        checkImage.gameObject.SetActive(isOn);
        uncheckImage.gameObject.SetActive(!isOn);

        if (!isOn)
        {
            if (gameObject.transform.name == "Sfx Button")
                SoundManager.Instance.StopSfx();
            else if (gameObject.transform.name == "Music Button")
                MusicManager.Instance.StopMusic();
        }
        else
        {
            if (gameObject.transform.name == "Sfx Button")
                SoundManager.Instance.ResumeSfx();
            else if (gameObject.transform.name == "Music Button")
                MusicManager.Instance.ResumeMusic();
        }
    }
}
