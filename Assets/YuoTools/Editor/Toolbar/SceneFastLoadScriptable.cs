using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor
{
    [CreateAssetMenu(fileName = "SceneFastLoad", menuName = "YuoTools/SceneFastLoad")]
    public class SceneFastLoadScriptable : ScriptableObject
    {
        public List<SceneAsset> scenes;
    }
}