using System;
using UnityEngine;
using UnityEditor;
namespace GeneralUtility
{
    namespace VariableObject
    {
        [CreateAssetMenu]
        public class IntVariable : ScriptableObject
        {
            public int Value;
        }

        [Serializable]
        public class IntReference
        {
            public bool UseConstant = true;
            public int ConstantValue;
            public IntVariable Variable;

            public int Value
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