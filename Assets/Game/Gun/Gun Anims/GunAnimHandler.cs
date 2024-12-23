using System.Collections;
using System.Collections.Generic;
using GeneralUtility.GameEventSystem;
using UnityEngine;

public class GunAnimHandler : MonoBehaviour
{
    [SerializeField] private GameEvent gunEnterEvent, gunExitEvent;
    [SerializeField] private Animator anim;

    private void Start()
    {
        var gunEnterListener = gameObject.AddComponent<GameEventListener>();
        gunEnterListener.Events.Add(gunEnterEvent);
        gunEnterListener.Response = new();
        gunEnterListener.Response.AddListener(() => GunEnterAnim());
        gunEnterEvent.RegisterListener(gunEnterListener);

        var gunExitListener = gameObject.AddComponent<GameEventListener>();
        gunExitListener.Events.Add(gunExitEvent);
        gunExitListener.Response = new();
        gunExitListener.Response.AddListener(() => GunExitAnim());
        gunExitEvent.RegisterListener(gunExitListener);
    }

    private void GunEnterAnim()
    {
        print("ENTERING");
        anim.SetTrigger("EnteringGun");
    }

    private void GunExitAnim()
    {
        print("EXITING");
        anim.SetTrigger("ExitingGun");
    }
}
