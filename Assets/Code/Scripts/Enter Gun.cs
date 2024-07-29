using Cinemachine;
using GeneralUtility.GameEventSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnterGun : MonoBehaviour, IInteractable
{
    private CinemachineVirtualCamera playerVCAM;
    private GameObject player;
    private PlayerInput playInput;
    [SerializeField] private GameEvent gunEnterEvent;
    // Will change the players controlls and change there camera view to the gun / turret.
    public void Interact() 
    {
        //Debug.Log("It worked!");
        playerVCAM.Priority = 0;
        playInput.SwitchCurrentActionMap("Gun");
        gunEnterEvent?.Trigger();
        AudioManager.instance.SetVolume("BGM", 0.015f);
    }

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerVCAM = player.GetComponentInChildren<CinemachineVirtualCamera>();
        playInput= player.GetComponent<PlayerInput>();

    }
}
