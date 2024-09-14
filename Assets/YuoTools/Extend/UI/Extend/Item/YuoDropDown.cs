using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace YuoTools.UI
{
    public class YuoDropDown : MonoBehaviour, IPointerClickHandler, ICancelHandler, IGenerateCode
    {
        public TextMeshProUGUI LabelText;
        public Image Arrow;

        [SerializeField] private GameObject dropPre;

        public ScrollRect DropList;

        [SerializeField] private List<SelectorItem> selectorItems = new();

        private Dictionary<string, SelectorItem> selectorKeys = new();

        public int DropCount
        {
            get => selectorItems.Count;
        }

        [HorizontalGroup("背景")] [SerializeField] [LabelWidth(25)]
        private Image _背景;

        [HorizontalGroup("背景")] [SerializeField] [LabelWidth(50)]
        private Sprite _背景图片;

        int defSortOrder;

        private void Awake()
        {
            _defScale = Arrow.transform.localScale;
        }

        private void Update()
        {
            if (!DropList.gameObject.activeSelf) return;
            if (Input.GetMouseButtonUp(0))
            {
                var go = new PointerEventData(EventSystem.current).selectedObject;
                if (go != null)
                {
                    if (go.GetComponentInParent<YuoDropDown>() == this) return;
                }

                HideDropDown();
            }
        }

        public SelectorItem AddItem(string key)
        {
            return AddItem(key, key);
        }

        public SelectorItem AddItem(string key, string text)
        {
            if (selectorKeys.TryGetValue(key, out var item)) return item;
 
            SelectorItem selectorItem = Instantiate(dropPre, dropPre.transform.parent).AddComponent<SelectorItem>();
            selectorItem.go = selectorItem.gameObject;
            selectorItem.name = key;
            selectorItem.go.SetActive(true);
            selectorItem.button = selectorItem.go.GetComponent<Button>();
            selectorItem.text = selectorItem.go.transform.Find("Label").GetComponent<TextMeshProUGUI>();
            selectorItem.arrow = selectorItem.go.transform.Find("Arrow").gameObject;
            selectorItem.button.onClick.RemoveAllListeners();
            selectorItem.button.onClick.AddListener(HideDropDown);
            selectorItem.button.onClick.AddListener(() => SetItem(selectorItem));
            selectorItem.text.text = text;
            selectorItem.index = selectorItems.Count;
            selectorItems.Add(selectorItem);
            selectorKeys.Add(key, selectorItem);
            return selectorItem;
        }

        public bool ContainsItem(string key)
        {
            return selectorKeys.ContainsKey(key);
        }

        private Vector3 _defScale;

        public void Clear()
        {
            foreach (var item in selectorItems)
            {
                Destroy(item.gameObject);
            }

            selectorItems.Clear();
            selectorKeys.Clear();
        }

        public void HideDropDown()
        {
            DropList.gameObject.SetActive(false);
            Arrow.transform.localScale = _defScale;
            transform.SetSiblingIndex(defSortOrder);
        }

        public void ShowDropDown()
        {
            DropList.gameObject.SetActive(true);

            Arrow.transform.localScale = _defScale * -1;
            if (NowItem != null)
                DropList.verticalNormalizedPosition = 1f - NowItem.index / (selectorItems.Count - 1f);
            defSortOrder = transform.GetSiblingIndex();
            transform.SetSiblingIndex(transform.parent.childCount - 1);
        }

        [SerializeField] private SelectorItem nowItem;

        public SelectorItem NowItem
        {
            get => nowItem;
            set
            {
                OnValueChanged?.Invoke(value);
                nowItem = value;
                LabelText.text = value.name;
            }
        }

        public UnityEvent<SelectorItem> OnValueChanged;

        public void SetItem(SelectorItem itemName)
        {
            NowItem = itemName;
            foreach (var item in selectorItems)
            {
                if (item.arrow.activeSelf)
                {
                    item.arrow.SetActive(false);
                }
            }

            itemName.arrow.SetActive(true);
        }

        public void SetItem(string key)
        {
            if (selectorKeys.ContainsKey(key))
            {
                foreach (var item in selectorItems)
                {
                    if (item.arrow.activeSelf)
                    {
                        item.arrow.SetActive(false);
                    }
                }

                NowItem = selectorKeys[key];
                selectorKeys[key].arrow.SetActive(true);
            }
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (DropList.gameObject.activeSelf)
            {
                if (eventData.selectedObject == gameObject)
                {
                    HideDropDown();
                }
            }
            else
            {
                ShowDropDown();
            }
            //EventSystem.current.SetSelectedGameObject(base.gameObject);
        }

        public virtual void OnCancel(BaseEventData eventData)
        {
            //print(eventData.selectedObject.name);
        }

        [System.Serializable]
        public class SelectorItem : MonoBehaviour
        {
            public new string name;
            public int index;
            public GameObject go;
            public Button button;
            public TextMeshProUGUI text;
            public GameObject arrow;
            public object Action;
        }
    }
}