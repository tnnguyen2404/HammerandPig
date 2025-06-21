using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeSlider : BaseSlider
{
    protected override void Awake()
    {
        base.Awake();
        slider.value = PlayerPrefs.GetFloat("Volume", 1f);
    }
    
    protected override void OnSliderValueChanged(float value)
    {
        if (MusicManager.Instance)
            MusicManager.Instance.SetVolume(value);
        
    }
}
