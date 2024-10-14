using GeneralUtility.GameEventSystem;
using GeneralUtility.VariableObject;
using UnityEngine;

[CreateAssetMenu(menuName = "Ammo System/Ammo Type", fileName = "Ammo Type - ", order = 0)]
public class AmmoType : ScriptableObject
{
    public GameObjectVariable bulletObj;
    public IntVariable damage;
    public GameEvent ammoTypeFiredEvent;
    public AudioClip[] shotSounds;
    //public IntVariable
}