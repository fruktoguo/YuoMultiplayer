using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using YuoTools;
using YuoTools.UI;

[RequireComponent(typeof(Button))]
public class YuoKeyButton : MonoBehaviour, IGenerateCode
{
    [HideInInspector] public Button button;

    public InputActionReference action;

    public TextMeshProUGUI keyCodeText;

    private PointerEventData pointerData;

    /// <summary>
    /// 如果需要被遮挡时无法点击则需要设置为关联的面板
    /// </summary>
    [HideInInspector] public UIComponent uiComponent;

    private void Awake()
    {
        button ??= GetComponent<Button>();
        pointerData = new PointerEventData(EventSystem.current)
        {
            button = PointerEventData.InputButton.Left
        };
    }

    void OnInputNameChange()
    {
        // 从动作获取显示字符串。
        if (keyCodeText != null)
        {
            var inputAction = action?.action;
            if (inputAction != null)
            {
                if (inputAction.bindings.Count > 0)
                {
                    var displayString = inputAction.GetBindingDisplayString(0, out var deviceLayoutName,
                        out var controlPath);
                    keyCodeText.text = displayString;
                }
            }
        }
    }

    private void OnEnable()
    {
        if (action != null && action.action != null)
        {
            action.action.started += OnActionStarted;
            action.action.canceled += OnActionCanceled;

            isClick = false;
            button.onClick.AddListener(OnClick);
        }

        OnInputNameChange();
    }

    bool isClick = false;

    void OnClick()
    {
        isClick = true;
        "OnClick".Log();
    }

    private void OnDisable()
    {
        if (action != null && action.action != null)
        {
            action.action.started -= OnActionStarted;
            action.action.canceled -= OnActionCanceled;
            button.onClick.RemoveListener(OnClick);
        }
    }

    bool Check
    {
        get
        {
            bool result = true;

            result &= button != null && button.interactable;

            if (uiComponent != null)
            {
                result &= uiComponent.HasComponent<TopViewComponent>();
            }

            return result;
        }
    }

    void OnActionStarted(InputAction.CallbackContext context)
    {
        if (Check) button?.OnPointerDown(pointerData);
    }

    void OnActionCanceled(InputAction.CallbackContext context)
    {
        button?.OnPointerUp(pointerData);
        if (!isClick)
        {
            //可能会导致点两次
            if (Check) button?.onClick?.Invoke();
        }

        isClick = false;
    }
}