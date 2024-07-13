using System;
using UnityEditor;
using UnityEngine;
namespace GeneralUtility
{
    namespace VariableObject
    {
        [CreateAssetMenu(menuName = "Variable Objects/Animation Curve Variable", fileName = "ACV - ", order = 1)]
        public class AnimationCurveVariable : VariableObject<AnimationCurve> { }
        [Serializable]
        public class AnimationCurveReference : VariableReference<AnimationCurve> { }
    }
}