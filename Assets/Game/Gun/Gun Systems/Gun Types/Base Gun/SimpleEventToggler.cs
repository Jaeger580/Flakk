using GeneralUtility.GameEventSystem;
using UnityEngine;
public class SimpleEventToggler : SimpleEventHandler
{
    [SerializeField] private GameObject[] objs;

    protected override void HandleEvent()
    {
        foreach (var obj in objs)
        {
            obj.SetActive(!obj.activeInHierarchy);
        }
    }
}
