using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

////TODO: 本地化支持

////TODO: 处理在不同控制方案中绑定了部分部分的复合体

namespace YuoTools
{
    /// <summary>
    /// 一个可重用的组件，带有一个自包含的UI，用于重新绑定单个动作。
    /// </summary>
    public class RebindActionUI : MonoBehaviour
    {
        /// <summary>
        /// 要重新绑定的动作的引用。
        /// </summary>
        public InputActionReference actionReference
        {
            get => m_Action;
            set
            {
                m_Action = value;
                UpdateActionLabel();
                UpdateBindingDisplay();
            }
        }

        /// <summary>
        /// 要在动作上重新绑定的绑定的ID（以字符串形式）。
        /// </summary>
        /// <seealso cref="InputBinding.id"/>
        public string bindingId
        {
            get => m_BindingId;
            set
            {
                m_BindingId = value;
                UpdateBindingDisplay();
            }
        }

        public InputBinding.DisplayStringOptions displayStringOptions
        {
            get => m_DisplayStringOptions;
            set
            {
                m_DisplayStringOptions = value;
                UpdateBindingDisplay();
            }
        }

        /// <summary>
        /// 接收动作名称的文本组件。可选。
        /// </summary>
        public TextMeshProUGUI actionLabel
        {
            get => m_ActionLabel;
            set
            {
                m_ActionLabel = value;
                UpdateActionLabel();
            }
        }

        /// <summary>
        /// 接收绑定的显示字符串的文本组件。
        /// </summary>
        public TextMeshProUGUI bindingText
        {
            get => m_BindingText;
            set
            {
                m_BindingText = value;
                UpdateBindingDisplay();
            }
        }

        /// <summary>
        /// 在等待控件被激活时接收文本提示的可选文本组件。
        /// </summary>
        /// <seealso cref="startRebindEvent"/>
        /// <seealso cref="rebindOverlay"/>
        public TextMeshProUGUI rebindPrompt
        {
            get => m_RebindText;
            set => m_RebindText = value;
        }

        /// <summary>
        /// 当交互式重新绑定开始时激活，当重新绑定完成时停用的可选UI。通常用于在系统等待控件被激活时在当前UI上显示叠加层。
        /// </summary>
        /// <remarks>
        /// 如果<see cref="rebindPrompt"/>和<c>rebindOverlay</c>都没有设置，组件将临时将<see cref="bindingText"/>（如果不为<c>null</c>）替换为<c>"Waiting..."</c>。
        /// </remarks>
        /// <seealso cref="startRebindEvent"/>
        /// <seealso cref="rebindPrompt"/>
        public GameObject rebindOverlay
        {
            get => m_RebindOverlay;
            set => m_RebindOverlay = value;
        }

        /// <summary>
        /// 每当UI更新以反映当前绑定时触发的事件。这可以用于将自定义可视化绑定到绑定上。
        /// </summary>
        public UpdateBindingUIEvent updateBindingUIEvent
        {
            get
            {
                if (m_UpdateBindingUIEvent == null)
                    m_UpdateBindingUIEvent = new UpdateBindingUIEvent();
                return m_UpdateBindingUIEvent;
            }
        }

        /// <summary>
        /// 当在动作上启动交互式重新绑定时触发的事件。
        /// </summary>
        public InteractiveRebindEvent startRebindEvent
        {
            get
            {
                if (m_RebindStartEvent == null)
                    m_RebindStartEvent = new InteractiveRebindEvent();
                return m_RebindStartEvent;
            }
        }

        /// <summary>
        /// 当交互式重新绑定完成或取消时触发的事件。
        /// </summary>
        public InteractiveRebindEvent stopRebindEvent
        {
            get
            {
                if (m_RebindStopEvent == null)
                    m_RebindStopEvent = new InteractiveRebindEvent();
                return m_RebindStopEvent;
            }
        }

        /// <summary>
        /// 当交互式重新绑定正在进行时，这是重新绑定操作控制器。否则，它为<code>null</code>。
        /// </summary>
        public InputActionRebindingExtensions.RebindingOperation ongoingRebind => m_RebindOperation;

        /// <summary>
        /// 根据组件的目标绑定返回动作和绑定索引。
        /// </summary>
        /// <param name="action"></param>
        /// <param name="bindingIndex"></param>
        /// <returns></returns>
        public bool ResolveActionAndBinding(out InputAction action, out int bindingIndex)
        {
            bindingIndex = -1;

            action = m_Action?.action;
            if (action == null)
                return false;

            if (string.IsNullOrEmpty(m_BindingId))
                return false;

            // 查找绑定索引。
            var bindingId = new Guid(m_BindingId);
            bindingIndex = action.bindings.IndexOf(x => x.id == bindingId);
            if (bindingIndex == -1)
            {
                Debug.LogError($"无法在'{action}'上找到ID为'{bindingId}'的绑定", this);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 触发当前显示的绑定的刷新。
        /// </summary>
        public void UpdateBindingDisplay()
        {
            var displayString = string.Empty;
            var deviceLayoutName = default(string);
            var controlPath = default(string);

            // 从动作获取显示字符串。
            var action = m_Action?.action;
            if (action != null)
            {
                var bindingIndex = action.bindings.IndexOf(x => x.id.ToString() == m_BindingId);
                if (bindingIndex != -1)
                    displayString = action.GetBindingDisplayString(bindingIndex, out deviceLayoutName, out controlPath,
                        displayStringOptions);
            }

            // 设置到标签上（如果有）。
            if (m_BindingText != null)
                m_BindingText.text = displayString;

            // 给监听器一个机会来配置UI。
            m_UpdateBindingUIEvent?.Invoke(this, displayString, deviceLayoutName, controlPath);
        }

        /// <summary>
        /// 移除当前应用的绑定覆盖。
        /// </summary>
        public void ResetToDefault()
        {
            if (!ResolveActionAndBinding(out var action, out var bindingIndex))
                return;

            if (action.bindings[bindingIndex].isComposite)
            {
                // 是一个复合体。从部分绑定中删除覆盖。
                for (var i = bindingIndex + 1; i < action.bindings.Count && action.bindings[i].isPartOfComposite; ++i)
                    action.RemoveBindingOverride(i);
            }
            else
            {
                action.RemoveBindingOverride(bindingIndex);
            }

            UpdateBindingDisplay();
        }

        /// <summary>
        /// 启动交互式重新绑定，让玩家激活控件以选择动作的新绑定。
        /// </summary>
        public void StartInteractiveRebind()
        {
            if (!ResolveActionAndBinding(out var action, out var bindingIndex))
                return;

            // 如果绑定是一个复合体，我们需要依次重新绑定每个部分。
            if (action.bindings[bindingIndex].isComposite)
            {
                var firstPartIndex = bindingIndex + 1;
                if (firstPartIndex < action.bindings.Count && action.bindings[firstPartIndex].isPartOfComposite)
                    PerformInteractiveRebind(action, firstPartIndex, allCompositeParts: true);
            }
            else
            {
                PerformInteractiveRebind(action, bindingIndex);
            }
        }

        private void PerformInteractiveRebind(InputAction action, int bindingIndex, bool allCompositeParts = false)
        {
            action.Disable();
            m_RebindOperation?.Cancel(); // 将m_RebindOperation设置为null。

            void CleanUp()
            {
                m_RebindOperation?.Dispose();
                m_RebindOperation = null;
            }

            // 配置重新绑定。
            m_RebindOperation = action.PerformInteractiveRebinding(bindingIndex)
                .OnCancel(
                    operation =>
                    {
                        m_RebindStopEvent?.Invoke(this, operation);
                        m_RebindOverlay?.SetActive(false);
                        UpdateBindingDisplay();
                        CleanUp();
                        action.Enable();
                    })
                .OnComplete(
                    operation =>
                    {
                        m_RebindOverlay?.SetActive(false);
                        m_RebindStopEvent?.Invoke(this, operation);
                        UpdateBindingDisplay();
                        CleanUp();
                        action.Enable();

                        // 如果还有更多的复合部分需要绑定，启动下一个部分的重新绑定。
                        if (allCompositeParts)
                        {
                            var nextBindingIndex = bindingIndex + 1;
                            if (nextBindingIndex < action.bindings.Count &&
                                action.bindings[nextBindingIndex].isPartOfComposite)
                                PerformInteractiveRebind(action, nextBindingIndex, true);
                        }
                    });

            // 如果是部分绑定，在UI中显示部分的名称。
            var partName = default(string);
            if (action.bindings[bindingIndex].isPartOfComposite)
                partName = $"绑定'{action.bindings[bindingIndex].name}'。";

            // 显示重新绑定叠加层，如果有的话。
            m_RebindOverlay?.SetActive(true);
            if (m_RebindText != null)
            {
                var text = !string.IsNullOrEmpty(m_RebindOperation.expectedControlType)
                    ? $"{partName}等待{m_RebindOperation.expectedControlType}输入..."
                    : $"{partName}等待输入...";
                m_RebindText.text = text;
            }

            // 如果没有重新绑定叠加层和回调，但有绑定文本标签，将绑定文本标签临时设置为"<Waiting>"。
            if (m_RebindOverlay == null && m_RebindText == null && m_RebindStartEvent == null && m_BindingText != null)
                m_BindingText.text = "<等待中...>";

            // 给监听器一个机会来处理重新绑定开始。
            m_RebindStartEvent?.Invoke(this, m_RebindOperation);

            m_RebindOperation.Start();
        }

        protected void OnEnable()
        {
            if (s_RebindActionUIs == null)
                s_RebindActionUIs = new List<RebindActionUI>();
            s_RebindActionUIs.Add(this);
            if (s_RebindActionUIs.Count == 1)
                InputSystem.onActionChange += OnActionChange;
        }

        protected void OnDisable()
        {
            m_RebindOperation?.Dispose();
            m_RebindOperation = null;

            s_RebindActionUIs.Remove(this);
            if (s_RebindActionUIs.Count == 0)
            {
                s_RebindActionUIs = null;
                InputSystem.onActionChange -= OnActionChange;
            }
        }

        // 当动作系统重新解析绑定时，我们希望根据此作出反应更新我们的UI。虽然这也会触发我们自己所做的更改，但它确保我们对其他地方所做的更改做出反应。如果用户更改键盘布局，例如，我们将收到BoundControlsChanged通知，并更新我们的UI以反映当前的键盘布局。
        private static void OnActionChange(object obj, InputActionChange change)
        {
            if (change != InputActionChange.BoundControlsChanged)
                return;

            var action = obj as InputAction;
            var actionMap = action?.actionMap ?? obj as InputActionMap;
            var actionAsset = actionMap?.asset ?? obj as InputActionAsset;

            for (var i = 0; i < s_RebindActionUIs.Count; ++i)
            {
                var component = s_RebindActionUIs[i];
                var referencedAction = component.actionReference?.action;
                if (referencedAction == null)
                    continue;

                if (referencedAction == action ||
                    referencedAction.actionMap == actionMap ||
                    referencedAction.actionMap?.asset == actionAsset)
                    component.UpdateBindingDisplay();
            }
        }

        [Tooltip("要从UI重新绑定的动作的引用。")] [SerializeField]
        private InputActionReference m_Action;

        [SerializeField] private string m_BindingId;

        [SerializeField] private InputBinding.DisplayStringOptions m_DisplayStringOptions;

        [Tooltip("将接收动作名称的文本标签。可选。设置为None以使重新绑定UI不显示动作的标签。")] [SerializeField]
        private TextMeshProUGUI m_ActionLabel;

        [Tooltip("将接收当前格式化绑定字符串的文本标签。")] [SerializeField]
        private TextMeshProUGUI m_BindingText;

        [Tooltip("将在重新绑定进行时显示的可选UI。")] [SerializeField]
        private GameObject m_RebindOverlay;

        [Tooltip("将更新为用户输入提示的可选文本标签。")] [SerializeField]
        private TextMeshProUGUI m_RebindText;

        [Tooltip(
            "当绑定显示方式需要更新时触发的事件。这允许以自定义方式显示绑定，例如使用图像而不是文本。")]
        [SerializeField]
        private UpdateBindingUIEvent m_UpdateBindingUIEvent;

        [Tooltip(
            "当启动交互式重新绑定时触发的事件。这可以用于在重新绑定进行时实现自定义UI行为。它还可以用于进一步自定义重新绑定。")]
        [SerializeField]
        private InteractiveRebindEvent m_RebindStartEvent;

        [Tooltip("当交互式重新绑定完成或被取消时触发的事件。")] [SerializeField]
        private InteractiveRebindEvent m_RebindStopEvent;

        private InputActionRebindingExtensions.RebindingOperation m_RebindOperation;

        private static List<RebindActionUI> s_RebindActionUIs;

        // 我们希望在编辑模式下也更新动作名称的标签，所以我们从这里开始。
#if UNITY_EDITOR
        protected void OnValidate()
        {
            UpdateActionLabel();
            UpdateBindingDisplay();
        }

#endif

        private void UpdateActionLabel()
        {
            if (m_ActionLabel != null)
            {
                var action = m_Action?.action;
                m_ActionLabel.text = action != null ? action.name : string.Empty;
            }
        }

        [Serializable]
        public class UpdateBindingUIEvent : UnityEvent<RebindActionUI, string, string, string>
        {
        }

        [Serializable]
        public class
            InteractiveRebindEvent : UnityEvent<RebindActionUI, InputActionRebindingExtensions.RebindingOperation>
        {
        }
    }
}