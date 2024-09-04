using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GeneralUtility
{
    namespace GameEventSystem
    {
        public class GameEventListener : MonoBehaviour
        {//WHEN WE EVENTUALLY REUSE THIS, REMEMBER TO GENERICIZE. (Allan has a WIP Folder)
            public List<GameEvent> Events = new();
            public UnityEvent Response;
            public UnityEvent<bool> BoolResponse;
            public UnityEvent<int> IntResponse;
            public UnityEvent<float> FloatResponse;
            public UnityEvent<Vector2> VecResponse;
            public UnityEvent<GameObject> GOResponse;

            private void OnEnable()
            {
                foreach (var e in Events)
                    e?.RegisterListener(this);
            }

            private void OnDestroy()
            {
                foreach (var e in Events)
                    e?.UnregisterListener(this);
            }

            public void OnEventTriggered()
            { Response?.Invoke(); }

            public void OnEventTriggered(bool val)
            { BoolResponse?.Invoke(val); }

            public void OnEventTriggered(int val)
            { IntResponse?.Invoke(val); }

            public void OnEventTriggered(float val)
            { FloatResponse?.Invoke(val); }

            public void OnEventTriggered(Vector2 vec)
            { VecResponse?.Invoke(vec); }

            public void OnEventTriggered(GameObject go)
            { GOResponse?.Invoke(go); }
        }
    }
}
