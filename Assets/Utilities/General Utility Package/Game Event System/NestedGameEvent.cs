using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GeneralUtility
{
    namespace GameEventSystem
    {
        [CreateAssetMenu(menuName = "GameEvents/NestedGameEvent")]
        public class NestedGameEvent : GameEvent
        {
            [SerializeField] private List<GameEvent> baseEvents = new();

            override public void Trigger()
            {
                base.Trigger();
                foreach (var e in baseEvents) e.Trigger();
            }
            override public void Trigger(bool val)
            {
                base.Trigger(val);
                foreach (var e in baseEvents) e.Trigger(val);
            }
            override public void Trigger(int val)
            {
                base.Trigger(val);
                foreach (var e in baseEvents) e.Trigger(val);
            }
            override public void Trigger(float val)
            {
                base.Trigger(val);
                foreach (var e in baseEvents) e.Trigger(val);
            }
            override public void Trigger(Vector2 val)
            {
                base.Trigger(val);
                foreach (var e in baseEvents) e.Trigger(val);
            }
            override public void Trigger(GameObject val)
            {
                base.Trigger(val);
                foreach (var e in baseEvents) e.Trigger(val);
            }
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(NestedGameEvent))]
        public class NestedGameEventEditor : Editor
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