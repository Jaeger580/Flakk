using UnityEngine;

public class AmmoCrateSpawner : MonoBehaviour
{
    public void SpawnCrate(AmmoCrateInteract cratePrefab)
    {
        Instantiate(cratePrefab, transform);
    }
}
