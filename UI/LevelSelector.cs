using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    public Button[] buttons;

    void Start()
    {
        int unlockedLevels = PlayerPrefs.GetInt("unlockedLevels", 1);

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].gameObject.SetActive((i+1) <= unlockedLevels);
        }
    }
}
