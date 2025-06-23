using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
    public Image[] digitImages;
    public Sprite[] digitSprites;

    public void UpdateDiamond(int quantity)
    {
        string diamondStr = quantity.ToString().PadLeft(digitImages.Length, '0');

        for (int i = 0; i < digitImages.Length; i++)
        {
            int digit = diamondStr[i] - '0';
            digitImages[i].sprite = digitSprites[digit];
        }
    }
}
