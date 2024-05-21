using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterationTool : MonoBehaviour
{
    [SerializeField] float interactRadius;
    [SerializeField] float interactAngle;
    [SerializeField] InventoryObject inventory;

    private float cosRange;

    private void Awake()
    {
        cosRange = Mathf.Cos(Mathf.Deg2Rad * interactAngle); // cos값을 얻기 위한 식
    }

    Collider[] colliders = new Collider[20];
    [SerializeField] LayerMask interactableLayer; // 필요하면 사용
    private void OnInteract()
    {
        Debug.Log("상호작용함수 시작부분");
        int size = Physics.OverlapSphereNonAlloc(transform.position, interactRadius, colliders, interactableLayer);
        Debug.Log(size);
        Debug.Log(colliders.Length);
        Debug.Log(interactRadius);
        Debug.Log(colliders);
        Debug.Log(interactableLayer);
        if (size > 0)
        {
            foreach (Collider collider in colliders)
            {
                HSH_IInteractable interactable = collider.GetComponent<HSH_IInteractable>();
                if (interactable == null)
                {
                    Debug.Log($"널값이래");
                    Debug.Log($"{collider.name}");
                    return;
                }

                Vector3 toTargetDir = (collider.transform.position - transform.position).normalized; // 대상의 방향
                if (Vector3.Dot(toTargetDir, transform.forward) < cosRange)
                    return;
                Debug.Log("상호작용툴에서 작동");
                interactable.Interact();
                Debug.Log("1함수 작동");
                interactable.Interact(inventory);
                Debug.Log("2함수 작동");
                break;
            }
        }
    }

    // Debug test..
    private enum Select { All, Right, Left, Down, Up }
    [Header("Gizmos")]
    [SerializeField] Select select;
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRadius);

        Gizmos.color = Color.red;
        Vector3 angleDir1 = transform.TransformDirection(new Vector3(Mathf.Sin(Mathf.Deg2Rad * interactAngle),
            0, Mathf.Cos(Mathf.Deg2Rad * interactAngle)));
        Vector3 angleDir2 = transform.TransformDirection(new Vector3(Mathf.Sin(Mathf.Deg2Rad * -interactAngle),
            0, Mathf.Cos(Mathf.Deg2Rad * -interactAngle)));
        Vector3 angleDir3 = transform.TransformDirection(new Vector3(0, Mathf.Sin(Mathf.Deg2Rad * -interactAngle),
             Mathf.Cos(Mathf.Deg2Rad * -interactAngle)));
        Vector3 angleDir4 = transform.TransformDirection(new Vector3(0, Mathf.Sin(Mathf.Deg2Rad * interactAngle),
             Mathf.Cos(Mathf.Deg2Rad * interactAngle)));
        switch (select)
        {
            case Select.All:
                Gizmos.DrawRay(transform.position, angleDir1 * interactRadius);
                Gizmos.DrawRay(transform.position, angleDir2 * interactRadius);
                Gizmos.DrawRay(transform.position, angleDir3 * interactRadius);
                Gizmos.DrawRay(transform.position, angleDir4 * interactRadius);
                break;
            case Select.Right:
                Gizmos.DrawRay(transform.position, angleDir1 * interactRadius);
                break;
            case Select.Left:
                Gizmos.DrawRay(transform.position, angleDir2 * interactRadius);
                break;
            case Select.Down:
                Gizmos.DrawRay(transform.position, angleDir3 * interactRadius);
                break;
            case Select.Up:
                Gizmos.DrawRay(transform.position, angleDir4 * interactRadius);
                break;
        }
    }
}
