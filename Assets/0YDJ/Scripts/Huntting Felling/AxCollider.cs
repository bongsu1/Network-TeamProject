using UnityEngine;

public class AxCollider : MonoBehaviour
{
    [SerializeField] LayerMask target;

    private void OnTriggerEnter(Collider other)
    {
        if (target.Contain(other.gameObject.layer)) //나무, 플레이어, 닭
        {
            IDamageble damageble = other.gameObject.GetComponent<IDamageble>();
            if (damageble != null)
            {
                Debug.Log("도끼에 닿음 HP 깍기");
                damageble.Damaged(1);
            }
        }
    }


}
