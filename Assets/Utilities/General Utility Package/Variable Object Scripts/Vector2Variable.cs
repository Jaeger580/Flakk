using System;
using UnityEngine;

namespace GeneralUtility
{
    namespace VariableObject
    {
        [CreateAssetMenu(menuName = "Variable Objects/Vector2 Variable", fileName = "V2V - ", order = 1)]
        public class Vector2Variable : VariableObject<Vector2> { }
        [Serializable]
        public class Vector2Reference : VariableReference<Vector2> { }
    }
}