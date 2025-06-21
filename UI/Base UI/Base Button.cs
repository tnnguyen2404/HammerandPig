using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BaseButton : MonoBehaviour
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
    }

    protected virtual void OnButtonClick()
    {
        SoundManager.Instance.PlaySound2D("ClickUI");
    }
}
