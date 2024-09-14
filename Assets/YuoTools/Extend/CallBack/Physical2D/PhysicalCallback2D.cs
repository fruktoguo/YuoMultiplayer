using UnityEngine;
using YuoTools.Main.Ecs;

namespace YuoTools.Extend.PhysicalCallback
{
    public class PhysicalCallback2D : MonoBehaviour
    {
        public PhysicalCallback2DComponent callbackComponent;
        public bool OpenOnCollisionEnter2D = true;
        public bool OpenOnCollisionExit2D = true;
        public bool OpenOnCollisionStay2D = true;

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!OpenOnCollisionEnter2D) return;
            if (callbackComponent.IsNull()) return;
            callbackComponent.eventData = other;
            callbackComponent.eventDataEntity = GameObjectMapManager.Get.GetMap(other.gameObject);

            YuoWorld.RunSystem<IOnCollisionEnter2D>(callbackComponent);

            callbackComponent.eventDataEntity = null;
            callbackComponent.eventData = null;
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (!OpenOnCollisionExit2D) return;
            if (callbackComponent.IsNull()) return;
            callbackComponent.eventData = other;
            callbackComponent.eventDataEntity = GameObjectMapManager.Get.GetMap(other.gameObject);

            YuoWorld.RunSystem<IOnCollisionExit2D>(callbackComponent);

            callbackComponent.eventDataEntity = null;
            callbackComponent.eventData = null;
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (!OpenOnCollisionStay2D) return;
            if (callbackComponent.IsNull()) return;
            callbackComponent.eventData = other;
            callbackComponent.eventDataEntity = GameObjectMapManager.Get.GetMap(other.gameObject);

            YuoWorld.RunSystem<IOnCollisionStay2D>(callbackComponent);

            callbackComponent.eventDataEntity = null;
            callbackComponent.eventData = null;
        }
    }

    public class PhysicalCallback2DComponent : YuoComponent
    {
        public Transform transform;
        public Collision2D eventData;
        public YuoEntity eventDataEntity;
        public PhysicalCallback2D callback;
    }

    public class PhysicalCallback2DComponentStartSystem : YuoSystem<PhysicalCallback2DComponent, TransformComponent>,
        IAwake, IDestroy
    {
        protected override void Run(PhysicalCallback2DComponent component, TransformComponent tran)
        {
            if (RunType == SystemTagType.Awake)
            {
                component.transform = tran.transform;

                GameObjectMapManager.Get.AddMap(component.transform.gameObject, component.Entity);

                //检查是否有物理回调组件
                var callback = component.transform.GetOrAddComponent<PhysicalCallback2D>();
                callback.callbackComponent = component;
                component.callback = callback;
                // "生成物理回调组件".Log();
            }

            if (RunType == SystemTagType.Destroy)
            {
                component.callback.callbackComponent = null;
                Object.Destroy(component.callback);
            }
        }
    }

    public interface IOnCollisionEnter2D : ISystemTag
    {
    }

    public interface IOnCollisionExit2D : ISystemTag
    {
    }

    public interface IOnCollisionStay2D : ISystemTag
    {
    }
}