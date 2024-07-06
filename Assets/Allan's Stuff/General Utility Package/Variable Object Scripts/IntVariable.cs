using System;
using UnityEngine;
using UnityEditor;
namespace GeneralUtility
{
    namespace VariableObject
    {
        [CreateAssetMenu(menuName = "Variable Objects/Int Variable", fileName = "IV - ", order = 0)]
        public class IntVariable : VariableObject<int> {}
        [Serializable]
        public class IntReference : VariableReference<int> {}
    }
}