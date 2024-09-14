using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace YuoTools.Extend.YuoXrTools
{
    [Serializable]
    public class YuoPoseSource
    {
        public List<InputActionReference> actionReferences = new();

        public T ReadValue<T>() where T : struct
        {
            var def = default(T);
            foreach (var actionReference in actionReferences)
            {
                var value = actionReference.action.ReadValue<T>();
                if (!value.Equals(def))
                {
                    return value;
                }
            }

            return def;
        }
    }
}