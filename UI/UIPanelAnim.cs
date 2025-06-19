using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIPanelAnim : MonoBehaviour
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
        gameObject.SetActive(true);
        rectTransform.anchoredPosition = new Vector2(-Screen.width, 0);
        
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
        gameObject.SetActive(true);
        rectTransform.anchoredPosition = new Vector2(Screen.width, 0);
        
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
        gameObject.SetActive(true);
        rectTransform.anchoredPosition = new Vector2(0, Screen.height);
        
        rectTransform.DOAnchorPos(centerPos, duration).SetEase(Ease.OutBack);
    }

    public void FlyOutToAbove()
    {
        rectTransform.DOAnchorPos(new Vector2(0, Screen.height), duration)
            .SetEase(Ease.InBack)
            .OnComplete(() => gameObject.SetActive(false));
    }
    
    public void FlyInFromBelow()
    {
        gameObject.SetActive(true);
        rectTransform.anchoredPosition = new Vector2(0, -Screen.height);
        
        rectTransform.DOAnchorPos(centerPos, duration).SetEase(Ease.OutBack);
    }

    public void FlyOutToBelow()
    {
        rectTransform.DOAnchorPos(new Vector2(0, -Screen.height), duration)
            .SetEase(Ease.InBack)
            .OnComplete(() => gameObject.SetActive(false));
    }

    public void Rotate()
    {
        gameObject.SetActive(true);
        transform.DORotate(new Vector3(0, 0, 360), 1f, RotateMode.FastBeyond360)
            .SetEase(Ease.OutQuart)
            .SetLoops(1);
    }

    public void PopOut()
    {
        transform.DOScale(Vector3.zero, 0.5f)
            .SetEase(Ease.InBounce)
            .OnComplete(() => gameObject.SetActive(false));
    }
}
