using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIPanelFlyAnim : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector2 centerPos;
    
    [SerializeField] float duration;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        centerPos = rectTransform.anchoredPosition;
    }

    public void FlyInFromLeft()
    {
        rectTransform.anchoredPosition = new Vector2(-Screen.width, 0);
        gameObject.SetActive(true);
        
        rectTransform.DOAnchorPos(centerPos, duration).SetEase(Ease.OutBack);
    }

    public void FlyOutToLeft()
    {
        rectTransform.DOAnchorPos(new Vector2(-Screen.width, 0), duration)
            .SetEase(Ease.InBack)
            .OnComplete(() => gameObject.SetActive(false));
    }
    
    public void FlyInFromRight()
    {
        rectTransform.anchoredPosition = new Vector2(Screen.width, 0);
        gameObject.SetActive(true);
        
        rectTransform.DOAnchorPos(centerPos, duration).SetEase(Ease.OutBack);
    }

    public void FlyOutToRight()
    {
        rectTransform.DOAnchorPos(new Vector2(Screen.width, 0), duration)
            .SetEase(Ease.InBack)
            .OnComplete(() => gameObject.SetActive(false));
    }
    
    public void FlyInFromAbove()
    {
        rectTransform.anchoredPosition = new Vector2(Screen.height, 0);
        gameObject.SetActive(true);
        
        rectTransform.DOAnchorPos(centerPos, duration).SetEase(Ease.OutBack);
    }

    public void FlyOutToAbove()
    {
        rectTransform.DOAnchorPos(new Vector2(Screen.height, 0), duration)
            .SetEase(Ease.InBack)
            .OnComplete(() => gameObject.SetActive(false));
    }
    
    public void FlyInFromBelow()
    {
        rectTransform.anchoredPosition = new Vector2(-Screen.height, 0);
        gameObject.SetActive(true);
        
        rectTransform.DOAnchorPos(centerPos, duration).SetEase(Ease.OutBack);
    }

    public void FlyOutToBelow()
    {
        rectTransform.DOAnchorPos(new Vector2(-Screen.height, 0), duration)
            .SetEase(Ease.InBack)
            .OnComplete(() => gameObject.SetActive(false));
    }
}
