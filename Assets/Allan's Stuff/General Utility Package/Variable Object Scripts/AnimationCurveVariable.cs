using System;
using UnityEngine;
namespace GeneralUtility
{
    namespace VariableObject
    {
        [CreateAssetMenu]
        public class AnimationCurveVariable : ScriptableObject
        {
            public AnimationCurve Value;
        }

        [Serializable]
        public class AnimationCurveReference
        {
            public bool UseConstant = true;
            public AnimationCurve ConstantValue;
            public AnimationCurveVariable Variable;

            public AnimationCurve Value
            {
                get { return UseConstant ? ConstantValue : Variable.Value; }
                set
                {
                    if (UseConstant)
                        ConstantValue = value;
                    else
                        Variable.Value = value;
                }
            }
        }
    }
}