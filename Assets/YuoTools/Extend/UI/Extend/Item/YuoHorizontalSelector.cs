using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace YuoTools.UI
{
    public class YuoHorizontalSelector : MonoBehaviour, IGenerateCode
    {
        [Header("Settings")] public int defaultIndex = 0;
        public bool invertAnimation;
        public bool loopSelection;
        public Transform indicatorParent;
        public GameObject indicatorObject;

        public UnityEvent<SelectorItem> OnValueChanged = new();


        public List<SelectorItem> selectorItems = new List<SelectorItem>();
        private Dictionary<string, SelectorItem> selectorKeys = new();

        private TextMeshProUGUI label;

        private TextMeshProUGUI labelHelper;

        private Animator selectorAnimator;
        public SelectorItem NowItem => selectorItems[NowIndex];

        public string NowTitle => NowItem.itemTitle;

        int index = 0;

        public int NowIndex
        {
            get => index;
            private set
            {
                var newValue = value.RClamp(0, selectorItems.Count - 1);
                if (newValue != index)
                {
                    index = newValue;
                    OnValueChanged.Invoke(NowItem);
                }
            }
        }

        [System.Serializable]
        public class SelectorItem
        {
            public string itemTitle = "Item Title";
            public GameObject GameObject;
            public GameObject On;
            public GameObject Off;
            public object Action;

            public void Select()
            {
                if (On != null)
                    On.SetActive(true);
                if (Off != null)
                    Off.SetActive(false);
            }

            public void Deselect()
            {
                if (On != null)
                    On.SetActive(false);
                if (Off != null)
                    Off.SetActive(true);
            }
        }

        void Awake()
        {
            selectorAnimator = gameObject.GetComponent<Animator>();
            label = transform.Find("Text").GetComponent<TextMeshProUGUI>();
            labelHelper = transform.Find("Text Helper").GetComponent<TextMeshProUGUI>();
        }

        void SelectItem(int selectIndex)
        {
            for (int i = 0; i < selectorItems.Count; i++)
            {
                if (i == selectIndex)
                {
                    label.text = selectorItems[i].itemTitle;
                    selectorItems[i].Select();
                }

                else
                {
                    selectorItems[i].Deselect();
                }
            }
        }

        public void Select(int selectIndex)
        {
            if (selectIndex == NowIndex) return;

            if (selectIndex < NowIndex)
            {
                PreviousAnima();
            }
            else
            {
                ForwardAnima();
            }

            NowIndex = selectIndex;
            SelectItem(selectIndex);
        }

        public void PreviousClick()
        {
            var newIndex = index - 1;

            if (newIndex < 0)
            {
                if (loopSelection)
                {
                    newIndex = selectorItems.Count - 1;
                }
                else
                {
                    return;
                }
            }

            PreviousAnima();

            NowIndex = newIndex;

            SelectItem(NowIndex);
        }

        public void PreviousAnima()
        {
            labelHelper.text = selectorItems[NowIndex].itemTitle;

            selectorAnimator.Play(null);
            selectorAnimator.StopPlayback();

            selectorAnimator.Play(invertAnimation ? "Forward" : "Previous");
        }

        public void ForwardClick()
        {
            var newIndex = index + 1;
            if (newIndex >= selectorItems.Count)
            {
                if (loopSelection)
                {
                    newIndex = 0;
                }
                else
                {
                    return;
                }
            }

            ForwardAnima();

            NowIndex = newIndex;

            SelectItem(NowIndex);
        }

        public void ForwardAnima()
        {
            labelHelper.text = selectorItems[NowIndex].itemTitle;

            selectorAnimator.Play(null);
            selectorAnimator.StopPlayback();

            selectorAnimator.Play(invertAnimation ? "Previous" : "Forward");
        }

        public SelectorItem AddItem(string key)
        {
            if (selectorKeys.TryGetValue(key, out var selectorItem)) return selectorItem;
            selectorItem = new SelectorItem();
            GameObject go =
                Instantiate(indicatorObject, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            go.transform.SetParent(indicatorParent, false);
            go.name = key;
            selectorItem.On = go.transform.Find("On").gameObject;
            selectorItem.Off = go.transform.Find("Off").gameObject;
            selectorItem.itemTitle = key;
            selectorItem.GameObject = go;
            selectorItems.Add(selectorItem);
            selectorKeys.Add(key, selectorItem);
            return selectorItem;
        }

        public void SetItem(string key)
        {
            if (selectorKeys.ContainsKey(key))
            {
                NowIndex = selectorItems.FindIndex(x => x.itemTitle == key);
                SelectItem(NowIndex);
            }
        }

        public void UpdateUI()
        {
            if (label == null || selectorItems.Count < index)
                return;

            label.text = selectorItems[index].itemTitle;

            SelectItem(index);
        }
    }
}