using UnityEngine;
using UnityEngine.UI;

namespace YuoTools.UI
{
    public partial class SpawnUICodeConfig
    {
        public void InitYuoComponent()
        {
            RemoveType.Add(typeof(YuoSlider), typeof(RectTransform));
            RemoveType.Add(typeof(YuoDropDown), typeof(Button));
        }
    }
}