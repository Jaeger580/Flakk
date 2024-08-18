using System;
using UnityEditor;
using UnityEngine;
namespace GeneralUtility
{
    namespace VariableObject
    {
        [CreateAssetMenu(menuName = "Variable Objects/Game Object Variable", fileName = "GOV - ", order = 1)]
        public class GameObjectVariable : VariableObject<GameObject> { }
        [Serializable]
        public class GameObjectReference : VariableReference<GameObject> { }
    }
}