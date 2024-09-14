using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using YuoTools.Main.Ecs;
using YuoTools.UI;

namespace YuoTools.Extend.Helper
{
    public static class UIHelper
    {
        public static void SetBtnClick(this Button btn, UnityAction action)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(action);
        }

        public static void AddUIClose(this Button btn, string windowName)
        {
            btn.onClick.AddListener(() => UIManagerComponent.Get.Close(windowName));
        }

        public static void SetUIClose(this Button btn, string windowName)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => UIManagerComponent.Get.Close(windowName));
        }

        public static void SetUICloseWaitAnima(this Button btn, string windowName)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                var view = UIManagerComponent.Get.GetUIView(windowName);
                var viewAnima = view.GetComponent<UIAnimaComponent>();
                if (viewAnima != null && viewAnima.Sate != UISetting.UISate.Show) return;
                UIManagerComponent.Get.Close(windowName);
            });
        }

        public static void AddUIOpen(this Button btn, string windowName)
        {
            btn.onClick.AddListener(Call);

            async void Call() => await UIManagerComponent.Get.OpenAsync(windowName);
        }

        public static void SetUIOpen(this Button btn, string windowName)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(Call);

            async void Call() => await UIManagerComponent.Get.OpenAsync(windowName);
        }

        public static void AddOnOpen(this Toggle toggle, UnityAction action)
        {
            toggle.onValueChanged.AddListener(x =>
            {
                if (x) action();
            });
        }

        public static void AddOnClose(this Toggle toggle, UnityAction action)
        {
            toggle.onValueChanged.AddListener(x =>
            {
                if (!x) action();
            });
        }
        
        public static async void DisableInteractable(this Selectable selectable,float time)
        {
            selectable.interactable = false;
            await YuoWait.WaitUnscaledTimeAsync(time);
            selectable.interactable = true;
        }


        public static Vector2 WorldPosToCanvas(this Canvas canvas, Vector3 worldPosition)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,
                    canvas.worldCamera.WorldToScreenPoint(worldPosition), canvas.worldCamera, out var position))
            {
                return position;
            }

            return Vector2.zero;
        }

        public static Vector2 WorldPosToCanvas(this Canvas canvas, Vector2 worldPosition, Camera camera)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,
                    camera.WorldToScreenPoint(new Vector3(worldPosition.x, worldPosition.y, 0)),
                    camera, out var position))
            {
                return position;
            }

            return Vector2.zero;
        }

        public static Vector2 ViewPosToCanvas(this Canvas canvas, Vector2 viewPos)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,
                    canvas.worldCamera.ViewportToScreenPoint(viewPos), canvas.worldCamera, out var position))
            {
                return position;
            }

            return Vector2.zero;
        }
    }
}