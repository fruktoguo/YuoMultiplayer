using UnityEngine.UIElements;

namespace YuoTools.UI
{
    public partial class SpawnUICodeConfig
    {
        public void InitUIDocument()
        {
            SpawnType.Add(typeof(UIDocument));
            ComponentAddNameSpace.Add(typeof(UIDocument), "using UnityEngine.UIElements;");
        }
    }
}