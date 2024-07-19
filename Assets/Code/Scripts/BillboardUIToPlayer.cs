using UnityEngine;

public class BillboardUIToPlayer : MonoBehaviour
{
    [SerializeField] private Transform gunCam;
    private Transform uiTrans;

    private void Start()
    {
        uiTrans = transform;
    }

    private void Update()
    {
        Vector3 dir = gunCam.position - uiTrans.position;
        dir.y = 0;
        uiTrans.rotation = Quaternion.LookRotation(dir);
    }
}