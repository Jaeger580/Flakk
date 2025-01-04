using System.Collections.Generic;
using GeneralUtility;
using GeneralUtility.EditorQoL;
using GeneralUtility.GameEventSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class InputHandler : MonoBehaviour
{
    [Header("Crate Handling Events")]
    [SerializeField] private GameEvent inputEvDropCrate;

    public void DropCrate(InputAction.CallbackContext context)
    {
        if (context.canceled)
            inputEvDropCrate.Trigger();
    }
}

public class AmmoCrateHolder : MonoBehaviour
{
    [SerializeField] private GameEvent inputEvDropCrate;
    [ReadOnly] [SerializeField] private Stack<(AmmoCrateInteract physicalCrate, AmmoStack ammoCrate)> registeredAmmoCrates = new();

    private void Start()
    {
        var dropCratesListener = gameObject.AddComponent<GameEventListener>();
        dropCratesListener.Events.Add(inputEvDropCrate);
        dropCratesListener.Response = new();
        dropCratesListener.Response.AddListener(() => DropAllCrates());
        inputEvDropCrate.RegisterListener(dropCratesListener);
    }

    public void PushCrateOnTop(AmmoCrateInteract physicalCrate, AmmoStack ammoCrate)
    {
        if (registeredAmmoCrates.Count > 0) { Editor_Utility.ThrowWarning("I can't hold all these lemons", this); return; }
        registeredAmmoCrates.Push((physicalCrate, ammoCrate));
    }

    public bool TryPopTopCrate(out (AmmoCrateInteract physicalCrate, AmmoStack ammoCrate) registeredCrate)
    {
        return registeredAmmoCrates.TryPop(out registeredCrate);
    }

    public void DropAllCrates()
    {
        foreach (var (physicalCrate, _) in registeredAmmoCrates)
        {
            physicalCrate.DetachFromPlayer();
        }
        registeredAmmoCrates.Clear();
    }
}
