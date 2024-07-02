using System;
using UnityEngine;
namespace GeneralUtility
{
    namespace VariableObject
    {
        [CreateAssetMenu]
        public class BoolVariable : ScriptableObject
        {
            public bool Value;
        }

        [Serializable]
        public class BoolReference
        {
            public bool UseConstant = true;
            public bool ConstantValue;
            public BoolVariable Variable;

            public bool Value
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