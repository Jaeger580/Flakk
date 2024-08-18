using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GeneralUtility
{
    namespace GameEventSystem
    {
        [CreateAssetMenu(menuName = "GameEvents/GameEvent")]
        public class GameEvent : ScriptableObject
        {//WHEN WE EVENTUALLY REUSE THIS, REMEMBER TO GENERICIZE. (Allan has a WIP Folder)
            public int exampleVal;
            private List<GameEventListener> listeners = new List<GameEventListener>();

            virtual public void Trigger()
            {
                for (int i = listeners.Count - 1; i >= 0; i--)
                    listeners[i].OnEventTriggered();
            }
            virtual public void Trigger(bool val)
            {
                for (int i = listeners.Count - 1; i >= 0; i--)
                    listeners[i].OnEventTriggered(val);
            }
            virtual public void Trigger(int val)
            {
                for (int i = listeners.Count - 1; i >= 0; i--)
                    listeners[i].OnEventTriggered(val);
            }
            virtual public void Trigger(float val)
            {
                for (int i = listeners.Count - 1; i >= 0; i--)
                    listeners[i].OnEventTriggered(val);
            }
            virtual public void Trigger(Vector2 vec)
            {
                for (int i = listeners.Count - 1; i >= 0; i--)
                    listeners[i].OnEventTriggered(vec);
            }
            virtual public void Trigger(GameObject go)
            {
                for (int i = listeners.Count - 1; i >= 0; i--)
                    listeners[i].OnEventTriggered(go);
            }
            public void RegisterListener(GameEventListener listener)
            {
                listeners.Add(listener);
            }
            public void UnregisterListener(GameEventListener listener)
            {
                if (listeners.Contains(listener))
                    listeners.Remove(listener);
                else
                    throw new System.Exception("LISTENER ERROR: " + listener.gameObject.name + " cannot be unregistered because it was never registered in the first place.");
            }
            public void UnregisterAllListeners()
            {
                listeners.Clear();
            }
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(GameEvent))]
        public class GameEventEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();

                GameEvent myGameEvent = (GameEvent)target;

                if (GUILayout.Button("Trigger()"))
                {
                    myGameEvent.Trigger();
                }
                if (GUILayout.Button("Variable Trigger: " + myGameEvent.exampleVal.ToString()))
                {
                    myGameEvent.Trigger(myGameEvent.exampleVal);
                }
            }
        }
#endif
    }
}