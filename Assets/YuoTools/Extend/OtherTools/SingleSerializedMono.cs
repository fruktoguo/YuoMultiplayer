using UnityEngine;

using Sirenix.OdinInspector;

namespace YuoTools
{
    public class SingletonSerializedMono<T> : SerializedMonoBehaviour where T : SingletonSerializedMono<T>
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObject(typeof(T).Name).AddComponent<T>();
                }

                return instance;
            }
            protected set => instance = value;
        }

        public virtual void Awake()
        {
            instance = this as T;
        }
    }
}