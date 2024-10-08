using System;
using UnityEngine;

namespace GeneralUtility
{
    namespace VariableObject
    {
        [CreateAssetMenu(menuName = "Variable Objects/Gradient Variable", fileName = "GV - ", order = 1)]
        public class GradientVariable : VariableObject<Gradient> { }
        [Serializable]
        public class GradientReference : VariableReference<Gradient> { }
    }
}