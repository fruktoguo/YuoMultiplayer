using UnityEngine.InputSystem;
using YuoTools.Main.Ecs;

namespace YuoTools.Extend
{
    public class YuoInputSystemHelper : YuoComponentGet<YuoInputSystemHelper>
    {
        public virtual bool TryGetInput(string inputName, out InputAction input)
        {
            input = null;
            return false;
        }
        
        public virtual bool IsKeyDown(string inputName)
        {
            return false;
        }
    }
}