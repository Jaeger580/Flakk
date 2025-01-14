using System.Collections;
using System.Collections.Generic;
using GeneralUtility.EditorQoL;
using UnityEngine;

namespace GeneralUtility
{
    namespace VariableObject
    {
        abstract public class RuntimeSet<T> : ScriptableObject
        {//Looking to create a list to hold research, usually of a specific type, for reference for each weapon
            [Expandable] public List<T> items = new List<T>();
            public void Add(T t)
            {
                if (!items.Contains(t))
                    items.Add(t);
            }
            public void Remove(T t)
            {
                if (items.Contains(t))
                    items.Remove(t);
            }

            /// <summary>
            ///   <para>Checks if this RuntimeSet is null or empty, or if the first item is null.</para>
            /// </summary>
            /// <returns>
            /// <para>True if null or empty, otherwise false.</para>
            /// </returns>
            public bool GetNullOrEmpty()
            {
                return items == null || items.Count <= 0 || (items.Count == 1 && items[0] == null);
            }
        }

        abstract public class RuntimeStack<T> : ScriptableObject where T : ScriptableObject
        {
            public IntReference maxStackSize;
            [Expandable] public List<T> stack = new List<T>();
            public void Push(T t)
            {
                stack.Add(t);
            }

            public bool TryPush(T t)
            {
                if (stack.Count >= maxStackSize.Value)
                {
                    Editor_Utility.ThrowWarning("ERR: Stack overflow, did you try to push to a full stack?", this);
                    return false;
                }

                Push(t);
                return true;
            }

            public T Pop()
            {
                T t = stack[^1];
                stack.Remove(t);
                return t;
            }

            public bool TryPop(out T t)
            {
                t = null;
                if (stack.Count <= 0) return false;

                t = Pop();
                return true;
            }

            public T Peek()
            {
                T t = stack[^1];
                return t;
            }

            public bool TryPeek(out T t)
            {
                t = null;
                if (stack.Count <= 0) return false;

                t = Peek();
                return true;
            }
        }
    }
}