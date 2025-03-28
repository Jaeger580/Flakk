using GeneralUtility.GameEventSystem;
using System.Collections;
using UnityEngine;

public class StockpileBusInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform busCenterTransform, ejectTransform;
    [SerializeField] private AmmoStack stockpileToLoad;

    [Tooltip("How long should a single ammo crate load into this bus (in seconds per crate)?")]
    [SerializeField] private float loadingRate = 1f;


    [SerializeField] private GameEvent CrateDeposit;

    [SerializeField]
    private AudioSource insertSFX;
    [SerializeField]
    private AudioSource loadSFX;
    [SerializeField]
    private AudioSource ejectSFX;

    public void Interact(object interactor)
    {
        print("Trying to interact with stockpile bus.");
        if (interactor is not Interactor playerInteractor) return;
        if (!playerInteractor.TryGetComponent(out AmmoCrateHolder ammoCrateHolder)) return;

        if (!ammoCrateHolder.TryPopTopCrate(out var poppedCrate)) return;

        CrateDeposit.Trigger();

        StartCoroutine(LoadStockpile(poppedCrate));
    }

    private IEnumerator LoadStockpile((AmmoCrateInteract physicalCrate, AmmoStack ammoCrate) poppedCrate)
    {
        var ammoCrate = poppedCrate.ammoCrate;
        var physicalCrate = poppedCrate.physicalCrate;

        int ammoInCrate = ammoCrate.stack.Count;
        var waitPerAmmo = new WaitForSeconds(loadingRate / ammoInCrate);

        physicalCrate.DepositInBus(busCenterTransform);
        CustomAudio.PlayOnceWithPitch(insertSFX, insertSFX.pitch);
        yield return new WaitForSeconds(0.25f);

        CustomAudio.PlayOnceWithPitch(loadSFX, loadSFX.pitch);
        yield return new WaitForSeconds(0.5f);


        while (ammoInCrate > 0)
        {//While there's still ammo in this crate,
            print("Still have ammo in the crate!");
            if (stockpileToLoad.TryPush(ammoCrate.Peek()))
            {//If I can fit it, try pushing ammo into the stockpile
                print("Loaded!");
                ammoInCrate--;  //If you did, register that you did
            }
            else
            {//otherwise, wait until I CAN fit ammo into the stockpile
                print("Waiting!");
                yield return new WaitUntil(() => stockpileToLoad.stack.Count < stockpileToLoad.maxStackSize.Value);
            }

            yield return waitPerAmmo;
        }

        // Forced delay to give time between insert and eject (For audio Primarly).
        CustomAudio.PlayOnceWithPitch(ejectSFX, ejectSFX.pitch);
        physicalCrate.EjectFromBus(ejectTransform.position);
    }
}
