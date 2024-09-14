using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace YuoTools.UI
{
    public class YuoSlider : MonoBehaviour, IGenerateCode
    {
        public Slider Slider;
        public TMP_InputField InputField;

        public string Format = "F2";

        // Start is called before the first frame update
        void Start()
        {
            Slider.onValueChanged.AddListener(OnSliderChange);
            InputField.onValueChanged.AddListener(OnInputFieldChange);
            OnSliderChange(Slider.value);
        }

        private void OnInputFieldChange(string value)
        {
            Slider.SetValueWithoutNotify(float.Parse(value));
        }

        void OnSliderChange(float value)
        {
            InputField.SetTextWithoutNotify(value.ToString(Format));
        }
    }
}