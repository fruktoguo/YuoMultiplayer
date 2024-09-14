using UnityEngine;
using UnityEngine.InputSystem;

namespace YuoTools
{
    public class RebindSaveLoad : MonoBehaviour
    {
        public InputActionAsset actions;

        public void OnEnable()
        {
            var rebinds = PlayerPrefs.GetString("rebinds").Log();
            if (!string.IsNullOrEmpty(rebinds))
            {
                actions.LoadBindingOverridesFromJson(rebinds);
            }

            actions.Enable();
        }

        public void OnDisable()
        {
            var rebinds = actions.SaveBindingOverridesAsJson();
            PlayerPrefs.SetString("rebinds", rebinds);
            actions.Disable();
        }
    }
}