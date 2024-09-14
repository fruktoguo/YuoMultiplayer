using UnityEngine;
using YuoTools;
using YuoTools.UI;

public class ImageFill : MonoBehaviour, IGenerateCode
{
    [SerializeField] [Range(0, 1)] private float fillAmount = 1;

    public float FillAmount
    {
        get => fillAmount;
        set
        {
            fillAmount = value.RClamp(0, 1);
            OnValidate();
        }
    }

    RectTransform _rectTransform;
    public RectTransform rectTransform => _rectTransform ??= GetComponent<RectTransform>();

    private void OnValidate()
    {
        rectTransform.anchorMin = rectTransform.anchorMin.RSetX(0);
        rectTransform.anchorMax = rectTransform.anchorMax.RSetX(fillAmount);
    }
}