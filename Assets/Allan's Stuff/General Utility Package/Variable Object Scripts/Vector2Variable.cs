using System;
using UnityEngine;

namespace GeneralUtility
{
    namespace VariableObject
    {
        [CreateAssetMenu]
        public class Vector2Variable : ScriptableObject
        {
            public Vector2 Value;
        }

        [Serializable]
        public class Vector2Reference
        {
            public bool UseConstant = true;
            public Vector2 ConstantValue;
            public Vector2Variable Variable;

            public Vector2 Value
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