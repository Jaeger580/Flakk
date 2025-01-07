using UnityEngine;

public class AmmoCrateSpawner : MonoBehaviour, IInteractable
{
    [SerializeField] private AmmoCrateInteract ammoCratePrefab;

    public void Interact(object interactor)
    {
        if (interactor is not Interactor player) return;

        var crateInstance = Instantiate(ammoCratePrefab);
        crateInstance.Interact(player);
    }
}
