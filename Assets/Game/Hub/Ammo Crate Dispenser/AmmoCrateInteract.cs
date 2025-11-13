using GeneralUtility;
using GeneralUtility.GameEventSystem;
using UnityEngine;

public class AmmoCrateInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private BoxCollider col;
    [SerializeField] private Rigidbody rb;

    [SerializeField] private Transform spawnPos;
    [SerializeField] private Camera mainCam;

    [SerializeField] private AmmoStack crateConfig;

    [SerializeField] private GameEvent CrateLifted;

    private bool attached, deposited;

    [SerializeField]
    private AudioClip[] collisionSounds;

    

    public void Interact(object interactor)
    {
        if (interactor is not Interactor player)
        {
            Editor_Utility.ThrowWarning("ERR: Interactor not valid.", this);
            return;
        }

        AttachToPlayer(player.transform);
    }

    private void AttachToPlayer(Transform playerTrans)
    {
        if (!playerTrans.TryGetComponent(out AmmoCrateHolder holder)) return;
        if (!holder.TryPushCrateOnTop(this, crateConfig)) return;

        transform.parent = playerTrans;
        var pos = Vector3.zero;
        pos.z += 1f;
        transform.localPosition = pos;
        transform.localRotation = Quaternion.identity;
        if (col == null)
        {
            Editor_Utility.ThrowWarning("ERR: Missing collider on ammo crate.", this);
            return;
        }
        attached = true;

        CrateLifted.Trigger();

        deposited = false;
        //Physics.IgnoreCollision(col, playerTrans.GetComponent<Collider>(), true);
        col.enabled = false;
        rb.isKinematic = true;
        //col.excludeLayers = (playerMask);
    }

    public void DepositInBus(Transform busTransform)
    {
        transform.parent = busTransform;
        var pos = Vector3.zero;

        transform.localPosition = pos;
        transform.localRotation = Quaternion.identity;

        if (col == null)
        {
            Editor_Utility.ThrowWarning("ERR: Missing collider on ammo crate.", this);
            return;
        }

        attached = false;

        deposited = true;
        col.enabled = false;
        rb.isKinematic = true;
    }

    public void EjectFromBus(Vector3 ejectPos)
    {
        transform.parent = null;
        col.enabled = true;
        rb.isKinematic = false;
        attached = false;
        deposited = false;

        rb.AddExplosionForce(1750f, ejectPos, 5f, 5f, ForceMode.Impulse);
        Destroy(gameObject, 3f);
        col.enabled = false;
    }

    private void Update()
    {
        if (!attached) return;
        //MoveCurrentPlaceableObjectToMouse();
    }

    public void DetachFromPlayer()
    {
        transform.parent = null;
        col.enabled = true;
        rb.isKinematic = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.tag.Equals("Player")) 
        {
            int ranClip = Random.Range(0, collisionSounds.Length);

            AudioSource audioSource = this.GetComponent<AudioSource>();

            audioSource.clip = collisionSounds[ranClip];

            CustomAudio.PlayWithPitch(audioSource, audioSource.pitch);
        }   
    }
}