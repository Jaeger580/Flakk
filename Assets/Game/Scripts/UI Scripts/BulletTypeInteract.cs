using UnityEngine;
using GeneralUtility.VariableObject;

public class BulletTypeInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObjectReference bulletToSwitchTo, currentBullet;

    public void Interact()
    {
        currentBullet.Value = bulletToSwitchTo.Value;
    }
}
