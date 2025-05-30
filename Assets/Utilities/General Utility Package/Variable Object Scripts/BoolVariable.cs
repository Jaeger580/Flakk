﻿using System;
using UnityEngine;
namespace GeneralUtility
{
    namespace VariableObject
    {
        [CreateAssetMenu(menuName = "Variable Objects/Bool Variable", fileName = "BV - ", order = 0)]
        public class BoolVariable : VariableObject<bool> { }
        [Serializable]
        public class BoolReference : VariableReference<bool> { }
    }
}