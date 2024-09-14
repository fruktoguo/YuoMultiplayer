using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using YuoTools.Main.Ecs;

namespace YuoTools.UI
{
    [RequireComponent(typeof(MaskableGraphic))]
    public class YuoUIDrag : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler,
        IPointerExitHandler, IGenerateCode
    {
        private static YuoUIDrag _current;
        [HideInInspector] public MaskableGraphic graphic;
        public UnityEvent<YuoUIDrag> onDragEnd;
        public UnityEvent onDragStart;

        private void Awake()
        {
            graphic = GetComponent<MaskableGraphic>();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            //print($"拖拽结束：{gameObject.name},拖拽到了 {(Current != null ? Current.name : null)}");
            if (YuoUIDragManager.Get.DragItem == this) YuoUIDragManager.Get.DragItem = null;
            onDragEnd?.Invoke(_current);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            onDragStart?.Invoke();
            YuoUIDragManager.Get.DragItem = this;
            print($"开始拖拽 {gameObject.name}");
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _current = this;
            //print(gameObject.name + " OnPointerEnter");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_current == this) _current = null;
        }
    }

    public class YuoUIDragManager : YuoComponentInstance<YuoUIDragManager>
    {
        YuoUIDrag dragItem;

        public YuoUIDrag DragItem
        {
            get => dragItem;
            set
            {
                if (dragItem != null) dragItem.graphic.raycastTarget = true;
                if (value != null) value.graphic.raycastTarget = false;
                dragItem = value;
            }
        }
    }

    public class YuoUIDragSystem : YuoSystem<YuoUIDragManager>, IUpdate
    {
        public override string Group => SystemGroupConst.Input;

        protected override void Run(YuoUIDragManager component)
        {
            if (component.DragItem != null)
            {
                //将拖拽物体移动到鼠标所在位置
                component.DragItem.graphic.raycastTarget = false;
                component.DragItem.transform.position = Input.mousePosition;
            }
        }
    }
}